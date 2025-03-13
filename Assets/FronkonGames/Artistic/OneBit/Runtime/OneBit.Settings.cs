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
using UnityEngine.Rendering.Universal;

namespace FronkonGames.Artistic.OneBit
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Settings. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class OneBit
  {
    /// <summary> Settings. </summary>
    [Serializable]
    public sealed class Settings
    {
      public Settings() => ResetDefaultValues();

      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      #region Common settings.

      /// <summary> Controls the intensity of the effect [0, 1]. Default 1. </summary>
      /// <remarks> An effect with Intensity equal to 0 will not be executed. </remarks>
      public float intensity = 1.0f;
      #endregion
      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      #region One Bit settings.

      /// <summary> Edges strength [0, 10]. Default 0.5. </summary>
      public float edges = 0.5f;

      /// <summary> Noise strength [0, 10]. Default 0.25. </summary>
      public float noiseStrength = 0.25f;

      /// <summary> Noise seed [0, 5]. Default 1.7. </summary>
      public float noiseSeed = 1.7f;

      /// <summary> Sets color blend mode. Default Multiply. </summary>
      public ColorBlends blendMode = ColorBlends.Multiply;

      /// <summary> Sets color mode (Solid, Gradient, Horizontal, Vertical or Circular). Default Solid. </summary>
      public ColorModes colorMode = ColorModes.Solid;

      /// <summary> Tint color. </summary>
      /// <remarks> Used in ColorModes.Solid color mode. </remarks>
      public Color color = Color.white;

      /// <summary> Tint color 1. </summary>
      /// <remarks> Used in ColorModes.Horizontal, ColorModes.Vertical and ColorModes.Circular color modes. </remarks>
      public Color color0 = Color.white;

      /// <summary> Tint color 2. </summary>
      /// <remarks> Used in ColorModes.Horizontal, ColorModes.Vertical and ColorModes.Circular color modes. </remarks>
      public Color color1 = Color.white;

      /// <summary> Color gradient. </summary>
      /// <remarks> Used in ColorModes.Gradient color mode. </remarks>
      public Gradient gradient = DefaultGradient;

      /// <summary> Minimum luminance value that is taken into account in the color mode Gradient [0, 1]. Default 0. </summary>
      /// <remarks> Should be less than LuminanceMax. Used in ColorModes.Gradient color mode. </remarks>
      public float luminanceMin = 0.0f;

      /// <summary> Maximum luminance value that is taken into account in the color mode Gradient [0, 1]. Default 1. </summary>
      /// <remarks> It must be greater than LuminanceMin. Used in ColorModes.Gradient color mode. </remarks>
      public float luminanceMax = 1.0f;

      /// <summary> Gradient radius [0, 10]. Default 2. </summary>
      /// <remarks> Used in ColorModes.Circular color mode. </remarks>
      public float circularRadius = 2.0f;

      /// <summary> Gradient offset horizontal [0, 2]. Default 1. </summary>
      /// <remarks> Used in ColorModes.Horizontal color mode. </remarks>
      public float horizontalOffset = 1.0f;

      /// <summary> Gradient offset vertical [0, 2]. Default 1. </summary>
      /// <remarks> Used in ColorModes.Vertical gradient. </remarks>
      public float verticalOffset = 1.0f;

      /// <summary> Red channel color levels [0, 255]. Default 255. </summary>
      public int redCount = 255;

      /// <summary> Green channel color levels [0, 255]. Default 255. </summary>
      public int greenCount = 255;

      /// <summary> Blue channel color levels [0, 255]. Default 255. </summary>
      public int blueCount = 255;

      /// <summary> Invert the color. </summary>
      public bool invertColor = false;
      #endregion
      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      #region Color settings.

      /// <summary> Brightness [-1.0, 1.0]. Default 0. </summary>
      public float brightness = 0.0f;

      /// <summary> Contrast [0.0, 10.0]. Default 1. </summary>
      public float contrast = 1.0f;

      /// <summary> Gamma [0.1, 10.0]. Default 1. </summary>
      public float gamma = 1.0f;

      /// <summary> The color wheel [0.0, 1.0]. Default 0. </summary>
      public float hue = 0.0f;

      /// <summary> Intensity of a colors [0.0, 2.0]. Default 1. </summary>
      public float saturation = 1.0f;
      #endregion
      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      #region Advanced settings.
#if !UNITY_6000_0_OR_NEWER
      /// <summary> Does it affect the Scene View? </summary>
      public bool affectSceneView = false;

      /// <summary> Enable render pass profiling. </summary>
      public bool enableProfiling = false;

      /// <summary> Filter mode. Default Bilinear. </summary>
      public FilterMode filterMode = FilterMode.Bilinear;
#endif
      /// <summary> Render pass injection. Default BeforeRenderingPostProcessing. </summary>
      public RenderPassEvent whenToInsert = RenderPassEvent.BeforeRenderingPostProcessing;
      #endregion
      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      public static readonly Gradient DefaultGradient = new()
      {
        colorKeys = new GradientColorKey[]
        {
          new(Color.white * 0.0f, 0.0f),
          new(Color.white * 0.33f, 0.2f),
          new(Color.white * 0.66f, 0.5f),
          new(Color.white * 1.0f, 1.0f)
        }
      };

      // Internal use.
      public bool forceGradientTextureUpdate;

      /// <summary> Reset to default values. </summary>
      public void ResetDefaultValues()
      {
        intensity = 1.0f;

        edges = 0.5f;
        noiseStrength = 0.25f;
        noiseSeed = 1.7f;
        blendMode = ColorBlends.Multiply;
        colorMode = ColorModes.Solid;
        color = color0 = color1 = Color.white;
        gradient = new Gradient() { colorKeys = DefaultGradient.colorKeys };
        luminanceMin = 0.0f;
        luminanceMax = 1.0f;
        circularRadius = 2.0f;
        horizontalOffset = 1.0f;
        verticalOffset = 1.0f;
        redCount = greenCount = blueCount = 255;
        invertColor = false;

        brightness = 0.0f;
        contrast = 1.0f;
        gamma = 1.0f;
        hue = 0.0f;
        saturation = 1.0f;

#if !UNITY_6000_0_OR_NEWER
        affectSceneView = false;
        enableProfiling = false;
        filterMode = FilterMode.Bilinear;
#endif
        whenToInsert = RenderPassEvent.BeforeRenderingPostProcessing;
      }
    }
  }
}
