///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Fronkon Games @FronkonGames <fronkongames@gmail.com>. All rights reserved.
//
// THIS FILE CAN NOT BE HOSTED IN PUBLIC REPOSITORIES.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FronkonGames.Artistic.OneBit
{
  /// <summary> Gradient search tool on colorlovers.com. </summary>
  public class ColourLoversSearch : EditorWindow
  {
    private string searchText;
    private string previusSearchText;

    private readonly List<Palette> colorEntries = new();

    private int pageCurrent = 0;
    private int pageCount = 0;

    private static MethodInfo repaintInspectors = null;

    public static void ShowTool()
    {
      EditorWindow window = GetWindow(typeof(ColourLoversSearch), false, "Gradient Search Tool");
      window.minSize = new Vector2(800.0f, 600.0f);
    }

    private void SearchGUI()
    {
      Inspector.Separator();

      GUILayout.BeginHorizontal(GUILayout.Height(20));
      {
        Inspector.Separator(10);

        searchText = GUILayout.TextField(searchText, GUILayout.ExpandWidth(true), GUILayout.Height(18));

        if (Event.current.isKey == true && Event.current.keyCode == KeyCode.Return && string.IsNullOrEmpty(searchText) == false)
        {
          SearchColors();

          this.Repaint();
        }

        Inspector.Separator(5);

        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Search Icon"), GUILayout.Width(55), GUILayout.Height(18)) == true)
          SearchColors();

        Inspector.Separator(5);

        if (GUILayout.Button("random", GUILayout.Width(55), GUILayout.Height(18)) == true)
        {
          colorEntries.Clear();
          pageCurrent = pageCount = 0;

          for (int i = 0; i < 5; ++i)
            colorEntries.Add(ColourLovers.Random());
        }

        Inspector.Separator(10);
      }
      GUILayout.EndHorizontal();
    }

    private void ColorGUI()
    {
      GUILayout.BeginVertical("box", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
      {
        if (colorEntries.Count == 0)
          Inspector.FlexibleSpace();
        else
        {
          for (int i = 0; i < colorEntries.Count; ++i)
          {
            if (colorEntries[i].colors != null && colorEntries[i].colors.Length == 5)
              ColorEntryGUI(colorEntries[i]);
          }
        }
      }
      GUILayout.EndVertical();
    }

    private void ColorEntryGUI(Palette colorEntry)
    {
      GUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.MaxHeight(200));
      {
        GUILayout.BeginVertical();
        {
          GUILayout.Label($"'{colorEntry.tittle}' by {colorEntry.author}");

          GUILayout.BeginHorizontal();
          {
            for (int i = 0; i < 5; ++i)
            {
              GUILayout.BeginVertical();
              {
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                {
                }
                GUILayout.EndHorizontal();

                Rect rect = GUILayoutUtility.GetLastRect();
                GUI.DrawTexture(rect, MakeTexture((int)rect.width, (int)rect.height, colorEntry.colors[i]));

                GUILayout.Label($"#{ColorUtility.ToHtmlStringRGB(colorEntry.colors[i])}");
              }
              GUILayout.EndVertical();
            }
          }
          GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        Inspector.Separator();

        GUILayout.BeginVertical();
        {
          Inspector.FlexibleSpace();

          if (GUILayout.Button("Use") == true)
          {
            UniversalRenderPipelineAsset pipeline = ((UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline);
            FieldInfo propertyInfo = pipeline.GetType().GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);
            if (propertyInfo != null)
            {
              ScriptableRendererData[] scriptableRendererDatas = (ScriptableRendererData[])propertyInfo?.GetValue(pipeline);
              for (int i = 0; i < scriptableRendererDatas.Length; ++i)
              {
                for (int j = 0; j < scriptableRendererDatas[i].rendererFeatures.Count; ++j)
                {
                  if (scriptableRendererDatas[i].rendererFeatures[j] is OneBit onebit)
                  {
                    OneBit.Settings settings = onebit.settings;

                    System.Array.Sort(colorEntry.colors, Luminance.Compare);

                    settings.gradient = new Gradient()
                    {
                      colorKeys = new GradientColorKey[]
                      {
                        new(colorEntry.colors[0], 0.0f),
                        new(colorEntry.colors[1], 0.25f),
                        new(colorEntry.colors[2], 0.5f),
                        new(colorEntry.colors[3], 0.75f),
                        new(colorEntry.colors[4], 1.0f),
                      }
                    };

                    RepaintInspectors();
                  }
                }
              }
            }
          }

          Inspector.Separator(20);
        }
        GUILayout.EndVertical();
      }
      GUILayout.EndHorizontal();
    }

    private void CommandGUI()
    {
      GUILayout.BeginHorizontal();
      {
        Inspector.Label("powered by ColourLovers.com");

        Inspector.FlexibleSpace();

        if (pageCount > 1)
        {
          if (GUILayout.Button(EditorGUIUtility.IconContent("Animation.FirstKey"), GUILayout.Height(16)) == true)
          {
            pageCurrent = 0;

            SearchColors();
          }

          if (GUILayout.Button(EditorGUIUtility.IconContent("Profiler.PrevFrame"), GUILayout.Height(16)) == true && pageCurrent > 0)
          {
            pageCurrent--;

            SearchColors();
          }

          Inspector.Separator();

          GUILayout.Label($"{pageCurrent} / {pageCount}");

          Inspector.Separator();

          if (GUILayout.Button(EditorGUIUtility.IconContent("Profiler.NextFrame"), GUILayout.Height(16)) == true && pageCurrent < pageCount)
          {
            pageCurrent++;

            SearchColors();
          }

          if (GUILayout.Button(EditorGUIUtility.IconContent("Animation.LastKey"), GUILayout.Height(16)) == true)
          {
            pageCurrent = pageCount;

            SearchColors();
          }
        }

        Inspector.Separator();
      }
      GUILayout.EndHorizontal();

      Inspector.Separator();
    }

    private void OnGUI()
    {
      GUILayout.BeginVertical();
      {
        SearchGUI();

        ColorGUI();

        CommandGUI();
      }
      GUILayout.EndVertical();
    }

    private void SearchColors()
    {
      colorEntries.Clear();
      if (previusSearchText != searchText)
      {
        pageCurrent = 0;
        previusSearchText = searchText;
      }

      colorEntries.AddRange(ColourLovers.Search(pageCurrent, searchText, out int totalResults));
      pageCount = totalResults / 5;
    }

    private Texture2D MakeTexture(int width, int height, Color col)
    {
      Color[] pix = new Color[width * height];

      for (int i = 0; i < pix.Length; ++i)
        pix[i] = col;

      Texture2D result = new(width, height, TextureFormat.RGB24, false);
      result.SetPixels(pix);
      result.Apply();

      return result;
    }

    private void RepaintInspectors()
    {
      if (repaintInspectors == null)
      {
        var inspectorWindow = typeof(EditorApplication).Assembly.GetType("UnityEditor.InspectorWindow");
        repaintInspectors = inspectorWindow.GetMethod("RepaintAllInspectors", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic );
      }

      repaintInspectors.Invoke(null, null);
    }
  }
}