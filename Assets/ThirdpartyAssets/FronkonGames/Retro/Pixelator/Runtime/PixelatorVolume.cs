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

namespace FronkonGames.Retro.Pixelator
{
  /// <summary> Pixelator Volume. </summary>
  [Serializable, VolumeComponentMenu("Fronkon Games/Retro/Pixelator"), HelpURL(Constants.Support.Documentation)]
  public sealed class PixelatorVolume : VolumeComponent, IPostProcessComponent
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
    #region Pixelator settings.

    /// <summary> The mode of the pixelation. Default Quad. </summary>
    [EnumDropdown((int)Pixelator.PixelationModes.Rectangle, "The mode of the pixelation. Default Quad.")]
    public EnumParameterNoInterpolation<Pixelator.PixelationModes> pixelationMode = new(Pixelator.PixelationModes.Rectangle);

    /// <summary> The size of the pixels [0, 1]. Default 0.75. </summary>
    [FloatSliderWithReset(0.75f, 0.0f, 1.0f, "The size of the pixels [0, 1]. Default 0.75.")]
    public ClampedFloatParameterNoInterpolation pixelSize = new(0.75f, 0.0f, 1.0f);

    /// <summary> Use the screen aspect ratio to calculate the pixel scale. Default true. </summary>
    [ToggleWithReset(true, "Use the screen aspect ratio to calculate the pixel scale. Default true.")]
    public BoolParameterNoInterpolation screenAspectRatio = new(true);

    /// <summary> Custom aspect ratio [0.2, 5.0]. Default 1. </summary>
    /// <remarks> Only used if screenAspectRatio is false. </remarks>
    [FloatSliderWithReset(1.0f, 0.2f, 5.0f, "Custom aspect ratio [0.2, 5.0]. Default 1.")]
    public ClampedFloatParameterNoInterpolation aspectRatio = new(1.0f, 0.2f, 5.0f);

    /// <summary> The scale of the pixels. Default (1, 1). </summary>
    [Vector2WithReset(1.0f, 1.0f, "The scale of the pixels. Default (1, 1).")]
    public Vector2ParameterNoInterpolation pixelScale = new(Vector2.one);

    /// <summary> The radius of the circle [0, 1]. Default 0.5. </summary>
    /// <remarks> Only used if pixelationMode is Circle. </remarks>
    [FloatSliderWithReset(0.5f, 0.0f, 1.0f, "The radius of the circle [0, 1]. Default 0.5.")]
    public ClampedFloatParameterNoInterpolation radius = new(0.5f, 0.0f, 1.0f);

    /// <summary> The background color. Default Black. </summary>
    [ColorWithReset(0x000000FF, "The background color. Default Black.")]
    public ColorParameterNoInterpolation background = new(Color.black);

    /// <summary> The number of threads [1, 8]. Default 3. </summary>
    /// <remarks> Only used if pixelationMode is Knitted. </remarks>
    [IntSliderWithReset(3, 1, 8, "The number of threads [1, 8]. Default 3.")]
    public ClampedIntParameterNoInterpolation threads = new(3, 1, 8);

    /// <summary> The chromatic aberration intensity [0, 10]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 10.0f, "The chromatic aberration intensity [0, 10]. Default 1.")]
    public ClampedFloatParameterNoInterpolation chromaticAberrationIntensity = new(1.0f, 0.0f, 10.0f);

    /// <summary> The chromatic aberration offset. Default (1.0, 2.0, -1.0). </summary>
    [Vector3WithReset(1.0f, 2.0f, -1.0f, "The chromatic aberration offset. Default (1.0, 2.0, -1.0).")]
    public Vector3ParameterNoInterpolation chromaticAberrationOffset = new(new Vector3(1.0f, 2.0f, -1.0f));

    /// <summary> The gradient intensity [0, 1]. Default 0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "The gradient intensity [0, 1]. Default 0.")]
    public ClampedFloatParameterNoInterpolation gradientIntensity = new(0.0f, 0.0f, 1.0f);

