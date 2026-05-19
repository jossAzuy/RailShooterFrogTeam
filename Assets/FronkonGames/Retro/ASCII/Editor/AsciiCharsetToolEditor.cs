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
using System.Collections.Generic;

namespace FronkonGames.Retro.ASCII
{
  /// <summary> Tool to generate Charsets. </summary>
  public class AsciiCharsetToolEditor : EditorWindow
  {
    private enum AsciiPatterns
    {
      Ascii_3,
      Ascii_5,
      Ascii_6,
      Ascii_8,
      Ascii_9,
      Ascii_10,
      Ascii_15,
      Ascii_16,
      Ascii_26,

      Extended_4,
      
      Custom
    }

    private Material MaterialText
    {
      get
      {
        if (materialText == null)
          materialText = new Material(Shader.Find("GUI/Text Shader")) { hideFlags = HideFlags.HideAndDontSave };

        return materialText;
      }
    }

    private readonly List<string> fontsAvailable = new();

    private int fontSelected;

    private int fontSize = 16;

    private AsciiPatterns asciiPattern = AsciiPatterns.Ascii_8;

    private string characters;

    private Material materialText;

    private readonly FontMesh fontMesh = new();

    private const string KeyDefaultAssetPath = "FronkonGames.Retro.Ascii.DefaultAssetPath";
    private string defaultAssetPath;

    [MenuItem("Window/Fronkon Games/Retro/ASCII/ASCII Charset Tool")]
    public static void Launch()
    {
      AsciiCharsetToolEditor asciiFontEditor = GetWindow<AsciiCharsetToolEditor>();
      asciiFontEditor.titleContent = new GUIContent("ASCII Charset Tool");
      asciiFontEditor.ShowUtility();
    }

    private void RefreshFonts()
    {
      fontsAvailable.Clear();

      string[] fonts = Font.GetOSInstalledFontNames();
      if (fonts.Length > 0)
      {
        for (int i = 0; i < fonts.Length; ++i)
        {
          EditorUtility.DisplayProgressBar("Searching monospaced fonts.", fonts[i], (float)i / fonts.Length);

          if (IsFontMonospaced(fonts[i]) == true)
            fontsAvailable.Add(fonts[i]);
        }
      }
      else
        Debug.LogWarning($"[{Constants.Asset.AssemblyName}] No fonts detected.");

      if (fonts.Length < fontSelected)
        fontSelected = 0;

      EditorUtility.ClearProgressBar();
    }

    private bool IsFontMonospaced(string fontName)
    {
      bool isMonospaced = false;
      const int fontSize = 14;

      Font font = Font.CreateDynamicFontFromOSFont(fontName, fontSize);
      if (font != null)
      {
        const string testCharacters = "XiW _i:";

        font.RequestCharactersInTexture(testCharacters, fontSize, FontStyle.Normal);

        int index = 0;
        int characterWidth = -1;
        isMonospaced = true;

        while (index < testCharacters.Length && isMonospaced == true)
        {
          if (font.GetCharacterInfo(testCharacters[index], out var characterInfo) == true)
          {
            if (characterWidth == -1)
              characterWidth = characterInfo.advance;
            else
              isMonospaced = characterInfo.advance > 0 && characterInfo.advance == characterWidth;
          }
          else
          {
            Debug.LogWarning($"[{Constants.Asset.AssemblyName}] Error when requesting information of a character in the font '{fontName}'.");

            isMonospaced = false;
          }

          index++;
        }

        DestroyImmediate(font);
      }
      else
        Debug.LogWarning($"[{Constants.Asset.AssemblyName}] Error creating font '{fontName}'.");

      return isMonospaced;
    }

    private CharacterInfo GetCharacterInfo(Font font, char character, int size)
    {
      font.RequestCharactersInTexture(character.ToString(), size, FontStyle.Normal);

      bool result = font.GetCharacterInfo(character, out CharacterInfo characterInfo);

      return result == true ? characterInfo : new CharacterInfo();
    }

    private string GetAsciiPattern(AsciiPatterns pattern)
    {
      string chars = string.Empty;

      switch (pattern)
      {
        case AsciiPatterns.Ascii_3:  chars = ":+#"; break;
        case AsciiPatterns.Ascii_5:  chars = ".:oO8"; break;
        case AsciiPatterns.Ascii_6:  chars = ".:oO8@"; break;
        case AsciiPatterns.Ascii_8:  chars = ".:coCO8@"; break;
        case AsciiPatterns.Ascii_9:  chars = ".:-=+*#%@"; break;
        case AsciiPatterns.Ascii_10: chars = ".:%oO$8@#M"; break;
        case AsciiPatterns.Ascii_15: chars = ".':!+ijY6XbKHNM"; break;
        case AsciiPatterns.Ascii_16: chars = ".:^\"~cso*wSO8Q0#"; break;
        case AsciiPatterns.Ascii_26: chars = ".'~:;!>+=icjtJY56SXDQKHNWM"; break;
        
        case AsciiPatterns.Extended_4: chars = "░▒▓█"; break;
      }

      return chars;
    }

