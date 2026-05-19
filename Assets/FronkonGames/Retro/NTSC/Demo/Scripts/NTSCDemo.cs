using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace FronkonGames.Retro.NTSC
{
  /// <summary> Retro: NTSC demo. </summary>
  /// <remarks>
  /// This code is designed for a simple demo, not for production environments.
  /// </remarks>
  public class NTSCDemo : MonoBehaviour
  {
    [Header("This code is only for the demo, not for production environments."), Space(20.0f)]

    [SerializeField]
    private VolumeProfile volumeProfile;

    private NTSCVolume volume;

    private GUIStyle styleTitle;
    private GUIStyle styleLabel;
    private GUIStyle styleButton;

    private void ResetEffect()
    {
      volume?.Reset();
    }

    private void Awake()
    {
      styleTitle = styleLabel = styleButton = null;

      if (NTSC.IsInRenderFeatures() == false)
      {
        Debug.LogWarning($"Effect '{Constants.Asset.Name}' not found. You must add it as a Render Feature.");
#if UNITY_EDITOR
        if (UnityEditor.EditorUtility.DisplayDialog($"Effect '{Constants.Asset.Name}' not found", $"You must add '{Constants.Asset.Name}' as a Render Feature.", "Quit") == true)
          UnityEditor.EditorApplication.isPlaying = false;
#endif
      }

      volume = volumeProfile != null && volumeProfile.TryGet(out NTSCVolume vol) ? vol : null;
      this.enabled = NTSC.IsInRenderFeatures() && volume != null;
    }

    private void Start() => ResetEffect();

    private void OnGUI()
    {
      if (volume == null)
        return;

      styleTitle = new GUIStyle(GUI.skin.label)
      {
        alignment = TextAnchor.LowerCenter,
        fontSize = 32,
        fontStyle = FontStyle.Bold
      };

      styleLabel = new GUIStyle(GUI.skin.label)
      {
        alignment = TextAnchor.UpperLeft,
        fontSize = 24
      };

      styleButton = new GUIStyle(GUI.skin.button)
      {
        fontSize = 24
      };

      GUILayout.BeginHorizontal();
      {
        GUILayout.BeginVertical("box", GUILayout.Width(600.0f), GUILayout.Height(Screen.height));
        {
          const float space = 10.0f;

          GUILayout.Space(space);

          GUILayout.Label(Constants.Asset.Name.ToUpper(), styleTitle);

          volume.intensity.value = SliderField("Intensity", volume.intensity.value, 0.0f, 1.0f);

          GUILayout.Space(space);

          volume.ntscWindowRadius.value = (int)SliderField("Window Radius", volume.ntscWindowRadius.value, 1, 40);
          volume.amCarrierSignalWavelength.value = SliderField("AM Carrier Signal", volume.amCarrierSignalWavelength.value, 0.5f, 10.0f);
          volume.yLowPassWavelength.value = SliderField("Y Low Pass", volume.yLowPassWavelength.value, 0.5f, 10.0f);
          volume.iLowPassWavelength.value = SliderField("I Low Pass", volume.iLowPassWavelength.value, 1.0f, 20.0f);
          volume.qLowPassWavelength.value = SliderField("Q Low Pass", volume.qLowPassWavelength.value, 1.0f, 20.0f);
          volume.colorburstWavelengthEncoder.value = SliderField("Colorburst Encoder", volume.colorburstWavelengthEncoder.value, 1.0f, 10.0f);
          volume.ntscScale.value = SliderField("NTSC Scale", volume.ntscScale.value, 0.1f, 5.0f);
          volume.ntscPhaseAlternation.value = SliderField("Phase Alternation", volume.ntscPhaseAlternation.value, 0.0f, 3.1415927f);
          volume.ntscNoiseStrength.value = SliderField("Noise Strength", volume.ntscNoiseStrength.value, 0.0f, 1.0f);
          volume.ntscWindowBias.value = SliderField("Window Bias", volume.ntscWindowBias.value, -1.0f, 1.0f);
          volume.amDemodulateWavelength.value = SliderField("AM Demodulate", volume.amDemodulateWavelength.value, 0.5f, 10.0f);
          volume.amDecodeHighPassWavelength.value = SliderField("AM High Pass", volume.amDecodeHighPassWavelength.value, 0.5f, 10.0f);
          volume.colorburstWavelengthDecoder.value = SliderField("Colorburst Decoder", volume.colorburstWavelengthDecoder.value, 1.0f, 10.0f);
          volume.decodeLowPassWavelength.value = SliderField("Decode Low Pass", volume.decodeLowPassWavelength.value, 1.0f, 10.0f);

          GUILayout.FlexibleSpace();

          if (GUILayout.Button("RESET", styleButton) == true)
            ResetEffect();

          GUILayout.Space(4.0f);

          if (GUILayout.Button("ONLINE DOCUMENTATION", styleButton) == true)
            Application.OpenURL(Constants.Support.Documentation);

          GUILayout.Space(4.0f);

          if (GUILayout.Button("❤️ LEAVE A REVIEW ❤️", styleButton) == true)
            Application.OpenURL(Constants.Support.Store);

          GUILayout.Space(space * 2.0f);
        }
        GUILayout.EndVertical();

        GUILayout.FlexibleSpace();
      }
      GUILayout.EndHorizontal();
    }

    private void OnDestroy()
    {
      volume?.Reset();
    }

    private bool ToggleField(string label, bool value)
    {
      GUILayout.BeginHorizontal();
      {
        GUILayout.Label(label, styleLabel);

        value = GUILayout.Toggle(value, string.Empty);
      }
      GUILayout.EndHorizontal();

      return value;
    }

    private float SliderField(string label, float value, float min = 0.0f, float max = 1.0f)
    {
      GUILayout.BeginHorizontal();
      {
        GUILayout.Label(label, styleLabel);

        value = GUILayout.HorizontalSlider(value, min, max);
      }
      GUILayout.EndHorizontal();

      return value;
    }

    private int SliderField(string label, int value, int min, int max = 1)
    {
      GUILayout.BeginHorizontal();
      {
        GUILayout.Label(label, styleLabel);

        value = (int)GUILayout.HorizontalSlider(value, min, max);
      }
      GUILayout.EndHorizontal();

      return value;
    }

    private Color ColorField(string label, Color value, bool alpha = true)
    {
      GUILayout.BeginHorizontal();
      {
        GUILayout.Label(label, styleLabel);

        float originalAlpha = value.a;

        Color.RGBToHSV(value, out float h, out float s, out float v);
        h = GUILayout.HorizontalSlider(h, 0.0f, 1.0f);
        value = Color.HSVToRGB(h, s, v);

        if (alpha == false)
          value.a = originalAlpha;
      }
      GUILayout.EndHorizontal();

      return value;
    }

    private Vector2 Vector2Field(string label, Vector2 value, float min, float max) => Vector2Field(label, value, "X", "Y", min, max);

    private Vector2 Vector2Field(string label, Vector2 value, string x = "X", string y = "Y", float min = 0.0f, float max = 1.0f)
    {
      GUILayout.Label(label, styleLabel);

      value.x = SliderField($"   {x}", value.x, min, max);
      value.y = SliderField($"   {y}", value.y, min, max);

      return value;
    }

    private Vector3 Vector3Field(string label, Vector3 value, float min, float max) => Vector3Field(label, value, "X", "Y", "Z", min, max);

    private Vector3 Vector3Field(string label, Vector3 value, string x = "X", string y = "Y", string z = "Z", float min = 0.0f, float max = 1.0f)
    {
      GUILayout.Label(label, styleLabel);

      value.x = SliderField($"   {x}", value.x, min, max);
      value.y = SliderField($"   {y}", value.y, min, max);
      value.z = SliderField($"   {z}", value.z, min, max);

      return value;
    }

    private Vector4 Vector4Field(string label, Vector4 value, float min, float max) => Vector4Field(label, value, "X", "Y", "Z", "W", min, max);

    private Vector4 Vector4Field(string label, Vector4 value, string x = "X", string y = "Y", string z = "Z", string w = "W", float min = 0.0f, float max = 1.0f)
    {
      GUILayout.Label(label, styleLabel);

      value.x = SliderField($"   {x}", value.x, min, max);
      value.y = SliderField($"   {y}", value.y, min, max);
      value.z = SliderField($"   {z}", value.z, min, max);
      value.w = SliderField($"   {w}", value.w, min, max);

      return value;
    }

    private T EnumField<T>(string label, T value) where T : Enum
    {
      string[] names = Enum.GetNames(typeof(T));
      Array values = Enum.GetValues(typeof(T));
      int index = Array.IndexOf(values, value);

      GUILayout.BeginHorizontal();
      {
        GUILayout.Label(label, styleLabel);

        if (GUILayout.Button("<", styleButton) == true)
          index = index > 0 ? index - 1 : values.Length - 1;

        GUILayout.Label(names[index], styleLabel);

        if (GUILayout.Button(">", styleButton) == true)
          index = index < values.Length - 1 ? index + 1 : 0;
      }
      GUILayout.EndHorizontal();

      return (T)(object)index;
    }
  }
}
