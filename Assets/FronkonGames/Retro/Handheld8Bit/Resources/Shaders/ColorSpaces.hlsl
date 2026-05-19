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

inline float3 rgb2xyz(float3 c)
{
  float3 tmp = float3((c.r > 0.04045) ? pow(abs((c.r + 0.055) / 1.055), 2.4) : c.r / 12.92,
                      (c.g > 0.04045) ? pow(abs((c.g + 0.055) / 1.055), 2.4) : c.g / 12.92,
                      (c.b > 0.04045) ? pow(abs((c.b + 0.055) / 1.055), 2.4) : c.b / 12.92);

  const float3x3 mat = float3x3(0.4124, 0.3576, 0.1805,
                                0.2126, 0.7152, 0.0722,
                                0.0193, 0.1192, 0.9505);

  return 100.0 * mul(mat, tmp);
}

inline float3 xyz2lab(const float3 c)
{
 float3 n = c / float3(95.047, 100.0, 108.883),
        v = float3((n.x > 0.008856) ? pow(abs(n.x), 1.0 / 3.0) : (7.787 * n.x) + (16.0 / 116.0),
                   (n.y > 0.008856) ? pow(abs(n.y), 1.0 / 3.0) : (7.787 * n.y) + (16.0 / 116.0),
                   (n.z > 0.008856) ? pow(abs(n.z), 1.0 / 3.0) : (7.787 * n.z) + (16.0 / 116.0));

  return float3((116.0 * v.y) - 16.0, 500.0 * (v.x - v.y), 200.0 * (v.y - v.z));
}

inline float3 rgb2lab(const float3 c)
{
  float3 lab=xyz2lab(rgb2xyz(c));

  return float3(lab.x / 100.0, 0.5 + 0.5 * (lab.y / 127.0), 0.5 + 0.5 * (lab.z / 127.0));
}

inline float3 lab2xyz(float3 c)
{
  const float fy = (c.x + 16.0) / 116.0;
  const float fx = c.y / 500.0 + fy;
  const float fz = fy - c.z / 200.0;

  return float3(95.047 * ((fx > 0.206897) ? fx * fx * fx : (fx - 16.0 / 116.0) / 7.787),
                100.0 *((fy > 0.206897)? fy * fy * fy : (fy - 16.0 / 116.0) / 7.787),
                108.883 * ((fz > 0.206897) ? fz * fz * fz : (fz - 16.0 / 116.0) / 7.787));
}

inline float3 xyz2rgb(const float3 c)
{
  const float3x3 mat = float3x3(3.2406, -1.5372, -0.4986,
                                -0.9689, 1.8758, 0.0415,
                                0.0557, -0.2040, 1.0570);

  float3 v = mul(mat, c / 100.0),
         r = float3((v.r > 0.0031308) ? ((1.055 * pow(abs(v.r), (1.0 / 2.4))) - 0.055) : 12.92 * v.r,
                    (v.g > 0.0031308) ? ((1.055 * pow(abs(v.g), (1.0/ 2.4))) - 0.055) : 12.92 * v.g,
                    (v.b > 0.0031308) ? ((1.055 * pow(abs(v.b), (1.0 / 2.4))) - 0.055) : 12.92 * v.b);

  return r;
}

inline float3 lab2rgb(float3 c)
{
  return xyz2rgb(lab2xyz(float3(100.0 * c.x, 2.0 * 127.0 * (c.y - 0.5), 2.0 * 127.0 * (c.z - 0.5))));
}
