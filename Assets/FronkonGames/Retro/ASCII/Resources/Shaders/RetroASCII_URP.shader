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
Shader "Hidden/Fronkon Games/Retro/ASCII URP"
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
      Name "Fronkon Games Retro ASCII"

      HLSLPROGRAM
      #include "Retro.hlsl"
      #include "ColorBlend.hlsl"

      #pragma vertex RetroVert
      #pragma fragment frag
      #pragma target 3.5
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile _ _USE_DRAW_PROCEDURAL      
      #pragma multi_compile ___ GRADIENT_HORIZONTAL GRADIENT_VERTICAL GRADIENT_CIRCULAR
      #pragma multi_compile ___ BLOCK_COLOR
      #pragma multi_compile ___ SHAPE_AWARE
      #pragma multi_compile ___ EDGE_DETECTION
      #pragma multi_compile ___ SUPERSAMPLING

      TEXTURE2D(_AsciiTex);
      float _Zoom;
      float3 _FontParams;
      float _Boost;

#if SUPERSAMPLING
      int _SupersamplingLevel;
      
      // Supersample a cell by taking NxN samples and averaging
      // Returns averaged color and luminance
      void SupersampleCell(float2 cellOrigin, float2 cellSize, int level, out float4 avgColor, out float avgLuminance)
      {
        avgColor = 0;
        avgLuminance = 0;
        float invLevel = 1.0 / float(level);
        float sampleCount = float(level * level);
        
        for (int y = 0; y < level; y++)
        {
          for (int x = 0; x < level; x++)
          {
            float2 offset = float2((float(x) + 0.5) * invLevel, (float(y) + 0.5) * invLevel);
            float2 samplePos = cellOrigin + cellSize * offset;
            float4 sampleColor = SAMPLE_MAIN(samplePos);
            avgColor += sampleColor;
            avgLuminance += Luminance601(sampleColor.rgb);
          }
        }
        
        avgColor /= sampleCount;
        avgLuminance /= sampleCount;
      }
#endif
      int _FontColorBlend;
      float4 _FontColor;
      int _BackgroundColorBlend;
      float4 _BackgroundColor;
      float3 _ColorGradient;
      float3 _ColorGradient0;
      float3 _ColorGradient1;
      float _GradientCircularRadius;
      float _GradientHorizontalOffset;
      float _GradientVerticalOffset;

#if EDGE_DETECTION
      float _EdgeSensitivity;
      float _EdgeContrast;
      
      // Sobel edge detection - returns edge magnitude and dominant direction
      // Direction: 0 = horizontal edge, 1 = vertical edge, 0.5 = diagonal
      void SobelEdgeDetection(float2 cellCenter, float2 cellSize, out float edgeMagnitude, out float2 edgeDirection)
      {
        // Sample 3x3 neighborhood
        float tl = Luminance601(SAMPLE_MAIN(cellCenter + float2(-cellSize.x, -cellSize.y)).rgb);
        float tc = Luminance601(SAMPLE_MAIN(cellCenter + float2(0, -cellSize.y)).rgb);
        float tr = Luminance601(SAMPLE_MAIN(cellCenter + float2(cellSize.x, -cellSize.y)).rgb);
        float ml = Luminance601(SAMPLE_MAIN(cellCenter + float2(-cellSize.x, 0)).rgb);
        float mr = Luminance601(SAMPLE_MAIN(cellCenter + float2(cellSize.x, 0)).rgb);
        float bl = Luminance601(SAMPLE_MAIN(cellCenter + float2(-cellSize.x, cellSize.y)).rgb);
        float bc = Luminance601(SAMPLE_MAIN(cellCenter + float2(0, cellSize.y)).rgb);
        float br = Luminance601(SAMPLE_MAIN(cellCenter + float2(cellSize.x, cellSize.y)).rgb);
        
        // Sobel operators
        float gx = (-tl - 2.0 * ml - bl) + (tr + 2.0 * mr + br);
        float gy = (-tl - 2.0 * tc - tr) + (bl + 2.0 * bc + br);
        
        // Edge magnitude with sensitivity
        edgeMagnitude = sqrt(gx * gx + gy * gy) * _EdgeSensitivity;
        
        // Normalized edge direction
        edgeDirection = normalize(float2(gx, gy) + 0.0001);
      }
      
      // Apply directional contrast enhancement based on edge detection
      float ApplyEdgeContrast(float value, float edgeMagnitude, float threshold)
      {
        // Enhance contrast at edges: push values away from 0.5
        float contrastBoost = edgeMagnitude * _EdgeContrast;
        float direction = sign(value - threshold);
        return saturate(value + contrastBoost * direction);
      }
