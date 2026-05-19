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
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace FronkonGames.Retro.VHS
{
  /// <summary>
  /// Int slider with reset button.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field)]
  public sealed class IntSliderWithResetAttribute : PropertyAttribute
  {
    public readonly int defaultValue;
    public readonly int min;
    public readonly int max;
    public readonly string tooltip;

    public IntSliderWithResetAttribute(int defaultValue, int min, int max, string tooltip = null)
    {
      this.defaultValue = defaultValue;
      this.min = min;
      this.max = max;
      this.tooltip = tooltip;
    }
  }

  /// <summary>
  /// Float slider with reset button.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field)]
  public sealed class FloatSliderWithResetAttribute : PropertyAttribute
  {
    public readonly float defaultValue;
    public readonly float min;
    public readonly float max;
    public readonly string tooltip;

    public FloatSliderWithResetAttribute(float defaultValue, float min, float max, string tooltip = null)
    {
      this.defaultValue = defaultValue;
      this.min = min;
      this.max = max;
      this.tooltip = tooltip;
    }
  }

  /// <summary>
  /// Toggle with reset button.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field)]
  public sealed class ToggleWithResetAttribute : PropertyAttribute
  {
    public readonly bool defaultValue;
    public readonly string tooltip;

    public ToggleWithResetAttribute(bool defaultValue, string tooltip = null)
    {
      this.defaultValue = defaultValue;
      this.tooltip = tooltip;
    }
  }

  /// <summary>
  /// Enum dropdown.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field)]
  public sealed class EnumDropdownAttribute : PropertyAttribute
  {
    public readonly int defaultValue;
    public readonly string tooltip;

    public EnumDropdownAttribute(int defaultValue, string tooltip = null)
    {
      this.defaultValue = defaultValue;
      this.tooltip = tooltip;
    }
  }

  /// <summary>
  /// Vector3 slider with reset button.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field)]
  public sealed class Vector3SliderWithResetAttribute : PropertyAttribute
  {
    public readonly Vector3 defaultValue;
    public readonly float min;
    public readonly float max;
    public readonly string tooltip;

    public Vector3SliderWithResetAttribute(float x, float y, float z, float min, float max, string tooltip = null)
    {
      this.defaultValue = new Vector3(x, y, z);
      this.min = min;
      this.max = max;
      this.tooltip = tooltip;
    }
  }

  /// <summary>
  /// Vector2 slider with reset button.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field)]
  public sealed class Vector2SliderWithResetAttribute : PropertyAttribute
  {
    public readonly Vector2 defaultValue;
    public readonly float min;
    public readonly float max;
    public readonly string tooltip;

    public Vector2SliderWithResetAttribute(float x, float y, float min, float max, string tooltip = null)
    {
      this.defaultValue = new Vector2(x, y);
      this.min = min;
      this.max = max;
      this.tooltip = tooltip;
    }
  }

  /// <summary>
  /// Color with reset button.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field)]
  public sealed class ColorWithResetAttribute : PropertyAttribute
  {
    public readonly Color defaultValue;
    public readonly string tooltip;

    public ColorWithResetAttribute(uint color, string tooltip = null)
    {
      this.defaultValue = ColorFromUInt(color);
      this.tooltip = tooltip;
    }

    private static Color ColorFromUInt(uint color)
    {
      return new Color(
          (color >> 24 & 0xFF) / 255.0f,
          (color >> 16 & 0xFF) / 255.0f,
          (color >> 8 & 0xFF) / 255.0f,
          (color & 0xFF) / 255.0f
      );
    }
  }

  /// <summary>
  /// Enum parameter for Volume.
  /// </summary>
  [Serializable]
  public sealed class EnumParameter<T> : VolumeParameter<T> where T : Enum
  {
    public EnumParameter(T value, bool overrideState = false) : base(value, overrideState) { }
  }

  /// <summary>
  /// Vector2Int parameter for Volume.
  /// </summary>
  [Serializable]
  public sealed class Vector2IntParameter : VolumeParameter<Vector2Int>
  {
    public Vector2IntParameter(Vector2Int value, bool overrideState = false) : base(value, overrideState) { }
  }
}
