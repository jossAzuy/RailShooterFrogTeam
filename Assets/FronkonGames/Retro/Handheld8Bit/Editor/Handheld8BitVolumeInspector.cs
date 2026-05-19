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
using UnityEditor;

namespace FronkonGames.Retro.Handheld8Bit.Editor
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Handheld 8-Bit Volume inspector. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  [CustomEditor(typeof(Handheld8BitVolume))]
  public class Handheld8BitVolumeInspector : Inspector
  {
    protected override void InspectorGUI()
    {
      // Common settings
      DrawFloatSliderWithReset("intensity");

      Separator();

      DrawFloatSliderWithReset("pixelSize");
      IndentLevel++;
      DrawFloatSliderWithReset("subPixel");
      DrawIntSliderWithReset("pixelDistance", "Distance");
      DrawVector2WithReset("pixelOffset", "Offset");
      IndentLevel--;

      // Shadow settings
      DrawFloatSliderWithReset("shadowSize", "Shadow Size");
      IndentLevel++;
      DrawFloatSliderWithReset("shadowDistance", "Distance");
      IndentLevel--;

      Label("Palette Colors (from darker to lighter)");
      IndentLevel++;
      DrawColorWithReset("palette0", "Color #1", Handheld8BitVolume.DefaultPaletteColor0);
      DrawColorWithReset("palette1", "Color #2", Handheld8BitVolume.DefaultPaletteColor1);
      DrawColorWithReset("palette2", "Color #3", Handheld8BitVolume.DefaultPaletteColor2);
      DrawColorWithReset("palette3", "Color #4", Handheld8BitVolume.DefaultPaletteColor3);
      DrawColorWithReset("grid", "Grid", Handheld8BitVolume.DefaultGridColor);
      DrawToggleWithReset("invert");
      IndentLevel--;

      DrawFloatSliderWithReset("luminosity");
      DrawFloatSliderWithReset("threshold");
    }

    protected override void ResetValues() => ((Handheld8BitVolume)target).Reset();

    protected override void CheckForErrors()
    {
      if (Handheld8Bit.IsInAnyRenderFeatures() == false)
      {
        Separator();

        EditorGUILayout.HelpBox($"Renderer Feature '{Constants.Asset.Name}' not found. You must add it as a Render Feature.", MessageType.Error);
      }
      else
      {
        Handheld8Bit[] effects = Handheld8Bit.Instances;

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
