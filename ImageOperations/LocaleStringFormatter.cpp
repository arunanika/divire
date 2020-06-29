/**
divire

Copyright (C) 2020 Aru Nanika

This program is released under the MIT License.
https://opensource.org/licenses/MIT
*/

#include "stdafx.h"
#include "LocaleStringFormatter.h"
#include <sstream>

LocaleStringFormatter::LocaleStringFormatter()
{
}

LocaleStringFormatter::~LocaleStringFormatter()
{
}

const std::string LocaleStringFormatter::GetMultiByteString(std::wstring wstr)
{
    char buffer[1024];
    int length = WideCharToMultiByte(CP_ACP, WC_NO_BEST_FIT_CHARS, wstr.c_str(), -1, NULL, 0, NULL, NULL);
    WideCharToMultiByte(CP_ACP, WC_NO_BEST_FIT_CHARS, wstr.c_str(), -1, buffer, length, NULL, NULL);

    const std::string strMultiByteString(buffer);

    return strMultiByteString;
}

const std::string LocaleStringFormatter::GetSuitableString(LPTSTR tszFileName)
{
    std::wostringstream  wofs;
    wofs << tszFileName;
    std::wstring wstr = wofs.str();

    std::string suitableString = GetMultiByteString(wstr);

    return suitableString;
}
