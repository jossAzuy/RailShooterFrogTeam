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

namespace FronkonGames.Retro.LoFi
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class LoFi
  {
    [DisallowMultipleRendererFeature]
    private sealed class RenderPass : ScriptableRenderPass
    {
      // Internal use only.
      internal Material material { get; set; }

      private LoFiVolume volume;

      private Texture2D gradientTex;

      private LoFiProfile previousProfile;
      private BlendModes previousMode;
      private bool previousInvert;
      private PaletteResolutions previousResolution;

      private static class ShaderIDs
      {
        internal static readonly int Intensity = Shader.PropertyToID("_Intensity");
        internal static readonly int EffectTime = Shader.PropertyToID("_EffectTime");

        internal static readonly int ColorThreshold = Shader.PropertyToID("_ColorThreshold");
        internal static readonly int ColorSamples   = Shader.PropertyToID("_ColorSamples");
        internal static readonly int LuminancePow   = Shader.PropertyToID("_LuminancePow");
        internal static readonly int RangeMin       = Shader.PropertyToID("_RangeMin");
        internal static readonly int RangeMax       = Shader.PropertyToID("_RangeMax");

        internal static readonly int Pixelate   = Shader.PropertyToID("_Pixelate");
        internal static readonly int PixelSize  = Shader.PropertyToID("_PixelSize");
        internal static readonly int Samples    = Shader.PropertyToID("_Samples");

        internal static readonly int PixelSobel               = Shader.PropertyToID("_PixelSobel");
        internal static readonly int PixelSobelPower          = Shader.PropertyToID("_PixelSobelPower");
        internal static readonly int PixelSobelLight          = Shader.PropertyToID("_PixelSobelLight");
        internal static readonly int PixelSobelLightIntensity = Shader.PropertyToID("_PixelSobelLightIntensity");
        internal static readonly int PixelSobelAmbient        = Shader.PropertyToID("_PixelSobelAmbient");

        internal static readonly int PixelRound = Shader.PropertyToID("_PixelRound");
        internal static readonly int PixelBlend = Shader.PropertyToID("_PixelBlend");
        internal static readonly int PixelTint  = Shader.PropertyToID("_PixelTint");
        internal static readonly int Bevel      = Shader.PropertyToID("_Bevel");

        internal static readonly int Scanline = Shader.PropertyToID("_Scanline");
        internal static readonly int ScanlineCount     = Shader.PropertyToID("_ScanlineCount");
        internal static readonly int ScanlineSpeed     = Shader.PropertyToID("_ScanlineSpeed");

        internal static readonly int Dither       = Shader.PropertyToID("_Dither");
        internal static readonly int Quantization = Shader.PropertyToID("_Quantization");
        internal static readonly int Vignette     = Shader.PropertyToID("_Vignette");

        internal static readonly int ChromaticAberration = Shader.PropertyToID("_ChromaticAberration");

        internal static readonly int Shine     = Shader.PropertyToID("_Shine");
        internal static readonly int ShineSize = Shader.PropertyToID("_ShineSize");

        internal static readonly int Aperture  = Shader.PropertyToID("_Aperture");
        internal static readonly int Curvature = Shader.PropertyToID("_Curvature");

        internal static readonly int BorderColor   = Shader.PropertyToID("_BorderColor");
        internal static readonly int BorderSmooth  = Shader.PropertyToID("_BorderSmooth");
        internal static readonly int BorderNoise   = Shader.PropertyToID("_BorderNoise");
        internal static readonly int BorderMargins = Shader.PropertyToID("_BorderMargins");

        internal static readonly int Brightness = Shader.PropertyToID("_Brightness");
        internal static readonly int Contrast   = Shader.PropertyToID("_Contrast");
        internal static readonly int Gamma      = Shader.PropertyToID("_Gamma");
        internal static readonly int Hue        = Shader.PropertyToID("_Hue");
        internal static readonly int Saturation = Shader.PropertyToID("_Saturation");
      }

      private static class Textures
      {
        internal static readonly int Gradient = Shader.PropertyToID("_GradientTex");
      }

      private static class Keywords
      {
        internal const string UsePalette         = "USE_PALETTE";
        internal const string UseLuminanceSample = "USE_LUMINANCE_SAMPLE";
        internal const string UseDistanceSample  = "USE_DISTANCE_SAMPLE";
        internal const string UseHSVSample       = "USE_HSV_SAMPLE";
        internal const string UseDominantSample  = "USE_DOMINANT_SAMPLE";
        internal const string UsePixelate        = "USE_PIXELATE";
        internal const string UseSobel           = "USE_SOBEL";
        internal const string UseQuantization    = "USE_QUANTIZATION";
        internal const string UseBorder          = "USE_BORDER";
      }

      /// <summary> Render pass constructor. </summary>
      public RenderPass() : base()
      {
        profilingSampler = new ProfilingSampler(Constants.Asset.AssemblyName);
      }

      /// <summary> Destroy the render pass. </summary>
      ~RenderPass() => material = null;

      private void UpdateMaterial()
      {
        if (material == null || volume == null)
          return;

        material.shaderKeywords = null;
        material.SetFloat(ShaderIDs.Intensity, volume.intensity.value);

        float time = volume.useScaledTime.value == true ? Time.time : Time.unscaledTime;
        material.SetVector(ShaderIDs.EffectTime, new Vector4(time / 20.0f, time, time * 2.0f, time * 3.0f));

        if (volume.palette.value == true && volume.profile.value != null)
        {
          material.EnableKeyword(Keywords.UsePalette);

          if (volume.profile.value != previousProfile ||
              volume.mode.value != previousMode ||
              volume.invert.value != previousInvert ||
              volume.resolution.value != previousResolution ||
              gradientTex == null)
          {
            gradientTex = volume.profile.value.ToTexture(volume.mode.value, volume.invert.value, (int)volume.resolution.value);
            
            previousProfile = volume.profile.value;
            previousMode = volume.mode.value;
            previousInvert = volume.invert.value;
            previousResolution = volume.resolution.value;
          }

          material.SetTexture(Textures.Gradient, gradientTex);

          switch (volume.sampleMethod.value)
          {
            case SampleMethod.Luminance:
              material.EnableKeyword(Keywords.UseLuminanceSample);
              material.SetFloat(ShaderIDs.LuminancePow, 2.0f - volume.luminancePow.value);
              material.SetFloat(ShaderIDs.RangeMin, Mathf.Min(volume.rangeMin.value, volume.rangeMax.value));
              material.SetFloat(ShaderIDs.RangeMax, Mathf.Max(volume.rangeMax.value, volume.rangeMin.value));
              break;
            case SampleMethod.Distance:
              material.EnableKeyword(Keywords.UseDistanceSample);
              material.SetFloat(ShaderIDs.ColorSamples, (int)volume.resolution.value);
              material.SetFloat(ShaderIDs.ColorThreshold, volume.colorThreshold.value * 0.1f);
              break;
            case SampleMethod.HSV:
              material.EnableKeyword(Keywords.UseHSVSample);
              material.SetFloat(ShaderIDs.ColorSamples, (int)volume.resolution.value);
              material.SetFloat(ShaderIDs.ColorThreshold, volume.colorThreshold.value * 0.1f);
              break;
            case SampleMethod.Dominant:
              material.EnableKeyword(Keywords.UseDominantSample);
              material.SetFloat(ShaderIDs.ColorSamples, (int)volume.resolution.value);
              material.SetFloat(ShaderIDs.ColorThreshold, volume.colorThreshold.value * 0.1f);
              break;
            case SampleMethod.Similarity:
              material.SetFloat(ShaderIDs.ColorSamples, (int)volume.resolution.value);
              material.SetFloat(ShaderIDs.ColorThreshold, volume.colorThreshold.value * 25.0f);
              break;
          }
        }

        if (volume.pixelate.value == true)
        {
          material.EnableKeyword(Keywords.UsePixelate);

          material.SetFloat(ShaderIDs.PixelSize, Mathf.Max(1.0f, volume.pixelSize.value));
          material.SetFloat(ShaderIDs.Samples, Mathf.Max(1.0f, volume.pixelSamples.value));
          material.SetColor(ShaderIDs.PixelTint, volume.pixelTint.value);

          if (volume.pixelSobel.value > 0.0f)
          {
            material.EnableKeyword(Keywords.UseSobel);
            material.SetFloat(ShaderIDs.PixelSobel, volume.pixelSobel.value);
            material.SetFloat(ShaderIDs.PixelSobelPower, volume.pixelSobelPower.value);

            Vector3 light = new(-Mathf.Sin(volume.pixelSobelAngle.value * Mathf.Deg2Rad),
                                Mathf.Cos(volume.pixelSobelAngle.value * Mathf.Deg2Rad),
                                1.0f);
            material.SetVector(ShaderIDs.PixelSobelLight, light.normalized);
            material.SetFloat(ShaderIDs.PixelSobelLightIntensity, volume.pixelSobelLightIntensity.value);
            material.SetFloat(ShaderIDs.PixelSobelAmbient, volume.pixelSobelAmbient.value);
          }

          material.SetFloat(ShaderIDs.PixelRound, Mathf.Lerp(0.25f, 0.7f, volume.pixelRound.value));
          material.SetInt(ShaderIDs.PixelBlend, (int)volume.pixelBlend.value);
          material.SetFloat(ShaderIDs.Bevel, volume.pixelBevel.value);
        }

        if (volume.quantization.value == true)
        {
          material.EnableKeyword(Keywords.UseQuantization);
          material.SetFloat(ShaderIDs.Quantization, Mathf.Max(2, volume.colors.value - 1));
        }

        material.SetFloat(ShaderIDs.Vignette, volume.vignette.value);

        material.SetFloat(ShaderIDs.ChromaticAberration, volume.chromaticAberration.value * 25.0f);

        material.SetFloat(ShaderIDs.Shine, volume.shine.value);
        material.SetFloat(ShaderIDs.ShineSize, volume.shineSize.value);

        material.SetFloat(ShaderIDs.Aperture, Mathf.SmoothStep(10.0f, 1.0f, volume.aperture.value));
        material.SetFloat(ShaderIDs.Curvature, volume.curvature.value);

        if (volume.border.value == true)
        {
          material.EnableKeyword(Keywords.UseBorder);
          material.SetColor(ShaderIDs.BorderColor, volume.borderColor.value);
          material.SetFloat(ShaderIDs.BorderSmooth, volume.borderSmooth.value * 0.01f);
          material.SetFloat(ShaderIDs.BorderNoise, volume.borderNoise.value * 0.1f);
          material.SetVector(ShaderIDs.BorderMargins, volume.borderMargins.value);
        }

        material.SetFloat(ShaderIDs.Scanline, volume.scanline.value);
        material.SetInt(ShaderIDs.ScanlineCount, volume.scanlineCount.value);
        material.SetFloat(ShaderIDs.ScanlineSpeed, volume.scanlineSpeed.value);

        material.SetFloat(ShaderIDs.Brightness, volume.brightness.value);
        material.SetFloat(ShaderIDs.Contrast, volume.contrast.value);
        material.SetFloat(ShaderIDs.Gamma, 1.0f / volume.gamma.value);
        material.SetFloat(ShaderIDs.Hue, volume.hue.value);
        material.SetFloat(ShaderIDs.Saturation, volume.saturation.value);
      }

      /// <inheritdoc/>
      public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
      {
        volume = VolumeManager.instance.stack.GetComponent<LoFiVolume>();
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