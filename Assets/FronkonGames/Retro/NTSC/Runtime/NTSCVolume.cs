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
using UnityEngine.Rendering;

namespace FronkonGames.Retro.NTSC
{
  /// <summary> NTSC Volume. </summary>
  [Serializable, VolumeComponentMenu("Fronkon Games/Retro/NTSC")]
  public sealed class NTSCVolume : VolumeComponent, IPostProcessComponent
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
    #region NTSC settings.

    /// <summary> Controls the size of the sinc filter window for NTSC passes. Larger values use more samples, affecting quality and performance. Range: [1, 64]. Default: 20. </summary>
    [IntSliderWithReset(20, 1, 64, "Controls the size of the sinc filter window for NTSC passes. Larger values use more samples, affecting quality and performance. Range: [1, 64]. Default: 20.")]
    public ClampedIntParameterNoInterpolation ntscWindowRadius = new(20, 1, 64);

    /// <summary> Wavelength of the AM carrier signal in the encoding pass. Range: [0.5, 10.0]. Default: 2.0f. </summary>
    [FloatSliderWithReset(2.0f, 0.5f, 10.0f, "Wavelength of the AM carrier signal in the encoding pass. Range: [0.5, 10.0]. Default: 2.0f.")]
    public FloatSliderParameterNoInterpolation amCarrierSignalWavelength = new(2.0f, 0.5f, 10.0f);

    /// <summary> Wavelength for low-pass filtering of luminance (Y) before encoding. Affects color fringing. Range: [0.1, 10.0]. Default: 1.0f. </summary>
    [FloatSliderWithReset(1.0f, 0.1f, 10.0f, "Wavelength for low-pass filtering of luminance (Y) before encoding. Affects color fringing. Range: [0.1, 10.0]. Default: 1.0f.")]
    public FloatSliderParameterNoInterpolation yLowPassWavelength = new(1.0f, 0.1f, 10.0f);

    /// <summary> Wavelength for low-pass filtering of the I chrominance component. Higher values increase color smearing. Range: [1.0, 20.0]. Default: 8.0f. </summary>
    [FloatSliderWithReset(8.0f, 1.0f, 20.0f, "Wavelength for low-pass filtering of the I chrominance component. Higher values increase color smearing. Range: [1.0, 20.0]. Default: 8.0f.")]
    public FloatSliderParameterNoInterpolation iLowPassWavelength = new(8.0f, 1.0f, 20.0f);

    /// <summary> Wavelength for low-pass filtering of the Q chrominance component. Higher values increase color smearing. Range: [1.0, 20.0]. Default: 11.0f. </summary>
    [FloatSliderWithReset(11.0f, 1.0f, 20.0f, "Wavelength for low-pass filtering of the Q chrominance component. Higher values increase color smearing. Range: [1.0, 20.0]. Default: 11.0f.")]
    public FloatSliderParameterNoInterpolation qLowPassWavelength = new(11.0f, 1.0f, 20.0f);

    /// <summary> Wavelength of the color signal for the NTSC encoder pass. Range: [1.0, 10.0]. Default: 3.0f. </summary>
    [FloatSliderWithReset(3.0f, 1.0f, 10.0f, "Wavelength of the color signal for the NTSC encoder pass. Range: [1.0, 10.0]. Default: 3.0f.")]
    public FloatSliderParameterNoInterpolation colorburstWavelengthEncoder = new(3.0f, 1.0f, 10.0f);

    /// <summary> Overall scaling factor for NTSC artifacts related to pixel width. Range: [0.1, 5.0]. Default: 1.0f. </summary>
    [FloatSliderWithReset(1.0f, 0.1f, 5.0f, "Overall scaling factor for NTSC artifacts related to pixel width. Range: [0.1, 5.0]. Default: 1.0f.")]
    public FloatSliderParameterNoInterpolation ntscScale = new(1.0f, 0.1f, 5.0f);

