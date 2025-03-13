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
using UnityEditor;
using UnityEngine;
using static FronkonGames.Artistic.OneBit.Inspector;

namespace FronkonGames.Artistic.OneBit
{
  /// <summary> Artistic One Bit inspector. </summary>
  [CustomPropertyDrawer(typeof(OneBit.Settings))]
  public class OneBitFeatureSettingsDrawer : Drawer
  {
    private OneBit.Settings settings;

    protected override void ResetValues() => settings?.ResetDefaultValues();

    protected override void InspectorGUI()
    {
      settings ??= GetSettings<OneBit.Settings>();

      /////////////////////////////////////////////////
      // Common.
      /////////////////////////////////////////////////
      settings.intensity = Slider("Intensity", "Controls the intensity of the effect [0, 1]. Default 1.", settings.intensity, 0.0f, 1.0f, 1.0f);

      /////////////////////////////////////////////////
      // One Bit.
      /////////////////////////////////////////////////
      Separator();

      settings.edges = Slider("Edges", "Edges strength [0, 10]. Default 0.5.", settings.edges, 0.0f, 10.0f, 0.5f);

      settings.noiseStrength = Slider("Noise", "Noise strength [0, 10]. Default 0.25.", settings.noiseStrength, 0.0f, 10.0f, 0.25f);
      IndentLevel++;
      settings.noiseSeed = Slider("Seed", "Noise seed [0, 5]. Default 1.7.", settings.noiseSeed, 0.0f, 5.0f, 1.7f);
      IndentLevel--;

      settings.blendMode = (ColorBlends)EnumPopup("Blend", "Color blend mode. Default Multiply.", settings.blendMode, ColorBlends.Multiply);
      settings.colorMode = (ColorModes)EnumPopup("Color", "Sets color mode (Solid, Gradient, Horizontal, Vertical or Circular). Default Solid.", settings.colorMode, ColorModes.Solid);

      IndentLevel++;

      switch (settings.colorMode)
      {
        case ColorModes.Solid:
          settings.color = ColorField("Color", "Used in Solid color mode.", settings.color, Color.white);
          break;
        case ColorModes.Gradient:
          BeginHorizontal();
          {
            float luminanceMin = settings.luminanceMin;
            float luminanceMax = settings.luminanceMax;

            EditorGUILayout.MinMaxSlider("Luminance", ref luminanceMin, ref luminanceMax, 0.0f, 1.0f);

            settings.luminanceMin = luminanceMin;
            settings.luminanceMax = luminanceMax;

            if (ResetButton() == true)
            {
              settings.luminanceMin = 0.0f;
              settings.luminanceMax = 1.0f;
            }
          }
          EndHorizontal();

          settings.gradient = GradientField("Gradient", "Color gradient mode", settings.gradient, OneBit.Settings.DefaultGradient);

          GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
          {
            FlexibleSpace();
#if false
            if (MiniButton("copy") == true)
            {
              GUIUtility.systemCopyBuffer = $"{settings.gradient.Evaluate(0.0f).ToString()}, " +
                                            $"{settings.gradient.Evaluate(0.25f).ToString()}, " +
                                            $"{settings.gradient.Evaluate(0.5f).ToString()}, " +
                                            $"{settings.gradient.Evaluate(0.75f).ToString()}, " +
                                            $"{settings.gradient.Evaluate(1.0f).ToString()}";
            }
#endif
            if (MiniButton("search") == true)
              ColourLoversSearch.ShowTool();

            Separator(19.0f);
          }
          EndHorizontal();
          break;
        case ColorModes.Horizontal:
          settings.horizontalOffset = Slider("Offset", "Horizontal offset.", settings.horizontalOffset, 0.0f, 2.0f, 1.0f);
          settings.color0 = ColorField("Color #1", "Used in horizontal, vertical and circular color modes.", settings.color0, Color.white);
          settings.color1 = ColorField("Color #2", "Used in horizontal, vertical and circular color modes.", settings.color1, Color.white);
          break;
        case ColorModes.Vertical:
          settings.verticalOffset = Slider("Offset", "Vertical offset.", settings.verticalOffset, 0.0f, 2.0f, 1.0f);
          settings.color0 = ColorField("Color #1", "Used in horizontal, vertical and circular color modes.", settings.color0, Color.white);
          settings.color1 = ColorField("Color #2", "Used in horizontal, vertical and circular color modes.", settings.color1, Color.white);
          break;
        case ColorModes.Circular:
          settings.circularRadius = Slider("Radius", "Gradient radius.", settings.circularRadius, 0.0f, 10.0f, 2.0f);
          settings.color0 = ColorField("Color #1", "Used in horizontal, vertical and circular color modes.", settings.color0, Color.white);
          settings.color1 = ColorField("Color #2", "Used in horizontal, vertical and circular color modes.", settings.color1, Color.white);
          break;
      }

      IndentLevel--;

      settings.redCount = Slider("Red count", "Red channel color levels [0, 255]. Default 255.", settings.redCount, 0, 255, 255);
      settings.greenCount = Slider("Green count", "Green channel color levels [0, 256]. Default 255.", settings.greenCount, 0, 255, 255);
      settings.blueCount = Slider("Blue count", "Blue channel color levels [0, 256]. Default 255.", settings.blueCount, 0, 255, 255);

      settings.invertColor = Toggle("Invert", "Invert the color.", settings.invertColor);

      /////////////////////////////////////////////////
      // Color.
      /////////////////////////////////////////////////
      Separator();

      if (Foldout("Color") == true)
      {
        IndentLevel++;

        settings.brightness = Slider("Brightness", "Brightness [-1.0, 1.0]. Default 0.", settings.brightness, -1.0f, 1.0f, 0.0f);
        settings.contrast = Slider("Contrast", "Contrast [0.0, 10.0]. Default 1.", settings.contrast, 0.0f, 10.0f, 1.0f);
        settings.gamma = Slider("Gamma", "Gamma [0.1, 10.0]. Default 1.", settings.gamma, 0.01f, 10.0f, 1.0f);
        settings.hue = Slider("Hue", "The color wheel [0.0, 1.0]. Default 0.", settings.hue, 0.0f, 1.0f, 0.0f);
        settings.saturation = Slider("Saturation", "Intensity of a colors [0.0, 2.0]. Default 1.", settings.saturation, 0.0f, 2.0f, 1.0f);

        IndentLevel--;
      }

      /////////////////////////////////////////////////
      // Advanced.
      /////////////////////////////////////////////////
      Separator();

      if (Foldout("Advanced") == true)
      {
        IndentLevel++;

#if !UNITY_6000_0_OR_NEWER
        settings.affectSceneView = Toggle("Affect the Scene View?", "Does it affect the Scene View?", settings.affectSceneView);
        settings.filterMode = (FilterMode)EnumPopup("Filter mode", "Filter mode. Default Bilinear.", settings.filterMode, FilterMode.Bilinear);
#endif
        settings.whenToInsert = (UnityEngine.Rendering.Universal.RenderPassEvent)EnumPopup("RenderPass event",
          "Render pass injection. Default BeforeRenderingPostProcessing.",
          settings.whenToInsert,
          UnityEngine.Rendering.Universal.RenderPassEvent.BeforeRenderingPostProcessing);
#if !UNITY_6000_0_OR_NEWER
        settings.enableProfiling = Toggle("Enable profiling", "Enable render pass profiling", settings.enableProfiling);
#endif

        IndentLevel--;
      }
    }

    protected override void InspectorChanged()
    {
      settings ??= GetSettings<OneBit.Settings>();

      settings.forceGradientTextureUpdate = true;
    }
  }
}
