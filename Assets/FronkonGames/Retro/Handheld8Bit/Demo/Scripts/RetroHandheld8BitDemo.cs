using UnityEngine;
using UnityEngine.Rendering;
using FronkonGames.Retro.Handheld8Bit;

/// <summary> Retro: Handheld 8-Bit demo. </summary>
/// <remarks>
/// This code is designed for a simple demo, not for production environments.
/// </remarks>
public class RetroHandheld8BitDemo : MonoBehaviour
{
  [Header("This code is only for the demo, not for production environments.")]

  [Space(20.0f), SerializeField]
  private VolumeProfile volumeProfile;
  
  [SerializeField]
  private Transform floor;

  [SerializeField, Range(0.0f, 10.0f)]
  private float angularVelocity;

  private Handheld8BitVolume volume;
  
  private GUIStyle styleFont;
  private GUIStyle styleButton;
  private GUIStyle styleLogo;
  private Vector2 scrollView;

  private const float BoxWidth = 600.0f;
  private const float Margin = 20.0f;
  private const float LabelSize = 250.0f;
  private const float OriginalScreenWidth = 1920.0f;

  private int Slider(string label, int value, int left, int right)
  {
    GUILayout.BeginHorizontal();
    {
      GUILayout.Space(Margin);
    
      GUILayout.Label(label, styleFont, GUILayout.Width(LabelSize));
    
      value = (int)GUILayout.HorizontalSlider(value, left, right, GUILayout.ExpandWidth(true));
    
      GUILayout.Space(Margin);
    }
    GUILayout.EndHorizontal();

    return value;
  }

  private float Slider(string label, float value, float left, float right)
  {
    GUILayout.BeginHorizontal();
    {
      GUILayout.Space(Margin);
    
      GUILayout.Label(label, styleFont, GUILayout.Width(LabelSize));
    
      value = GUILayout.HorizontalSlider(value, left, right, GUILayout.ExpandWidth(true));
    
      GUILayout.Space(Margin);
    }
    GUILayout.EndHorizontal();

    return value;
  }

  private bool Toggle(string label, bool value)
  {
    GUILayout.BeginHorizontal();
    {
      GUILayout.Space(Margin);
    
      GUILayout.Label(label, styleFont, GUILayout.Width(LabelSize));
    
      value = GUILayout.Toggle(value, string.Empty);
    
      GUILayout.Space(Margin);
    }
    GUILayout.EndHorizontal();

    return value;
  }

  private void Awake()
  {
    styleFont = styleButton = styleLogo = null;
    
    if (Handheld8Bit.IsInRenderFeatures() == false)
    {
      Debug.LogWarning($"Effect '{Constants.Asset.Name}' not found. You must add it as a Render Feature.");
#if UNITY_EDITOR
      if (UnityEditor.EditorUtility.DisplayDialog($"Effect '{Constants.Asset.Name}' not found", $"You must add '{Constants.Asset.Name}' as a Render Feature.", "Quit") == true)
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    volume = volumeProfile != null && volumeProfile.TryGet(out Handheld8BitVolume vol) ? vol : null;
    this.enabled = Handheld8Bit.IsInRenderFeatures() && volume != null;
  }

  private void OnEnable() => volume?.Reset();

  private void Update()
  {
    if (floor != null && angularVelocity > 0.0f)
      floor.rotation = Quaternion.Euler(0.0f, floor.rotation.eulerAngles.y + Time.deltaTime * angularVelocity * 10.0f, 0.0f);
  }

  private void OnGUI()
  {
    Matrix4x4 guiMatrix = GUI.matrix;
    GUI.matrix = Matrix4x4.Scale(Vector3.one * (Screen.width / OriginalScreenWidth));

    styleFont ??= new GUIStyle(GUI.skin.label)
    {
      alignment = TextAnchor.UpperLeft,
      fontStyle = FontStyle.Bold,
      fontSize = 28
    };

    styleButton ??= new GUIStyle(GUI.skin.button)
    {
      fontStyle = FontStyle.Bold,
      fontSize = 28
    };

    styleLogo ??= new GUIStyle(GUI.skin.label)
    {
      alignment = TextAnchor.MiddleCenter,
      fontStyle = FontStyle.Bold,
      fontSize = 42
    };

    if (volume != null)
    {
      GUILayout.BeginVertical("box", GUILayout.Width(BoxWidth), GUILayout.ExpandHeight(true));
      {
        GUILayout.Space(Margin);
      
        GUILayout.BeginHorizontal();
        {
          GUILayout.FlexibleSpace();
          GUILayout.Label("Retro: Handheld 8-Bit", styleLogo);
          GUILayout.FlexibleSpace();
        }
        GUILayout.EndHorizontal();

        scrollView = GUILayout.BeginScrollView(scrollView);
        {
          GUILayout.Space(Margin * 0.5f);

          volume.intensity.value = Slider("Intensity", volume.intensity.value, 0.0f, 1.0f);

          volume.pixelSize.value = Slider("Pixel Size", volume.pixelSize.value, 1.0f, 20.0f);
          volume.subPixel.value = Slider("SubPixel", volume.subPixel.value, 0.1f, 2.0f);
          volume.shadowSize.value = Slider("Shadow Size", volume.shadowSize.value, 0.0f, 20.0f);
          volume.gamma.value = Slider("Gamma", volume.gamma.value, 0.1f, 10.0f);
          
          volume.luminosity.value = Slider("Luminosity", volume.luminosity.value, 0.0f, 1.0f);
          volume.threshold.value = Slider("Threshold", volume.threshold.value, 0.0f, 2.0f);

          GUILayout.Space(Margin);

          volume.brightness.value = Slider("Brightness", volume.brightness.value, -1.0f, 1.0f);
          volume.contrast.value = Slider("Contrast", volume.contrast.value, 0.0f, 10.0f);
          volume.hue.value = Slider("HUE", volume.hue.value, 0.0f, 1.0f);
          volume.saturation.value = Slider("Saturation", volume.saturation.value, 0.0f, 2.0f);

          GUILayout.Space(Margin);
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

        GUILayout.Space(Margin * 2.0f);
      }
      GUILayout.EndVertical();
    }
    else
      GUILayout.Label($"URP not available or '{Constants.Asset.Name}' is not correctly configured, please consult the documentation", styleLogo);

    GUI.matrix = guiMatrix;
  }
}
