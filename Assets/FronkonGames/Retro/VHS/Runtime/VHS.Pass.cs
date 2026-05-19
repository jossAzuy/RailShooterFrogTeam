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

namespace FronkonGames.Retro.VHS
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class VHS
  {
    [DisallowMultipleRendererFeature]
    private sealed class RenderPass : ScriptableRenderPass
    {
      // Internal use only.
      internal Material material { get; set; }

      private VHSVolume volume;

      private readonly Texture2D noiseTexture;
      
      private static class ShaderIDs
      {
        public static readonly int Intensity = Shader.PropertyToID("_Intensity");
        public static readonly int EffectTime = Shader.PropertyToID("_EffectTime");

        public static readonly int Samples = Shader.PropertyToID("_Samples");
        public static readonly int Vignette = Shader.PropertyToID("_Vignette");
        public static readonly int ShadowTint = Shader.PropertyToID("_ShadowTint");
        public static readonly int WhiteLevel = Shader.PropertyToID("_WhiteLevel");
        public static readonly int BlackLevel = Shader.PropertyToID("_BlackLevel");
        public static readonly int ColorNoise = Shader.PropertyToID("_ColorNoise");
        public static readonly int ChromaBand = Shader.PropertyToID("_ChromaBand");
        public static readonly int LumaBand = Shader.PropertyToID("_LumaBand");
        public static readonly int TapeNoiseHigh = Shader.PropertyToID("_TapeNoiseHigh");
        public static readonly int TapeNoiseLow = Shader.PropertyToID("_TapeNoiseLow");
        public static readonly int ACBeatVelocity = Shader.PropertyToID("_ACBeatVelocity");
        public static readonly int ACBeatCount = Shader.PropertyToID("_ACBeatCount");
        public static readonly int ACBeatStrength = Shader.PropertyToID("_ACBeatStrength");
        public static readonly int TapeCreaseStrength = Shader.PropertyToID("_TapeCreaseStrength");
        public static readonly int TapeCreaseVelocity = Shader.PropertyToID("_TapeCreaseVelocity");
        public static readonly int TapeCreaseCount = Shader.PropertyToID("_TapeCreaseCount");
        public static readonly int TapeCreaseNoise = Shader.PropertyToID("_TapeCreaseNoise");
        public static readonly int TapeCreaseDistortion = Shader.PropertyToID("_TapeCreaseDistortion");
        public static readonly int BottomWarpHeight = Shader.PropertyToID("_BottomWarpHeight");
        public static readonly int BottomWarpDistortion = Shader.PropertyToID("_BottomWarpDistortion");
        public static readonly int BottomWarpJitterExtent = Shader.PropertyToID("_BottomWarpJitterExtent");
        public static readonly int YIQ = Shader.PropertyToID("_YIQ");

        public static readonly int NoiseTexture = Shader.PropertyToID("_NoiseTex");
        
        public static readonly int Brightness = Shader.PropertyToID("_Brightness");
        public static readonly int Contrast = Shader.PropertyToID("_Contrast");
        public static readonly int Gamma = Shader.PropertyToID("_Gamma");
        public static readonly int Hue = Shader.PropertyToID("_Hue");
        public static readonly int Saturation = Shader.PropertyToID("_Saturation");      
      }

      /// <summary> Render pass constructor. </summary>
      public RenderPass() : base()
      {
        if (noiseTexture == null)
          noiseTexture = Resources.Load<Texture2D>("Textures/Noise");

        profilingSampler = new ProfilingSampler(Constants.Asset.AssemblyName);
      }

      /// <summary> Destroy the render pass. </summary>
      ~RenderPass()
      {
        material = null;
        Resources.UnloadAsset(noiseTexture);
      }

      private void UpdateMaterial()
      {
        material.shaderKeywords = null;
        material.SetFloat(ShaderIDs.Intensity, volume.intensity.value);

        float time = volume.useScaledTime.value == true ? Time.time : Time.unscaledTime;
        material.SetVector(ShaderIDs.EffectTime, new Vector4(time / 20.0f, time, time * 2.0f, time * 3.0f));

        material.SetFloat(ShaderIDs.Brightness, volume.brightness.value);
        material.SetFloat(ShaderIDs.Contrast, volume.contrast.value);
        material.SetFloat(ShaderIDs.Gamma, 1.0f / volume.gamma.value);
        material.SetFloat(ShaderIDs.Hue, volume.hue.value);
        material.SetFloat(ShaderIDs.Saturation, volume.saturation.value);

        material.SetFloat(ShaderIDs.Vignette, volume.vignette.value);
        material.SetColor(ShaderIDs.ShadowTint, volume.shadowTint.value);
        material.SetFloat(ShaderIDs.WhiteLevel, volume.whiteLevel.value);
        material.SetFloat(ShaderIDs.BlackLevel, volume.blackLevel.value);
        material.SetFloat(ShaderIDs.ColorNoise, volume.colorNoise.value);
        material.SetFloat(ShaderIDs.ChromaBand, volume.chromaBand.value > 0 ? 1.0f / volume.chromaBand.value : 1.0f);
        material.SetFloat(ShaderIDs.LumaBand, volume.lumaBand.value > 0 ? 1.0f / volume.lumaBand.value : 1.0f);
        material.SetFloat(ShaderIDs.TapeNoiseHigh, volume.tapeNoiseHigh.value);
        material.SetFloat(ShaderIDs.TapeNoiseLow, volume.tapeNoiseLow.value);
        material.SetFloat(ShaderIDs.ACBeatStrength, volume.acBeatStrength.value);
        material.SetFloat(ShaderIDs.ACBeatVelocity, volume.acBeatVelocity.value);
        material.SetFloat(ShaderIDs.ACBeatCount, volume.acBeatCount.value);
        material.SetFloat(ShaderIDs.TapeCreaseStrength, volume.tapeCreaseStrength.value);
        material.SetFloat(ShaderIDs.TapeCreaseVelocity, volume.tapeCreaseVelocity.value);
        material.SetFloat(ShaderIDs.TapeCreaseCount, volume.tapeCreaseCount.value);
        material.SetFloat(ShaderIDs.TapeCreaseNoise, volume.tapeCreaseNoise.value);
        material.SetFloat(ShaderIDs.BottomWarpHeight, volume.bottomWarpHeight.value);
        material.SetFloat(ShaderIDs.BottomWarpDistortion, volume.bottomWarpDistortion.value * 1000.0f);
        material.SetFloat(ShaderIDs.BottomWarpJitterExtent, volume.bottomWarpJitterExtent.value);
        material.SetVector(ShaderIDs.YIQ, volume.yiq.value);

        if (volume.quality.value == Quality.HighFidelity)
        {
          material.SetInt(ShaderIDs.Samples, volume.samples.value);
          material.SetFloat(ShaderIDs.TapeCreaseDistortion, volume.tapeCreaseDistortion.value);

          material.SetTexture(ShaderIDs.NoiseTexture, noiseTexture);
        }
      }

      /// <inheritdoc/>
      public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
      {
        volume = VolumeManager.instance.stack.GetComponent<VHSVolume>();
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
