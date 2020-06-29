/**
divire

Copyright (C) 2020 Aru Nanika

This program is released under the MIT License.
https://opensource.org/licenses/MIT
*/

#include "stdafx.h"
#include "LocaleStringFormatter.h"
#include "DividingOperation.h"

//================================//
//==    Declarations            ==//
//================================//

extern "C" __declspec(dllexport) void SetDivisionScheme(BOOL hasPeriphery, BOOL hasRegions, BOOL hasMisc);

extern "C" __declspec(dllexport) void SetColorsForDivision(int* regionColors, int regionsCount, int peripheryColor, int miscColor);

extern "C" __declspec(dllexport) void SetOrderOfRegionsForDivision(BOOL hasOrderOfArea, BOOL isDescendingAreaOrder);

extern "C" __declspec(dllexport) void SetBackgroundStyleForDivision(BOOL hasBackground, BOOL duplicatesSource);

extern "C" __declspec(dllexport) void SetLayerOptionsForDivision(LPTSTR tszBlendMode, UCHAR opacity);

extern "C" __declspec(dllexport) void SetThresholdForDivision(int threshold);

extern "C" __declspec(dllexport) void SetReferenceOfPeriphery(BOOL refersToRowEnd, BOOL refersToColumnEnd);

extern "C" __declspec(dllexport) void SetPeripheryManagement(BOOL excludesPeriphery);

extern "C" __declspec(dllexport) void SetColorOverwhelming(BOOL isOverwhelming, int maxThickness, BOOL isOpeningEnabled);

extern "C" __declspec(dllexport) BOOL ExecuteDividing(LPTSTR tszDestination, LPTSTR tszSource, BOOL isLayeredStyle, BOOL hasAlpha);

//================================//
//==    Implementation          ==//
//================================//

extern "C" __declspec(dllexport) void SetDivisionScheme(BOOL hasPeriphery, BOOL hasRegions, BOOL hasMisc)
{
    DividingOperation::SetDivisionScheme(hasPeriphery, hasRegions, hasMisc);
}

extern "C" __declspec(dllexport) void SetColorsForDivision(int* regionColors, int regionsCount, int peripheryColor, int miscColor)
{
    DividingOperation::SetColorsForDivision(regionColors, regionsCount, peripheryColor, miscColor);
}

extern "C" __declspec(dllexport) void SetOrderOfRegionsForDivision(BOOL hasOrderOfArea, BOOL isDescendingAreaOrder)
{
    DividingOperation::SetOrderOfRegions(hasOrderOfArea, isDescendingAreaOrder);
}

extern "C" __declspec(dllexport) void SetBackgroundStyleForDivision(BOOL hasBackground, BOOL duplicatesSource)
{
    DividingOperation::SetBackgroundStyle(hasBackground, duplicatesSource);
}

extern "C" __declspec(dllexport) void SetLayerOptionsForDivision(LPTSTR tszBlendMode, UCHAR opacity)
{
    DividingOperation::SetLayerOptions(LocaleStringFormatter::GetSuitableString(tszBlendMode), opacity);
}

extern "C" __declspec(dllexport) void SetThresholdForDivision(int threshold)
{
    DividingOperation::SetThresholdForDivision(threshold);
}

extern "C" __declspec(dllexport) void SetReferenceOfPeriphery(BOOL refersToRowEnd, BOOL refersToColumnEnd)
{
    DividingOperation::SetReferenceOfPeriphery(refersToRowEnd, refersToColumnEnd);
}

extern "C" __declspec(dllexport) void SetPeripheryManagement(BOOL excludesPeriphery)
{
    DividingOperation::SetPeripheryManagement(excludesPeriphery);
}

extern "C" __declspec(dllexport) void SetColorOverwhelming(BOOL isOverwhelming, int maxThickness, BOOL isOpeningEnabled)
{
    DividingOperation::SetColorOverwhelming(isOverwhelming, maxThickness, isOpeningEnabled);
}

extern "C" __declspec(dllexport) BOOL ExecuteDividing(LPTSTR tszDestination, LPTSTR tszSource, BOOL isLayeredStyle, BOOL hasAlpha)
{
    BOOL bResult = FALSE;

    // Get suitable file names for OpenCV.
    auto sourceFileName = LocaleStringFormatter::GetSuitableString(tszSource);
    auto destinationFileName = LocaleStringFormatter::GetSuitableString(tszDestination);

    bResult = DividingOperation::ExecuteDividing(destinationFileName, sourceFileName, isLayeredStyle, hasAlpha);

    return bResult;
}
