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
using UnityEditor;
using UnityEditor.Rendering;

namespace FronkonGames.Retro.Pixelator.Editor
{
  /// <summary> Pixelator Volume inspector. </summary>
  [CustomEditor(typeof(PixelatorVolume))]
  public class PixelatorVolumeInspector : Inspector
  {
    protected override void InspectorGUI()
    {
      PixelatorVolume volume = (PixelatorVolume)target;

      /////////////////////////////////////////////////
      // Common.
      /////////////////////////////////////////////////
      DrawFloatSliderWithReset("intensity");

      /////////////////////////////////////////////////
      // Pixelator.
      /////////////////////////////////////////////////
      Separator();

      DrawEnumDropdownWithReset<Pixelator.PixelationModes>("pixelationMode", "Mode");
      IndentLevel++;
      DrawFloatSliderWithReset("pixelSize", "Size");
      DrawToggleWithReset("screenAspectRatio", true);
      if (volume.screenAspectRatio.value == false)
        DrawFloatSliderWithReset("aspectRatio", "Aspect ratio");
      
      DrawVector2WithReset("pixelScale", "Scale", Vector2.one);

      if (volume.pixelationMode.value == Pixelator.PixelationModes.Circle || volume.pixelationMode.value == Pixelator.PixelationModes.Led)
      {
        DrawFloatSliderWithReset("radius", "Radius");
        DrawColorWithReset("background", "Background", Color.black);
      }

      if (volume.pixelationMode.value == Pixelator.PixelationModes.Knitted)
        DrawIntSliderWithReset("threads", "Threads");

      IndentLevel--;

      Separator();

      DrawFloatSliderWithReset("gradientIntensity", "Gradient");
      if (volume.gradientIntensity.value > 0.0f)
      {
        IndentLevel++;
        DrawGradientWithReset("gradient", "Color gradient");
        DrawEnumDropdownWithReset("gradientMappingMode", "Mapping mode", Pixelator.GradientMappingMode.CIELAB);

        if (volume.gradientMappingMode.value == Pixelator.GradientMappingMode.CIELAB)
        {
          IndentLevel++;
          DrawIntSliderWithReset("gradientCIELabSamples", "Samples");
          IndentLevel--;
        }

        EditorGUI.BeginChangeCheck();
        SerializedDataParameter luminanceMin = UnpackParameter("luminanceMin");
        SerializedDataParameter luminanceMax = UnpackParameter("luminanceMax");
        float min = luminanceMin.value.floatValue;
        float max = luminanceMax.value.floatValue;
        MinMaxSlider("Luminance range", "Luminance range used to change colors.", ref min, ref max, 0.0f, 1.0f, 0.0f, 1.0f);
        luminanceMin.value.floatValue = min;
        luminanceMax.value.floatValue = max;

        DrawToggleWithReset("gradientApplyLuminance", true);

        IndentLevel--;
      }

      DrawFloatSliderWithReset("chromaticAberrationIntensity", "Chromatic aberration");
      if (volume.chromaticAberrationIntensity.value > 0.0f)
      {
        IndentLevel++;
        DrawVector3WithReset("chromaticAberrationOffset", "Offset", new Vector3(1.0f, 2.0f, -1.0f));
        IndentLevel--;
      }

      DrawFloatSliderWithReset("bevel", "Bevel");

      DrawFloatSliderWithReset("ditherIntensity", "Dither");
      if (volume.ditherIntensity.value > 0.0f)
      {
        IndentLevel++;
        DrawIntSliderWithReset("ditherPatternScale", "Pattern scale");
        DrawFloatSliderWithReset("ditherThresholdScale", "Threshold scale");
        DrawIntSliderWithReset("ditherColorSteps", "Color steps");
        IndentLevel--;
      }

      DrawFloatSliderWithReset("posterizeIntensity", "Posterize");
      if (volume.posterizeIntensity.value > 0.0f)
      {
        IndentLevel++;
        DrawVector3IntWithReset("posterizeRGBSteps", "RGB steps", new Vector3Int(24, 24, 24));
        DrawIntSliderWithReset("posterizeLuminanceSteps", "Luminance steps");
        DrawVector3IntWithReset("posterizeHSVSteps", "HSV steps", new Vector3Int(24, 24, 24));
        DrawFloatSliderWithReset("posterizeGamma", "Gamma");
        IndentLevel--;
      }

      DrawFloatSliderWithReset("filtersIntensity", "Filters");
      if (volume.filtersIntensity.value > 0.0f)
      {
        IndentLevel++;
        DrawFloatSliderWithReset("sepiaIntensity", "Sepia");
        DrawFloatSliderWithReset("coolBlueIntensity", "Cool blue");
        DrawFloatSliderWithReset("warmFilterIntensity", "Warm filter");
        DrawFloatSliderWithReset("invertColorIntensity", "Invert color");
        DrawFloatSliderWithReset("hudsonIntensity", "Hudson");
        DrawFloatSliderWithReset("hefeIntensity", "Hefe");
        DrawFloatSliderWithReset("xproIntensity", "X-Pro");
        DrawFloatSliderWithReset("riseIntensity", "Rise");
        DrawFloatSliderWithReset("toasterIntensity", "Toaster");
        DrawFloatSliderWithReset("irFilterIntensity", "Infrared");
        DrawFloatSliderWithReset("thermalFilterIntensity", "Thermal");
        
        DrawFloatSliderWithReset("duotoneIntensity", "Duotone");
        if (volume.duotoneIntensity.value > 0.0f)
        {
          IndentLevel++;
          DrawColorWithReset("duotoneColorA", "Color A", new Color(0.1f, 0.2f, 0.5f, 1.0f));
          DrawColorWithReset("duotoneColorB", "Color B", new Color(0.9f, 0.9f, 0.2f, 1.0f));
          IndentLevel--;
        }

        DrawFloatSliderWithReset("nightVisionIntensity", "Night vision");
        DrawFloatSliderWithReset("popArtIntensity", "Pop art");
        
        DrawFloatSliderWithReset("blueprintIntensity", "Blueprint");
        if (volume.blueprintIntensity.value > 0.0f)
        {
          IndentLevel++;
          DrawColorWithReset("blueprintEdgeColor", "Edge color", new Color(0.6f, 0.8f, 1.0f, 1.0f));
          DrawColorWithReset("blueprintBackgroundColor", "Background color", new Color(0.1f, 0.2f, 0.4f, 1.0f));
          DrawFloatSliderWithReset("blueprintEdgeThreshold", "Edge threshold");
          IndentLevel--;
        }
        IndentLevel--;
      }
    }

    protected override void ResetValues() => ((PixelatorVolume)target).Reset();

    protected override void CheckForErrors()
    {
      if (Pixelator.IsInAnyRenderFeatures() == false)
      {
        Separator();
        EditorGUILayout.HelpBox($"Renderer Feature '{Constants.Asset.Name}' not found. You must add it as a Render Feature.", MessageType.Error);
      }
      else
      {
        Pixelator[] effects = Pixelator.Instances;
        bool anyEnabled = false;
        for (int i = 0; i < effects.Length; i++)
        {
          if (effects[i].isActive == true)
          {
            anyEnabled = true;
            break;
          }
        }

        if (anyEnabled == false)
        {
          Separator();
          EditorGUILayout.HelpBox($"No Renderer Feature '{Constants.Asset.Name}' is active. You must activate it in the Render Features.", MessageType.Warning);
        }
      }
    }
  }
}