    /// <summary> Color gradient. </summary>
    public GradientParameterNoInterpolation gradient = new(new Gradient()
    {
      colorKeys = new GradientColorKey[]
      {
        new(Color.white * 0.0f, 0.0f),
        new(Color.white * 0.33f, 0.2f),
        new(Color.white * 0.66f, 0.5f),
        new(Color.white * 1.0f, 1.0f)
      }
    });

    /// <summary> Minimum luminance value that is taken into account in the color mode Gradient [0, 1]. Default 0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Minimum luminance value that is taken into account in the color mode Gradient [0, 1]. Default 0.")]
    public ClampedFloatParameterNoInterpolation luminanceMin = new(0.0f, 0.0f, 1.0f);

    /// <summary> Maximum luminance value that is taken into account in the color mode Gradient [0, 1]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 1.0f, "Maximum luminance value that is taken into account in the color mode Gradient [0, 1]. Default 1.")]
    public ClampedFloatParameterNoInterpolation luminanceMax = new(1.0f, 0.0f, 1.0f);

    /// <summary> How the gradient is mapped. Default CIELAB. </summary>
    [EnumDropdown((int)Pixelator.GradientMappingMode.CIELAB, "How the gradient is mapped. Default CIELAB.")]
    public EnumParameterNoInterpolation<Pixelator.GradientMappingMode> gradientMappingMode = new(Pixelator.GradientMappingMode.CIELAB);

    /// <summary> Number of samples along the gradient to check when using CIELAB mapping mode [2, 64]. Default 16. </summary>
    [IntSliderWithReset(16, 2, 64, "Number of samples along the gradient to check when using CIELAB mapping mode [2, 64]. Default 16.")]
    public ClampedIntParameterNoInterpolation gradientCIELabSamples = new(16, 2, 64);

    /// <summary> Apply original luminance to the final color. </summary>
    [ToggleWithReset(true, "Apply original luminance to the final color.")]
    public BoolParameterNoInterpolation gradientApplyLuminance = new(true);

    /// <summary> Strength of the bevel effect [0, 10]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 10.0f, "Strength of the bevel effect [0, 10]. Default 1.")]
    public ClampedFloatParameterNoInterpolation bevel = new(1.0f, 0.0f, 10.0f);

    /// <summary> Dither intensity [0, 1]. Default 0.5. </summary>
    [FloatSliderWithReset(0.5f, 0.0f, 1.0f, "Dither intensity [0, 1]. Default 0.5.")]
    public ClampedFloatParameterNoInterpolation ditherIntensity = new(0.5f, 0.0f, 1.0f);

    /// <summary> Scale of the dither pattern (2 for 2x2, 4 for 4x4, 8 for 8x8). Default 4. </summary>
    [IntSliderWithReset(4, 2, 8, "Scale of the dither pattern (2 for 2x2, 4 for 4x4, 8 for 8x8). Default 4.")]
    public ClampedIntParameterNoInterpolation ditherPatternScale = new(4, 2, 8);

    /// <summary> Threshold scale for dithering [0, 1]. Adjusts dither pattern influence. Default 0.75. </summary>
    [FloatSliderWithReset(0.75f, 0.0f, 1.0f, "Threshold scale for dithering [0, 1]. Adjusts dither pattern influence. Default 0.75.")]
    public ClampedFloatParameterNoInterpolation ditherThresholdScale = new(0.75f, 0.0f, 1.0f);

    /// <summary> Number of color steps for dithering [2, 16]. Default 8. </summary>
    [IntSliderWithReset(8, 2, 16, "Number of color steps for dithering [2, 16]. Default 8.")]
    public ClampedIntParameterNoInterpolation ditherColorSteps = new(8, 2, 16);

    /// <summary> Overall intensity of the posterize effect [0, 1]. Default 0.0. An intensity of 0 effectively disables posterization. </summary>
    [FloatSliderWithReset(0.5f, 0.0f, 1.0f, "Overall intensity of the posterize effect [0, 1]. Default 0.0.")]
    public ClampedFloatParameterNoInterpolation posterizeIntensity = new(0.5f, 0.0f, 1.0f);