    /// <summary> Simulates phase alteration per scanline. 0.0 for off, ~3.1415927 (PI) for on. Range: [0.0, 3.1415927]. Default: 0.0f. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 3.1415927f, "Simulates phase alteration per scanline. 0.0 for off, ~3.1415927 (PI) for on. Range: [0.0, 3.1415927]. Default: 0.0f.")]
    public FloatSliderParameterNoInterpolation ntscPhaseAlternation = new(0.0f, 0.0f, 3.1415927f);

    /// <summary> Controls the amount of TV static/noise. Range: [0.0, 1.0]. Default: 0.1f. </summary>
    [FloatSliderWithReset(0.1f, 0.0f, 1.0f, "Controls the amount of TV static/noise. Range: [0.0, 1.0]. Default: 0.1f.")]
    public FloatSliderParameterNoInterpolation ntscNoiseStrength = new(0.1f, 0.0f, 1.0f);

    /// <summary> Offsets the sinc window shape, can make artifacts smear to one side or the other. Range: [-1.0, 1.0]. Default: 0.0f. </summary>
    [FloatSliderWithReset(0.0f, -1.0f, 1.0f, "Offsets the sinc window shape, can make artifacts smear to one side or the other. Range: [-1.0, 1.0]. Default: 0.0f.")]
    public FloatSliderParameterNoInterpolation ntscWindowBias = new(0.0f, -1.0f, 1.0f);

    /// <summary> Wavelength for AM signal demodulation in the second NTSC pass. Range: [0.5, 10.0]. Default: 2.0f. </summary>
    [FloatSliderWithReset(2.0f, 0.5f, 10.0f, "Wavelength for AM signal demodulation in the second NTSC pass. Range: [0.5, 10.0]. Default: 2.0f.")]
    public FloatSliderParameterNoInterpolation amDemodulateWavelength = new(2.0f, 0.5f, 10.0f);

    /// <summary> Wavelength for high-pass filtering in AM decoding. Range: [0.5, 10.0]. Default: 2.0f. </summary>
    [FloatSliderWithReset(2.0f, 0.5f, 10.0f, "Wavelength for high-pass filtering in AM decoding. Range: [0.5, 10.0]. Default: 2.0f.")]
    public FloatSliderParameterNoInterpolation amDecodeHighPassWavelength = new(2.0f, 0.5f, 10.0f);

    /// <summary> Wavelength of the color signal for the NTSC decoder pass. Range: [1.0, 10.0]. Default: 3.0f. </summary>
    [FloatSliderWithReset(3.0f, 1.0f, 10.0f, "Wavelength of the color signal for the NTSC decoder pass. Range: [1.0, 10.0]. Default: 3.0f.")]
    public FloatSliderParameterNoInterpolation colorburstWavelengthDecoder = new(3.0f, 1.0f, 10.0f);

    /// <summary> Wavelength for low-pass filtering during final NTSC decode. Affects blurriness and striping. Range: [1.0, 10.0]. Default: 4.5f. </summary>
    [FloatSliderWithReset(4.5f, 1.0f, 10.0f, "Wavelength for low-pass filtering during final NTSC decode. Affects blurriness and striping. Range: [1.0, 10.0]. Default: 4.5f.")]
    public FloatSliderParameterNoInterpolation decodeLowPassWavelength = new(4.5f, 1.0f, 10.0f);

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

    /// <summary> Reset to default values. </summary>
    public void Reset()
    {
      intensity.value = 1.0f;

      ntscWindowRadius.value = 20;
      amCarrierSignalWavelength.value = 2.0f;
      yLowPassWavelength.value = 1.0f;
      iLowPassWavelength.value = 8.0f;
      qLowPassWavelength.value = 11.0f;
      colorburstWavelengthEncoder.value = 3.0f;
      ntscScale.value = 1.0f;
      ntscPhaseAlternation.value = 0.0f;
      ntscNoiseStrength.value = 0.1f;
      ntscWindowBias.value = 0.0f;
      amDemodulateWavelength.value = 2.0f;
      amDecodeHighPassWavelength.value = 2.0f;
      colorburstWavelengthDecoder.value = 3.0f;
      decodeLowPassWavelength.value = 4.5f;

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
