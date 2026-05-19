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

namespace FronkonGames.Retro.ASCII.Editor
{
  /// <summary> ASCII Volume inspector. </summary>
  [CustomEditor(typeof(ASCIIVolume))]
  public class ASCIIVolumeInspector : Inspector
  {
    public override void OnEnable()
    {
      if (serializedObject == null)
        return;
    }

    protected override void InspectorGUI()
    {
      ASCIIVolume volume = (ASCIIVolume)target;

      /////////////////////////////////////////////////
      // Common.
      /////////////////////////////////////////////////
      DrawFloatSliderWithReset("intensity");

      /////////////////////////////////////////////////
      // ASCII.
      /////////////////////////////////////////////////
      Separator();

      volume.charset.value = (ASCIICharset)EditorGUILayout.ObjectField("Charset", volume.charset.value, typeof(ASCIICharset), false);

      DrawEnumDropdownWithReset("selectionMode", "Selection mode", CharacterSelectionMode.Luminance);

      if (volume.selectionMode == CharacterSelectionMode.ShapeAware)
      {
        IndentLevel++;

        // Check if charset has shape data
        if (volume.charset != null && !volume.charset.value.HasShapeData)
          EditorGUILayout.HelpBox("This charset doesn't have shape data. Please regenerate it using the ASCII Charset Tool.", MessageType.Warning);

        DrawFloatSliderWithReset("shapeWeight");

        IndentLevel--;
      }

      DrawToggleWithReset("edgeDetection");
      if (volume.edgeDetection == true)
      {
        IndentLevel++;
        DrawFloatSliderWithReset("edgeSensitivity", "Sensitivity");
        DrawFloatSliderWithReset("edgeContrast", "Contrast");
        IndentLevel--;
      }

      DrawToggleWithReset("superSampling");
      if (volume.superSampling == true)
      {
        IndentLevel++;
        DrawIntSliderWithReset("superSamplingLevel", "Quality");
        IndentLevel--;
      }

      DrawFloatSliderWithReset("zoom");
      DrawFloatSliderWithReset("boost");
      DrawToggleWithReset("blockColor", true);

      DrawEnumDropdownWithReset("fontColorBlend", "Text Color Blend", ColorBlends.Multiply);
      IndentLevel++;
      DrawColorWithReset("fontColor", "Color", Color.white);
      IndentLevel--;

      DrawEnumDropdownWithReset("backgroundColorBlend", "Background Color Blend", ColorBlends.Multiply);
      IndentLevel++;
      DrawColorWithReset("backgroundColor", "Color", Color.black);
      IndentLevel--;
    }

    protected override void ResetValues() => ((ASCIIVolume)target).Reset();

    protected override void CheckForErrors()
    {
      if (ASCII.IsInAnyRenderFeatures() == false)
      {
        Separator();

        EditorGUILayout.HelpBox($"Renderer Feature '{Constants.Asset.Name}' not found. You must add it as a Render Feature.", MessageType.Error);
      }
      else
      {
        ASCII[] effects = ASCII.Instances;

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

      ASCIIVolume volume = (ASCIIVolume)target;
      if (volume.charset == null)
      {
        Separator();

        EditorGUILayout.HelpBox($"Charset is not assigned.", MessageType.Warning);
      }
    }
  }
}