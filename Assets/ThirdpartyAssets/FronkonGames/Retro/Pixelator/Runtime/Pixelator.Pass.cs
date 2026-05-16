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

namespace FronkonGames.Retro.Pixelator
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class Pixelator
  {
    [DisallowMultipleRendererFeature]
    private sealed class RenderPass : ScriptableRenderPass
    {
      // Internal use only.
      internal Material material { get; set; }

      private PixelatorVolume volume;

      private Gradient gradient = null;
      private Texture2D gradientTexture;

      private static class ShaderIDs
      {
        public static readonly int Intensity = Shader.PropertyToID("_Intensity");
        public static readonly int EffectTime = Shader.PropertyToID("_EffectTime");

        public static readonly int PixelationMode = Shader.PropertyToID("_PixelationMode");
        public static readonly int PixelSize = Shader.PropertyToID("_PixelSize");
        public static readonly int PixelScale = Shader.PropertyToID("_PixelScale");
        public static readonly int AspectRatio = Shader.PropertyToID("_AspectRatio");
        public static readonly int Radius = Shader.PropertyToID("_Radius");
        public static readonly int Background = Shader.PropertyToID("_Background");
        public static readonly int Threads = Shader.PropertyToID("_Threads");

        public static readonly int GradientIntensity = Shader.PropertyToID("_GradientIntensity");
        public static readonly int LuminanceMin = Shader.PropertyToID("_LuminanceMin");
        public static readonly int LuminanceMax = Shader.PropertyToID("_LuminanceMax");
        public static readonly int GradientCIELabSamples = Shader.PropertyToID("_GradientCIELabSamples");

        public static readonly int Bevel = Shader.PropertyToID("_Bevel");
        public static readonly int BevelLightColor = Shader.PropertyToID("_BevelLightColor");
        public static readonly int BevelShadowColor = Shader.PropertyToID("_BevelShadowColor");
        public static readonly int BevelDepthSensitivity = Shader.PropertyToID("_BevelDepthSensitivity");
        public static readonly int BevelLightDirection = Shader.PropertyToID("_BevelLightDirection");

        public static readonly int ChromaticAberrationIntensity = Shader.PropertyToID("_ChromaticAberrationIntensity");
        public static readonly int ChromaticAberrationOffset = Shader.PropertyToID("_ChromaticAberrationOffset");

        public static readonly int DitherIntensity = Shader.PropertyToID("_DitherIntensity");
        public static readonly int DitherPatternScale = Shader.PropertyToID("_DitherPatternScale");
        public static readonly int DitherThresholdScale = Shader.PropertyToID("_DitherThresholdScale");
        public static readonly int DitherColorSteps = Shader.PropertyToID("_DitherColorSteps");

        public static readonly int PosterizeIntensity = Shader.PropertyToID("_PosterizeIntensity");
        public static readonly int PosterizeStepsRGB = Shader.PropertyToID("_PosterizeStepsRGB");
        public static readonly int PosterizeLuminanceSteps = Shader.PropertyToID("_PosterizeLuminanceSteps");
        public static readonly int PosterizeStepsHSV = Shader.PropertyToID("_PosterizeStepsHSV");
        public static readonly int PosterizeGamma = Shader.PropertyToID("_PosterizeGamma");

        public static readonly int FiltersIntensity = Shader.PropertyToID("_FiltersIntensity");
        public static readonly int SepiaIntensity = Shader.PropertyToID("_SepiaIntensity");
        public static readonly int CoolBlueIntensity = Shader.PropertyToID("_CoolBlueIntensity");
        public static readonly int WarmFilterIntensity = Shader.PropertyToID("_WarmFilterIntensity");
        public static readonly int InvertColorIntensity = Shader.PropertyToID("_InvertColorIntensity");
        public static readonly int HudsonIntensity = Shader.PropertyToID("_HudsonIntensity");
        public static readonly int HefeIntensity = Shader.PropertyToID("_HefeIntensity");
        public static readonly int XProIntensity = Shader.PropertyToID("_XProIntensity");
        public static readonly int RiseIntensity = Shader.PropertyToID("_RiseIntensity");
        public static readonly int ToasterIntensity = Shader.PropertyToID("_ToasterIntensity");
        public static readonly int IRFilterIntensity = Shader.PropertyToID("_IRFilterIntensity");
        public static readonly int ThermalFilterIntensity = Shader.PropertyToID("_ThermalFilterIntensity");
        public static readonly int DuotoneIntensity = Shader.PropertyToID("_DuotoneIntensity");
        public static readonly int DuotoneColorA = Shader.PropertyToID("_DuotoneColorA");
        public static readonly int DuotoneColorB = Shader.PropertyToID("_DuotoneColorB");
        public static readonly int NightVisionIntensity = Shader.PropertyToID("_NightVisionIntensity");
        public static readonly int PopArtIntensity = Shader.PropertyToID("_PopArtIntensity");
        public static readonly int BlueprintIntensity = Shader.PropertyToID("_BlueprintIntensity");
        public static readonly int BlueprintEdgeColor = Shader.PropertyToID("_BlueprintEdgeColor");
        public static readonly int BlueprintBackgroundColor = Shader.PropertyToID("_BlueprintBackgroundColor");
        public static readonly int BlueprintEdgeThreshold = Shader.PropertyToID("_BlueprintEdgeThreshold");

        public static readonly int Brightness = Shader.PropertyToID("_Brightness");
        public static readonly int Contrast = Shader.PropertyToID("_Contrast");
        public static readonly int Gamma = Shader.PropertyToID("_Gamma");
        public static readonly int Hue = Shader.PropertyToID("_Hue");
        public static readonly int Saturation = Shader.PropertyToID("_Saturation");
      }

      private static class Keywords
      {
        public static readonly string PixelationRectangle = "_PIXELATION_MODE_RECTANGLE";
        public static readonly string PixelationHexagon   = "_PIXELATION_MODE_HEXAGON";
        public static readonly string PixelationDiamond   = "_PIXELATION_MODE_DIAMOND";
        public static readonly string PixelationCircle    = "_PIXELATION_MODE_CIRCLE";
        public static readonly string PixelationTriangle  = "_PIXELATION_MODE_TRIANGLE";
        public static readonly string PixelationSquare    = "_PIXELATION_MODE_SQUARE";
        public static readonly string PixelationLeaf      = "_PIXELATION_MODE_LEAF";
        public static readonly string PixelationLed       = "_PIXELATION_MODE_LED";
        public static readonly string PixelationKnitted   = "_PIXELATION_MODE_KNITTED";

        public static readonly string Gradient            = "_GRADIENT";
        public static readonly string GradientCIELab      = "_GRADIENT_CIELAB";
        public static readonly string GradientLuminance   = "_GRADIENT_APPLY_LUMINANCE";

        public static readonly string Bevel               = "_BEVEL";

        public static readonly string Dither              = "_DITHER";

        public static readonly string ChromaticAberration = "_CHROMATIC_ABERRATION";

        public static readonly string Posterize           = "_POSTERIZE";

        public static readonly string Filters             = "_FILTERS";
      }

      private static class Textures
      {
        internal static readonly int GradientTexture = Shader.PropertyToID("_GradientTex");
      }

      /// <summary> Render pass constructor. </summary>
      public RenderPass() : base()
      {
        profilingSampler = new ProfilingSampler(Constants.Asset.AssemblyName);
      }

      /// <summary> Destroy the render pass. </summary>
      ~RenderPass()
      {
        gradient = null;
        gradientTexture = null;
      }

      /// <summary> Update gradient texture. </summary>
      private void UpdateGradientTexture()
      {
        gradient = volume.gradient.value;

        const int width = 256;
        const int height = 4;
        gradientTexture = new Texture2D(width, height, TextureFormat.RGB24, false) { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };

        const float inv = 1.0f / (width - 1);
        for (int y = 0; y < height; ++y)
        {
          for (int x = 0; x < width; ++x)
            gradientTexture.SetPixel(x, y, gradient.Evaluate(x * inv));
        }

        gradientTexture.Apply();
      }

      private void UpdateMaterial()
      {
        material.shaderKeywords = null;
        material.SetFloat(ShaderIDs.Intensity, volume.intensity.value);

        float time = volume.useScaledTime.value == true ? Time.time : Time.unscaledTime;
        material.SetVector(ShaderIDs.EffectTime, new Vector4(time / 20.0f, time, time * 2.0f, time * 3.0f));

        float aspectRatio = volume.screenAspectRatio.value == true ? Screen.width / Screen.height : volume.aspectRatio.value;

        switch (volume.pixelationMode.value)
        {
          case PixelationModes.Rectangle:
            material.EnableKeyword(Keywords.PixelationRectangle);
            material.SetFloat(ShaderIDs.PixelSize, (1.01f - volume.pixelSize.value) * 500.0f);
            material.SetVector(ShaderIDs.PixelScale, volume.pixelScale.value);
            material.SetFloat(ShaderIDs.AspectRatio, aspectRatio);
            break;
          case PixelationModes.Circle:
            material.EnableKeyword(Keywords.PixelationCircle);
            material.SetFloat(ShaderIDs.PixelSize, (1.01f - volume.pixelSize.value) * 200.0f);
            material.SetFloat(ShaderIDs.Radius, volume.radius.value);
            material.SetColor(ShaderIDs.Background, volume.background.value);
            break;
          case PixelationModes.Triangle:
            material.EnableKeyword(Keywords.PixelationTriangle);
            material.SetFloat(ShaderIDs.PixelSize, (1.01f - volume.pixelSize.value) * 500.0f);
            material.SetVector(ShaderIDs.PixelScale, volume.pixelScale.value);
            material.SetFloat(ShaderIDs.AspectRatio, aspectRatio);
            break;
          case PixelationModes.Diamond:
            material.EnableKeyword(Keywords.PixelationDiamond);
            material.SetFloat(ShaderIDs.PixelSize, volume.pixelSize.value * 0.2f);
            break;
          case PixelationModes.Hexagon:
            material.EnableKeyword(Keywords.PixelationHexagon);
            material.SetFloat(ShaderIDs.PixelSize, volume.pixelSize.value * 0.02f);
            material.SetVector(ShaderIDs.PixelScale, volume.pixelScale.value);
            material.SetFloat(ShaderIDs.AspectRatio, aspectRatio);
            break;
          case PixelationModes.Leaf:
            material.EnableKeyword(Keywords.PixelationLeaf);
            material.SetFloat(ShaderIDs.PixelSize, (1.01f - volume.pixelSize.value) * 10.0f);
            material.SetVector(ShaderIDs.PixelScale, volume.pixelScale.value * 20.0f);
            material.SetFloat(ShaderIDs.AspectRatio, aspectRatio);
            break;
          case PixelationModes.Led:
            material.EnableKeyword(Keywords.PixelationLed);
            material.SetFloat(ShaderIDs.PixelSize, (1.01f - volume.pixelSize.value) * 300.0f);
            material.SetFloat(ShaderIDs.AspectRatio, aspectRatio);
            material.SetFloat(ShaderIDs.Radius, volume.radius.value);
            material.SetColor(ShaderIDs.Background, volume.background.value);
            break;
          case PixelationModes.Knitted:
            material.EnableKeyword(Keywords.PixelationKnitted);
            material.SetFloat(ShaderIDs.PixelSize, Mathf.Max(volume.pixelSize.value * 32.0f, 0.05f));
            material.SetVector(ShaderIDs.PixelScale, volume.pixelScale.value);
            material.SetInt(ShaderIDs.Threads, volume.threads.value);
            break;
        }

        if (volume.gradientIntensity.value > 0.0f)
        {
          material.EnableKeyword(Keywords.Gradient);

          material.SetFloat(ShaderIDs.GradientIntensity, volume.gradientIntensity.value);
          if (gradient == null || gradient != volume.gradient.value)
            UpdateGradientTexture();
          material.SetTexture(Textures.GradientTexture, gradientTexture);

          if (volume.gradientApplyLuminance.value == true)
            material.EnableKeyword(Keywords.GradientLuminance);

          material.SetFloat(ShaderIDs.LuminanceMin, volume.luminanceMin.value);
          material.SetFloat(ShaderIDs.LuminanceMax, volume.luminanceMax.value);

          if (volume.gradientMappingMode.value == GradientMappingMode.CIELAB)
          {
            material.EnableKeyword(Keywords.GradientCIELab);
            material.SetInt(ShaderIDs.GradientCIELabSamples, volume.gradientCIELabSamples.value);
          }
        }

        if (volume.chromaticAberrationIntensity.value > 0.0f)
        {
          material.EnableKeyword(Keywords.ChromaticAberration);
          material.SetFloat(ShaderIDs.ChromaticAberrationIntensity, volume.chromaticAberrationIntensity.value);
          material.SetVector(ShaderIDs.ChromaticAberrationOffset, volume.chromaticAberrationOffset.value);
        }

        material.SetFloat(ShaderIDs.Brightness, volume.brightness.value);
        material.SetFloat(ShaderIDs.Contrast, volume.contrast.value);
        material.SetFloat(ShaderIDs.Gamma, 1.0f / volume.gamma.value);
        material.SetFloat(ShaderIDs.Hue, volume.hue.value);
        material.SetFloat(ShaderIDs.Saturation, volume.saturation.value);        

        if (volume.bevel.value > 0.0f)
        {
          material.EnableKeyword(Keywords.Bevel);
          material.SetFloat(ShaderIDs.Bevel, volume.bevel.value);
        }

        if (volume.ditherIntensity.value > 0.0f)
        {
          material.EnableKeyword(Keywords.Dither);
          material.SetFloat(ShaderIDs.DitherIntensity, volume.ditherIntensity.value);
          material.SetInt(ShaderIDs.DitherPatternScale, volume.ditherPatternScale.value);
          material.SetFloat(ShaderIDs.DitherThresholdScale, volume.ditherThresholdScale.value);
          material.SetInt(ShaderIDs.DitherColorSteps, volume.ditherColorSteps.value);
        }

        if (volume.posterizeIntensity.value > 0.0f)
        {
          material.EnableKeyword(Keywords.Posterize);
          material.SetFloat(ShaderIDs.PosterizeIntensity, volume.posterizeIntensity.value);
          material.SetVector(ShaderIDs.PosterizeStepsRGB, (Vector3)volume.posterizeRGBSteps.value);
          material.SetFloat(ShaderIDs.PosterizeLuminanceSteps, volume.posterizeLuminanceSteps.value);
          material.SetVector(ShaderIDs.PosterizeStepsHSV, (Vector3)volume.posterizeHSVSteps.value);
          material.SetFloat(ShaderIDs.PosterizeGamma, volume.posterizeGamma.value);
        }

        if (volume.filtersIntensity.value > 0.0f)
        {
          material.EnableKeyword(Keywords.Filters);
          material.SetFloat(ShaderIDs.FiltersIntensity, volume.filtersIntensity.value);
          material.SetFloat(ShaderIDs.SepiaIntensity, volume.sepiaIntensity.value);
          material.SetFloat(ShaderIDs.CoolBlueIntensity, volume.coolBlueIntensity.value);
          material.SetFloat(ShaderIDs.WarmFilterIntensity, volume.warmFilterIntensity.value);
          material.SetFloat(ShaderIDs.InvertColorIntensity, volume.invertColorIntensity.value);
          material.SetFloat(ShaderIDs.HudsonIntensity, volume.hudsonIntensity.value);
          material.SetFloat(ShaderIDs.HefeIntensity, volume.hefeIntensity.value);
          material.SetFloat(ShaderIDs.XProIntensity, volume.xproIntensity.value);
          material.SetFloat(ShaderIDs.RiseIntensity, volume.riseIntensity.value);
          material.SetFloat(ShaderIDs.ToasterIntensity, volume.toasterIntensity.value);
          material.SetFloat(ShaderIDs.IRFilterIntensity, volume.irFilterIntensity.value);
          material.SetFloat(ShaderIDs.ThermalFilterIntensity, volume.thermalFilterIntensity.value);
          material.SetFloat(ShaderIDs.DuotoneIntensity, volume.duotoneIntensity.value);
          material.SetColor(ShaderIDs.DuotoneColorA, volume.duotoneColorA.value);
          material.SetColor(ShaderIDs.DuotoneColorB, volume.duotoneColorB.value);
          material.SetFloat(ShaderIDs.NightVisionIntensity, volume.nightVisionIntensity.value);
          material.SetFloat(ShaderIDs.PopArtIntensity, volume.popArtIntensity.value);
          material.SetFloat(ShaderIDs.BlueprintIntensity, volume.blueprintIntensity.value);
          material.SetColor(ShaderIDs.BlueprintEdgeColor, volume.blueprintEdgeColor.value);
          material.SetColor(ShaderIDs.BlueprintBackgroundColor, volume.blueprintBackgroundColor.value);
          material.SetFloat(ShaderIDs.BlueprintEdgeThreshold, volume.blueprintEdgeThreshold.value);
        }
      }

      /// <inheritdoc/>
      public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
      {
        volume = VolumeManager.instance.stack.GetComponent<PixelatorVolume>();
        if (material == null || volume == null || volume.IsActive() == false)
          return;

        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        if (resourceData.isActiveTargetBackBuffer == true)
          return;

        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        if (cameraData.camera.cameraType == CameraType.SceneView && volume.affectSceneView.value == false || cameraData.postProcessEnabled == false)
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