    /// <summary> Number of color steps per channel for RGB mode [2, 256]. Default (8,8,8). </summary>
    [Vector3IntWithReset(24, 24, 24, "Number of color steps per channel for RGB mode [2, 256]. Default (8,8,8).")]
    public ClampedVector3IntParameterNoInterpolation posterizeRGBSteps = new(new Vector3Int(24, 24, 24), 2, 256);

    /// <summary> Number of color steps for Luminance mode [2, 256]. Default 24. </summary>
    [IntSliderWithReset(24, 2, 256, "Number of color steps for Luminance mode [2, 256]. Default 24.")]
    public ClampedIntParameterNoInterpolation posterizeLuminanceSteps = new(24, 2, 256);

    /// <summary> Number of color steps per channel for HSV mode [H:2-64, S:2-32, V:2-32]. Default (8,8,8). </summary>
    [Vector3IntWithReset(24, 24, 24, "Number of color steps per channel for HSV mode [H:2-64, S:2-32, V:2-32]. Default (8,8,8).")]
    public ClampedVector3IntParameterNoInterpolation posterizeHSVSteps = new(new Vector3Int(24, 24, 24), 2, 256);

    /// <summary> Gamma value for posterization. Default 1.0 (no correction). </summary>
    [FloatSliderWithReset(1.0f, 0.1f, 10.0f, "Gamma value for posterization. Default 1.0 (no correction).")]
    public ClampedFloatParameterNoInterpolation posterizeGamma = new(1.0f, 0.1f, 10.0f);

    /// <summary> Global intensity for color filters [0, 1]. Default 0.0 (off). </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Global intensity for color filters [0, 1]. Default 0.0 (off).")]
    public ClampedFloatParameterNoInterpolation filtersIntensity = new(0.0f, 0.0f, 1.0f);

    /// <summary> Intensity for Sepia filter [0, 1]. Default 0.0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Intensity for Sepia filter [0, 1]. Default 0.0.")]
    public ClampedFloatParameterNoInterpolation sepiaIntensity = new(0.0f, 0.0f, 1.0f);

    /// <summary> Intensity for Cool Blue filter [0, 1]. Default 0.0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Intensity for Cool Blue filter [0, 1]. Default 0.0.")]
    public ClampedFloatParameterNoInterpolation coolBlueIntensity = new(0.0f, 0.0f, 1.0f);

    /// <summary> Intensity for Warm filter [0, 1]. Default 0.0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Intensity for Warm filter [0, 1]. Default 0.0.")]
    public ClampedFloatParameterNoInterpolation warmFilterIntensity = new(0.0f, 0.0f, 1.0f);

    /// <summary> Intensity for Invert Color filter [0, 1]. Default 0.0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Intensity for Invert Color filter [0, 1]. Default 0.0.")]
    public ClampedFloatParameterNoInterpolation invertColorIntensity = new(0.0f, 0.0f, 1.0f);

    /// <summary> Intensity for Hudson filter [0, 1]. Default 0.0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Intensity for Hudson filter [0, 1]. Default 0.0.")]
    public ClampedFloatParameterNoInterpolation hudsonIntensity = new(0.0f, 0.0f, 1.0f);

    /// <summary> Intensity for Hefe filter [0, 1]. Default 0.0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Intensity for Hefe filter [0, 1]. Default 0.0.")]
    public ClampedFloatParameterNoInterpolation hefeIntensity = new(0.0f, 0.0f, 1.0f);

    /// <summary> Intensity for X-Pro filter [0, 1]. Default 0.0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Intensity for X-Pro filter [0, 1]. Default 0.0.")]
    public ClampedFloatParameterNoInterpolation xproIntensity = new(0.0f, 0.0f, 1.0f);

