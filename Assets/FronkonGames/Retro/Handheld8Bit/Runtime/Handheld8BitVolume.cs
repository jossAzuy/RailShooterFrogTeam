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
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace FronkonGames.Retro.Handheld8Bit
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Retro: Handheld 8-Bit Volume. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  [Serializable, VolumeComponentMenu("Fronkon Games/Retro/Handheld 8-Bit")]
  public sealed class Handheld8BitVolume : VolumeComponent, IPostProcessComponent
  {
    #region Common settings.
    /// <summary> Controls the intensity of the effect [0, 1]. Default 1. </summary>
    /// <remarks> An effect with Intensity equal to 0 will not be executed. </remarks>
    [FloatSliderWithReset(1.0f, 0.0f, 1.0f, "Controls the intensity of the effect [0, 1]. Default 1.")]
    public FloatSliderParameterLinear intensity = new(1.0f, 0.0f, 1.0f);
    #endregion

    #region Handheld 8-Bit settings.
    /// <summary> Pixel size [1.0 - 20.0]. Default 10.0. </summary>
    [FloatSliderWithReset(10.0f, 1.0f, 20.0f, "Pixel size [1.0 - 20.0]. Default 10.0.")]
    public FloatSliderParameterNoInterpolation pixelSize = new(10.0f, 1.0f, 20.0f);

    /// <summary> Subpixel size [0.1 - 2.0]. Default 1.0. </summary>
    [FloatSliderWithReset(1.0f, 0.1f, 2.0f, "Subpixel size [0.1 - 2.0]. Default 1.0.")]
    public FloatSliderParameterNoInterpolation subPixel = new(1.0f, 0.1f, 2.0f);

    /// <summary> Pixel distance [0 - 5]. Default 1. </summary>
    [IntSliderWithReset(1, 0, 5, "Pixel distance [0 - 5]. Default 1.")]
    public ClampedIntParameterNoInterpolation pixelDistance = new(1, 0, 5);

    /// <summary> Pixel offset. Default (0, 0). </summary>
    [Vector2WithReset(0.0f, 0.0f, "Pixel offset. Default (0, 0).")]
    public Vector2Parameter pixelOffset = new(Vector2.zero);

    /// <summary> Luminosity [0.0 - 2.0]. Default 1.0. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 2.0f, "Luminosity [0.0 - 2.0]. Default 1.0.")]
    public FloatSliderParameterNoInterpolation luminosity = new(1.0f, 0.0f, 2.0f);

    /// <summary> Threshold of the palette [0.0 - 2.0]. Default 1.0. </summary>
    [FloatSliderWithReset(2.0f, 0.0f, 5.0f, "Threshold of the palette [0.0 - 5.0]. Default 2.0.")]
    public FloatSliderParameterNoInterpolation threshold = new(2.0f, 0.0f, 5.0f);

    /// <summary> Invert colors. Default false. </summary>
    [ToggleWithReset(false, "Invert colors. Default false.")]
    public BoolParameterNoInterpolation invert = new(false);

    /// <summary> Palette colors. </summary>
    [ColorWithReset(0xFF020000, "Palette color 0.")]
    
    public ColorParameterNoInterpolation palette0 = new(DefaultPaletteColor0);
    
    [ColorWithReset(0xFF020000, "Palette color 1.")]
    
    public ColorParameterNoInterpolation palette1 = new(DefaultPaletteColor1);
    
    [ColorWithReset(0xFF030000, "Palette color 2.")]
    
    public ColorParameterNoInterpolation palette2 = new(DefaultPaletteColor2);
    
    [ColorWithReset(0xFF030000, "Palette color 3.")]
    
    public ColorParameterNoInterpolation palette3 = new(DefaultPaletteColor3);
    
    /// <summary> Grid color. </summary>
    [ColorWithReset(0xFF972600, "Grid color.")]
    
    public ColorParameterNoInterpolation grid = new(DefaultGridColor);
    #endregion

    #region Shadow settings.
    /// <summary> Shadow size [0.0 - 20.0]. Default 8.0. </summary>
    [FloatSliderWithReset(8.0f, 0.0f, 20.0f, "Shadow size [0.0 - 20.0]. Default 8.0.")]
    public FloatSliderParameterNoInterpolation shadowSize = new(8.0f, 0.0f, 20.0f);

    /// <summary> Shadow distance [-10.0 - 10.0]. Default 0.0. </summary>
    [FloatSliderWithReset(0.0f, -10.0f, 10.0f, "Shadow distance [-10.0 - 10.0]. Default 0.0.")]
    public FloatSliderParameterNoInterpolation shadowDistance = new(0.0f, -10.0f, 10.0f);
    #endregion

    #region Color settings.
    /// <summary> Brightness [-1.0, 1.0]. Default 0. </summary>
    [FloatSliderWithReset(0.0f, -1.0f, 1.0f, "Brightness [-1.0, 1.0]. Default 0.")]
    public FloatSliderParameterNoInterpolation brightness = new(0.0f, -1.0f, 1.0f);
    
    /// <summary> Contrast [0.0, 10.0]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 10.0f, "Contrast [0.0, 10.0]. Default 1.")]
    public FloatSliderParameterNoInterpolation contrast = new(1.0f, 0.0f, 10.0f);
    
    /// <summary> Gamma [0.1, 10.0]. Default 0.9. </summary>      
    [FloatSliderWithReset(0.9f, 0.1f, 10.0f, "Gamma [0.1, 10.0]. Default 0.9.")]
    public FloatSliderParameterNoInterpolation gamma = new(0.9f, 0.1f, 10.0f);
    
    /// <summary> The color wheel [0.0, 1.0]. Default 0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "The color wheel [0.0, 1.0]. Default 0.")]
    public FloatSliderParameterNoInterpolation hue = new(0.0f, 0.0f, 1.0f);
    
    /// <summary> Intensity of a colors [0.0, 2.0]. Default 1. </summary>      
    [FloatSliderWithReset(1.0f, 0.0f, 2.0f, "Intensity of a colors [0.0, 2.0]. Default 1.")]
    public FloatSliderParameterNoInterpolation saturation = new(1.0f, 0.0f, 2.0f);
    #endregion

    #region Advanced settings.
    /// <summary> Does it affect the Scene View? </summary>
    [ToggleWithReset(false, "Does it affect the Scene View?")]
    public BoolParameterNoInterpolation affectSceneView = new(false);

    /// <summary> Use scaled time. </summary>
    public BoolParameterNoInterpolation useScaledTime = new(true);
    #endregion

    public static Color DefaultPaletteColor0 = new(0.095f, 0.3f, 0.004f);
    public static Color DefaultPaletteColor1 = new(0.15f, 0.32f, 0.0f);
    public static Color DefaultPaletteColor2 = new(0.32f, 0.43f, 0.0f);
    public static Color DefaultPaletteColor3 = new(0.5f, 0.6f, 0.012f);
    public static Color DefaultGridColor = new(0.6f, 0.6f, 0.01f);

    /// <summary> Reset to default values. </summary>
    public void Reset()
    {
      intensity.value = 1.0f;
      
      pixelSize.value = 10.0f;
      subPixel.value = 1.0f;
      pixelDistance.value = 1;
      pixelOffset.value = Vector2.zero;
      
      luminosity.value = 1.0f;
      threshold.value = 2.0f;
      invert.value = false;
      
      palette0.value = DefaultPaletteColor0;
      palette1.value = DefaultPaletteColor1;
      palette2.value = DefaultPaletteColor2;
      palette3.value = DefaultPaletteColor3;
      grid.value = DefaultGridColor;

      shadowSize.value = 8.0f;
      shadowDistance.value = 0.0f;
      
      brightness.value = 0.0f;
      contrast.value = 1.0f;
      gamma.value = 0.9f;
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
