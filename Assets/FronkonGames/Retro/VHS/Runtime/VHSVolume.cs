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
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace FronkonGames.Retro.VHS
{
  /// <summary> VHS Volume. </summary>
  [Serializable, VolumeComponentMenu("Fronkon Games/Retro/VHS")]
  public sealed class VHSVolume : VolumeComponent, IPostProcessComponent
  {
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Common settings.

    /// <summary> Controls the intensity of the effect [0, 1]. Default 1. </summary>
    /// <remarks> An effect with Intensity equal to 0 will not be executed. </remarks>
    [FloatSliderWithReset(1.0f, 0.0f, 1.0f, "Controls the intensity of the effect [0, 1]. Default 1.")]
    public FloatSliderParameterLinear intensity = new(1.0f, 0.0f, 1.0f);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region VHS settings.

    /// <summary> Shader quality. Default HighFidelity. </summary>
    [EnumDropdown((int)Quality.HighFidelity, "Shader quality. Default HighFidelity.")]
    public EnumParameterNoInterpolation<Quality> quality = new(Quality.HighFidelity);

    /// <summary> Number of samples used during calculations in YIQ color space [2, 10]. Default 6. </summary>
    /// <remarks> Only available in HighFidelity quality. </remarks>
    [IntSliderWithReset(6, 2, 10, "Number of samples used during calculations in YIQ color space [2, 10]. Default 6.")]
    public ClampedIntParameterNoInterpolation samples = new(6, 2, 10);

    /// <summary> Final image resolution. Default Quarter (1/4). </summary>
    [EnumDropdown((int)Resolution.Quarter, "Final image resolution. Default Quarter (1/4).")]
    public EnumParameterNoInterpolation<Resolution> resolution = new(Resolution.Quarter);

    /// <summary> YIQ color space (x: luma, y: in-phase, z: quadrature). </summary>
    [Vector3SliderWithReset(0.9f, 1.1f, 1.5f, 0.0f, 2.0f, "YIQ color space (x: luma, y: in-phase, z: quadrature).")]
    public Vector3ParameterNoInterpolation yiq = new(DefaultYIQ);

    /// <summary> Shadow tint color. </summary>
    [ColorWithReset(0xB200E5FF, "Shadow tint color.")]
    public ColorParameterNoInterpolation shadowTint = new(DefaultShadowTint);

    /// <summary> Color levels (white) [0, 1]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 1.0f, "Color levels (white) [0, 1]. Default 1.")]
    public FloatSliderParameterNoInterpolation whiteLevel = new(1.0f, 0.0f, 1.0f);

    /// <summary> Color levels (black) [0, 1]. Default 0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Color levels (black) [0, 1]. Default 0.")]
    public FloatSliderParameterNoInterpolation blackLevel = new(0.0f, 0.0f, 1.0f);

    /// <summary> Noise band that also deforms the color [0, 1]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 1.0f, "Noise band that also deforms the color [0, 1]. Default 1.")]
    public FloatSliderParameterNoInterpolation tapeCreaseStrength = new(1.0f, 0.0f, 1.0f);

    /// <summary> Number of bands [0, 50]. Default 8. </summary>
    [FloatSliderWithReset(8.0f, 0.0f, 50.0f, "Number of bands [0, 50]. Default 8.")]
    public FloatSliderParameterNoInterpolation tapeCreaseCount = new(8.0f, 0.0f, 50.0f);

    /// <summary> Band speed [-5, 5]. Default 1.2. </summary>
    [FloatSliderWithReset(1.2f, -5.0f, 5.0f, "Band speed [-5, 5]. Default 1.2.")]
    public FloatSliderParameterNoInterpolation tapeCreaseVelocity = new(1.2f, -5.0f, 5.0f);

    /// <summary> Band noise [0, 1]. Default 0.7. </summary>
    [FloatSliderWithReset(0.7f, 0.0f, 1.0f, "Band noise [0, 1]. Default 0.7.")]
    public FloatSliderParameterNoInterpolation tapeCreaseNoise = new(0.7f, 0.0f, 1.0f);

    /// <summary> Band color distortion [0, 1]. Default 0.2. </summary>
    /// <remarks> Only available in HighFidelity quality. </remarks>
    [FloatSliderWithReset(0.2f, 0.0f, 1.0f, "Band color distortion [0, 1]. Default 0.2.")]
    public FloatSliderParameterNoInterpolation tapeCreaseDistortion = new(0.2f, 0.0f, 1.0f);

    /// <summary> Tape noise [0, 1]. Default 0.1. </summary>
    [FloatSliderWithReset(0.1f, 0.0f, 1.0f, "Tape noise [0, 1]. Default 0.1.")]
    public FloatSliderParameterNoInterpolation colorNoise = new(0.1f, 0.0f, 1.0f);

    /// <summary> In VHS chroma band is a much lower resolution (technically 1/16th) [1, 64]. Default 16. </summary>
    [IntSliderWithReset(16, 1, 64, "In VHS chroma band is a much lower resolution (technically 1/16th) [1, 64]. Default 16.")]
    public ClampedIntParameterNoInterpolation chromaBand = new(16, 1, 64);

    /// <summary> In VHS the luma band is half of the resolution [1, 16]. Default 2. </summary>
    [IntSliderWithReset(2, 1, 16, "In VHS the luma band is half of the resolution [1, 16]. Default 2.")]
    public ClampedIntParameterNoInterpolation lumaBand = new(2, 1, 16);

    /// <summary> Tape distortion (high frequency) [0, 1]. Default 0.1. </summary>
    [FloatSliderWithReset(0.1f, 0.0f, 1.0f, "Tape distortion (high frequency) [0, 1]. Default 0.1.")]
    public FloatSliderParameterNoInterpolation tapeNoiseHigh = new(0.1f, 0.0f, 1.0f);

    /// <summary> Tape distortion (low frequency) [0, 1]. Default 0.1. </summary>
    [FloatSliderWithReset(0.1f, 0.0f, 1.0f, "Tape distortion (low frequency) [0, 1]. Default 0.1.")]
    public FloatSliderParameterNoInterpolation tapeNoiseLow = new(0.1f, 0.0f, 1.0f);

    /// <summary> Amount of AC interferrences [0, 1]. Default 0.1. </summary>
    [FloatSliderWithReset(0.1f, 0.0f, 1.0f, "Amount of AC interferrences [0, 1]. Default 0.1.")]
    public FloatSliderParameterNoInterpolation acBeatStrength = new(0.1f, 0.0f, 1.0f);

    /// <summary> AC interferrences density [0, 1]. Default 0.1. </summary>
    [FloatSliderWithReset(0.1f, 0.0f, 1.0f, "AC interferrences density [0, 1]. Default 0.1.")]
    public FloatSliderParameterNoInterpolation acBeatCount = new(0.1f, 0.0f, 1.0f);

    /// <summary> AC interferrences velocity [-1, 1]. Default 0.2. </summary>
    [FloatSliderWithReset(0.2f, -1.0f, 1.0f, "AC interferrences velocity [-1, 1]. Default 0.2.")]
    public FloatSliderParameterNoInterpolation acBeatVelocity = new(0.2f, -1.0f, 1.0f);

    /// <summary> 'Head-switching' noise height [0, 100]. Default 15. </summary>
    [FloatSliderWithReset(15.0f, 0.0f, 100.0f, "'Head-switching' noise height [0, 100]. Default 15.")]
    public FloatSliderParameterNoInterpolation bottomWarpHeight = new(15.0f, 0.0f, 100.0f);

    /// <summary> Distortion strength [-1, 1]. Default 0.1. </summary>
    [FloatSliderWithReset(0.1f, -1.0f, 1.0f, "Distortion strength [-1, 1]. Default 0.1.")]
    public FloatSliderParameterNoInterpolation bottomWarpDistortion = new(0.1f, -1.0f, 1.0f);

    /// <summary> Extra noise [0, 100]. Default 50. </summary>
    [FloatSliderWithReset(50.0f, 0.0f, 100.0f, "Extra noise [0, 100]. Default 50.")]
    public FloatSliderParameterNoInterpolation bottomWarpJitterExtent = new(50.0f, 0.0f, 100.0f);

    /// <summary> Vignette effect strength [0, 1]. Default 0.25. </summary>
    [FloatSliderWithReset(0.25f, 0.0f, 1.0f, "Vignette effect strength [0, 1]. Default 0.25.")]
    public FloatSliderParameterNoInterpolation vignette = new(0.25f, 0.0f, 1.0f);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Color settings.

    /// <summary> Brightness [-1, 1]. Default 0. </summary>
    [FloatSliderWithReset(0.0f, -1.0f, 1.0f, "Brightness [-1, 1]. Default 0.")]
    public FloatSliderParameterNoInterpolation brightness = new(0.0f, -1.0f, 1.0f);

    /// <summary> Contrast [0, 10]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 10.0f, "Contrast [0, 10]. Default 1.")]
    public FloatSliderParameterNoInterpolation contrast = new(1.0f, 0.0f, 10.0f);

    /// <summary> Gamma [0.1, 10]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.1f, 10.0f, "Gamma [0.1, 10]. Default 1.")]
    public FloatSliderParameterNoInterpolation gamma = new(1.0f, 0.1f, 10.0f);

    /// <summary> The color wheel [0, 1]. Default 0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "The color wheel [0, 1]. Default 0.")]
    public FloatSliderParameterNoInterpolation hue = new(0.0f, 0.0f, 1.0f);

    /// <summary> Intensity of a colors [0, 2]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 2.0f, "Intensity of a colors [0, 2]. Default 1.")]
    public FloatSliderParameterNoInterpolation saturation = new(1.0f, 0.0f, 2.0f);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Advanced settings.

    /// <summary> Does it affect the Scene View? </summary>
    [ToggleWithReset(false, "Does it affect the Scene View?")]
    public BoolParameterNoInterpolation affectSceneView = new(false);

    /// <summary> Use scaled time. </summary>
    [ToggleWithReset(true, "Use scaled time.")]
    public BoolParameterNoInterpolation useScaledTime = new(true);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public static readonly Vector3 DefaultYIQ = new(0.9f, 1.1f, 1.5f);
    public static readonly Color DefaultShadowTint = new(0.7f, 0.0f, 0.9f);

    /// <summary> Reset to default values. </summary>
    public void Reset()
    {
      intensity.value = 1.0f;

      quality.value = Quality.HighFidelity;
      samples.value = 6;
      resolution.value = Resolution.Quarter;
      yiq.value = DefaultYIQ;
      shadowTint.value = DefaultShadowTint;
      whiteLevel.value = 1.0f;
      blackLevel.value = 0.0f;
      tapeCreaseStrength.value = 1.0f;
      tapeCreaseCount.value = 8.0f;
      tapeCreaseVelocity.value = 1.2f;
      tapeCreaseNoise.value = 0.7f;
      tapeCreaseDistortion.value = 0.2f;
      colorNoise.value = 0.1f;
      chromaBand.value = 16;
      lumaBand.value = 2;
      tapeNoiseHigh.value = 0.1f;
      tapeNoiseLow.value = 0.1f;
      acBeatStrength.value = 0.1f;
      acBeatCount.value = 0.1f;
      acBeatVelocity.value = 0.2f;
      bottomWarpHeight.value = 15.0f;
      bottomWarpDistortion.value = 0.1f;
      bottomWarpJitterExtent.value = 50.0f;
      vignette.value = 0.25f;

      brightness.value = 0.0f;
      contrast.value = 1.0f;
      gamma.value = 1.0f;
      hue.value = 0.0f;
      saturation.value = 1.0f;

      affectSceneView.value = false;
      useScaledTime.value = true;
    }

    /// <summary> Is the effect active? </summary>
    public bool IsActive() => intensity.overrideState && intensity.value > 0.0f;

    /// <summary> Is the effect tile compatible? </summary>
    public bool IsTileCompatible() => false;
  }
}