    /// <summary> Intensity for Rise filter [0, 1]. Default 0.0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Intensity for Rise filter [0, 1]. Default 0.0.")]
    public ClampedFloatParameterNoInterpolation riseIntensity = new(0.0f, 0.0f, 1.0f);

    /// <summary> Intensity for Toaster filter [0, 1]. Default 0.0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Intensity for Toaster filter [0, 1]. Default 0.0.")]
    public ClampedFloatParameterNoInterpolation toasterIntensity = new(0.0f, 0.0f, 1.0f);

    /// <summary> Intensity for Infrared (IR) filter [0, 1]. Default 0.0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Intensity for Infrared (IR) filter [0, 1]. Default 0.0.")]
    public ClampedFloatParameterNoInterpolation irFilterIntensity = new(0.0f, 0.0f, 1.0f);

    /// <summary> Intensity for Thermal filter [0, 1]. Default 0.0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Intensity for Thermal filter [0, 1]. Default 0.0.")]
    public ClampedFloatParameterNoInterpolation thermalFilterIntensity = new(0.0f, 0.0f, 1.0f);

    /// <summary> Intensity for Duotone filter [0, 1]. Default 0.0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Intensity for Duotone filter [0, 1]. Default 0.0.")]
    public ClampedFloatParameterNoInterpolation duotoneIntensity = new(0.0f, 0.0f, 1.0f);

    /// <summary> First color for Duotone filter. Default Dark Blue. </summary>
    [ColorWithReset(0x1A3380FF, "First color for Duotone filter. Default Dark Blue.")]
    public ColorParameterNoInterpolation duotoneColorA = new(new Color(0.1f, 0.2f, 0.5f, 1.0f));

    /// <summary> Second color for Duotone filter. Default Bright Yellow. </summary>
    [ColorWithReset(0xE6E633FF, "Second color for Duotone filter. Default Bright Yellow.")]
    public ColorParameterNoInterpolation duotoneColorB = new(new Color(0.9f, 0.9f, 0.2f, 1.0f));

    /// <summary> Intensity for Night Vision filter [0, 1]. Default 0.0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Intensity for Night Vision filter [0, 1]. Default 0.0.")]
    public ClampedFloatParameterNoInterpolation nightVisionIntensity = new(0.0f, 0.0f, 1.0f);

    /// <summary> Intensity for Pop Art filter [0, 1]. Default 0.0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Intensity for Pop Art filter [0, 1]. Default 0.0.")]
    public ClampedFloatParameterNoInterpolation popArtIntensity = new(0.0f, 0.0f, 1.0f);

    /// <summary> Intensity for Blueprint filter [0, 1]. Default 0.0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Intensity for Blueprint filter [0, 1]. Default 0.0.")]
    public ClampedFloatParameterNoInterpolation blueprintIntensity = new(0.0f, 0.0f, 1.0f);

    /// <summary> Edge color for Blueprint filter. Default Light Blue. </summary>
    [ColorWithReset(0x99CCFFFF, "Edge color for Blueprint filter. Default Light Blue.")]
    public ColorParameterNoInterpolation blueprintEdgeColor = new(new Color(0.6f, 0.8f, 1.0f, 1.0f));

    /// <summary> Background color for Blueprint filter. Default Dark Blue. </summary>
    [ColorWithReset(0x1A3366FF, "Background color for Blueprint filter. Default Dark Blue.")]
    public ColorParameterNoInterpolation blueprintBackgroundColor = new(new Color(0.1f, 0.2f, 0.4f, 1.0f));

    /// <summary> Edge detection threshold for Blueprint filter [0.05, 0.5]. Default 0.1. </summary>
    [FloatSliderWithReset(0.1f, 0.05f, 0.5f, "Edge detection threshold for Blueprint filter [0.05, 0.5]. Default 0.1.")]
    public ClampedFloatParameterNoInterpolation blueprintEdgeThreshold = new(0.1f, 0.05f, 0.5f);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Color settings.

