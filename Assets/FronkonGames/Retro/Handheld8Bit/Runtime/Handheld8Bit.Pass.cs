///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Martin Bustos @FronkonGames <fronkongames@gmail.com>. All rights reserved.
//
// THIS FILE CAN NOT BE HOSTED IN PUBLIC REPOSITORIES.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

namespace FronkonGames.Retro.Handheld8Bit
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class Handheld8Bit
  {
    [DisallowMultipleRendererFeature]
    private sealed class RenderPass : ScriptableRenderPass
    {
      // Internal use only.
      internal Material material { get; set; }

      private Handheld8BitVolume volume;

      private static class ShaderIDs
      {
        public static readonly int Intensity = Shader.PropertyToID("_Intensity");
        public static readonly int EffectTime = Shader.PropertyToID("_EffectTime");

        public static readonly int PixelSize = Shader.PropertyToID("_PixelSize");
        public static readonly int SubPixel = Shader.PropertyToID("_SubPixel");
        public static readonly int PixelDistance = Shader.PropertyToID("_PixelDistance");
        public static readonly int PixelOffset = Shader.PropertyToID("_PixelOffset");
        
        public static readonly int Luminosity = Shader.PropertyToID("_Luminosity");
        public static readonly int Threshold = Shader.PropertyToID("_Threshold");
        
        public static readonly int Invert = Shader.PropertyToID("_Invert");

        public static readonly int Palette0 = Shader.PropertyToID("_Palette0");
        public static readonly int Palette1 = Shader.PropertyToID("_Palette1");
        public static readonly int Palette2 = Shader.PropertyToID("_Palette2");
        public static readonly int Palette3 = Shader.PropertyToID("_Palette3");
        public static readonly int Grid = Shader.PropertyToID("_Grid");

        public static readonly int ShadowSize = Shader.PropertyToID("_ShadowSize");
        public static readonly int ShadowDistance = Shader.PropertyToID("_ShadowDistance");

        public static readonly int Brightness = Shader.PropertyToID("_Brightness");
        public static readonly int Contrast = Shader.PropertyToID("_Contrast");
        public static readonly int Gamma = Shader.PropertyToID("_Gamma");
        public static readonly int Hue = Shader.PropertyToID("_Hue");
        public static readonly int Saturation = Shader.PropertyToID("_Saturation");      
      }

      /// <summary> Render pass constructor. </summary>
      public RenderPass()
      {
        profilingSampler = new ProfilingSampler(Constants.Asset.AssemblyName);
      }

      /// <summary> Destroys the render pass. </summary>
      ~RenderPass() => material = null;

      private void UpdateMaterial()
      {
        material.shaderKeywords = null;
        material.SetFloat(ShaderIDs.Intensity, volume.intensity.value);

        float time = volume.useScaledTime.value == true ? Time.time : Time.unscaledTime;
        material.SetVector(ShaderIDs.EffectTime, new Vector4(time / 20.0f, time, time * 2.0f, time * 3.0f));

        material.SetFloat(ShaderIDs.PixelSize, volume.pixelSize.value);
        material.SetFloat(ShaderIDs.SubPixel, volume.subPixel.value);
        material.SetInt(ShaderIDs.PixelDistance, volume.pixelDistance.value);
        material.SetVector(ShaderIDs.PixelOffset, volume.pixelOffset.value);
        
        material.SetFloat(ShaderIDs.Luminosity, volume.luminosity.value);
        material.SetFloat(ShaderIDs.Threshold, volume.threshold.value);
        
        material.SetInt(ShaderIDs.Invert, volume.invert.value ? 1 : 0);

        material.SetColor(ShaderIDs.Palette0, volume.palette0.value);
        material.SetColor(ShaderIDs.Palette1, volume.palette1.value);
        material.SetColor(ShaderIDs.Palette2, volume.palette2.value);
        material.SetColor(ShaderIDs.Palette3, volume.palette3.value);
        material.SetColor(ShaderIDs.Grid, volume.grid.value);

        material.SetFloat(ShaderIDs.ShadowSize, volume.shadowSize.value);
        material.SetFloat(ShaderIDs.ShadowDistance, volume.shadowDistance.value);

        material.SetFloat(ShaderIDs.Brightness, volume.brightness.value);
        material.SetFloat(ShaderIDs.Contrast, volume.contrast.value);
        material.SetFloat(ShaderIDs.Gamma, volume.gamma.value); 
        material.SetFloat(ShaderIDs.Hue, volume.hue.value);
        material.SetFloat(ShaderIDs.Saturation, volume.saturation.value);
      }

      /// <inheritdoc/>
      public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
      {
        volume = VolumeManager.instance.stack.GetComponent<Handheld8BitVolume>();
        if (volume == null || volume.IsActive() == false || material == null)
          return;

        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        if (resourceData.isActiveTargetBackBuffer == true)
          return;

        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        if (cameraData.camera.cameraType == CameraType.SceneView && volume.affectSceneView.value == false || cameraData.postProcessEnabled == false)
          return;

        TextureHandle source = resourceData.activeColorTexture;
        TextureHandle destination = renderGraph.CreateTexture(source.GetDescriptor(renderGraph));
        TextureHandle intermediate = renderGraph.CreateTexture(source.GetDescriptor(renderGraph));

        UpdateMaterial();

        // Pass 0: Source -> Intermediate (Pixelate & Color)
        RenderGraphUtils.BlitMaterialParameters pass0 = new(source, intermediate, material, 0);
        renderGraph.AddBlitPass(pass0, $"{Constants.Asset.AssemblyName}.Pass0");

        // Pass 1: Intermediate -> Destination (Blur/Shadow)
        RenderGraphUtils.BlitMaterialParameters pass1 = new(intermediate, destination, material, 1);
        renderGraph.AddBlitPass(pass1, $"{Constants.Asset.AssemblyName}.Pass1");

        resourceData.cameraColor = destination;
      }
    }
  }
}
