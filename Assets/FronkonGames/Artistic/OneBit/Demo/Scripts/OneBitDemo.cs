using System;
using UnityEngine;
using FronkonGames.Artistic.OneBit;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(OneBitDemo))]
public class OneBitDemoWarning : Editor
{
  private GUIStyle Style => style ??= new GUIStyle(GUI.skin.GetStyle("HelpBox")) { richText = true, fontSize = 14, alignment = TextAnchor.MiddleCenter };
  private GUIStyle style;
  public override void OnInspectorGUI() =>
    EditorGUILayout.TextArea($"\nThis code is only for the demo\n\n<b>DO NOT USE</b> it in your projects\n\nIf you have any questions,\ncheck the <a href='{Constants.Support.Documentation}'>online help</a> or use the <a href='mailto:{Constants.Support.Email}'>support email</a>,\n<b>thanks!</b>\n", Style);
}
#endif

/// <summary> Artistic: One bit demo. </summary>
/// <remarks> This code is designed for a simple demo, not for production environments. </remarks>
public class OneBitDemo : MonoBehaviour
{
  private OneBit.Settings settings;

  private GUIStyle styleTitle;
  private GUIStyle styleLabel;
  private GUIStyle styleButton;
  private GUIStyle styleColor;

  private void Awake()
  {
    if (OneBit.IsInRenderFeatures() == false)
    {
      Debug.LogWarning($"Effect '{Constants.Asset.Name}' not found. You must add it as a Render Feature.");
#if UNITY_EDITOR
      if (EditorUtility.DisplayDialog($"Effect '{Constants.Asset.Name}' not found", $"You must add '{Constants.Asset.Name}' as a Render Feature.", "Quit") == true)
        EditorApplication.isPlaying = false;
#endif
    }

    this.enabled = OneBit.IsInRenderFeatures();
  }

  private void Start()
  {
    settings = OneBit.GetSettings();
    ResetDemo();
  }

