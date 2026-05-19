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
float _AMDemodulateWavelength;
float _AMDecodeHighPassWavelength;
float _NTSCScale;
float _WindowBias;

float NTSC_DecodeAM(float2 uv, float pixelWidth)
{
  float phaseAM = uv.x * PI / (_AMDemodulateWavelength * pixelWidth);
  
  float decoded = 0.0;
  float windowWeight = 0.0;

  for (int i = -_WindowRadius; i <= _WindowRadius; ++i)
  {
    float window = WindowCosine(float(i) / float(_WindowRadius + 1), _WindowBias); 
    float2 uvWithOffset = float2(uv.x + float(i) * pixelWidth, uv.y);
    float sinc = Sinc(float(i) / _AMDecodeHighPassWavelength) / _AMDecodeHighPassWavelength; 
    float encodedSample = SAMPLE_MAIN(uvWithOffset).x;
      
    decoded += encodedSample * sinc * window;
    windowWeight += window;
  }
  
  return (decoded) * sin(phaseAM) * 4.0;
}

float3 DecodeAM(float3 pixel, float2 uv)
{
  float2 pixelSize = 1.0 / _ScreenParams.xy;
  float pixelWidthToPass = pixelSize.x * _NTSCScale;

  float decodedValue = NTSC_DecodeAM(uv, pixelWidthToPass);

  return decodedValue;
}