#endif

#if SHAPE_AWARE
      TEXTURE2D(_ShapeVectorTex);
      float _ShapeWeight;
      int _ShapeCharCount;
      
      // Sample a 2x3 grid within a cell to create a shape vector (6 values)
      // Returns two float3: first = top-left, top-right, mid-left; second = mid-right, bot-left, bot-right
      void SampleCellShapeVector(float2 cellOrigin, float2 cellSize, out float3 shapeVec1, out float3 shapeVec2)
      {
        float2 regionSize = cellSize / float2(2.0, 3.0);
        
        // Sample 6 regions (2 columns x 3 rows)
        // Row 0 (top)
        float topLeft = Luminance601(SAMPLE_MAIN(cellOrigin + regionSize * float2(0.5, 0.5)).rgb);
        float topRight = Luminance601(SAMPLE_MAIN(cellOrigin + regionSize * float2(1.5, 0.5)).rgb);
        // Row 1 (middle)
        float midLeft = Luminance601(SAMPLE_MAIN(cellOrigin + regionSize * float2(0.5, 1.5)).rgb);
        float midRight = Luminance601(SAMPLE_MAIN(cellOrigin + regionSize * float2(1.5, 1.5)).rgb);
        // Row 2 (bottom)
        float botLeft = Luminance601(SAMPLE_MAIN(cellOrigin + regionSize * float2(0.5, 2.5)).rgb);
        float botRight = Luminance601(SAMPLE_MAIN(cellOrigin + regionSize * float2(1.5, 2.5)).rgb);
        
        shapeVec1 = float3(topLeft, topRight, midLeft);
        shapeVec2 = float3(midRight, botLeft, botRight);
      }
      
      // Find best matching character using shape vectors
      // Returns the character index (0 to charCount-1)
      int FindBestShapeMatch(float3 inputVec1, float3 inputVec2, float inputDensity)
      {
        int bestIndex = 0;
        float bestScore = 1e10;
        
        for (int i = 0; i < _ShapeCharCount; i++)
        {
          // Sample character's shape vector from texture
          // Each character uses 2 pixels (for 6 values total)
          float2 uv1 = float2((float(i * 2) + 0.5) / float(_ShapeCharCount * 2), 0.5);
          float2 uv2 = float2((float(i * 2 + 1) + 0.5) / float(_ShapeCharCount * 2), 0.5);
          
          float3 charVec1 = SAMPLE_TEXTURE2D_LOD(_ShapeVectorTex, sampler_LinearClamp, uv1, 0).rgb;
          float3 charVec2 = SAMPLE_TEXTURE2D_LOD(_ShapeVectorTex, sampler_LinearClamp, uv2, 0).rgb;
          
          // Calculate character density (average of all 6 values)
          float charDensity = (charVec1.x + charVec1.y + charVec1.z + charVec2.x + charVec2.y + charVec2.z) / 6.0;
          
          // Euclidean distance for shape matching
          float3 diff1 = inputVec1 - charVec1;
          float3 diff2 = inputVec2 - charVec2;
          float shapeDistance = sqrt(dot(diff1, diff1) + dot(diff2, diff2));
          
          // Density difference
          float densityDistance = abs(inputDensity - charDensity);
          
          // Combined score: weighted blend of shape and density
          float score = _ShapeWeight * shapeDistance + (1.0 - _ShapeWeight) * densityDistance;
          
          if (score < bestScore)
          {
            bestScore = score;
            bestIndex = i;
          }
        }
        
        return bestIndex;
      }
#endif

      float4 frag(const RetroVaryings input) : SV_Target 
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;

        const float2 texel = float2(TEXEL_SIZE.x * _Zoom * _FontParams.x, TEXEL_SIZE.y * _Zoom * _FontParams.y);

        const float4 color = SAMPLE_MAIN(uv);
        
        float2 cellOrigin = floor(uv / texel) * texel;
        float2 cellCenter = cellOrigin + texel * 0.5;
        
#if EDGE_DETECTION
        // Compute Sobel edge detection
        float edgeMagnitude;
        float2 edgeDirection;
        SobelEdgeDetection(cellCenter, texel, edgeMagnitude, edgeDirection);
#endif
        
#if SUPERSAMPLING
        // Supersampled color and luminance
        float4 ssColor;
        float ssLuminance;
        SupersampleCell(cellOrigin, texel, _SupersamplingLevel, ssColor, ssLuminance);
