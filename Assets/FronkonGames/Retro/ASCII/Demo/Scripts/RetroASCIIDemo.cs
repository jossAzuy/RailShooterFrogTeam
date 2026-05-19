using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using FronkonGames.Retro.ASCII;

/// <summary> Retro: ASCII demo. </summary>
/// <remarks>
/// This code is designed for a simple demo, not for production environments.
/// </remarks>
public class RetroASCIIDemo : MonoBehaviour
{
  [Header("This code is only for the demo, not for production environments.")]

  [Space(20.0f), SerializeField]
  private VolumeProfile volumeProfile;

  [SerializeField]
  private List<ASCIICharset> charsets = new();
  
  [Space]
  
  [SerializeField]
  private Transform floor;

  [SerializeField, Range(0.0f, 10.0f)]
  private float angularVelocity;
  
  private ASCIIVolume volume;

  private int selectedCharset = 0;
  
  private GUIStyle styleFont;
  private GUIStyle styleButton;
  private GUIStyle styleLogo;
  private Vector2 scrollView;

  private const float BoxWidth = 500.0f;
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
    if (ASCII.IsInRenderFeatures() == false)
    {
      Debug.LogWarning($"Effect '{Constants.Asset.Name}' not found. You must add it as a Render Feature.");
#if UNITY_EDITOR
      if (UnityEditor.EditorUtility.DisplayDialog($"Effect '{Constants.Asset.Name}' not found", $"You must add '{Constants.Asset.Name}' as a Render Feature.", "Quit") == true)
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    styleFont = styleButton = styleLogo = null;

    volume = volumeProfile != null && volumeProfile.TryGet(out ASCIIVolume vol) ? vol : null;
    this.enabled = ASCII.IsInRenderFeatures();
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
      fontSize = 30
    };

    if (volume != null)
    {
      GUILayout.BeginVertical("box", GUILayout.Width(BoxWidth), GUILayout.ExpandHeight(true));
      {
        GUILayout.Space(Margin);
      
        GUILayout.BeginHorizontal();
        {
          GUILayout.FlexibleSpace();
          GUILayout.Label("Retro: ASCII", styleLogo);
          GUILayout.FlexibleSpace();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(Margin * 0.5f);
        
        scrollView = GUILayout.BeginScrollView(scrollView);
        {
          volume.intensity.value = Slider("Intensity", volume.intensity.value, 0.0f, 1.0f);
          
          GUILayout.BeginHorizontal();
          {
            GUILayout.Space(Margin);
            
            if (GUILayout.Button("<<", styleButton) == true)
            {
              selectedCharset = selectedCharset > 0 ? selectedCharset - 1 : charsets.Count - 1;  
              if (charsets[selectedCharset] != null)
                volume.charset.value = charsets[selectedCharset];
            }
            
            GUILayout.FlexibleSpace();

            string charsetName = charsets[selectedCharset] != null 
              ? charsets[selectedCharset].ToString() 
              : "(None)";
            charsetName = charsetName.Replace(" (FronkonGames.Retro.ASCII.ASCIICharset)", "");
            charsetName = charsetName.Replace("_", " ");
            GUILayout.Label(charsetName, styleFont);

            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button(">>", styleButton) == true)
            {
              selectedCharset = selectedCharset < charsets.Count - 1 ? selectedCharset + 1 : 0;  
              if (charsets[selectedCharset] != null)
                volume.charset.value = charsets[selectedCharset];
            }
          }
          GUILayout.EndHorizontal();
          
          GUILayout.Space(Margin);

          volume.selectionMode.value = Toggle("Shape aware", volume.selectionMode.value == CharacterSelectionMode.ShapeAware) == true 
            ? CharacterSelectionMode.ShapeAware 
            : CharacterSelectionMode.Luminance;

          volume.edgeDetection.value = Toggle("Edge detection", volume.edgeDetection.value);

          volume.superSampling.value = Toggle("Supersampling", volume.superSampling.value);

          volume.zoom.value = Slider("Zoom", volume.zoom.value, 0.0f, 10.0f);
          volume.boost.value = Slider("Boost", volume.boost.value, 0.0f, 5.0f);
          volume.blockColor.value = Toggle("Block color", volume.blockColor.value);
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

        GUILayout.Space(20.0f);
      }
      GUILayout.EndVertical();
    }
    else
      GUILayout.Label($"URP not available or '{Constants.Asset.Name}' is not correctly configured, please consult the documentation", styleLogo);

    GUI.matrix = guiMatrix;
  }
}