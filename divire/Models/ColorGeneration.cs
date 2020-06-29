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
    public static class ColorGeneration
    {
        public static IEnumerable<int> CreateHueTransition(int leadColor, int tailColor, int totalNumber)
        {
            var leadHsv = ColorCalculation.GetHSVFromRgbValue(leadColor);
            var tailHsv = ColorCalculation.GetHSVFromRgbValue(tailColor);

            var hueDifference = tailHsv.Item1 - leadHsv.Item1;
            var saturationDifference = tailHsv.Item2 - leadHsv.Item2;
            var valueDifference = tailHsv.Item3 - leadHsv.Item3;

            var numberOfPartitions = totalNumber - 1;
            var colors = Enumerable.Range(1, (numberOfPartitions - 1)).
                Select(e => ColorCalculation.GetRgbValueFromHsv((leadHsv.Item1 + hueDifference * e / numberOfPartitions),
                                                                (leadHsv.Item2 + saturationDifference * e / numberOfPartitions),
                                                                (leadHsv.Item3 + valueDifference * e / numberOfPartitions))
                                                                );

            return colors;
        }

        public static IEnumerable<int> CreateHueTransitionViaRed(int leadColor, int tailColor, int totalNumber)
        {
            var leadHsv = ColorCalculation.GetHSVFromRgbValue(leadColor);
            var tailHsv = ColorCalculation.GetHSVFromRgbValue(tailColor);

            var leadHue = leadHsv.Item1;
            var tailHue = tailHsv.Item1;

            var startHsv = (leadHue > tailHue) ? leadHsv : tailHsv;
            var endHsv = (leadHue > tailHue) ? tailHsv : leadHsv;

            var hueDifference = 360.0 + endHsv.Item1 - startHsv.Item1;
            var saturationDifference = endHsv.Item2 - startHsv.Item2;
            var valueDifference = endHsv.Item3 - startHsv.Item3;

            var numberOfPartitions = totalNumber - 1;
            var colors = Enumerable.Range(1, (numberOfPartitions - 1)).
                Select(e => ColorCalculation.GetRgbValueFromHsv(GetPartingHue(startHsv.Item1, hueDifference, e, numberOfPartitions),
                                                                (startHsv.Item2 + saturationDifference * e / numberOfPartitions),
                                                                (startHsv.Item3 + valueDifference * e / numberOfPartitions))
                                                                );

            if (leadHue <= tailHue)
            {
                colors = colors.Reverse();
            }

            return colors;
        }

        public static IEnumerable<int> CreateRgbTransition(int leadColor, int tailColor, int totalNumber)
        {
            double leadR = ((leadColor & 0xff0000) >> 16);
            double leadG = ((leadColor & 0x00ff00) >> 8);
            double leadB = leadColor & 0x0000ff;
            double tailR = ((tailColor & 0xff0000) >> 16);
            double tailG = ((tailColor & 0x00ff00) >> 8);
            double tailB = tailColor & 0x0000ff;

            var rDifference = tailR - leadR;
            var gDifference = tailG - leadG;
            var bDifference = tailB - leadB;

            var numberOfPartitions = totalNumber - 1;
            var colors = Enumerable.Range(1, (numberOfPartitions - 1)).
                Select(e => GetRgbValue(Round(leadR + rDifference * e / numberOfPartitions),
                                        Round(leadG + gDifference * e / numberOfPartitions),
                                        Round(leadB + bDifference * e / numberOfPartitions))
                                        );

            return colors;
        }

        private static double GetPartingHue(double start, double difference, int index, int numberOfPartitions)
        {
            var parting = start + (difference * index / numberOfPartitions);
            if (360.0 <= parting)
            {
                parting -= 360.0;
            }

            return parting;
        }

        private static int GetRgbValue(int r, int g, int b)
        {
            return ((r & 0xff) << 16) | ((g & 0xff) << 8) | (b & 0xff);
        }

        private static int Round(double value)
        {
            return (int)Math.Round(value, 0, MidpointRounding.AwayFromZero);
        }
    }
}
