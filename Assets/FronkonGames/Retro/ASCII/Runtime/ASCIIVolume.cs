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

namespace FronkonGames.Retro.ASCII
{
  /// <summary> ASCII Volume. </summary>
  [Serializable, VolumeComponentMenu("Fronkon Games/Retro/ASCII"), HelpURL(Constants.Support.Documentation)]
  public sealed class ASCIIVolume : VolumeComponent, IPostProcessComponent
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
    #region ASCII settings.

    /// <summary> Selected charset. </summary> 
    /// <remarks> Create them using the tool at 'Window > Fronkon Games > Retro > ASCII > ASCII Charset Tool'. </remarks>
    public ASCIICharsetParameter charset = new(null);

    /// <summary> Character selection algorithm. Default Luminance (classic). </summary>
    [EnumDropdown((int)CharacterSelectionMode.Luminance, "Character selection algorithm. Default Luminance (classic).")]
    public EnumParameterNoInterpolation<CharacterSelectionMode> selectionMode = new(CharacterSelectionMode.Luminance);

    /// <summary> Character zoom [0, 10]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 10.0f, "Character zoom [0, 10]. Default 1.")]
    public FloatSliderParameterNoInterpolation zoom = new(1.0f, 0.0f, 10.0f);

    /// <summary> Increased brightness [0, 5]. Default 2. </summary>
    [FloatSliderWithReset(2.0f, 0.0f, 5.0f, "Increased brightness [0, 5]. Default 2.")]
    public FloatSliderParameterNoInterpolation boost = new(2.0f, 0.0f, 5.0f);

    /// <summary> Shape matching weight vs density (only for ShapeAware mode) [0, 1]. Default 0.7. </summary>
    /// <remarks> Higher values prioritize shape matching, lower values prioritize density. </remarks>
    [FloatSliderWithReset(0.7f, 0.0f, 1.0f, "Shape matching weight vs density (only for ShapeAware mode) [0, 1]. Default 0.7.")]
    public FloatSliderParameterNoInterpolation shapeWeight = new(0.7f, 0.0f, 1.0f);

    /// <summary> Enable edge detection for sharper character boundaries. Default false. </summary>
    [ToggleWithReset(false, "Enable edge detection for sharper character boundaries. Default false.")]
    public BoolParameterNoInterpolation edgeDetection = new(false);

    /// <summary> Edge detection sensitivity - how easily edges are detected [0, 2]. Default 1. </summary>
    /// <remarks> Higher values detect more subtle edges. </remarks>
    [FloatSliderWithReset(1.0f, 0.0f, 2.0f, "Edge detection sensitivity - how easily edges are detected [0, 2]. Default 1.")]
    public FloatSliderParameterNoInterpolation edgeSensitivity = new(1.0f, 0.0f, 2.0f);

    /// <summary> Edge contrast enhancement strength [0, 2]. Default 0.5. </summary>
    /// <remarks> How much to boost contrast at detected edges. </remarks>
    [FloatSliderWithReset(0.5f, 0.0f, 2.0f, "Edge contrast enhancement strength [0, 2]. Default 0.5.")]
    public FloatSliderParameterNoInterpolation edgeContrast = new(0.5f, 0.0f, 2.0f);

    /// <summary> Enable supersampling anti-aliasing for smoother edges. Default false. </summary>
    /// <remarks> Takes multiple samples per cell and averages them. Improves quality at cost of performance. </remarks>
    [ToggleWithReset(false, "Enable supersampling anti-aliasing for smoother edges. Default false.")]
    public BoolParameterNoInterpolation superSampling = new(false);

    /// <summary> Supersampling level [2, 4]. Default 2. </summary>
    /// <remarks> 2 = 2x2 (4 samples), 3 = 3x3 (9 samples), 4 = 4x4 (16 samples). Higher = smoother but slower. </remarks>
    [IntSliderWithReset(2, 2, 4, "Supersampling level [2, 4]. Default 2.")]
    public IntParameterNoInterpolation superSamplingLevel = new(2);

    /// <summary> Each block of text in a single color. Default true. </summary>
    [ToggleWithReset(true, "Each block of text in a single color. Default true.")]
    public BoolParameterNoInterpolation blockColor = new(true);

