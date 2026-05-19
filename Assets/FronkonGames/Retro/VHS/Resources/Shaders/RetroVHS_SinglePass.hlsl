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
#pragma once

inline float3 SampleNoise(const in float2 uv)
{
  return Rand(uv);
}

float4 frag(const RetroVaryings input) : SV_Target 
{
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
  const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;
  float2 uvn = uv;

  const float4 color = SAMPLE_MAIN(uv);
  float4 pixel = color;

  // Tape wave.
  uvn.x += (SampleNoise(float2(uvn.y / 10.0, _EffectTime.y / 10.0) / 1.0).x - 0.5) / _ScreenParams.x * _TapeNoiseLow * 10.0;
  uvn.x += (SampleNoise(float2(uvn.y, _EffectTime.y * 1.0)).x - 0.5) / _ScreenParams.x * _TapeNoiseHigh * 10.0;

  // Tape crease.
  const float tcPhase = smoothstep(0.9, 0.96, sin(uvn.y * _TapeCreaseCount - (_EffectTime.y + 0.14 * SampleNoise(_EffectTime.y * float2(0.67, 0.59)).x) * PI * _TapeCreaseVelocity)) * _TapeCreaseStrength;
  const float tcNoise = smoothstep(0.3, 1.0, SampleNoise(float2(uvn.y * 4.77, _EffectTime.y)).x);
  float tc = tcPhase * tcNoise;
  uvn.x = uvn.x - tc / _ScreenParams.x * 8.0;  

  // Switching noise.
  const float snPhase = smoothstep(6.0 / _ScreenParams.y, 0.0, uvn.y);
  uvn.y += snPhase * 0.3;
  uvn.x += snPhase * ((SampleNoise(float2(uv.y * 100.0, _EffectTime.y * 10.0)).x - 0.5) / _ScreenParams.x * 24.0);

  // Wrap bottom.
  const float uvHeight = _BottomWarpHeight / _ScreenParams.y;
  UNITY_BRANCH
  if (uv.y <= uvHeight)
  {
    const float offsetUV = (uv.y / uvHeight) * (_BottomWarpDistortion / _ScreenParams.x);
    const float jitterUV = (GoldNoise(float2(500.0, 500.0), frac(_EffectTime.y)) * _BottomWarpJitterExtent) / _ScreenParams.x; 
  
    uvn = float2(uv.x - offsetUV - jitterUV, uv.y);
  }

  // In VHS chroma band is a much lower resolution (technically 1/16th)
  pixel.rgb = Shrink(uvn * _ScreenParams.xy, _ChromaBand);

  // In VHS the luma band is half of the resolution.
  float3 luma = Shrink(uvn * _ScreenParams.xy, _LumaBand);
  luma = saturate(BlendLuminosity(float3(0.5, 0.5, 0.5), luma));

  pixel.rgb = BlendColor(luma, pixel.rgb);

  // Crease noise.
  const float cn = tcNoise * (0.3 + _TapeCreaseNoise * tcPhase);
  UNITY_BRANCH
  if (0.3 < cn)
  {
    const float2 V = float2(0.0, 1.0);
    const float2 uvt = (uvn + V.yx * SampleNoise(float2(uvn.y, _EffectTime.y)).x) * float2(0.1, 1.0);
    const float n0 = SampleNoise(uvt).x;
    const float n1 = SampleNoise(uvt + V.yx / _ScreenParams.x).x;
    if (n1 < n0)
      pixel.rgb = lerp(pixel.rgb, 2.0 * V.yyy, pow(n0, 10.0));
  }

  // AC beat.
  pixel.rgb *= 1.0 + _ACBeatStrength * smoothstep(0.4, 0.6, SampleNoise(float2(0.0, _ACBeatCount * (uv.y + _EffectTime.y * _ACBeatVelocity)) / 10.0).x);

  // Color noise.
  pixel.rgb *= (1.0 - _ColorNoise) + _ColorNoise * SampleNoise(mod(uvn * float2(1.0, 1.0) + _EffectTime.y * float2(5.97, 4.45), (float2)1.0));
  pixel.rgb = saturate(pixel.rgb);

  // YIQ space color.
  pixel.rgb = rgb2yiq(pixel.rgb);
  pixel.rgb = float3(0.1, -0.1, 0.0) + _YIQ * pixel.rgb;
  pixel.rgb = yiq2rgb(pixel.rgb);

  pixel.rgb = ClampLevels(pixel.rgb, _BlackLevel, _WhiteLevel);

  pixel = TintShadows(pixel, _ShadowTint);

  // Vignette.
  pixel.rgb *= Vignette(uv);

  pixel.rgb = ColorAdjust(pixel.rgb, _Contrast, _Brightness, _Hue, _Gamma, _Saturation);

  return lerp(color, pixel, _Intensity);
}