    /// <summary> Brightness [-1, 1]. Default 0. </summary>
    [FloatSliderWithReset(0.0f, -1.0f, 1.0f, "Brightness [-1, 1]. Default 0.")]
    public ClampedFloatParameterNoInterpolation brightness = new(0.0f, -1.0f, 1.0f);

    /// <summary> Contrast [0, 10]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 10.0f, "Contrast [0, 10]. Default 1.")]
    public ClampedFloatParameterNoInterpolation contrast = new(1.0f, 0.0f, 10.0f);

    /// <summary> Gamma [0.1, 10]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.1f, 10.0f, "Gamma [0.1, 10]. Default 1.")]
    public ClampedFloatParameterNoInterpolation gamma = new(1.0f, 0.1f, 10.0f);

    /// <summary> The color wheel [0, 1]. Default 0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "The color wheel [0, 1]. Default 0.")]
    public ClampedFloatParameterNoInterpolation hue = new(0.0f, 0.0f, 1.0f);

    /// <summary> Intensity of a colors [0, 2]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 2.0f, "Intensity of a colors [0, 2]. Default 1.")]
    public ClampedFloatParameterNoInterpolation saturation = new(1.0f, 0.0f, 2.0f);

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

      pixelationMode.value = Pixelator.PixelationModes.Rectangle;
      pixelSize.value = 0.75f;
      screenAspectRatio.value = true;
      aspectRatio.value = 1.0f;
      pixelScale.value = Vector2.one;
      radius.value = 0.5f;
      background.value = Color.black;
      threads.value = 3;
      chromaticAberrationIntensity.value = 1.0f;
      chromaticAberrationOffset.value = new Vector3(1.0f, 2.0f, -1.0f);
      gradientIntensity.value = 0.0f;
      gradient.value = new Gradient()
      {
        colorKeys = new GradientColorKey[]
        {
          new(Color.white * 0.0f, 0.0f),
          new(Color.white * 0.33f, 0.2f),
          new(Color.white * 0.66f, 0.5f),
          new(Color.white * 1.0f, 1.0f)
        }
      };
      luminanceMin.value = 0.0f;
      luminanceMax.value = 1.0f;
      gradientMappingMode.value = Pixelator.GradientMappingMode.CIELAB;
      gradientCIELabSamples.value = 16;
      gradientApplyLuminance.value = true;
      bevel.value = 1.0f;

      ditherIntensity.value = 0.5f;
      ditherPatternScale.value = 4;
      ditherThresholdScale.value = 0.75f;
      ditherColorSteps.value = 8;

      posterizeIntensity.value = 0.5f;
      posterizeRGBSteps.value = new Vector3Int(24, 24, 24);
      posterizeLuminanceSteps.value = 24;
      posterizeHSVSteps.value = new Vector3Int(24, 24, 24);
      posterizeGamma.value = 1.0f;

      filtersIntensity.value = 0.0f;
      sepiaIntensity.value = 0.0f;
      coolBlueIntensity.value = 0.0f;
      warmFilterIntensity.value = 0.0f;
      invertColorIntensity.value = 0.0f;
      hudsonIntensity.value = 0.0f;
      hefeIntensity.value = 0.0f;
      xproIntensity.value = 0.0f;
      riseIntensity.value = 0.0f;
      toasterIntensity.value = 0.0f;
      irFilterIntensity.value = 0.0f;
      thermalFilterIntensity.value = 0.0f;
      duotoneIntensity.value = 0.0f;
      duotoneColorA.value = new Color(0.1f, 0.2f, 0.5f, 1.0f);
      duotoneColorB.value = new Color(0.9f, 0.9f, 0.2f, 1.0f);
      nightVisionIntensity.value = 0.0f;
      popArtIntensity.value = 0.0f;
      blueprintIntensity.value = 0.0f;
      blueprintEdgeColor.value = new Color(0.6f, 0.8f, 1.0f, 1.0f);
      blueprintBackgroundColor.value = new Color(0.1f, 0.2f, 0.4f, 1.0f);
      blueprintEdgeThreshold.value = 0.1f;
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
