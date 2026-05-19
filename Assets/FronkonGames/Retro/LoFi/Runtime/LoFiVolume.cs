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

namespace FronkonGames.Retro.LoFi
{
  /// <summary> Lo-Fi profile parameter. </summary>
  [Serializable]
  public sealed class LoFiProfileParameter : VolumeParameter<LoFiProfile>
  {
    public LoFiProfileParameter(LoFiProfile value, bool overrideState = false) : base(value, overrideState) { }
  }

  /// <summary> Lo-Fi Volume. </summary>
  [Serializable, VolumeComponentMenu("Fronkon Games/Retro/Lo-Fi"), HelpURL(Constants.Support.Documentation)]
  public sealed class LoFiVolume : VolumeComponent, IPostProcessComponent
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
    #region LoFi settings.

    /// <summary> Enables color palette conversion for authentic retro color schemes. </summary>
    [ToggleWithReset(true, "Enables color palette conversion for authentic retro color schemes.")]
    public BoolParameterNoInterpolation palette = new(true);

    /// <summary> Lo-Fi color palette profile containing the color set and configuration. </summary>
    public LoFiProfileParameter profile = new(null);

    /// <summary> Determines how colors are interpolated in the palette - from sharp transitions to smooth gradients. Default BlendModes.Blend. </summary>
    [EnumDropdown((int)BlendModes.Blend, "Determines how colors are interpolated in the palette - from sharp transitions to smooth gradients. Default BlendModes.Blend.")]
    public EnumParameterNoInterpolation<BlendModes> mode = new(BlendModes.Blend);

    /// <summary> Method used to sample color from the palette. </summary>
    [EnumDropdown((int)SampleMethod.Distance, "Method used to sample color from the palette.")]
    public EnumParameterNoInterpolation<SampleMethod> sampleMethod = new(SampleMethod.Distance);

    /// <summary> Resolution of the internal palette texture. Higher resolutions reduce color banding but use more memory. Default 16. </summary>
    [EnumDropdown((int)PaletteResolutions._16, "Resolution of the internal palette texture. Higher resolutions reduce color banding but use more memory. Default PaletteResolutions._16.")]
    public EnumParameterNoInterpolation<PaletteResolutions> resolution = new(PaletteResolutions._16);

    /// <summary> Maximum distance between colors [0, 1]. Small values result in colors more similar to the palette. Default 0.5. </summary>
    [FloatSliderWithReset(0.5f, 0.0f, 1.0f, "Maximum distance between colors [0, 1]. Small values result in colors more similar to the palette. Default 0.5.")]
    public FloatSliderParameterNoInterpolation colorThreshold = new(0.5f, 0.0f, 1.0f);

    /// <summary> Controls contrast between areas of different brightness [0, 2]. Higher values create more pronounced color shifts. Default 1.5. </summary>
    [FloatSliderWithReset(1.5f, 0.0f, 2.0f, "Controls contrast between areas of different brightness [0, 2]. Higher values create more pronounced color shifts. Default 1.5.")]
    public FloatSliderParameterNoInterpolation luminancePow = new(1.5f, 0.0f, 2.0f);

    /// <summary> Minimum luminance value for color remapping [0, 1]. Controls the darkest parts of the image. Default 0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Minimum luminance value for color remapping [0, 1]. Controls the darkest parts of the image. Default 0.")]
    public FloatSliderParameterNoInterpolation rangeMin = new(0.0f, 0.0f, 1.0f);

    /// <summary> Maximum luminance value for color remapping [0, 1]. Controls the brightest parts of the image. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 1.0f, "Maximum luminance value for color remapping [0, 1]. Controls the brightest parts of the image. Default 1.")]
    public FloatSliderParameterNoInterpolation rangeMax = new(1.0f, 0.0f, 1.0f);

    /// <summary> Inverts the color palette order for negative/film-like effects. Default false. </summary>
    [ToggleWithReset(false, "Inverts the color palette order for negative/film-like effects. Default false.")]
    public BoolParameterNoInterpolation invert = new(false);

