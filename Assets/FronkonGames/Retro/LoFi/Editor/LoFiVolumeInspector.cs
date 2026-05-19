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
using UnityEditor;
using UnityEditor.Rendering;

namespace FronkonGames.Retro.LoFi.Editor
{
  /// <summary> Lo-Fi Volume inspector. </summary>
  [CustomEditor(typeof(LoFiVolume))]
  public class LoFiVolumeInspector : Inspector
  {
    protected override void InspectorGUI()
    {
      LoFiVolume volume = (LoFiVolume)target;

      /////////////////////////////////////////////////
      // Common.
      /////////////////////////////////////////////////
      DrawFloatSliderWithReset("intensity");

      /////////////////////////////////////////////////
      // Lo-Fi.
      /////////////////////////////////////////////////
      Separator();

      DrawToggleWithReset("palette", true);
      IndentLevel++;

      EditorGUILayout.BeginHorizontal();
      {
        DrawObjectWithReset<LoFiProfile>("profile");

        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Search Icon"), EditorStyles.miniLabel, GUILayout.Width(20.0f), GUILayout.Height(20.0f)) == true)
          PaletteBrowser.ShowWindow();
      }
      EditorGUILayout.EndHorizontal();

      DrawEnumDropdownWithReset("mode", "Mode", BlendModes.Blend);
      DrawEnumDropdownWithReset("sampleMethod", "Sample Method", SampleMethod.Distance);

      switch (volume.sampleMethod.value)
      {
        case SampleMethod.Luminance:
          IndentLevel++;
          DrawFloatSliderWithReset("luminancePow", "Luminance");
          
          SerializedDataParameter rangeMin = UnpackParameter("rangeMin");
          SerializedDataParameter rangeMax = UnpackParameter("rangeMax");
          float min = rangeMin.value.floatValue;
          float max = rangeMax.value.floatValue;
          MinMaxSlider("Remap luminance", "Luminance range used to change colors.", ref min, ref max, 0.0f, 1.0f, 0.0f, 1.0f);
          rangeMin.value.floatValue = min;
          rangeMax.value.floatValue = max;

          DrawToggleWithReset("invert");
          IndentLevel--;
          break;
        case SampleMethod.Distance:
        case SampleMethod.HSV:
        case SampleMethod.Similarity:
        case SampleMethod.Dominant:
          IndentLevel++;
          DrawFloatSliderWithReset("colorThreshold");
          IndentLevel--;
          break;
      }

      DrawEnumDropdownWithReset("resolution", "Resolution", PaletteResolutions._16);
      
      if (volume.profile.value != null)
      {
        GUILayout.Label(string.Empty);
        Rect rect = GUILayoutUtility.GetLastRect();
        rect.xMin += EditorGUIUtility.labelWidth;

        EditorGUI.DrawPreviewTexture(rect, volume.profile.value.ToTexture(volume.mode.value, volume.invert.value, (int)volume.resolution.value));
      }
      IndentLevel--;

      DrawToggleWithReset("pixelate", true);
      IndentLevel++;
      DrawIntSliderWithReset("pixelSize");
      volume.pixelSize.value = RoundToNearestEven(Math.Max(2, volume.pixelSize.value));
      
      IndentLevel++;
      DrawEnumDropdownWithReset("pixelBlend", "Blend", ColorBlends.Solid);
      DrawColorWithReset("pixelTint", "Tint", Color.white);
      IndentLevel--;

      DrawFloatSliderWithReset("pixelSobel", "Sobel");
      IndentLevel++;
      DrawFloatSliderWithReset("pixelSobelPower", "Power");
      DrawFloatSliderWithReset("pixelSobelAngle", "Light angle");
      DrawFloatSliderWithReset("pixelSobelLightIntensity", "Light intensity");
      DrawFloatSliderWithReset("pixelSobelAmbient", "Ambient");
      IndentLevel--;

      DrawFloatSliderWithReset("pixelRound", "Round");
      IndentLevel++;
      DrawFloatSliderWithReset("pixelBevel", "Bevel");
      IndentLevel--;

      DrawIntSliderWithReset("pixelSamples", "Samples");
      IndentLevel--;

      DrawFloatSliderWithReset("scanline");
      IndentLevel++;
      DrawIntSliderWithReset("scanlineCount", "Count");
      DrawFloatSliderWithReset("scanlineSpeed", "Speed");
      IndentLevel--;

      DrawFloatSliderWithReset("vignette");

      DrawToggleWithReset("quantization");
      IndentLevel++;
      DrawIntSliderWithReset("colors");
      IndentLevel--;

      DrawFloatSliderWithReset("chromaticAberration");

      DrawFloatSliderWithReset("shine", "Glass shine");
      IndentLevel++;
      DrawFloatSliderWithReset("shineSize", "Size");
      IndentLevel--;

      DrawFloatSliderWithReset("aperture");
      DrawFloatSliderWithReset("curvature");
      
      IndentLevel++;
      IndentLevel--;

      DrawToggleWithReset("border", true);
      IndentLevel++;
      DrawColorWithReset("borderColor", "Color", LoFiVolume.DefaultBorderColor);
      DrawFloatSliderWithReset("borderSmooth", "Smooth");
      DrawFloatSliderWithReset("borderNoise", "Noise");
      DrawVector2WithReset("borderMargins", "Margins", Vector2.one);
      
      var margins = volume.borderMargins.value;
      margins.x = Mathf.Clamp(margins.x, 1.0f, 2.0f);
      margins.y = Mathf.Clamp(margins.y, 1.0f, 2.0f);
      volume.borderMargins.value = margins;
      IndentLevel--;
    }

    protected override void ResetValues() => ((LoFiVolume)target).Reset();

    protected override void CheckForErrors()
    {
      if (LoFi.IsInAnyRenderFeatures() == false)
      {
        Separator();
        EditorGUILayout.HelpBox($"Renderer Feature '{Constants.Asset.Name}' not found. You must add it as a Render Feature.", MessageType.Error);
      }
      else
      {
        LoFi[] effects = LoFi.Instances;
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

    private int RoundToNearestEven(int number) => (int)Math.Round(number / 2.0) * 2;
  }
}