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

namespace FronkonGames.Retro.NTSC.Editor
{
  /// <summary> NTSC Volume inspector. </summary>
  [CustomEditor(typeof(NTSCVolume))]
  public class NTSCVolumeInspector : Inspector
  {
    protected override void InspectorGUI()
    {
      /////////////////////////////////////////////////
      // Common.
      /////////////////////////////////////////////////
      DrawFloatSliderWithReset("intensity");

      /////////////////////////////////////////////////
      // NTSC.
      /////////////////////////////////////////////////
      Separator();

      DrawIntSliderWithReset("ntscWindowRadius", "Window radius");
      DrawFloatSliderWithReset("ntscScale", "Scale");
      DrawFloatSliderWithReset("ntscPhaseAlternation", "Phase alternation");
      
      DrawFloatSliderWithReset("ntscNoiseStrength", "Noise strength");
      IndentLevel++;
      DrawFloatSliderWithReset("ntscWindowBias", "Window bias");
      IndentLevel--;

      Label("Encoder");
      IndentLevel++;
      DrawFloatSliderWithReset("amCarrierSignalWavelength", "AM carrier Wavelength");
      Label("Low-pass filters");
      IndentLevel++;
      DrawFloatSliderWithReset("yLowPassWavelength", "Y low-pass");
      DrawFloatSliderWithReset("iLowPassWavelength", "I low-pass");
      DrawFloatSliderWithReset("qLowPassWavelength", "Q low-pass");
      IndentLevel--;
      DrawFloatSliderWithReset("colorburstWavelengthEncoder", "Colorburst encoder");
      IndentLevel--;

      Label("Decoder");
      IndentLevel++;
      DrawFloatSliderWithReset("amDemodulateWavelength", "AM demodulate wavelength");
      DrawFloatSliderWithReset("amDecodeHighPassWavelength", "AM decode high-pass");
      DrawFloatSliderWithReset("colorburstWavelengthDecoder", "Colorburst decoder");
      DrawFloatSliderWithReset("decodeLowPassWavelength", "Decode low-pass");
      IndentLevel--;
    }

    protected override void ResetValues() => ((NTSCVolume)target).Reset();

    protected override void CheckForErrors()
    {
      if (NTSC.IsInAnyRenderFeatures() == false)
      {
        Separator();
        EditorGUILayout.HelpBox($"Renderer Feature '{Constants.Asset.Name}' not found. You must add it as a Render Feature.", MessageType.Error);
      }
      else
      {
        NTSC[] effects = NTSC.Instances;
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
