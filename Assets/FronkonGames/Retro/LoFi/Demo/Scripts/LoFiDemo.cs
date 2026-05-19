using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace FronkonGames.Retro.LoFi
{
  /// <summary> Retro: Lo-Fi demo. </summary>
  /// <remarks>
  /// This code is designed for a simple demo, not for production environments.
  /// </remarks>
  public class LoFiDemo : MonoBehaviour
  {
    [Header("This code is only for the demo, not for production environments."), Space(20.0f)]

    [SerializeField]
    private VolumeProfile volumeProfile;

    public List<LoFiProfile> profiles = new();

    private LoFiVolume volume;

    private GUIStyle styleTitle;
    private GUIStyle styleLabel;
    private GUIStyle styleButton;

    private void ResetEffect()
    {
      if (volume != null)
      {
        volume.Reset();
        if (profiles.Count > 0)
          volume.profile.value = profiles[UnityEngine.Random.Range(0, profiles.Count)];
        volume.colorThreshold.value = 0.0f;
      }
    }

    private void Awake()
    {
      styleTitle = styleLabel = styleButton = null;
      
      if (LoFi.IsInRenderFeatures() == false)
      {
        Debug.LogWarning($"Effect '{Constants.Asset.Name}' not found. You must add it as a Render Feature.");
#if UNITY_EDITOR
        if (UnityEditor.EditorUtility.DisplayDialog($"Effect '{Constants.Asset.Name}' not found", $"You must add '{Constants.Asset.Name}' as a Render Feature.", "Quit") == true)
          UnityEditor.EditorApplication.isPlaying = false;
#endif
      }

      volume = volumeProfile != null && volumeProfile.TryGet(out LoFiVolume vol) ? vol : null;
      this.enabled = LoFi.IsInRenderFeatures() && volume != null;
    }

    private void Start() => volume?.Reset();

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
        GUILayout.BeginVertical("box", GUILayout.Width(400.0f), GUILayout.Height(Screen.height));
        {
          const float space = 10.0f;

          GUILayout.Space(space);

          GUILayout.Label(Constants.Asset.Name.ToUpper(), styleTitle);

          GUILayout.Space(space);

          volume.palette.value = ToggleField("Enable palette", volume.palette.value);
          if (volume.palette.value == true)
          {
            if (GUILayout.Button("Change profile") == true && profiles.Count > 0)
              volume.profile.value = profiles[UnityEngine.Random.Range(0, profiles.Count)];
          }

          GUILayout.Space(space);

          volume.pixelate.value = ToggleField("Enable pixelate", volume.pixelate.value);
          volume.pixelSize.value = SliderField("  Size", volume.pixelSize.value, 1, 32);
          volume.pixelRound.value = SliderField("  Round", volume.pixelRound.value);
          volume.pixelBevel.value = SliderField("  Bevel", volume.pixelBevel.value);

          GUILayout.Space(space);

          volume.quantization.value = ToggleField("Quantization", volume.quantization.value);
          volume.colors.value = SliderField("  Colors", volume.colors.value, 2, 64);

          GUILayout.Space(space);

          volume.border.value = ToggleField("Border", volume.border.value);

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
  }
}