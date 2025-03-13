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
Shader "Hidden/Fronkon Games/Artistic/One Bit URP"
{
  Properties
  {
    _MainTex("Main Texture", 2D) = "white" {}
  }

  SubShader
  {
    Tags
    {
      "RenderType" = "Opaque"
      "RenderPipeline" = "UniversalPipeline"
    }
    LOD 100
    ZTest Always ZWrite Off Cull Off

    Pass
    {
      Name "Fronkon Games Artistic One Bit"

      HLSLPROGRAM
      #include "Artistic.hlsl"
      #include "ColorBlend.hlsl"
      #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

      #pragma vertex ArtisticVert
      #pragma fragment ArtisticFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile _ _USE_DRAW_PROCEDURAL
      #pragma multi_compile ___ COLORMODE_SOLID COLORMODE_GRADIENT COLORMODE_HORIZONTAL COLORMODE_VERTICAL COLORMODE_CIRCULAR

      int _InvertColor;
      int _Blend;
      float4 _Color;
      float4 _Color0;
      float4 _Color1;
      TEXTURE2D_X(_GradientTex);
      float _LumRangeMin;
      float _LumRangeMax;
      float _GradientRadius;
      float _GradientHorizontalOffset;
      float _GradientVerticalOffset;        
      int _RedCount;
      int _GreenCount;
      int _BlueCount;

      float _Edges;
      float _NoiseStrength;
      float _NoiseSeed;

      static const float3x3 Blue = float3x3(float3(0.0833333, 0.0833333, 0.0833333),
                                            float3(0.0833333, 0.3333333, 0.0833333),
                                            float3(0.0833333, 0.0833333, 0.0833333));

      float2 StepNoise(float2 p, float size)
      {
        p += 10.0;
        float x = floor(p.x / size) * size;
        float y = floor(p.y / size) * size;
        
        x = frac(x * 0.1) + 1.0 + x * 0.0002;
        y = frac(y * 0.1) + 1.0 + y * 0.0003;
        
        float a = frac(1.0 / (0.000001 * x * y + 0.00001));
        a = frac(1.0 / (0.000001234 * a + 0.00001));
        
        float b = frac(1.0 / (0.000002 * (x * y + x) + 0.00001));
        b = frac(1.0 / (0.0000235 * b + 0.00001));
        
        return float2(a, b);
      }

      inline float Poly(float a, float b, float c, float ta, float tb, float tc)
      {
        return (a * ta + b * tb + c * tc) / (ta + tb + tc);
      }

      float Mask(float2 p)
      {
        p += (StepNoise(p, 5.5) - 0.5) * 8.12235325;
        float f = frac(p[0] * _NoiseSeed + p[1] / (_NoiseSeed + 0.15555)) * _NoiseStrength * 0.1;
        
        return (SafePositivePow_float(f, 150.0) + f) / 2.3;
      }

      float LuminanceGamma(float x, float y, float2 uv)
      {
        float3 pixel = SAMPLE_MAIN(float2(x, y) / _ScreenParams.xy + uv).rgb;

        return dot(float3(0.2126, 0.7152, 0.0722), pixel);
      }

      half4 ArtisticFrag(VertexOutput input) : SV_Target0
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;

        const float4 color = SAMPLE_MAIN(uv);
        float4 pixel = color;

        float f1 = LuminanceGamma( 0.0,  0.0, uv);
        float f =  LuminanceGamma(-1.0, -1.0, uv) * Blue[0][0] + LuminanceGamma(-1.0, 0.0, uv) * Blue[0][1] + LuminanceGamma(-1.0, 1.0, uv) * Blue[0][2] +
                   LuminanceGamma( 0.0, -1.0, uv) * Blue[1][0] + f1                            * Blue[1][1] + LuminanceGamma( 0.0, 1.0, uv) * Blue[1][2] +
                   LuminanceGamma( 1.0, -1.0, uv) * Blue[2][0] + LuminanceGamma( 1.0, 0.0, uv) * Blue[2][1] + LuminanceGamma( 1.0, 1.0, uv) * Blue[2][2];

        f = (f - f1) * _Edges;
        f = f1 - f;

        pixel.rgb = f >= Mask(uv * _ScreenParams.xy);

        pixel.rgb = lerp(pixel.rgb, 1.0 - pixel.rgb, _InvertColor);

        pixel.rgb = ColorBlend(_Blend, color.rgb, pixel.rgb);

#ifdef COLORMODE_SOLID
        pixel.rgb *= _Color.rgb;
#elif COLORMODE_GRADIENT
        float lum = clamp((1.0 / (_LumRangeMax - _LumRangeMin)) * (Luminance(pixel.rgb) - _LumRangeMin), 0.0, 1.0);

        pixel.rgb = SAMPLE_TEXTURE2D(_GradientTex, sampler_LinearClamp, half2(lum, 0.5));
#elif COLORMODE_HORIZONTAL
        pixel.rgb *= lerp(_Color1, _Color0, uv.y + _GradientHorizontalOffset);
#elif COLORMODE_VERTICAL
        pixel.rgb *= lerp(_Color0, _Color1, uv.x + _GradientVerticalOffset);
#elif COLORMODE_CIRCULAR
        pixel.rgb *= lerp(_Color0, _Color1, distance(float2(0.5, 0.5), uv) * _GradientRadius);
#endif
        pixel.r = floor((_RedCount - 1.0) * pixel.r + 0.5) / (_RedCount - 1.0);
        pixel.g = floor((_GreenCount - 1.0) * pixel.g + 0.5) / (_GreenCount - 1.0);
        pixel.b = floor((_BlueCount - 1.0) * pixel.b + 0.5) / (_BlueCount - 1.0);

        // Color adjust.
        pixel.rgb = ColorAdjust(pixel.rgb);

        return lerp(color, pixel, _Intensity);
      }

      ENDHLSL
    }
  }
  
  FallBack "Diffuse"
}
