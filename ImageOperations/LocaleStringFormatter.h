/**
divire

Copyright (C) 2020 Aru Nanika

This program is released under the MIT License.
https://opensource.org/licenses/MIT
*/

#pragma once
class LocaleStringFormatter
{
public:
	LocaleStringFormatter();
	~LocaleStringFormatter();

private:
	static const std::string GetMultiByteString(std::wstring wstr);

public:
	static const std::string GetSuitableString(LPTSTR tszFileName);
};
