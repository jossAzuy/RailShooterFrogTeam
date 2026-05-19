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
float _AMCarrierSignalWavelength;
float _YLowPassWavelength;
float _ILowPassWavelength;
float _QLowPassWavelength;
float _ColorburstWavelengthEncoder;
float _NTSCScale;
float _PhaseAlternation;
float _NoiseStrength;
float _WindowBias;

float _CurrentTime;
int _CurrentFrame;

float NTSC_Encode(float2 uv, float pixelWidth, bool useAlternatePhaseLogic)
{
  float3 yiq = float3(0.0, 0.0, 0.0);
  float windowWeight = 0.0;

  for (int i = -_WindowRadius; i <= _WindowRadius; ++i)
  {
    float window = WindowCosine(float(i) / float(_WindowRadius + 1), _WindowBias);

    float sincY = Sinc(float(i) / _YLowPassWavelength) / _YLowPassWavelength;
    float sincI = Sinc(float(i) / _ILowPassWavelength) / _ILowPassWavelength;
    float sincQ = Sinc(float(i) / _QLowPassWavelength) / _QLowPassWavelength;
    
    float2 uvWithOffset = float2(uv.x + float(i) * pixelWidth, uv.y);

    float3 yiqSample = mul(MatrixRGBToYIQ, clamp(SAMPLE_MAIN(uvWithOffset).xyz, 0.0, 1.0));

    yiq.x += yiqSample.x * sincY * window;
    yiq.y += yiqSample.y * sincI * window;
    yiq.z += yiqSample.z * sincQ * window;
    windowWeight += window;
  }

  float phase = uv.x * PI / (_ColorburstWavelengthEncoder * pixelWidth);
  if (useAlternatePhaseLogic)
    phase += _PhaseAlternation;
  
  float phaseAM = uv.x * PI / (_AMCarrierSignalWavelength * pixelWidth);

  return (yiq.x + sin(phase) * yiq.y + cos(phase) * yiq.z) * sin(phaseAM);
}

float3 Encode(float3 pixel, float2 uv)
{
  float2 coord = uv * _ScreenParams.xy;

  uint rngState = uint(uint(coord.x) * uint(1973) + uint(coord.y) * uint(9277) + uint(_CurrentFrame) * uint(26699)) | uint(1);
  uint rngStateRow = uint(uint(coord.y) * uint(9277) + uint(_CurrentFrame) * uint(26699)) | uint(1);

  float2 pixelSize = 1.0 / _ScreenParams.xy;

  bool useAlternatePhaseLogic = (_CurrentFrame + uint(coord.y)) % 2 == 0;

  float encoded = NTSC_Encode(uv, pixelSize.x * _NTSCScale, useAlternatePhaseLogic);
  
  float snowNoise = RandomFloat01(rngState) - 0.5;
  float sineNoise = sin(uv.x * 200.0 + uv.y * -50.0 + frac(_EffectTime.y * 100.0) * PI * 2.0) * 0.065;

  float saltPepperNoise = RandomFloat01(rngState) * 2.0 - 1.0;
  saltPepperNoise = sign(saltPepperNoise) * pow(abs(saltPepperNoise), 200.0) * 10.0; 

  float rowNoise = RandomFloat01(rngStateRow) * 2.0 - 1.0;
  rowNoise *= 0.1;

  float rowSaltPepper = RandomFloat01(rngStateRow) * 2.0 - 1.0;
  rowSaltPepper = sign(rowSaltPepper) * pow(abs(rowSaltPepper), 200.0) * 1.0;

  encoded += (snowNoise + saltPepperNoise + sineNoise + rowNoise + rowSaltPepper) * _NoiseStrength;
   
  return encoded;
}
