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
Shader "Hidden/Fronkon Games/Retro/NTSC URP"
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
      Name "Fronkon Games Retro NTSC (Encode)"

      HLSLPROGRAM
      #include "Retro.hlsl"
      #include "Encode.hlsl"

      #pragma vertex RetroVert
      #pragma fragment RetroFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile _ _USE_DRAW_PROCEDURAL
      
      half4 RetroFrag(RetroVaryings input) : SV_Target
      {
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;
        const half3 color = SAMPLE_MAIN(uv).rgb;
        half3 pixel = Encode(color, uv);

#if 0
        pixel = PixelDemo(color, pixel, uv);
#endif
        return half4(lerp(color, pixel, _Intensity), 1.0);
      }
      ENDHLSL
    }

    Pass
    {
      Name "Fronkon Games Retro NTSC (Decode AM)"

      HLSLPROGRAM
      #include "Retro.hlsl"
      #include "DecodeAM.hlsl"

      #pragma vertex RetroVert
      #pragma fragment RetroFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile _ _USE_DRAW_PROCEDURAL

      half4 RetroFrag(RetroVaryings input) : SV_Target
      {
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;
        const half3 color = SAMPLE_MAIN(uv).rgb;
        half3 pixel = DecodeAM(pixel, uv);

#if 0
        pixel = PixelDemo(color, pixel, uv);
#endif
        return half4(lerp(color, pixel, _Intensity), 1.0);
      }
      ENDHLSL
    }

    Pass
    {
      Name "Fronkon Games Retro NTSC (Decode NTSC)"

      HLSLPROGRAM
      #include "Retro.hlsl"
      #include "DecodeNTSC.hlsl"

      #pragma vertex RetroVert
      #pragma fragment RetroFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile _ _USE_DRAW_PROCEDURAL

      half4 RetroFrag(RetroVaryings input) : SV_Target
      {
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;
        const half3 color = SAMPLE_MAIN(uv).rgb;
        half3 pixel = DecodeNTSC(color, uv);

        pixel = ColorAdjust(pixel);
#if 0
        pixel = PixelDemo(color, pixel, uv);
#endif
        return half4(lerp(color, pixel, _Intensity), 1.0);
      }
      ENDHLSL
    }
  }
  
  FallBack "Diffuse"
}
