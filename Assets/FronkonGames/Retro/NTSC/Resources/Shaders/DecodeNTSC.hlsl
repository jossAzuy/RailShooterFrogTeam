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

int _WindowRadius;
float _ColorburstWavelengthDecoder;
float _DecodeLowPassWavelength;
float _NTSCScale;
float _NoiseStrength;
float _WindowBias;
float _PhaseAlternation;
int _CurrentFrame;

float3 NTSC_DecodeNTSC(float2 uv, float pixelWidth, int2 rngSeedComponents, bool useAlternatePhaseLogic)
{
  uint seed = rngSeedComponents.y; 
  float2 originalUV = uv;
  
  float3 rowNoiseIntensity = float3(RandomFloat01(seed), RandomFloat01(seed), RandomFloat01(seed));
  rowNoiseIntensity = pow(rowNoiseIntensity, float3(500.0, 500.0, 500.0)) * 1.0;
  
  float horizOffsetNoise = RandomFloat01(seed) * 2.0 - 1.0;
  horizOffsetNoise *= rowNoiseIntensity.x * 0.5 * _NoiseStrength;
  uv.x += horizOffsetNoise;
  
  float phaseNoise = RandomFloat01(seed) * 2.0 - 1.0;
  phaseNoise *= rowNoiseIntensity.y * 0.5 * PI * _NoiseStrength;

  float frequencyNoise = RandomFloat01(seed) * 2.0 - 1.0;
  frequencyNoise *= rowNoiseIntensity.z * 0.5 * PI * _NoiseStrength;
  
  float alt = 0.0;  
  if (useAlternatePhaseLogic)
    alt = _PhaseAlternation;
  
  float3 yiq = float3(0.0, 0.0, 0.0);
  float windowWeight = 0.0;

  for (int i = -_WindowRadius; i <= _WindowRadius; ++i)
  {
    float window = WindowCosine(float(i) / float(_WindowRadius + 1), _WindowBias); 
      
    float2 uvWithOffset = float2(uv.x + float(i) * pixelWidth, uv.y);
    float2 originalUVWithOffset = float2(originalUV.x + float(i) * pixelWidth, originalUV.y);
      
    float phase = originalUVWithOffset.x * PI / ((_ColorburstWavelengthDecoder + frequencyNoise) * pixelWidth) + phaseNoise + alt;

    float sincY = Sinc(float(i) / _DecodeLowPassWavelength) / _DecodeLowPassWavelength; 
    float sinI = sin(phase);
    float sinQ = cos(phase);
      
    float encodedSample = SAMPLE_MAIN(uvWithOffset).x;
      
    yiq.x += encodedSample * sincY * window;
    yiq.y += encodedSample * sinI * window;
    yiq.z += encodedSample * sinQ * window;

    windowWeight += window;
  }
  
  yiq.yz = windowWeight > 0.0 ? yiq.yz * (_Saturation * 4.0) / windowWeight : 0.0;

  return max(0.0, mul(MatrixYIQToRGB, yiq));
}

float3 DecodeNTSC(float3 pixel, float2 uv)
{
  float2 coord = uv * _ScreenParams.xy;

  float2 pixelSize = 1.0 / _ScreenParams.xy; 

  uint rngStateRowUint = uint(uint(coord.y) * uint(9277) + uint(_CurrentFrame) * uint(26699)) | uint(1);    
  uint rngStateColUint = uint(uint(coord.x) * uint(1973) + uint(_CurrentFrame) * uint(26699)) | uint(1);    
  int2 rngParam = int2(rngStateColUint, rngStateRowUint);

  bool useAlternatePhaseLogic = (_CurrentFrame + uint(coord.y)) % 2 == 0;

  float3 resultColor = NTSC_DecodeNTSC(uv, pixelSize.x * _NTSCScale, rngParam, useAlternatePhaseLogic);

  return resultColor;
}
