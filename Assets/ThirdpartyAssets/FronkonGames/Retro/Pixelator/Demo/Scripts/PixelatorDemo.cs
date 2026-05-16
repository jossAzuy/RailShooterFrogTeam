using System;
using UnityEngine;
using UnityEngine.Rendering;
using FronkonGames.Retro.Pixelator;

/// <summary> Retro: Pixelator demo. </summary>
/// <remarks>
/// This code is designed for a simple demo, not for production environments.
/// </remarks>
public class PixelatorDemo : MonoBehaviour
{
  [Header("This code is only for the demo, not for production environments."), Space(20.0f)]

  [SerializeField]
  private VolumeProfile volumeProfile;

  [Space]

  [SerializeField]
  private Transform floor;

  [SerializeField, Range(0.0f, 10.0f)]
  private float angularVelocity = 5.0f;
  
  private PixelatorVolume volume;
  
  private GUIStyle styleTitle;
  private GUIStyle styleLabel;
  private GUIStyle styleButton;
  private Vector2 scrollView;

  private const float OriginalScreenWidth = 1920.0f;

  private void Awake()
  {
    styleTitle = styleLabel = styleButton = null;

    if (Pixelator.IsInRenderFeatures() == false)
    {
      Debug.LogWarning($"Effect '{Constants.Asset.Name}' not found. You must add it as a Render Feature.");
#if UNITY_EDITOR
      if (UnityEditor.EditorUtility.DisplayDialog($"Effect '{Constants.Asset.Name}' not found", $"You must add '{Constants.Asset.Name}' as a Render Feature.", "Quit") == true)
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    volume = volumeProfile != null && volumeProfile.TryGet(out PixelatorVolume vol) ? vol : null;
    this.enabled = Pixelator.IsInRenderFeatures() && volume != null;
  }

  private void Start() => volume?.Reset();

  private void OnDestroy() => volume?.Reset();

  private void Update()
  {
    if (floor != null && angularVelocity > 0.0f)
      floor.rotation = Quaternion.Euler(0.0f, floor.rotation.eulerAngles.y + Time.deltaTime * angularVelocity * 10.0f, 0.0f);
  }

  private void OnGUI()
  {
    if (volume == null)
      return;

    Matrix4x4 guiMatrix = GUI.matrix;
    GUI.matrix = Matrix4x4.Scale(Vector3.one * (Screen.width / OriginalScreenWidth));

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
      GUILayout.BeginVertical("box", GUILayout.Width(600.0f), GUILayout.Height(Screen.height / (Screen.width / OriginalScreenWidth)));
      {
        const float space = 10.0f;

        GUILayout.Space(space);

        GUILayout.Label("Retro: Pixelator", styleTitle);

        scrollView = GUILayout.BeginScrollView(scrollView);
        {
          volume.intensity.value = SliderField("Intensity", volume.intensity.value, 0.0f, 1.0f);

          GUILayout.Space(space);

          volume.pixelationMode.value = EnumField("Shape", volume.pixelationMode.value);

          volume.pixelSize.value = SliderField("Size", volume.pixelSize.value, 0.0f, 1.0f);

          volume.posterizeIntensity.value = SliderField("Posterization", volume.posterizeIntensity.value, 0.0f, 1.0f);

          volume.ditherIntensity.value = SliderField("Dither", volume.ditherIntensity.value, 0.0f, 1.0f);

          volume.gradientIntensity.value = SliderField("Gradient", volume.gradientIntensity.value, 0.0f, 1.0f);

          volume.chromaticAberrationIntensity.value = SliderField("Chromatic Aberration", volume.chromaticAberrationIntensity.value, 0.0f, 1.0f);

          GUILayout.Space(space);
        }
        GUILayout.EndScrollView();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("RESET", styleButton) == true)
          volume?.Reset();

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

    GUI.matrix = guiMatrix;
  }

  private float SliderField(string label, float value, float min = 0.0f, float max = 1.0f)
  {
    GUILayout.BeginHorizontal();
    {
      GUILayout.Label(label, styleLabel, GUILayout.Width(250.0f));

      value = GUILayout.HorizontalSlider(value, min, max);
    }
    GUILayout.EndHorizontal();

    return value;
  }

  private T EnumField<T>(string label, T value) where T : Enum
  {
    string[] names = Enum.GetNames(typeof(T));
    Array values = Enum.GetValues(typeof(T));
    int index = Array.IndexOf(values, value);

    GUILayout.BeginHorizontal();
    {
      GUILayout.Label(label, styleLabel, GUILayout.Width(250.0f));

      if (GUILayout.Button("<", styleButton, GUILayout.Width(40.0f)) == true)
        index = index > 0 ? index - 1 : values.Length - 1;

      GUILayout.Label(names[index], styleLabel, GUILayout.ExpandWidth(true));

      if (GUILayout.Button(">", styleButton, GUILayout.Width(40.0f)) == true)
        index = index < values.Length - 1 ? index + 1 : 0;
    }
    GUILayout.EndHorizontal();

    return (T)values.GetValue(index);
  }
}