    /// <summary> Enables pixelation for that classic low-resolution look. </summary>
    [ToggleWithReset(true, "Enables pixelation for that classic low-resolution look.")]
    public BoolParameterNoInterpolation pixelate = new(true);

    /// <summary> Controls the size of each pixel block [2 and up]. Larger values create a more chunky, retro appearance. Even values only. Default 12. </summary>
    [IntSliderWithReset(12, 2, 64, "Controls the size of each pixel block [2 and up]. Larger values create a more chunky, retro appearance. Even values only. Default 12.")]
    public ClampedIntParameterNoInterpolation pixelSize = new(12, 2, 64);

    /// <summary> Applies edge detection with Sobel filter [0, 2]. Creates a pseudo-3D effect on pixel boundaries. Default 0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 2.0f, "Applies edge detection with Sobel filter [0, 2]. Creates a pseudo-3D effect on pixel boundaries. Default 0.")]
    public FloatSliderParameterNoInterpolation pixelSobel = new(0.0f, 0.0f, 2.0f);

    /// <summary> Controls the intensity of the Sobel edge effect [0.01, 10]. Higher values create more pronounced edges. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.01f, 10.0f, "Controls the intensity of the Sobel edge effect [0.01, 10]. Higher values create more pronounced edges. Default 1.")]
    public FloatSliderParameterNoInterpolation pixelSobelPower = new(1.0f, 0.01f, 10.0f);

    /// <summary> Direction of the light source for Sobel effect [0, 360]. Affects shadow direction. Default 45. </summary>
    [FloatSliderWithReset(45.0f, 0.0f, 360.0f, "Direction of the light source for Sobel effect [0, 360]. Affects shadow direction. Default 45.")]
    public FloatSliderParameterNoInterpolation pixelSobelAngle = new(45.0f, 0.0f, 360.0f);

    /// <summary> Brightness of the light source for Sobel effect [0, 2]. Controls contrast between lit and shadowed areas. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 2.0f, "Brightness of the light source for Sobel effect [0, 2]. Controls contrast between lit and shadowed areas. Default 1.")]
    public FloatSliderParameterNoInterpolation pixelSobelLightIntensity = new(1.0f, 0.0f, 2.0f);

    /// <summary> Base lighting level for Sobel effect [0, 0.5]. Prevents shadows from becoming too dark. Default 0.2. </summary>
    [FloatSliderWithReset(0.2f, 0.0f, 0.5f, "Base lighting level for Sobel effect [0, 0.5]. Prevents shadows from becoming too dark. Default 0.2.")]
    public FloatSliderParameterNoInterpolation pixelSobelAmbient = new(0.2f, 0.0f, 0.5f);

    /// <summary> Controls pixel corner rounding [0, 1]. At 0, pixels are square; at 1, pixels become circular. Default 0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Controls pixel corner rounding [0, 1]. At 0, pixels are square; at 1, pixels become circular. Default 0.")]
    public FloatSliderParameterNoInterpolation pixelRound = new(0.0f, 0.0f, 1.0f);

    /// <summary> Number of samples per pixel for anti-aliasing [1, 8]. Higher values reduce jagged edges but impact performance. Default 2. </summary>
    [IntSliderWithReset(2, 1, 8, "Number of samples per pixel for anti-aliasing [1, 8]. Higher values reduce jagged edges but impact performance. Default 2.")]
    public ClampedIntParameterNoInterpolation pixelSamples = new(2, 1, 8);

    /// <summary> Blending method between pixelated and original image. Affects how colors are combined. Default ColorBlends.Solid. </summary>
    [EnumDropdown((int)ColorBlends.Solid, "Blending method between pixelated and original image. Affects how colors are combined. Default ColorBlends.Solid.")]
    public EnumParameterNoInterpolation<ColorBlends> pixelBlend = new(ColorBlends.Solid);

    /// <summary> Color tint applied to pixels. Useful for creating monochrome or duotone effects. Default white (no tinting). </summary>
    [ColorWithReset(0xFFFFFF, "Color tint applied to pixels. Useful for creating monochrome or duotone effects. Default white (no tinting).")]
    public ColorParameterNoInterpolation pixelTint = new(Color.white);

