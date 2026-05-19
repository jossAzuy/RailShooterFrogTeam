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

namespace FronkonGames.Retro.VHS
{
  /// <summary> Qualities. </summary>
  public enum Quality
  {
    /// <summary> Better performance but worse graphic quality. Recommended for old mobile devices. </summary>
    Performant,
    
    /// <summary> More similar to a real VHS. Default option. </summary>
    HighFidelity,
  }

  /// <summary> Resolutions. </summary>
  public enum Resolution
  {
    /// <summary> Same resolution. </summary>
    Same,
    
    /// <summary> Half. </summary>
    Half,
    
    /// <summary> One quarter. Default option. </summary>
    Quarter,
    
    /// <summary> One eighth. </summary>
    Eighth,
    
    /// <summary> One sixteenth. </summary>
    Sixteenth
  }
}