  private void OnGUI()
  {
    styleTitle = new GUIStyle(GUI.skin.label)
    {
      alignment = TextAnchor.LowerCenter,
      fontSize = 32,
      fontStyle = FontStyle.Bold
    };

    styleLabel = new GUIStyle(GUI.skin.label)
    {
      alignment = TextAnchor.UpperLeft,
      fontSize = 24
    };

    styleButton = new GUIStyle(GUI.skin.button)
    {
      fontSize = 24
    };

    styleColor = new GUIStyle(GUI.skin.button);
    styleColor.normal.background = styleColor.hover.background = Texture2D.whiteTexture;

    GUILayout.BeginHorizontal();
    {
      GUILayout.BeginVertical("box", GUILayout.Width(400.0f), GUILayout.Height(Screen.height));
      {
        const float space = 10.0f;

        GUILayout.Space(space);

        GUILayout.Label("ONE BIT DEMO", styleTitle);

        GUILayout.Space(space);

        settings.intensity = Slider("Intensity", settings.intensity);

        GUILayout.Space(space);

        settings.edges = Slider("Edges", settings.edges, 0.0f, 10.0f);
        settings.noiseStrength = Slider("Noise", settings.noiseStrength, 0.0f, 10.0f);
        settings.blendMode = Enum("Blend", settings.blendMode);

        settings.colorMode = Enum("Mode", settings.colorMode);
        switch (settings.colorMode)
        {
          case ColorModes.Solid:
            settings.color = Color("  Color", settings.color);
            break;
          case ColorModes.Gradient:
            break;
          case ColorModes.Horizontal:
            settings.horizontalOffset = Slider("  Offset", settings.horizontalOffset, 0.0f, 2.0f);
            settings.color0 = Color("  Color #0", settings.color0);
            settings.color1 = Color("  Color #1", settings.color1);
            break;
          case ColorModes.Vertical:
            settings.verticalOffset = Slider("  Offset", settings.verticalOffset, 0.0f, 2.0f);
            settings.color0 = Color("  Color #0", settings.color0);
            settings.color1 = Color("  Color #1", settings.color1);
            break;
          case ColorModes.Circular:
            settings.circularRadius = Slider("  Radius", settings.circularRadius, 0.0f, 10.0f);
            settings.color0 = Color("  Color #0", settings.color0);
            settings.color1 = Color("  Color #1", settings.color1);
            break;
        }

        settings.invertColor = Toogle("Invert", settings.invertColor);

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("RESET", styleButton) == true)
          ResetDemo();

        GUILayout.Space(4.0f);

        if (GUILayout.Button("ONLINE DOCUMENTATION", styleButton) == true)
          Application.OpenURL(Constants.Support.Documentation);

        GUILayout.Space(4.0f);

        if (GUILayout.Button("❤️ LEAVE A REVIEW ❤️", styleButton) == true)
          Application.OpenURL(Constants.Support.Store);

        GUILayout.Space(space * 2.0f);
      }
      GUILayout.EndVertical();

      GUILayout.FlexibleSpace();
    }
    GUILayout.EndHorizontal();
  }

  private void OnDestroy()
  {
    settings?.ResetDefaultValues();
  }

  private void ResetDemo()
  {
    settings.ResetDefaultValues();
    settings.luminanceMax = 0.7f;
  }

  private bool Toogle(string label, bool value)
  {
    GUILayout.BeginHorizontal();
    {
      GUILayout.Label(label, styleLabel);

      value = GUILayout.Toggle(value, string.Empty);
    }
    GUILayout.EndHorizontal();

    return value;
  }

  private float Slider(string label, float value, float min = 0.0f, float max = 1.0f)
  {
    GUILayout.BeginHorizontal();
    {
      GUILayout.Label(label, styleLabel);

      value = GUILayout.HorizontalSlider(value, min, max);
    }
    GUILayout.EndHorizontal();

    return value;
  }

  private int Slider(string label, int value, int min, int max)
  {
    GUILayout.BeginHorizontal();
    {
      GUILayout.Label(label, styleLabel);

      value = (int)GUILayout.HorizontalSlider(value, min, max);
    }
    GUILayout.EndHorizontal();

    return value;
  }

  private Color Color(string label, Color value, bool alpha = true)
  {
    GUILayout.BeginHorizontal();
    {
      GUILayout.Label(label, styleLabel);
      float originalAlpha = value.a;

      GUI.backgroundColor = value;

      if (GUILayout.Button(string.Empty, styleColor, GUILayout.Height(24.0f)) == true)
        value = NextColor(value);

      GUI.backgroundColor = UnityEngine.Color.white;

      if (alpha == false)
        value.a = originalAlpha;
    }
    GUILayout.EndHorizontal();

    return value;
  }

  private Vector3 Vector3(string label, Vector3 value, string x = "X", string y = "Y", string z = "Z", float min = 0.0f, float max = 1.0f)
  {
    GUILayout.Label(label, styleLabel);

    value.x = Slider($"   {x}", value.x, min, max);
    value.y = Slider($"   {y}", value.y, min, max);
    value.z = Slider($"   {z}", value.z, min, max);

    return value;
  }

  private T Enum<T>(string label, T value) where T : Enum
  {
    string[] names = System.Enum.GetNames(typeof(T));
    Array values = System.Enum.GetValues(typeof(T));
    int index = Array.IndexOf(values, value);

    GUILayout.BeginHorizontal();
    {
      GUILayout.Label(label, styleLabel);

      if (GUILayout.Button("<", styleButton) == true)
        index = index > 0 ? index - 1 : values.Length - 1;

      GUILayout.Label(names[index], styleLabel);

      if (GUILayout.Button(">", styleButton) == true)
        index = index < values.Length - 1 ? index + 1 : 0;
    }
    GUILayout.EndHorizontal();

    return (T)(object)index;
  }

  private Color NextColor(Color value)
  {
    if (value == UnityEngine.Color.white)
      return UnityEngine.Color.red;
    if (value == UnityEngine.Color.red)
      return UnityEngine.Color.magenta;
    if (value == UnityEngine.Color.magenta)
      return UnityEngine.Color.blue;
    if (value == UnityEngine.Color.blue)
      return UnityEngine.Color.cyan;
    if (value == UnityEngine.Color.cyan)
      return UnityEngine.Color.green;
    if (value == UnityEngine.Color.green)
      return UnityEngine.Color.yellow;
    if (value == UnityEngine.Color.yellow)
      return UnityEngine.Color.black;

    return UnityEngine.Color.white;
  }
}