    /// <summary> Color operation for the text. Default ColorBlends.Multiply. </summary>
    [EnumDropdown((int)ColorBlends.Multiply, "Color operation for the text. Default ColorBlends.Multiply.")]
    public EnumParameterNoInterpolation<ColorBlends> fontColorBlend = new(ColorBlends.Multiply);
    
    /// <summary> Text color. Default white. </summary>
    [ColorWithReset(0xFFFFFF, "Text color. Default white.")]
    public ColorParameterNoInterpolation fontColor = new(Color.white);

    /// <summary> Color operation for the background. Default ColorBlends.Multiply. </summary>
    [EnumDropdown((int)ColorBlends.Multiply, "Color operation for the background. Default ColorBlends.Multiply.")]
    public EnumParameterNoInterpolation<ColorBlends> backgroundColorBlend = new(ColorBlends.Multiply);
    
    /// <summary> Background color. Default black. </summary>
    [ColorWithReset(0x000000, "Background color. Default black.")]
    public ColorParameterNoInterpolation backgroundColor = new(Color.black);

    /// <summary> Gradient mode. Default ColorGradients.None. </summary>
    [EnumDropdown((int)ColorGradients.None, "Gradient mode. Default ColorGradients.None.")]
    public EnumParameterNoInterpolation<ColorGradients> colorGradient = new(ColorGradients.None);

    /// <summary> Text color 0. Used in horizontal, vertical and circular gradients. Default white. </summary>
    [ColorWithReset(0xFFFFFF, "Text color 0. Used in horizontal, vertical and circular gradients. Default white.")]
    public ColorParameterNoInterpolation colorGradient0 = new(Color.white);

    /// <summary> Text color 1. Used in horizontal, vertical and circular gradients. Default white. </summary>
    [ColorWithReset(0xFFFFFF, "Text color 1. Used in horizontal, vertical and circular gradients. Default white.")]
    public ColorParameterNoInterpolation colorGradient1 = new(Color.white);

    /// <summary> Gradient radius, used in circular gradient [0, 10]. Default 2. </summary>
    [FloatSliderWithReset(2.0f, 0.0f, 10.0f, "Gradient radius, used in circular gradient [0, 10]. Default 2.")]
    public FloatParameterNoInterpolation gradientCircularRadius = new(2.0f);

    /// <summary> Gradient offset horizontal, used in horizontal gradient [0, 2]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 2.0f, "Gradient offset horizontal, used in horizontal gradient [0, 2]. Default 1.")]
    public FloatParameterNoInterpolation gradientHorizontalOffset = new(1.0f);

    /// <summary> Gradient offset vertical, used in vertical gradient [0, 2]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 2.0f, "Gradient offset vertical, used in vertical gradient [0, 2]. Default 1.")]
    public FloatParameterNoInterpolation gradientVerticalOffset = new(1.0f);

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

    /// <summary> Reset to default values. </summary>
    public void Reset()
    {
      intensity.value = 1.0f;

      selectionMode.value = CharacterSelectionMode.Luminance;
      zoom.value = 1.0f;
      boost.value = 2.0f;
      shapeWeight.value = 0.7f;
      edgeDetection.value = false;
      edgeSensitivity.value = 1.0f;
      edgeContrast.value = 0.5f;
      superSampling.value = false;
      superSamplingLevel.value = 2;
      blockColor.value = true;
      fontColorBlend.value = ColorBlends.Multiply;
      fontColor.value = Color.white;
      backgroundColorBlend.value = ColorBlends.Multiply;
      backgroundColor.value = Color.black;
      colorGradient.value = ColorGradients.None;
      colorGradient0.value = Color.white;
      colorGradient1.value = Color.white;
      gradientCircularRadius.value = 2.0f;
      gradientHorizontalOffset.value = 1.0f;
      gradientVerticalOffset.value = 1.0f;

      brightness.value = 0.0f;
      contrast.value = 1.0f;
      gamma.value = 1.0f;
      hue.value = 0.0f;
      saturation.value = 1.0f;

      affectSceneView.value = false;
      useScaledTime.value = true;
    }

    /// <summary> Is the effect active? </summary>
    public bool IsActive() => intensity.overrideState == true && intensity.value > 0.0f && charset.value != null;

    /// <summary> Is the effect tile compatible? </summary>
    public bool IsTileCompatible() => false;
  }
}