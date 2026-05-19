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
using UnityEngine.Rendering;

namespace FronkonGames.Retro.ASCII
{
  /// <summary> Ascii charset parameter. </summary>
  [System.Serializable]
  public sealed class ASCIICharsetParameter : VolumeParameter<ASCIICharset>
  {
    public ASCIICharsetParameter(ASCIICharset value, bool overrideState = false) : base(value, overrideState) { }
  }

  /// <summary> Ascii charset asset. </summary>
  public sealed class ASCIICharset : ScriptableObject
  {
    /// <summary> Texture with characters </summary>
    [SerializeField]
    public Texture2D texture;

    /// <summary> Character count. </summary>
    [SerializeField]
    public int characterCount;

    /// <summary> Shape vectors texture for shape-aware selection (2x3 grid = 6 values per character). </summary>
    /// <remarks> 
    /// Each character has a 6-dimensional shape vector stored as 2 pixels (RGB each).
    /// Texture dimensions: (characterCount * 2) x 1, where each pair of pixels represents one character.
    /// First pixel RGB = top-left, top-right, mid-left densities.
    /// Second pixel RGB = mid-right, bottom-left, bottom-right densities.
    /// </remarks>
    [SerializeField]
    public Texture2D shapeVectorTexture;

    /// <summary> Average density per character for combined luminance+shape matching. </summary>
    [SerializeField]
    public float[] characterDensities;

    /// <summary> Returns true if shape-aware data is available. </summary>
    public bool HasShapeData => shapeVectorTexture != null && characterDensities != null && characterDensities.Length == characterCount;
  }
}
