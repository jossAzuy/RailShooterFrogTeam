////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Martin Bustos @FronkonGames <fronkongames@gmail.com>. All rights reserved.
//
// THIS FILE CAN NOT BE HOSTED IN PUBLIC REPOSITORIES.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

namespace FronkonGames.Retro.ASCII
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class ASCII
  {
    [DisallowMultipleRendererFeature]
    private sealed class RenderPass : ScriptableRenderPass
    {
      // Internal use only.
      internal Material material { get; set; }

      private ASCIIVolume volume;

      /// <summary> Font parameters: xy char size, z char count. </summary>
      private Vector3 fontParams = Vector3.one;
      
      private static class ShaderIDs
      {
        public static readonly int Intensity = Shader.PropertyToID("_Intensity");
        public static readonly int EffectTime = Shader.PropertyToID("_EffectTime");

        public static readonly int AsciiTex = Shader.PropertyToID("_AsciiTex");
        public static readonly int Zoom = Shader.PropertyToID("_Zoom");
        public static readonly int Boost = Shader.PropertyToID("_Boost");
        public static readonly int FontColorBlend = Shader.PropertyToID("_FontColorBlend");
        public static readonly int FontColor = Shader.PropertyToID("_FontColor");
        public static readonly int BackgroundColorBlend = Shader.PropertyToID("_BackgroundColorBlend");
        public static readonly int BackgroundColor = Shader.PropertyToID("_BackgroundColor");
        public static readonly int ColorGradient = Shader.PropertyToID("_ColorGradient");
        public static readonly int ColorGradient0 = Shader.PropertyToID("_ColorGradient0");
        public static readonly int ColorGradient1 = Shader.PropertyToID("_ColorGradient1");
        public static readonly int GradientCircularRadius = Shader.PropertyToID("_GradientCircularRadius");
        public static readonly int GradientHorizontalOffset = Shader.PropertyToID("_GradientHorizontalOffset");
        public static readonly int GradientVerticalOffset = Shader.PropertyToID("_GradientVerticalOffset");
        public static readonly int FontParams = Shader.PropertyToID("_FontParams");
        
        // Shape-aware selection parameters.
        public static readonly int ShapeVectorTex = Shader.PropertyToID("_ShapeVectorTex");
        public static readonly int ShapeWeight = Shader.PropertyToID("_ShapeWeight");
        public static readonly int ShapeCharCount = Shader.PropertyToID("_ShapeCharCount");
        
        // Edge detection parameters.
        public static readonly int EdgeSensitivity = Shader.PropertyToID("_EdgeSensitivity");
        public static readonly int EdgeContrast = Shader.PropertyToID("_EdgeContrast");
        
        // Supersampling parameters.
        public static readonly int SuperSamplingLevel = Shader.PropertyToID("_SupersamplingLevel");
        
        public static readonly int Brightness = Shader.PropertyToID("_Brightness");
        public static readonly int Contrast = Shader.PropertyToID("_Contrast");
        public static readonly int Gamma = Shader.PropertyToID("_Gamma");
        public static readonly int Hue = Shader.PropertyToID("_Hue");
        public static readonly int Saturation = Shader.PropertyToID("_Saturation");
      }

      private static class Keywords
      {
        public const string BlockColor = "BLOCK_COLOR";
        public const string GradientHorizontal = "GRADIENT_HORIZONTAL";
        public const string GradientVertical = "GRADIENT_VERTICAL";
        public const string GradientCircular = "GRADIENT_CIRCULAR";
        public const string ShapeAware = "SHAPE_AWARE";
        public const string EdgeDetection = "EDGE_DETECTION";
        public const string SuperSampling = "SUPERSAMPLING";
      }

      /// <summary> Render pass constructor. </summary>
      public RenderPass() : base() => profilingSampler = new ProfilingSampler(Constants.Asset.AssemblyName);

      /// <summary> Destroy the render pass. </summary>
      ~RenderPass() => material = null;

      private void UpdateMaterial()
      {
        if (volume.charset.value == null || volume.charset.value.texture == null)
          return;

        material.shaderKeywords = null;
        material.SetFloat(ShaderIDs.Intensity, volume.intensity.value);

        float time = volume.useScaledTime.value == true ? Time.time : Time.unscaledTime;
        material.SetVector(ShaderIDs.EffectTime, new Vector4(time / 20.0f, time, time * 2.0f, time * 3.0f));

        if (volume.blockColor == true)
          material.EnableKeyword(Keywords.BlockColor);

        // Edge detection (works in both Luminance and ShapeAware modes)
        if (volume.edgeDetection.value == true)
        {
          material.EnableKeyword(Keywords.EdgeDetection);
          material.SetFloat(ShaderIDs.EdgeSensitivity, volume.edgeSensitivity.value);
          material.SetFloat(ShaderIDs.EdgeContrast, volume.edgeContrast.value);
        }

        // Super Sampling anti-aliasing (works in both modes)
        if (volume.superSampling.value == true)
        {
          material.EnableKeyword(Keywords.SuperSampling);
          material.SetInt(ShaderIDs.SuperSamplingLevel, Mathf.Clamp(volume.superSamplingLevel.value, 2, 4));
        }

        // Shape-aware selection mode
        if (volume.selectionMode == CharacterSelectionMode.ShapeAware && volume.charset.value.HasShapeData)
        {
          material.EnableKeyword(Keywords.ShapeAware);
          material.SetTexture(ShaderIDs.ShapeVectorTex, volume.charset.value.shapeVectorTexture);
          material.SetFloat(ShaderIDs.ShapeWeight, volume.shapeWeight.value);
          material.SetInt(ShaderIDs.ShapeCharCount, volume.charset.value.characterCount);
        }

        material.SetTexture(ShaderIDs.AsciiTex, volume.charset.value.texture);
        material.SetFloat(ShaderIDs.Zoom, volume.zoom.value);
        material.SetFloat(ShaderIDs.Boost, volume.boost.value);
        material.SetColor(ShaderIDs.FontColor, volume.fontColor.value);
        material.SetInt(ShaderIDs.FontColorBlend, (int)volume.fontColorBlend.value);
        material.SetColor(ShaderIDs.BackgroundColor, volume.backgroundColor.value);
        material.SetInt(ShaderIDs.BackgroundColorBlend, (int)volume.backgroundColorBlend.value);

        switch (volume.colorGradient.value)
        {
          case ColorGradients.None: break;
          case ColorGradients.Horizontal:
            material.EnableKeyword(Keywords.GradientHorizontal);
            material.SetColor(ShaderIDs.ColorGradient0, volume.colorGradient0.value);
            material.SetColor(ShaderIDs.ColorGradient1, volume.colorGradient1.value);
            material.SetFloat(ShaderIDs.GradientHorizontalOffset, volume.gradientHorizontalOffset.value - 1.0f);
            break;
          case ColorGradients.Vertical:
            material.EnableKeyword(Keywords.GradientVertical);
            material.SetColor(ShaderIDs.ColorGradient0, volume.colorGradient0.value);
            material.SetColor(ShaderIDs.ColorGradient1, volume.colorGradient1.value);
            material.SetFloat(ShaderIDs.GradientVerticalOffset, volume.gradientVerticalOffset.value);
            break;
          case ColorGradients.Circular:
            material.EnableKeyword(Keywords.GradientCircular);
            material.SetColor(ShaderIDs.ColorGradient0, volume.colorGradient0.value);
            material.SetColor(ShaderIDs.ColorGradient1, volume.colorGradient1.value);
            material.SetFloat(ShaderIDs.GradientCircularRadius, volume.gradientCircularRadius.value);
            break;
        }

        // Font parameters: xy char size, z char count.
        fontParams.x = volume.charset.value.texture.width / volume.charset.value.characterCount;
        fontParams.y = volume.charset.value.texture.height;
        fontParams.z = volume.charset.value.characterCount;
        material.SetVector(ShaderIDs.FontParams, fontParams);

        material.SetFloat(ShaderIDs.Brightness, volume.brightness.value);
        material.SetFloat(ShaderIDs.Contrast, volume.contrast.value);
        material.SetFloat(ShaderIDs.Gamma, 1.0f / volume.gamma.value);
        material.SetFloat(ShaderIDs.Hue, volume.hue.value);
        material.SetFloat(ShaderIDs.Saturation, volume.saturation.value);
      }

      /// <inheritdoc/>
      public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
      {
        volume = VolumeManager.instance.stack.GetComponent<ASCIIVolume>();
        if (volume == null || volume.IsActive() == false || material == null)
          return;

        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        if (resourceData.isActiveTargetBackBuffer == true)
          return;

        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        if (cameraData.camera.cameraType == CameraType.SceneView && volume.affectSceneView == false || cameraData.postProcessEnabled == false)
          return;

        TextureHandle source = resourceData.activeColorTexture;
        TextureHandle destination = renderGraph.CreateTexture(source.GetDescriptor(renderGraph));

        UpdateMaterial();

        RenderGraphUtils.BlitMaterialParameters pass = new(source, destination, material, 0);
        renderGraph.AddBlitPass(pass, $"{Constants.Asset.AssemblyName}.Pass");

        resourceData.cameraColor = destination;        
      }
    }
  }
}
