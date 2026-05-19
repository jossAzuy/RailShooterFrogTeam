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
Shader "Hidden/Fronkon Games/Retro/VHS URP"
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
      Name "Fronkon Games Retro VHS Single Pass"

      HLSLPROGRAM
      #include "Retro.hlsl"
      
      #pragma vertex RetroVert
      #pragma fragment frag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile _ _USE_DRAW_PROCEDURAL

      #include "RetroVHS_SinglePass.hlsl"
      ENDHLSL
    }
    
    Pass
    {
      Name "Fronkon Games Retro VHS Multi Pass 1"

      HLSLPROGRAM
      #include "Retro.hlsl"

      #pragma vertex RetroVert
      #pragma fragment frag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile _ _USE_DRAW_PROCEDURAL

      #include "RetroVHS_Pass1.hlsl"
      ENDHLSL
    }
    
    Pass
    {
      Name "Fronkon Games Retro VHS Multi Pass 2"

      HLSLPROGRAM
      #include "Retro.hlsl"
      
      #pragma vertex RetroVert
      #pragma fragment frag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile _ _USE_DRAW_PROCEDURAL

      #include "RetroVHS_Pass2.hlsl"
      ENDHLSL
    }
  }
  
  FallBack "Diffuse"
}
