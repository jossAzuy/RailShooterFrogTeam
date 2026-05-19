using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace FronkonGames.Retro.VHS
{
  /// <summary> Retro: VHS demo. </summary>
  /// <remarks>
  /// This code is designed for a simple demo, not for production environments.
  /// </remarks>
  public class RetroVHSDemo : MonoBehaviour
  {
    [Header("This code is only for the demo, not for production environments."), Space(20.0f)]

    [SerializeField]
    private VolumeProfile volumeProfile;

    [Space]

    [SerializeField]
    private Transform floor;

    [SerializeField, Range(0.0f, 10.0f)]
    private float angularVelocity;

    private VHSVolume volume;

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

      if (VHS.IsInRenderFeatures() == false)
      {
        Debug.LogWarning($"Effect '{Constants.Asset.Name}' not found. You must add it as a Render Feature.");
#if UNITY_EDITOR
        if (UnityEditor.EditorUtility.DisplayDialog($"Effect '{Constants.Asset.Name}' not found", $"You must add '{Constants.Asset.Name}' as a Render Feature.", "Quit") == true)
          UnityEditor.EditorApplication.isPlaying = false;
#endif
      }

      volume = volumeProfile != null && volumeProfile.TryGet(out VHSVolume vol) ? vol : null;
      this.enabled = VHS.IsInRenderFeatures() && volume != null;
    }

    private void Start() => ResetEffect();

    private void Update()
    {
      if (floor != null && angularVelocity > 0.0f)
        floor.rotation = Quaternion.Euler(0.0f, floor.rotation.eulerAngles.y + Time.deltaTime * angularVelocity * 10.0f, 0.0f);
    }

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

          volume.quality.value = EnumField("Quality", volume.quality.value);
          if (volume.quality.value == Quality.HighFidelity)
            volume.samples.value = SliderField("Samples", volume.samples.value, 2, 10);

          volume.resolution.value = EnumField("Resolution", volume.resolution.value);

          volume.yiq.value = Vector3Field("YIQ", volume.yiq.value, 0.0f, 2.0f);

          volume.whiteLevel.value = SliderField("White Level", volume.whiteLevel.value, 0.0f, 1.0f);
          volume.blackLevel.value = SliderField("Black Level", volume.blackLevel.value, 0.0f, 1.0f);

          volume.tapeCreaseStrength.value = SliderField("Tape Crease", volume.tapeCreaseStrength.value, 0.0f, 1.0f);
          volume.colorNoise.value = SliderField("Color Noise", volume.colorNoise.value, 0.0f, 1.0f);
          volume.chromaBand.value = SliderField("Chroma Band", volume.chromaBand.value, 1, 64);
          volume.lumaBand.value = SliderField("Luma Band", volume.lumaBand.value, 1, 16);
          volume.tapeNoiseHigh.value = volume.tapeNoiseLow.value = SliderField("Tape Noise", volume.tapeNoiseHigh.value, 0.0f, 1.0f);
          volume.acBeatStrength.value = SliderField("AC Beat", volume.acBeatStrength.value, 0.0f, 1.0f);
          volume.bottomWarpHeight.value = SliderField("Bottom Warp", volume.bottomWarpHeight.value, 0.0f, 100.0f);
          volume.vignette.value = SliderField("Vignette", volume.vignette.value, 0.0f, 1.0f);

          GUILayout.FlexibleSpace();

          if (GUILayout.Button("RESET", styleButton) == true)
            ResetEffect();

          GUILayout.Space(4.0f);

          if (GUILayout.Button("ONLINE DOCUMENTATION", styleButton) == true)
            Application.OpenURL(Constants.Support.Documentation);

          GUILayout.Space(4.0f);

          if (GUILayout.Button("❤️ WRITE A REVIEW ❤️", styleButton) == true)
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

    private Vector3 Vector3Field(string label, Vector3 value, float min = 0.0f, float max = 1.0f)
    {
      GUILayout.Label(label, styleLabel);

      value.x = SliderField("   Luma", value.x, min, max);
      value.y = SliderField("   In-phase", value.y, min, max);
      value.z = SliderField("   Quadrature", value.z, min, max);

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
