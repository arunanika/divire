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
using divire.Protocols;

namespace divire.Models
{
    public static class ImageOperationsWrapper
    {
        public static void SetDivisionScheme(bool hasPeriphery, bool hasRegions, bool hasMisc)
        {
            NativeMethods.SetDivisionScheme(hasPeriphery, hasRegions, hasMisc);
        }

        public static void SetColorsForDivision(IEnumerable<int> regionColors, int peripheryColor, int miscColor)
        {
            var length = regionColors.Count();
            int[] colors = regionColors.ToArray();

            NativeMethods.SetColorsForDivision(colors, length, peripheryColor, miscColor);
        }

        public static void SetOrderOfRegions(OperationProtocol.OrderOfRegions order)
        {
            var hasOrderOfArea = (OperationProtocol.OrderOfRegions.FromUpperLeftToLowerRight != order);
            var isDescendingAreaOrder = (OperationProtocol.OrderOfRegions.AreaAscending != order);

            NativeMethods.SetOrderOfRegionsForDivision(hasOrderOfArea, isDescendingAreaOrder);
        }

        public static void SetBackgroundStyle(bool hasBackground, OperationProtocol.BackgroundStyle style)
        {
            var duplicates = (OperationProtocol.BackgroundStyle.SourceImage == style);

            NativeMethods.SetBackgroundStyleForDivision(hasBackground, duplicates);
        }

        public static void SetLayerOptions(OperationProtocol.BlendMode blendMode, byte opacity)
        {
            var identifier = OperationProtocol.GetBlendModeIdentifier(blendMode);

            NativeMethods.SetLayerOptionsForDivision(identifier, opacity);
        }

        public static void SetThreshold(byte threshold)
        {
            NativeMethods.SetThresholdForDivision(threshold);
        }

        public static void SetReferenceOfPeriphery(OperationProtocol.CornerPoint corner)
        {
            var refersToRowEnd = ((OperationProtocol.CornerPoint.TopRight == corner) || (OperationProtocol.CornerPoint.BottomRight == corner));
            var refersToColumnEnd = ((OperationProtocol.CornerPoint.BottomLeft == corner) || (OperationProtocol.CornerPoint.BottomRight == corner));

            NativeMethods.SetReferenceOfPeriphery(refersToRowEnd, refersToColumnEnd);
        }

        public static void SetPeripheryManagement(bool excludesPeriphery)
        {
            NativeMethods.SetPeripheryManagement(excludesPeriphery);
        }

        public static void SetColorOverwhelming(bool isOverwhelming, int maxThickness, bool isOpeningEnabled)
        {
            NativeMethods.SetColorOverwhelming(isOverwhelming, maxThickness, isOpeningEnabled);
        }

        public static bool ExecuteDividing(string destination, string source, OperationProtocol.ExportStyle exportStyle)
        {
            var isLayeredStyle = (OperationProtocol.ExportStyle.Psd == exportStyle);
            var hasAlpha = !(OperationProtocol.ExportStyle.Jpg == exportStyle);

            return NativeMethods.ExecuteDividing(destination, source, isLayeredStyle, hasAlpha);
        }
    }
}