    /// <summary> Creates a 3D-like bevel effect on pixel edges [0, 1]. Adds depth to the pixelated image. Default 0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Creates a 3D-like bevel effect on pixel edges [0, 1]. Adds depth to the pixelated image. Default 0.")]
    public FloatSliderParameterNoInterpolation pixelBevel = new(0.0f, 0.0f, 1.0f);

    /// <summary> Enables color quantization to reduce the color depth. Creates that limited color palette look. </summary>
    [ToggleWithReset(false, "Enables color quantization to reduce the color depth. Creates that limited color palette look.")]
    public BoolParameterNoInterpolation quantization = new(false);

    /// <summary> Maximum number of colors in the quantized image [2, 64]. Lower values create more pronounced banding. Default 32. </summary>
    [IntSliderWithReset(32, 2, 64, "Maximum number of colors in the quantized image [2, 64]. Lower values create more pronounced banding. Default 32.")]
    public ClampedIntParameterNoInterpolation colors = new(32, 2, 64);

    /// <summary> Darkens the screen edges to simulate CRT monitor light falloff [0, 1]. Higher values create more pronounced darkening. Default 0.3. </summary>
    [FloatSliderWithReset(0.3f, 0.0f, 1.0f, "Darkens the screen edges to simulate CRT monitor light falloff [0, 1]. Higher values create more pronounced darkening. Default 0.3.")]
    public FloatSliderParameterNoInterpolation vignette = new(0.3f, 0.0f, 1.0f);

    /// <summary> Horizontal scanline intensity [0, 1]. Creates those classic CRT monitor lines. Default 0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Horizontal scanline intensity [0, 1]. Creates those classic CRT monitor lines. Default 0.")]
    public FloatSliderParameterNoInterpolation scanline = new(0.0f, 0.0f, 1.0f);

    /// <summary> Number of scanlines across the screen [0, ...]. Higher values create finer lines. Default 500. </summary>
    [IntSliderWithReset(500, 0, 2000, "Number of scanlines across the screen [0, ...]. Higher values create finer lines. Default 500.")]
    public ClampedIntParameterNoInterpolation scanlineCount = new(500, 0, 2000);

    /// <summary> Animation speed of scanlines [-25, 25]. Negative values move upward, positive move downward. Default 10. </summary>
    [FloatSliderWithReset(10.0f, -25.0f, 25.0f, "Animation speed of scanlines [-25, 25]. Negative values move upward, positive move downward. Default 10.")]
    public FloatSliderParameterNoInterpolation scanlineSpeed = new(10.0f, -25.0f, 25.0f);

    /// <summary> Color fringing effect [0, 2]. Simulates lens imperfections or poor composite video. Default 0.5. </summary>
    [FloatSliderWithReset(0.5f, 0.0f, 2.0f, "Color fringing effect [0, 2]. Simulates lens imperfections or poor composite video. Default 0.5.")]
    public FloatSliderParameterNoInterpolation chromaticAberration = new(0.5f, 0.0f, 2.0f);

    /// <summary> Adds a reflective highlight to simulate glass screen [0, 1]. Creates that authentic CRT screen glare. Default 0.1. </summary>
    [FloatSliderWithReset(0.1f, 0.0f, 1.0f, "Adds a reflective highlight to simulate glass screen [0, 1]. Creates that authentic CRT screen glare. Default 0.1.")]
    public FloatSliderParameterNoInterpolation shine = new(0.1f, 0.0f, 1.0f);

    /// <summary> Controls the size of the glass reflection effect [0, 1]. Larger values create a more diffuse reflection. Default 0.4. </summary>
    [FloatSliderWithReset(0.4f, 0.0f, 1.0f, "Controls the size of the glass reflection effect [0, 1]. Larger values create a more diffuse reflection. Default 0.4.")]
    public FloatSliderParameterNoInterpolation shineSize = new(0.4f, 0.0f, 1.0f);

