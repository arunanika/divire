/**
divire

Copyright (C) 2020 Aru Nanika

This program is released under the MIT License.
https://opensource.org/licenses/MIT
*/

#include "stdafx.h"
#include "RleCompression.h"

//================================//
//==    Constructor             ==//
//================================//

RleCompression::RleCompression()
{
}

//================================//
//==    Destructor              ==//
//================================//

RleCompression::~RleCompression()
{
}

//================================================================//
//==    Private Methods (Instance)                              ==//
//================================================================//

void RleCompression::StoreFirstByteOfPacket(UCHAR firstByte)
{
    m_lastByte = firstByte;
    m_packetLength = 1;
    m_repeatingLength = 1;
}

void RleCompression::OutputRlePacketData(std::vector<UCHAR>& destination, std::vector<UCHAR>& source)
{
    if (m_packsRepeat)
    {
        // Write the packing style byte as "0xFF - LENGTH OF REPEATS + 2".
        destination[m_outputPosition++] = 0xff - m_repeatingLength + 2;

        // Write the repeating value.
        destination[m_outputPosition++] = m_lastByte;
    }
    else
    {
        // Write the packet of raw bytes.
        OutputRawBytesRlePacket(m_packetLength, destination, source);
    }
}

void RleCompression::OutputRawBytesRlePacket(UCHAR countOfRawBytes, std::vector<UCHAR>& destination, std::vector<UCHAR>& source)
{
    // Write the packing style byte as "LENGTH OF RAW BYTES - 1".
    destination[m_outputPosition++] = (countOfRawBytes - 1);

    // Copy raw bytes.
    std::copy(
        (source.begin() + m_positionOfHeadByte),
        (source.begin() + m_positionOfHeadByte + countOfRawBytes),
        (destination.begin() + m_outputPosition)
    );
    m_outputPosition += countOfRawBytes;
}

//================================================================//
//==    Public Methods (Static)                                 ==//
//================================================================//

int RleCompression::GetMaxLengthOfCompressedRow(int width)
{
    return ((width / 3) * 4 + 3);
}

int RleCompression::GetLengthOfCompressedSingleValueRow(int width)
{
    int quotient = width / 0x80;
    int remainder = width % 0x80;

    int countOfRepeatExpression = (0 == remainder) ? quotient : (quotient + 1);
    int rleLengthOfRow = 2 * countOfRepeatExpression;

    return rleLengthOfRow;
}

void RleCompression::ArrangeRlePatternOfSingleValueRow(std::vector<UCHAR>& destination, UCHAR value, int width)
{
    int quotient = width / 0x80;
    int remainder = width % 0x80;

    int countOfRepeatExpression = (0 == remainder) ? quotient : (quotient + 1);
    int rleLengthOfRow = 2 * countOfRepeatExpression;

    for (int i = 0; i < quotient; i++)
    {
        destination[(2 * i)] = 0x81;
        destination[(2 * i) + 1] = value;
    }

    if (0 != remainder)
    {
        if (1 != remainder) {
            UCHAR repeatExpression = 0xff - remainder + 2;

            destination[(2 * quotient)] = repeatExpression;
            destination[(2 * quotient) + 1] = value;
        }
        else
        {
            destination[(2 * quotient)] = 0x00;
            destination[(2 * quotient) + 1] = value;
        }
    }
}

//================================================================//
//==    Public Methods (Instance)                               ==//
//================================================================//

int RleCompression::ArrangeRlePatternOfRow(std::vector<UCHAR>& destination, std::vector<UCHAR>& source)
{
    // Initialise members.
    m_packsRepeat = FALSE;
    m_lastByte = source[0];
    m_packetLength = 0;

    m_positionOfHeadByte = 0;
    m_outputPosition = 0;

    // Get length of source bytes.
    int bytesLength = static_cast<int>(source.size());

    // Scan source bytes.
    for (int i = 0; i < bytesLength; i++)
    {
        int readByte = source[i];

        if (0 == m_packetLength)
        {
            StoreFirstByteOfPacket(readByte);
        }

        else if (1 == m_packetLength)
        {
            m_packsRepeat = (readByte == m_lastByte);
            m_packetLength++;

            if (m_packsRepeat)
            {
                m_repeatingLength++;
            }
            else
            {
                m_repeatingLength = 1;
                m_lastByte = readByte;
            }
        }

        else if (0x80 == m_packetLength)
        {
            OutputRlePacketData(destination, source);

            StoreFirstByteOfPacket(readByte);

            m_positionOfHeadByte = i;
        }

        else
        {
            if (m_packsRepeat)
            {
                if (readByte == m_lastByte)
                {
                    m_packetLength++;
                    m_repeatingLength++;
                }
                else
                {
                    OutputRlePacketData(destination, source);

                    StoreFirstByteOfPacket(readByte);

                    m_packsRepeat = FALSE;
                    m_positionOfHeadByte = i;
                }
            }

            else
            {
                m_packetLength++;

                if (readByte == m_lastByte)
                {
                    m_repeatingLength++;

                    // If bytes array contains more than 2 repeating bytes, RLE compression can decrease size of bytes.
                    if (2 < m_repeatingLength)
                    {
                        OutputRawBytesRlePacket((m_packetLength - 3), destination, source);

                        m_packsRepeat = TRUE;
                        m_positionOfHeadByte = i - 2;

                        StoreFirstByteOfPacket(readByte);
                        m_packetLength += 2;
                        m_repeatingLength += 2;
                    }
                }
                else
                {
                    m_repeatingLength = 1;
                    m_lastByte = readByte;
                }
            }
        }
    }

    // After scanning.
    if (0 < m_packetLength)
    {
        OutputRlePacketData(destination, source);
    }

    return m_outputPosition;
}
