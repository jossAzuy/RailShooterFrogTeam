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

namespace FronkonGames.Retro.NTSC
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class NTSC
  {
    [DisallowMultipleRendererFeature]
    private sealed class RenderPass : ScriptableRenderPass
    {
      // Internal use only.
      internal Material material { get; set; }

      private NTSCVolume volume;

      private TextureHandle renderTextureHandle0;
      private TextureHandle renderTextureHandle1;
      private TextureHandle renderTextureHandle2;

      private static class ShaderIDs
      {
        public static readonly int Intensity = Shader.PropertyToID("_Intensity");
        public static readonly int EffectTime = Shader.PropertyToID("_EffectTime");

        public static readonly int WindowRadius = Shader.PropertyToID("_WindowRadius");
        public static readonly int AMCarrierSignalWavelength = Shader.PropertyToID("_AMCarrierSignalWavelength");
        public static readonly int YLowPassWavelength = Shader.PropertyToID("_YLowPassWavelength");
        public static readonly int ILowPassWavelength = Shader.PropertyToID("_ILowPassWavelength");
        public static readonly int QLowPassWavelength = Shader.PropertyToID("_QLowPassWavelength");
        public static readonly int ColorburstWavelengthEncoder = Shader.PropertyToID("_ColorburstWavelengthEncoder");
        public static readonly int NTSCScale = Shader.PropertyToID("_NTSCScale");
        public static readonly int PhaseAlternation = Shader.PropertyToID("_PhaseAlternation");
        public static readonly int NoiseStrength = Shader.PropertyToID("_NoiseStrength");
        public static readonly int WindowBias = Shader.PropertyToID("_WindowBias");
        public static readonly int AMDemodulateWavelength = Shader.PropertyToID("_AMDemodulateWavelength");
        public static readonly int AMDecodeHighPassWavelength = Shader.PropertyToID("_AMDecodeHighPassWavelength");
        public static readonly int ColorburstWavelengthDecoder = Shader.PropertyToID("_ColorburstWavelengthDecoder");
        public static readonly int DecodeLowPassWavelength = Shader.PropertyToID("_DecodeLowPassWavelength");
        
        public static readonly int CurrentTime = Shader.PropertyToID("_CurrentTime");
        public static readonly int CurrentFrame = Shader.PropertyToID("_CurrentFrame");
        
        public static readonly int Brightness = Shader.PropertyToID("_Brightness");
        public static readonly int Contrast = Shader.PropertyToID("_Contrast");
        public static readonly int Saturation = Shader.PropertyToID("_Saturation");
        public static readonly int Gamma = Shader.PropertyToID("_Gamma");
        public static readonly int Hue = Shader.PropertyToID("_Hue");
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
        if (material != null && volume != null)
        {
          material.shaderKeywords = null;
          material.SetFloat(ShaderIDs.Intensity, volume.intensity.value);

          material.SetInt(ShaderIDs.WindowRadius, volume.ntscWindowRadius.value);
          material.SetFloat(ShaderIDs.AMCarrierSignalWavelength, volume.amCarrierSignalWavelength.value);
          material.SetFloat(ShaderIDs.YLowPassWavelength, volume.yLowPassWavelength.value);
          material.SetFloat(ShaderIDs.ILowPassWavelength, volume.iLowPassWavelength.value);
          material.SetFloat(ShaderIDs.QLowPassWavelength, volume.qLowPassWavelength.value);
          material.SetFloat(ShaderIDs.ColorburstWavelengthEncoder, volume.colorburstWavelengthEncoder.value);
          material.SetFloat(ShaderIDs.NTSCScale, volume.ntscScale.value);
          material.SetFloat(ShaderIDs.PhaseAlternation, volume.ntscPhaseAlternation.value);
          material.SetFloat(ShaderIDs.NoiseStrength, volume.ntscNoiseStrength.value * 0.5f);
          material.SetFloat(ShaderIDs.WindowBias, volume.ntscWindowBias.value);
          material.SetFloat(ShaderIDs.AMDemodulateWavelength, volume.amDemodulateWavelength.value);
          material.SetFloat(ShaderIDs.AMDecodeHighPassWavelength, volume.amDecodeHighPassWavelength.value);
          material.SetFloat(ShaderIDs.ColorburstWavelengthDecoder, volume.colorburstWavelengthDecoder.value);
          material.SetFloat(ShaderIDs.DecodeLowPassWavelength, volume.decodeLowPassWavelength.value);

          float time = volume.useScaledTime.value == true ? Time.time : Time.unscaledTime;
          material.SetVector(ShaderIDs.EffectTime, new Vector4(time / 20.0f, time, time * 2.0f, time * 3.0f));
          material.SetFloat(ShaderIDs.CurrentTime, time);
          material.SetInt(ShaderIDs.CurrentFrame, Time.frameCount);

          material.SetFloat(ShaderIDs.Brightness, volume.brightness.value);
          material.SetFloat(ShaderIDs.Contrast, volume.contrast.value);
          material.SetFloat(ShaderIDs.Gamma, 1.0f / volume.gamma.value);
          material.SetFloat(ShaderIDs.Hue, volume.hue.value);
          material.SetFloat(ShaderIDs.Saturation, volume.saturation.value);
        }
      }

      /// <inheritdoc/>
      public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
      {
        volume = VolumeManager.instance.stack.GetComponent<NTSCVolume>();
        if (material == null || volume == null || volume.IsActive() == false)
          return;

        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        if (resourceData.isActiveTargetBackBuffer == true)
          return;

        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        if (cameraData.camera.cameraType == CameraType.SceneView && volume.affectSceneView.value == false || cameraData.postProcessEnabled == false)
          return;

        TextureHandle source = resourceData.activeColorTexture;
        TextureDesc sourceDesc = source.GetDescriptor(renderGraph);

        renderTextureHandle0 = renderGraph.CreateTexture(sourceDesc);
        renderTextureHandle1 = renderGraph.CreateTexture(sourceDesc);
        renderTextureHandle2 = renderGraph.CreateTexture(source.GetDescriptor(renderGraph));

        UpdateMaterial();

        renderGraph.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(source, renderTextureHandle0, material, 0), $"{Constants.Asset.AssemblyName}.Pass0");
        renderGraph.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(renderTextureHandle0, renderTextureHandle1, material, 1), $"{Constants.Asset.AssemblyName}.Pass1");
        renderGraph.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(renderTextureHandle1, renderTextureHandle2, material, 2), $"{Constants.Asset.AssemblyName}.Pass2");

        resourceData.cameraColor = renderTextureHandle2;
      }
    }
  }
}