    /// <summary> Controls screen aperture/mask [0, 1]. Lower values create a more pronounced frame around the image. Default 0.95. </summary>
    [FloatSliderWithReset(0.95f, 0.0f, 1.0f, "Controls screen aperture/mask [0, 1]. Lower values create a more pronounced frame around the image. Default 0.95.")]
    public FloatSliderParameterNoInterpolation aperture = new(0.95f, 0.0f, 1.0f);

    /// <summary> Screen curvature amount [0, ...]. Simulates the curved surface of CRT displays. Default 0.5. </summary>
    [FloatSliderWithReset(0.5f, 0.0f, 1.0f, "Screen curvature amount [0, ...]. Simulates the curved surface of CRT displays. Default 0.5.")]
    public FloatSliderParameterNoInterpolation curvature = new(0.5f, 0.0f, 1.0f);

    /// <summary> Enables a decorative border around the screen to simulate a monitor bezel. </summary>
    [ToggleWithReset(true, "Enables a decorative border around the screen to simulate a monitor bezel.")]
    public BoolParameterNoInterpolation border = new(true);

    /// <summary> Color of the monitor border/bezel. Default is a vintage beige color. </summary>
    [ColorWithReset(0x4C4026, "Color of the monitor border/bezel. Default is a vintage beige color.")]
    public ColorParameterNoInterpolation borderColor = new(DefaultBorderColor);

    /// <summary> Controls the softness of the border edges [0, 1]. Higher values create a more gradual transition. Default 0.5. </summary>
    [FloatSliderWithReset(0.5f, 0.0f, 1.0f, "Controls the softness of the border edges [0, 1]. Higher values create a more gradual transition. Default 0.5.")]
    public FloatSliderParameterNoInterpolation borderSmooth = new(0.5f, 0.0f, 1.0f);

    /// <summary> Texture of the border material [0, 1]. Default 0.2. </summary>
    [FloatSliderWithReset(0.2f, 0.0f, 1.0f, "Texture of the border material [0, 1]. Default 0.2.")]
    public FloatSliderParameterNoInterpolation borderNoise = new(0.2f, 0.0f, 1.0f);

    /// <summary> Border margins. </summary>
    [Vector2WithReset(1.0f, 1.0f, "Border margins.")]
    public Vector2ParameterNoInterpolation borderMargins = new(Vector2.one);

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
    public BoolParameterNoInterpolation useScaledTime = new(true);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public static readonly Color DefaultBorderColor = new(0.30f, 0.25f, 0.15f);

    /// <summary> Reset to default values. </summary>
    public void Reset()
    {
      intensity.value = 1.0f;

      palette.value = true;
      mode.value = BlendModes.Blend;
      sampleMethod.value = SampleMethod.Distance;
      resolution.value = PaletteResolutions._16;
      colorThreshold.value = 0.5f;
      luminancePow.value = 1.5f;
      rangeMin.value = 0.0f;
      rangeMax.value = 1.0f;
      invert.value = false;

      pixelate.value = true;
      pixelSamples.value = 2;
      pixelSize.value = 12;
      pixelRound.value = 0.0f;
      pixelBlend.value = ColorBlends.Solid;
      pixelTint.value = Color.white;
      pixelBevel.value = 0.0f;
      pixelSobel.value = 0.0f;
      pixelSobelPower.value = 1.0f;
      pixelSobelAngle.value = 45.0f;
      pixelSobelLightIntensity.value = 1.0f;
      pixelSobelAmbient.value = 0.2f;

      scanline.value = 0.0f;
      scanlineCount.value = 500;
      scanlineSpeed.value = 10.0f;

      quantization.value = false;
      colors.value = 32;
      vignette.value = 0.3f;

      chromaticAberration.value = 0.5f;

      shine.value = 0.1f;
      shineSize.value = 0.4f;

      aperture.value = 0.95f;
      curvature.value = 0.5f;

      border.value = true;
      borderColor.value = new Color(0.30f, 0.25f, 0.15f);
      borderSmooth.value = 0.5f;
      borderNoise.value = 0.2f;
      borderMargins.value = Vector2.one;

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