#endif

#if SHAPE_AWARE
        // Shape-aware selection: sample 2x3 grid within the cell
        float3 inputVec1, inputVec2;
        SampleCellShapeVector(cellOrigin, texel, inputVec1, inputVec2);
        
        // Calculate average density for the cell
        float inputDensity = (inputVec1.x + inputVec1.y + inputVec1.z + inputVec2.x + inputVec2.y + inputVec2.z) / 6.0;
        
  #if EDGE_DETECTION
        // Apply directional edge contrast enhancement to shape vectors
        inputVec1.x = ApplyEdgeContrast(inputVec1.x, edgeMagnitude, 0.5);
        inputVec1.y = ApplyEdgeContrast(inputVec1.y, edgeMagnitude, 0.5);
        inputVec1.z = ApplyEdgeContrast(inputVec1.z, edgeMagnitude, 0.5);
        inputVec2.x = ApplyEdgeContrast(inputVec2.x, edgeMagnitude, 0.5);
        inputVec2.y = ApplyEdgeContrast(inputVec2.y, edgeMagnitude, 0.5);
        inputVec2.z = ApplyEdgeContrast(inputVec2.z, edgeMagnitude, 0.5);
        inputDensity = ApplyEdgeContrast(inputDensity, edgeMagnitude, 0.5);
  #endif
        
        // Find best matching character
        int charIndex = FindBestShapeMatch(inputVec1, inputVec2, inputDensity * _Boost);
        float index = float(charIndex);
        
  #if SUPERSAMPLING
        float4 pixelated = saturate(ssColor);
  #else
        float4 pixelated = saturate(SAMPLE_MAIN(cellCenter));
  #endif
#else
        // Classic luminance-based selection
  #if SUPERSAMPLING
        float4 pixelated = saturate(ssColor);
        float luminanceForIndex = ssLuminance;
  #else
        float4 pixelated = saturate(SAMPLE_MAIN(cellOrigin));
        float luminanceForIndex = Luminance601(pixelated.rgb);
  #endif
        
  #if EDGE_DETECTION
        // Apply edge-aware contrast enhancement to luminance
        luminanceForIndex = ApplyEdgeContrast(luminanceForIndex, edgeMagnitude, 0.5);
  #endif
        
        luminanceForIndex = clamp(luminanceForIndex * _Boost, 0.0, 0.9999);
        float index = floor(luminanceForIndex * _FontParams.z);
#endif

        float2 asciiUV = frac(uv / texel);
        asciiUV.x = (index / _FontParams.z) + (asciiUV.x / _FontParams.z);
        asciiUV.y %= _FontParams.z;
        float4 pixel = SAMPLE_TEXTURE2D(_AsciiTex, sampler_LinearClamp, asciiUV);

        float luminance = clamp(Luminance601(pixel.rgb) * _Boost, 0.0, 0.9999);
#if BLOCK_COLOR
        const float3 fontColor = ColorBlend(_FontColorBlend, pixelated.rgb, _FontColor.rgb) * luminance * _FontColor.a;
        const float3 bgColor = ColorBlend(_BackgroundColorBlend, pixelated.rgb, _BackgroundColor.rgb) * (_Boost - luminance) * _BackgroundColor.a;
#else
        const float3 fontColor = ColorBlend(_FontColorBlend, color.rgb, _FontColor.rgb) * luminance * _FontColor.a;
        const float3 bgColor = ColorBlend(_BackgroundColorBlend, color.rgb, _BackgroundColor.rgb) * (_Boost - luminance) * _BackgroundColor.a;
#endif
        pixel.rgb = lerp(bgColor, fontColor, luminance);

#if GRADIENT_HORIZONTAL
        pixel.rgb *= lerp(_ColorGradient1, _ColorGradient0, uv.y + _GradientHorizontalOffset);
#elif GRADIENT_VERTICAL
        pixel.rgb *= lerp(_ColorGradient0, _ColorGradient1, uv.x + _GradientVerticalOffset);
#elif GRADIENT_CIRCULAR
        pixel.rgb *= lerp(_ColorGradient0, _ColorGradient1, distance((float2)0.5, uv) * _GradientCircularRadius);
#endif

        // Color adjust.
        pixel.rgb = ColorAdjust(pixel.rgb, _Contrast, _Brightness, _Hue, _Gamma, _Saturation);

        return lerp(color, pixel, _Intensity);
      }

      ENDHLSL
    }
  }
  
  FallBack "Diffuse"
}
