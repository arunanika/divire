//
//  divire
//
//  Copyright (C) 2020 Aru Nanika
//
//  This program is released under the MIT License.
//  https://opensource.org/licenses/MIT
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace divire.Models
{
    /// <summary>
    ///  The static class which provides various methods for color calculation.
    /// </summary>
    public static class ColorCalculation
    {
        /// <summary>
        /// The count of lights in RGB color model.
        /// </summary>
        private static readonly int CountOfRGB = 3;

        /// <summary>
        /// The degrees of whole hue range.
        /// </summary>
        private static readonly int RangeOfTotalHue = 360;

        /// <summary>
        /// The degrees of each hue section (red, green, or blue).
        /// </summary>
        private static readonly int RangeOfHueSection = 120;

        /// <summary>
        /// The hue range represented by RGB or their composite.
        /// </summary>
        private static readonly int RangeOfSextet = 60;

        /// <summary>
        /// Get light values as tuple from HSV parameters.
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="saturation"></param>
        /// <param name="brightness"></param>
        /// <returns>a tuple of light values</returns>
        public static Tuple<double, double, double> GetRgbLightsFromHsv(double hue, double saturation, double brightness)
        {
            var hueDegrees = (int)hue;
            var indexOfPrimaryLight = ((hueDegrees + RangeOfSextet) % RangeOfTotalHue) / RangeOfHueSection;

            var criterion = ((hueDegrees + RangeOfSextet) / RangeOfHueSection) * RangeOfHueSection;
            var offsetOfIndex = (0 < (hue - criterion)) ? 1 : -1;
            var indexOfSecondaryLight = ((CountOfRGB + indexOfPrimaryLight) + offsetOfIndex) % CountOfRGB;

            var ratioOfSecondary = Math.Abs(hue - criterion) / RangeOfSextet;

            var chroma = saturation * brightness;
            var valueOfSecondaryLight = chroma * ratioOfSecondary;

            var offsetOfLights = brightness - chroma;

            var valuesOfLights = Enumerable.Repeat<double>(offsetOfLights, CountOfRGB).ToArray();
            valuesOfLights[indexOfPrimaryLight] += chroma;
            valuesOfLights[indexOfSecondaryLight] += valueOfSecondaryLight;

            return new Tuple<double, double, double>(valuesOfLights[0], valuesOfLights[1], valuesOfLights[2]);
        }

        /// <summary>
        /// Get RGB value as Hex expression from HSV parameters.
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="saturation"></param>
        /// <param name="brightness"></param>
        /// <returns>RGB value</returns>
        public static int GetRgbValueFromHsv(double hue, double saturation, double brightness)
        {
            var lights = GetRgbLightsFromHsv(hue, saturation, brightness);
            var valuesOfLights = new double[] { lights.Item1, lights.Item2, lights.Item3 };

            int result = 0;

            for (int i = 0; i < CountOfRGB; i++)
            {
                result <<= 8; 

                var hexValue = (int)Math.Round((valuesOfLights[i] * 255.0), 0, MidpointRounding.AwayFromZero);

                result |= hexValue;
            }

            return result;
        }

        /// <summary>
        /// Get light values from Hex expression of RGB color.
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns>a tuple of light values</returns>
        public static Tuple<double, double, double> GetRgbLightsFromRgbValue(int rgb)
        {
            var valuesOfLights = new double[CountOfRGB];

            var sourceValue = rgb;

            for (int i = 0; i < CountOfRGB; i++)
            {
                var ratio = (sourceValue & 0xff) / 255.0;
                valuesOfLights[(CountOfRGB - i) - 1] = ratio;
                sourceValue >>= 8;
            }

            return new Tuple<double, double, double>(valuesOfLights[0], valuesOfLights[1], valuesOfLights[2]);
        }

        /// <summary>
        /// Get HSV values from RGB parameters.
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns>a tuple of HSV values</returns>
        public static Tuple<double,double,double> GetHSVFromRgbValue(int rgb)
        {
            var hue = 0.0;
            var saturation = 0.0;
            var brightness = 0.0;
            var indexOfMax = 0;
            var indexOfMin = 0;

            var lights = GetRgbLightsFromRgbValue(rgb);
            var valuesOfLights = new double[] { lights.Item1, lights.Item2, lights.Item3 };
            var differencesFromNextLight = new double[CountOfRGB];

            for (int i = 0; i < CountOfRGB; i++)
            {
                var indexOfNextLight = ((CountOfRGB + i) + 1) % CountOfRGB;
                differencesFromNextLight[i] = valuesOfLights[i] - valuesOfLights[indexOfNextLight];
            }

            for (int i = 1; i < CountOfRGB; i++)
            {
                if (differencesFromNextLight[i] > 0)
                {
                    if (differencesFromNextLight[(i - 1)] <= 0)
                    {
                        indexOfMax = i;
                    }
                }
                else
                {
                    if (differencesFromNextLight[(i - 1)] > 0)
                    {
                        indexOfMin = i;
                    }
                }
            }

            brightness = valuesOfLights[indexOfMax];

            var difference = brightness - valuesOfLights[indexOfMin];

            saturation = (0 < brightness) ? (difference / brightness) : 0;

            var indexOfBias = ((CountOfRGB + indexOfMax) + 1) % CountOfRGB;

            var hueAngle = 0.0;
            if (0 < difference)
            {
                hueAngle = RangeOfHueSection * (indexOfMax + ((differencesFromNextLight[indexOfBias] / difference) / 2));
            }

            hue = (0 > hueAngle) ? (hueAngle + RangeOfTotalHue) : hueAngle;

            return new Tuple<double, double, double>(hue, saturation, brightness);
        }

        public static double GetHueFromRgbValue(int rgb)
        {
            return GetHSVFromRgbValue(rgb).Item1;
        }

        public static double GetSaturationFromRgbValue(int rgb)
        {
            return GetHSVFromRgbValue(rgb).Item2;
        }

        public static double GetBrightnessFromRgbValue(int rgb)
        {
            return GetHSVFromRgbValue(rgb).Item3;
        }
    }
}
