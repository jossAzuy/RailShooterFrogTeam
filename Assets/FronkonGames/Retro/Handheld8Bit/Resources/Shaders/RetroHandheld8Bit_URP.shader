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
Shader "Hidden/Fronkon Games/Retro/Handheld 8-Bit URP"
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

    // PASS 0: Pixelation, Color Quantization, Grid
    Pass
    {
      Name "Fronkon Games Retro Handheld 8-Bit Pass 0"

      HLSLPROGRAM
      #include "Retro.hlsl"
      #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
      #include "ColorSpaces.hlsl"

      #pragma vertex RetroVert
      #pragma fragment frag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash

      float _PixelSize;
      float _SubPixel;
      int _PixelDistance;
      float2 _PixelOffset;
      int _Invert;
      
      float _Luminosity;
      float _Threshold;
      
      float4 _Palette0;
      float4 _Palette1;
      float4 _Palette2;
      float4 _Palette3;
      float4 _Grid;

      inline float3 FindClosest(const float3 color)
      {
        float3 col[4];
        col[0] = _Palette0.rgb;
        col[1] = _Palette1.rgb;
        col[2] = _Palette2.rgb;
        col[3] = _Palette3.rgb;

        float _min = 10000.0;
        float3 res = color;
        float3 ccol;
        
        UNITY_UNROLL
        for (int a = 0; a < 4; a++)
        {
          ccol = rgb2lab(col[a]);
          
          if (distance(color, ccol) < _min)
          {
            _min = distance(color, ccol);
            res = ccol;
          }
        }

        return res; 
      }

      float4 frag(const RetroVaryings input) : SV_Target 
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        
        float2 fragCoord = input.texcoord.xy * _ScreenParams.xy;
        float2 pixelSizeVec = float2(_PixelSize, _PixelSize);
        float subpixel = _SubPixel;
        float pixeldistance = (float)_PixelDistance;
        
        // Pixelation logic
        float2 uv_pixelated = (fragCoord - fmod(fragCoord, pixelSizeVec) + _PixelOffset) / _ScreenParams.xy;
        
        // Sample
        float3 c = SAMPLE_MAIN(uv_pixelated).rgb;
        
        // Invert
        if (_Invert == 1) 
        {
            c.rgb = 1.0 - c.rgb;
        }

        // Apply Threshold (Input Gamma/Power)
        // Uses Threshold instead of Gamma for input decoding to support legacy/custom control
        // If Threshold is 0, we avoid division by zero (though Slider limits to 0.0, we should be safe or clamp)
        float thresh = max(_Threshold, 0.001);
        float3 c_processed = pow(abs(c), float3(1.0 / thresh, 1.0 / thresh, 1.0 / thresh));
        
        // Convert to LAB, Find Nearest, Convert back
        float3 sum = lab2rgb(FindClosest(rgb2lab(c_processed)));
        
        // Apply Luminosity
        sum *= _Luminosity;

        // Grid Lines
        if (fmod(fragCoord.x, _PixelSize / subpixel) < pixeldistance)
        {
             sum = _Grid.rgb;
        }
        if (fmod(fragCoord.y, _PixelSize / subpixel) < pixeldistance)
        {
             sum = _Grid.rgb;
        }

        return float4(sum, 1.0);
      }

      ENDHLSL
    }

    // PASS 1: Shadow / Blur & Color Adjust
    Pass
    {
      Name "Fronkon Games Retro Handheld 8-Bit Pass 1"

      HLSLPROGRAM
      #include "Retro.hlsl"
      
      #pragma vertex RetroVert
      #pragma fragment frag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      
      // _Intensity, _Brightness, _Contrast, _Hue, _Saturation, _Gamma defined in Retro.hlsl

      float _ShadowSize;
      float _ShadowDistance;

      float4 frag(const RetroVaryings input) : SV_Target 
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        float2 uv = input.texcoord.xy;
        
        float4 LCDDisp = SAMPLE_MAIN(uv);
        
        float2 radius = float2(_ShadowSize, _ShadowSize);
        float maxDist = sqrt(radius.x * radius.x + radius.y * radius.y);
        float3 blur = float3(0.0, 0.0, 0.0);
        float sum = 0.0;

        UNITY_LOOP
        for(float u = -radius.x; u <= radius.x; u++)
        {
            for(float v = -radius.y; v <= radius.y; v++)
            {
                float weight = maxDist - sqrt(u * u + v * v);
                
                float2 offset = float2(u - _ShadowDistance, v + _ShadowDistance);
                blur += weight * SAMPLE_MAIN(uv + offset / _ScreenParams.xy).rgb;
                sum += weight;
            }
        }
        
        if (sum > 0.0)
            blur /= sum;
        else
            blur = LCDDisp.rgb;
        
        float4 result = min(LCDDisp, float4(blur, 1.0));
        
        // Apply Color Adjustments
        // Using 1.0 for gamma here as we used Threshold for input decoding. 
        // If user wants output gamma correction, they can use Gamma slider, but we need to decide if we pass it here.
        // For now, let's respect that "Gamma" slider might be for output gamma.
        // BUT Handheld8BitVolume passes volume.gamma.value to _Gamma.
        // And we typically want some gamma control.
        // I will use _Gamma here. If default is 0.9, it applies 0.9 gamma.
        result.rgb = ColorAdjust(result.rgb, _Contrast, _Brightness, _Hue, _Gamma, _Saturation);
        
        return result;
      }

      ENDHLSL
    }
  }
  
  FallBack "Diffuse"
}
