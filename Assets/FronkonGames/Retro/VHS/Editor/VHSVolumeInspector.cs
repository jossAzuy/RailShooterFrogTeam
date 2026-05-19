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
using UnityEditor;

namespace FronkonGames.Retro.VHS.Editor
{
  /// <summary> VHS Volume inspector. </summary>
  [CustomEditor(typeof(VHSVolume))]
  public class VHSVolumeInspector : Inspector
  {
    protected override void InspectorGUI()
    {
      VHSVolume volume = (VHSVolume)target;

      /////////////////////////////////////////////////
      // Common.
      /////////////////////////////////////////////////
      DrawFloatSliderWithReset("intensity");

      /////////////////////////////////////////////////
      // VHS.
      /////////////////////////////////////////////////
      Separator();

      DrawEnumDropdownWithReset("quality", "Quality", Quality.HighFidelity);
      IndentLevel++;
      if (volume.quality.value == Quality.HighFidelity)
        DrawIntSliderWithReset("samples");
      IndentLevel--;

      DrawEnumDropdownWithReset("resolution", "Resolution", Resolution.Quarter);
      
      DrawVector3SliderWithReset("yiq", "YIQ color space", VHSVolume.DefaultYIQ);
      IndentLevel++;
      DrawIntSliderWithReset("lumaBand");
      DrawFloatSliderWithReset("colorNoise");
      DrawIntSliderWithReset("chromaBand");
      IndentLevel--;

      DrawColorWithReset("shadowTint", "Shadow Tint", VHSVolume.DefaultShadowTint);
      IndentLevel++;
      DrawFloatSliderWithReset("whiteLevel");
      DrawFloatSliderWithReset("blackLevel");
      IndentLevel--;

      DrawFloatSliderWithReset("tapeCreaseStrength", "Tape Crease");
      IndentLevel++;
      DrawFloatSliderWithReset("tapeCreaseNoise", "Noise");
      DrawFloatSliderWithReset("tapeCreaseVelocity", "Velocity");
      DrawFloatSliderWithReset("tapeCreaseCount", "Count");
      if (volume.quality.value == Quality.HighFidelity)
        DrawFloatSliderWithReset("tapeCreaseDistortion", "Distortion");
      IndentLevel--;

      DrawFloatSliderWithReset("tapeNoiseHigh");
      IndentLevel++;
      DrawFloatSliderWithReset("tapeNoiseLow");
      IndentLevel--;

      DrawFloatSliderWithReset("acBeatStrength", "AC Beat");
      IndentLevel++;
      DrawFloatSliderWithReset("acBeatCount", "Count");
      DrawFloatSliderWithReset("acBeatVelocity", "Velocity");
      IndentLevel--;

      DrawFloatSliderWithReset("bottomWarpHeight");
      IndentLevel++;
      DrawFloatSliderWithReset("bottomWarpDistortion", "Distortion");
      DrawFloatSliderWithReset("bottomWarpJitterExtent", "Jitter Extent");
      IndentLevel--;
      
      DrawFloatSliderWithReset("vignette");
    }

    protected override void ResetValues() => ((VHSVolume)target).Reset();

    protected override void CheckForErrors()
    {
      if (VHS.IsInAnyRenderFeatures() == false)
      {
        Separator();
        EditorGUILayout.HelpBox($"Renderer Feature '{Constants.Asset.Name}' not found. You must add it as a Render Feature.", MessageType.Error);
      }
      else
      {
        VHS[] effects = VHS.Instances;
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
