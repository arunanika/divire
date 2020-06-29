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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace divire.Models
{
    internal static class NativeMethods
    {
        [DllImport("ImageOperations.dll")]
        public static extern void SetDivisionScheme(
            [param: MarshalAs(UnmanagedType.Bool)] bool hasPeriphery,
            [param: MarshalAs(UnmanagedType.Bool)] bool hasRegions,
            [param: MarshalAs(UnmanagedType.Bool)] bool hasMisc
            );

        [DllImport("ImageOperations.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetColorsForDivision(
            [In, Out] int[] regionColors,
            int regionsCount,
            int peripheryColor,
            int miscColor
            );

        [DllImport("ImageOperations.dll")]
        public static extern void SetOrderOfRegionsForDivision(
            [param: MarshalAs(UnmanagedType.Bool)] bool hasOrderOfArea,
            [param: MarshalAs(UnmanagedType.Bool)] bool isDescendingAreaOrder
            );

        [DllImport("ImageOperations.dll")]
        public static extern void SetBackgroundStyleForDivision(
            [param: MarshalAs(UnmanagedType.Bool)] bool hasBackground,
            [param: MarshalAs(UnmanagedType.Bool)] bool duplicatesSource
            );

        [DllImport("ImageOperations.dll", CharSet = CharSet.Unicode)]
        public static extern void SetLayerOptionsForDivision(
            string blendMode,
            byte opacity
            );

        [DllImport("ImageOperations.dll")]
        public static extern void SetThresholdForDivision(int threshold);

        [DllImport("ImageOperations.dll")]
        public static extern void SetReferenceOfPeriphery(
            [param: MarshalAs(UnmanagedType.Bool)] bool refersToRowEnd,
            [param: MarshalAs(UnmanagedType.Bool)] bool refersToColumnEnd
            );

        [DllImport("ImageOperations.dll")]
        public static extern void SetPeripheryManagement(
            [param: MarshalAs(UnmanagedType.Bool)] bool excludesPeriphery
            );

        [DllImport("ImageOperations.dll")]
        public static extern void SetColorOverwhelming(
            [param: MarshalAs(UnmanagedType.Bool)] bool isOverwhelming,
            int maxThickness,
            [param: MarshalAs(UnmanagedType.Bool)] bool isOpeningEnabled
            );

        [DllImport("ImageOperations.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ExecuteDividing(
            string destination,
            string source,
            [param: MarshalAs(UnmanagedType.Bool)] bool isLayeredStyle,
            [param: MarshalAs(UnmanagedType.Bool)] bool hasAlpha
            );
    }
}