    private void CreateAsciiCharset(string fontName, string filePath)
    {
      Font font = Font.CreateDynamicFontFromOSFont(fontName, fontSize);
      
      if (font != null && characters.Length > 0)
      {
        int characterWidth = 0;
        int characterHeight = 0;

        for (int i = 0; i < characters.Length; ++i)
        {
          CharacterInfo charInfo = GetCharacterInfo(font, characters[i], fontSize);

          if (characterWidth < charInfo.advance)
            characterWidth = charInfo.advance;

          if (characterHeight < charInfo.glyphHeight)
            characterHeight = charInfo.glyphHeight;
        }

        int textureWidth = characters.Length * characterWidth;

        Texture2D grabTexture = new(textureWidth, characterHeight, TextureFormat.RGB24, false) { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };
        RenderTexture renderTexture = RenderTexture.GetTemporary(textureWidth, characterHeight, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        RenderTexture originalRenderTexture = RenderTexture.active;

        RenderTexture.active = renderTexture;

        GL.PushMatrix();
        GL.Clear(true, true, Color.black);
        GL.LoadPixelMatrix(0, textureWidth, 0, characterHeight);

        font.RequestCharactersInTexture(characters, fontSize);
        font.material.mainTexture.filterMode = FilterMode.Point;
        MaterialText.mainTexture = font.material.mainTexture;
        MaterialText.color = Color.white;

        fontMesh.BuildTextMesh(characters, font, characterHeight, fontSize);

        MaterialText.SetPass(0);

        Graphics.DrawMeshNow(fontMesh.Mesh, new Vector3(0.0f, 0.0f, -1.0f), Quaternion.identity);

        grabTexture.ReadPixels(new Rect(0, 0, textureWidth, characterHeight), 0, 0);
        grabTexture.Apply(false);

        GL.PopMatrix();

        RenderTexture.active = originalRenderTexture;

        RenderTexture.ReleaseTemporary(renderTexture);

        // Generate shape vectors for shape-aware selection
        Texture2D shapeVectorTexture;
        float[] characterDensities;
        ComputeShapeVectors(grabTexture, characterWidth, characterHeight, characters.Length, out shapeVectorTexture, out characterDensities);

        ASCIICharset asset = CreateInstance<ASCIICharset>();
        asset.texture = grabTexture;
        asset.characterCount = characters.Length;
        asset.shapeVectorTexture = shapeVectorTexture;
        asset.characterDensities = characterDensities;

        AssetDatabase.CreateAsset(asset, ToRelativePath(filePath));
        AssetDatabase.AddObjectToAsset(grabTexture, asset);
        AssetDatabase.AddObjectToAsset(shapeVectorTexture, asset);
        AssetDatabase.SaveAssets();
      }
    }

    /// <summary>
    /// Computes shape vectors for each character in the charset.
    /// Each character is divided into a 2x3 grid (6 regions), and we compute the average luminance in each region.
    /// The shape vectors are stored in a texture where each character uses 2 pixels (RGB each = 6 values total).
    /// </summary>
    private void ComputeShapeVectors(Texture2D charsetTexture, int charWidth, int charHeight, int charCount, out Texture2D shapeVectorTexture, out float[] characterDensities)
    {
      // Each character needs 2 pixels (6 values total for 2x3 grid)
      shapeVectorTexture = new Texture2D(charCount * 2, 1, TextureFormat.RGBAFloat, false)
      {
        filterMode = FilterMode.Point,
        wrapMode = TextureWrapMode.Clamp,
        name = "ShapeVectors"
      };

      characterDensities = new float[charCount];

      Color[] pixels = charsetTexture.GetPixels();
      Color[] shapePixels = new Color[charCount * 2];

      // Region dimensions (2 columns x 3 rows)
      float regionWidth = charWidth / 2.0f;
      float regionHeight = charHeight / 3.0f;

      for (int charIndex = 0; charIndex < charCount; charIndex++)
      {
        int charStartX = charIndex * charWidth;

        // Sample 6 regions and compute average luminance for each
        float[] regionLuminances = new float[6];
        
        for (int row = 0; row < 3; row++)
        {
          for (int col = 0; col < 2; col++)
          {
            int regionIndex = row * 2 + col;
            
            int startX = charStartX + Mathf.FloorToInt(col * regionWidth);
            int endX = charStartX + Mathf.FloorToInt((col + 1) * regionWidth);
            int startY = Mathf.FloorToInt(row * regionHeight);
            int endY = Mathf.FloorToInt((row + 1) * regionHeight);

            float totalLuminance = 0;
            int sampleCount = 0;

            for (int y = startY; y < endY && y < charHeight; y++)
            {
              for (int x = startX; x < endX && x < charsetTexture.width; x++)
              {
                int pixelIndex = y * charsetTexture.width + x;
                if (pixelIndex < pixels.Length)
                {
                  Color pixel = pixels[pixelIndex];
                  // Rec. 601 luminance
                  float luminance = 0.299f * pixel.r + 0.587f * pixel.g + 0.114f * pixel.b;
                  totalLuminance += luminance;
                  sampleCount++;
                }
              }
            }

            regionLuminances[regionIndex] = sampleCount > 0 ? totalLuminance / sampleCount : 0;
          }
        }

        // Store in shape vector texture (2 pixels per character)
        // First pixel: top-left, top-right, mid-left (indices 0, 1, 2)
        shapePixels[charIndex * 2] = new Color(regionLuminances[0], regionLuminances[1], regionLuminances[2], 1.0f);
        // Second pixel: mid-right, bottom-left, bottom-right (indices 3, 4, 5)
        shapePixels[charIndex * 2 + 1] = new Color(regionLuminances[3], regionLuminances[4], regionLuminances[5], 1.0f);

        // Compute overall density (average of all 6 regions)
        characterDensities[charIndex] = (regionLuminances[0] + regionLuminances[1] + regionLuminances[2] + 
                                         regionLuminances[3] + regionLuminances[4] + regionLuminances[5]) / 6.0f;
      }

      shapeVectorTexture.SetPixels(shapePixels);
      shapeVectorTexture.Apply(false);
    }

    private string ToRelativePath(string absPath)
    {
      string relPath = absPath;
      if (absPath.StartsWith(Application.dataPath) == true)
        relPath = $"Assets{absPath.Substring(Application.dataPath.Length)}";

      return relPath;
    }

    private void OnEnable()
    {
      defaultAssetPath = EditorPrefs.GetString(KeyDefaultAssetPath, "Assets/");

      RefreshFonts();
    }

    private void OnGUI()
    {
      GUILayout.BeginVertical("box");
      {
        EditorGUILayout.Separator();

        if (fontsAvailable.Count > 0)
        {
          GUILayout.BeginHorizontal();
          {
            fontSelected = EditorGUILayout.Popup("Font", fontSelected, fontsAvailable.ToArray());

            if (GUILayout.Button("Refresh") == true)
              RefreshFonts();
          }
          GUILayout.EndHorizontal();

          fontSize = EditorGUILayout.IntSlider(new GUIContent("Size", "Font size"), fontSize, 1, 72);

          asciiPattern = (AsciiPatterns)EditorGUILayout.EnumPopup("Pattern", asciiPattern);

          if (asciiPattern == AsciiPatterns.Custom)
          {
            characters = EditorGUILayout.TextArea(characters);

            EditorGUILayout.HelpBox("Sort the characters from least bright (eg '.') to most (eg '#').", MessageType.Info);
          }
          else
          {
            GUI.enabled = false;

            characters = GetAsciiPattern(asciiPattern);

            EditorGUILayout.TextArea(characters);
          }

          EditorGUILayout.Separator();

          GUI.enabled = string.IsNullOrEmpty(characters) == false;

          if (GUILayout.Button("Create ASCII Charset") == true)
          {
            defaultAssetPath = EditorUtility.SaveFilePanel("Create new ASCII Charset",
                                                           defaultAssetPath,
                                                           $"{fontsAvailable[fontSelected]}_{fontSize}",
                                                           "asset");
            if (string.IsNullOrEmpty(defaultAssetPath) == false)
            {
              EditorPrefs.SetString(KeyDefaultAssetPath, defaultAssetPath);

              CreateAsciiCharset(fontsAvailable[fontSelected], defaultAssetPath);
            }
          }

          GUI.enabled = true;

          EditorGUILayout.Separator();

          GUILayout.FlexibleSpace();
        }
        else
        {
          if (GUILayout.Button("Refresh") == true)
            RefreshFonts();

          GUILayout.FlexibleSpace();

          EditorGUILayout.HelpBox("No monospaced font available. Install one and press 'Refresh' please.", MessageType.Warning);
        }

        EditorGUILayout.Separator();
      }
      GUILayout.EndVertical();
    }    
  }
}