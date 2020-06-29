/**
divire

Copyright (C) 2020 Aru Nanika

This program is released under the MIT License.
https://opensource.org/licenses/MIT
*/

#pragma once

//----------------------------------------------------------------
// RLE Compression Class
//----------------------------------------------------------------
class RleCompression
{
private:
    static const int PACK_MODE_UNSPECIFIED = -1;
    static const int PACK_MODE_REPEAT = 0;
    static const int PACK_MODE_RAW = 1;

private:
    BOOL m_packsRepeat;
    UCHAR m_lastByte;
    UCHAR m_packetLength;
    UCHAR m_repeatingLength;

    int m_positionOfHeadByte;
    int m_outputPosition;

public:
    RleCompression();
    ~RleCompression();

private:
    void StoreFirstByteOfPacket(UCHAR firstByte);
    void OutputRlePacketData(std::vector<UCHAR>& destination, std::vector<UCHAR>& source);
    void OutputRawBytesRlePacket(UCHAR countOfRawBytes, std::vector<UCHAR>& destination, std::vector<UCHAR>& source);

public:
    static int GetMaxLengthOfCompressedRow(int width);
    static int GetLengthOfCompressedSingleValueRow(int width);
    static void ArrangeRlePatternOfSingleValueRow(std::vector<UCHAR>& destination, UCHAR value, int width);

public:
    int ArrangeRlePatternOfRow(std::vector<UCHAR>& destination, std::vector<UCHAR>& source);
};
