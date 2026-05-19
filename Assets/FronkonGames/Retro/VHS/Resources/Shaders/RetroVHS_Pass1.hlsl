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

// Lowers horizontal resolution of luminance and color / chroma.
float4 frag(const RetroVaryings input) : SV_Target 
{
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
  const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;

  const float4 color = SAMPLE_MAIN(uv);
  float4 pixel = color;

  // In VHS chroma band is a much lower resolution (technically 1/16th)
  pixel.rgb = Shrink(uv * _ScreenParams.xy, _ChromaBand);

  // In VHS the luma band is half of the resolution.
  float3 luma = saturate(Shrink(uv * _ScreenParams.xy, _LumaBand));
  luma = BlendLuminosity(float3(0.5, 0.5, 0.5), luma);

  pixel.rgb = BlendColor(luma, pixel.rgb);

  return lerp(color, pixel, _Intensity);
}
