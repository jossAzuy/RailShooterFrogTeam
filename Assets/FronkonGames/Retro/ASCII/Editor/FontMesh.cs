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
using System.Collections.Generic;

namespace FronkonGames.Retro.ASCII
{
  /// <summary> Font mesh. </summary>
  internal class FontMesh
  {
    public Mesh Mesh
    {
      get
      {
        if (mesh == null)
        {
          mesh = new Mesh();
          mesh.hideFlags = HideFlags.HideAndDontSave;
        }

        return mesh;
      }
    }

    private Mesh mesh;

    private readonly List<int> tris = new();
    private readonly List<Vector3> verts = new();
    private readonly List<Vector2> uvs = new();

    private static readonly int[] DefaultTris = { 0, 3, 2, 0, 2, 1 };

    public void BuildTextMesh(string characters, Font font, int height, int fontSize)
    {
      int advance = 0;

      verts.Clear();
      tris.Clear();
      uvs.Clear();
      Mesh.Clear();

      // HACK: Get y-offset using the lowest character: 'j'.
      font.RequestCharactersInTexture("j", fontSize, FontStyle.Normal);
      font.GetCharacterInfo('j', out var jCharInfo, fontSize, FontStyle.Normal);
      float yOffset = fontSize + jCharInfo.minY;

      for (int i = 0; i < characters.Length; ++i)
      {
        font.GetCharacterInfo(characters[i], out var charInfo, fontSize, FontStyle.Normal);

        // Vertices.
        verts.Add(new Vector3(advance + charInfo.minX, yOffset - charInfo.minY, 0.0f));
        verts.Add(new Vector3(advance + charInfo.maxX, yOffset - charInfo.minY, 0.0f));
        verts.Add(new Vector3(advance + charInfo.maxX, yOffset - charInfo.maxY, 0.0f));
        verts.Add(new Vector3(advance + charInfo.minX, yOffset - charInfo.maxY, 0.0f));

        // Triangles.
        int vertIndex = ((verts.Count / 4) - 1) * 4;
        for (int j = 0; j < 6; ++j)
          tris.Add(vertIndex + DefaultTris[j]);

        // UVs.
        uvs.Add(charInfo.uvTopLeft);
        uvs.Add(charInfo.uvTopRight);
        uvs.Add(charInfo.uvBottomRight);
        uvs.Add(charInfo.uvBottomLeft);

        advance += charInfo.advance;
      }

      Mesh.SetVertices(verts);
      Mesh.SetTriangles(tris, 0);
      Mesh.SetUVs(0, uvs);
    }
  }  
}
