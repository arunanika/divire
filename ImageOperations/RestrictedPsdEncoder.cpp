/**
divire

Copyright (C) 2020 Aru Nanika

This program is released under the MIT License.
https://opensource.org/licenses/MIT
*/

#include "stdafx.h"
#include "RestrictedPsdEncoder.h"

//================================//
//==    Constructor             ==//
//================================//

RestrictedPsdEncoder::RestrictedPsdEncoder()
{
}

//================================//
//==    Destructor              ==//
//================================//

RestrictedPsdEncoder::~RestrictedPsdEncoder()
{
    CloseFile();
}

//========================================//
//==    Instance methods (Private)      ==//
//========================================//

const std::array<uchar, 2>& RestrictedPsdEncoder::GetBytesOf16BitUnsigned(ushort number)
{
    static std::array<uchar, 2> bytes;

    bytes[0] = static_cast<uchar>(number >> 8);
    bytes[1] = static_cast<uchar>(number & 0xff);

    return bytes;
}

const std::array<uchar, 4>& RestrictedPsdEncoder::GetBytesOf32BitSigned(int number)
{
    static std::array<uchar, 4> bytes;

    bytes[0] = static_cast<uchar>(number >> 24);
    bytes[1] = static_cast<uchar>((number >> 16) & 0xff);
    bytes[2] = static_cast<uchar>((number >> 8) & 0xff);
    bytes[3] = static_cast<uchar>(number & 0xff);

    return bytes;
}

BOOL RestrictedPsdEncoder::Write16BitUnsigned(ushort number)
{
    return WriteData(&(GetBytesOf16BitUnsigned(number))[0], 2);
}

BOOL RestrictedPsdEncoder::Write32BitSigned(int number)
{
    return WriteData(&(GetBytesOf32BitSigned(number))[0], 4);
}

BOOL RestrictedPsdEncoder::WritePadding(size_t length)
{
    std::vector<uchar> padding(length, 0);

    return WriteData(padding.data(), length);
}

int RestrictedPsdEncoder::GetFixedLayerInformationLength(int nameLength)
{
    int dataLength;

    BOOL isEvenNumber = (0 == (nameLength % 2));
    int paddingLength = (isEvenNumber) ? 1 : 2;

    dataLength =
        LENGTH_OF_INFORMATION_BASIC_PART
        + 4                 // The length of extended part.
        + 4                 // The mask data length.
        + 4                 // The blending range data length.
        + 1                 // The layer name length.
        + nameLength        // The layer name.
        + paddingLength     // Padding.
        + 4                 // "8BIM".
        + 4                 // "luni".
        + 4                 // The data length of "luni".
        + 4                 // The Unicode name length.
        + (nameLength * 2)  // The Unicode layer name.
        ;

    return dataLength;
}

long RestrictedPsdEncoder::GetCurrentChannelDataOffset()
{
    int layerIndex = m_writtenChannelCount / CHANNELS_COUNT;
    int channelIndex = m_writtenChannelCount % CHANNELS_COUNT;
    int channelOffset = OFFSET_OF_CHANNEL_LENGTH_ALPHA;

    if (CHANNEL_RED == channelIndex)
    {
        channelOffset = OFFSET_OF_CHANNEL_LENGTH_RED;
    }
    else if (CHANNEL_GREEN == channelIndex)
    {
        channelOffset = OFFSET_OF_CHANNEL_LENGTH_GREEN;
    }
    else if (CHANNEL_BLUE == channelIndex)
    {
        channelOffset = OFFSET_OF_CHANNEL_LENGTH_BLUE;
    }

    return (m_positionsOfLayerInformation[layerIndex] + channelOffset);
}

BOOL RestrictedPsdEncoder::WriteData(const uchar* dataBytes, size_t length)
{
    BOOL bResult = FALSE;

    auto written = fwrite(dataBytes, 1, length, m_fileIO);
    bResult = (length == bResult);

    return bResult;
}

//========================================//
//==    Instance methods (Public)       ==//
//========================================//

BOOL RestrictedPsdEncoder::OpenFile(std::string destinationPath)
{
    fopen_s(&m_fileIO, destinationPath.c_str(), "wb");

    return (0 != m_fileIO);
}

void RestrictedPsdEncoder::Initialize(int width, int height, int layersCount)
{
    m_imageWidth = width;
    m_imageHeight = height;
    m_layersCount = layersCount;
    m_positionsOfLayerInformation.reserve(m_layersCount);

    m_writtenSize = 0;
    m_writtenChannelCount = 0;
}

void RestrictedPsdEncoder::WriteBeforeLayersBriefInformation()
{
    if (0 == m_fileIO)
    {
        return;
    }

    fseek(m_fileIO, 0, SEEK_SET);

    // [00] | The psd file signature ("8BPS").
    // [01] |
    // [02] |
    // [03] |
    WriteData(PSD_SIGNATURE, 4);

    // [04] | The psd file version.
    // [05] | Use the version "1".
    Write16BitUnsigned(1);

    // [06] | Padding.
    // [07] |
    // [08] |
    // [09] |
    // [0A] |
    // [0B] |
    WritePadding(6);

    // [0C] | The number of channels.
    // [0D] |
    Write16BitUnsigned(4);

    // [0E] | The image height.
    // [0F] |
    // [10] |
    // [11] |
    Write32BitSigned(m_imageHeight);

    // [12] | The image width.
    // [13] |
    // [14] |
    // [15] |
    Write32BitSigned(m_imageWidth);

    // [16] | The bit depth.
    // [17] | (Use 8Bit.)
    Write16BitUnsigned(8);

    // [18] | The color mode.
    // [19] | (Use 3.)
    Write16BitUnsigned(3);

    // [1A] | The color data length.
    // [1B] | In this class,
    // [1C] | use 0 to omit it intentionally.
    // [1D] |
    Write32BitSigned(0);

    // [1E] | The resource data length.
    // [1F] | In this class,
    // [20] | use 0 to omit it intentionally.
    // [21] |
    Write32BitSigned(0);

    // [22] | The total size of the information section.
    // [23] | In this method,
    // [24] | write preliminary bytes.
    // [25] |
    Write32BitSigned(0x7fffffff);

    // [26] | The total size of the layers information.
    // [27] | In this method,
    // [28] | write preliminary bytes.
    // [29] |
    Write32BitSigned(0x7fffffff);

    // [2A] | The number of layers.
    // [2B] |
    Write16BitUnsigned(static_cast<ushort>(m_layersCount));
}

void RestrictedPsdEncoder::SetLayerNames(std::vector<std::string>& names)
{
    m_layerNames.clear();
    m_layerNames.reserve(m_layersCount);
    m_layerNames = names;
}

void RestrictedPsdEncoder::WriteLayersBriefInformation(std::string blendMode, uchar opacity, BOOL hasSolidBackground)
{
    if (0 == m_fileIO)
    {
        return;
    }

    long offsetOfLayerInformation = LENGTH_BEFORE_LAYERS_INFORMATION;

    m_positionsOfLayerInformation.clear();

    std::vector<uchar> vecBasicPart(LENGTH_OF_INFORMATION_BASIC_PART);
    // The basic part.
    std::copy(
        std::begin(LAYERS_INFORMATION_BASIC_PART_FRAME),
        std::end(LAYERS_INFORMATION_BASIC_PART_FRAME),
        vecBasicPart.begin()
    );
    // Blend mode
    std::copy(
        std::begin(blendMode),
        std::end(blendMode),
        (vecBasicPart.begin() + OFFSET_OF_BLEND_MODE)
    );
    // Opacity
    vecBasicPart[OFFSET_OF_OPACITY] = opacity;

    // Set all layers information.
    for (int i = 0; i < m_layersCount; i++)
    {
        // Store the position of each layer information. 
        m_positionsOfLayerInformation.push_back(offsetOfLayerInformation);

        // Initialize a data length with the layer name.
        std::string layerName = m_layerNames[i];
        int nameLength = static_cast<int>(layerName.length());
        int dataLength = GetFixedLayerInformationLength(nameLength);
        std::vector<uchar> vecData(dataLength, 0x00);

        // The basic part.
        if ((0 == i) && hasSolidBackground)
        {
            std::copy(
                std::begin(LAYERS_INFORMATION_BASIC_PART_FRAME),
                std::end(LAYERS_INFORMATION_BASIC_PART_FRAME),
                vecData.begin()
            );
        }
        else
        {
            std::copy(
                std::begin(vecBasicPart),
                std::end(vecBasicPart),
                vecData.begin()
            );
        }

        // The length of extended part.
        auto bytesOfExtendedLength = GetBytesOf32BitSigned((dataLength - LENGTH_OF_INFORMATION_BASIC_PART - 4));
        std::copy(
            std::begin(bytesOfExtendedLength),
            std::end(bytesOfExtendedLength),
            (vecData.begin() + LENGTH_OF_INFORMATION_BASIC_PART)
        );

        // The mask data length. -> 0 (Omit data.)
        // The blending range data length. -> 0 (Omit data.)

        // The layer name length.
        vecData[OFFSET_OF_LAYER_NAME_LENGTH] = static_cast<uchar>(nameLength);

        // The layer name.
        std::copy(
            layerName.begin(),
            layerName.end(),
            (vecData.begin() + OFFSET_OF_LAYER_NAME)
        );

        int paddingLength = (0 == (nameLength % 2)) ? 1 : 2;
        int offsetOfTag = OFFSET_OF_LAYER_NAME + nameLength + paddingLength;

        // "8BIM"
        std::copy(
            std::begin(PSD_INFORMATION_TAG),
            std::end(PSD_INFORMATION_TAG),
            (vecData.begin() + offsetOfTag)
        );

        // "luni"
        std::copy(
            std::begin(TAG_ELEMENT_UNICODE),
            std::end(TAG_ELEMENT_UNICODE),
            (vecData.begin() + offsetOfTag + 4)
        );

        // The data length of "luni".
        auto bytesOfTagDataLength = GetBytesOf32BitSigned((2 * nameLength) + 4);
        std::copy(
            std::begin(bytesOfTagDataLength),
            std::end(bytesOfTagDataLength),
            (vecData.begin() + offsetOfTag + 8)
        );

        // The Unicode name length.
        auto bytesOfUnicodeNameLength = GetBytesOf32BitSigned(nameLength);
        std::copy(
            std::begin(bytesOfUnicodeNameLength),
            std::end(bytesOfUnicodeNameLength),
            (vecData.begin() + offsetOfTag + 12)
        );

        // The Unicode name.
        int unicodeNamePosition = offsetOfTag + 16;
        for (int j = 0; j < nameLength; j++)
        {
            vecData[unicodeNamePosition + (2 * j) + 1] = layerName[j];
        }

        // Write layer information.
        WriteData(vecData.data(), dataLength);

        offsetOfLayerInformation += dataLength;
    }

    m_writtenSize = offsetOfLayerInformation;
}

void RestrictedPsdEncoder::WriteLayerCoordinates(int layerIndex, int top, int left, int height, int width)
{
    if (0 == m_fileIO)
    {
        return;
    }

    long offset = m_positionsOfLayerInformation[layerIndex];

    fseek(m_fileIO, offset, SEEK_SET);

    Write32BitSigned(top);
    Write32BitSigned(left);
    Write32BitSigned(top + height);
    Write32BitSigned(left + width);

    fseek(m_fileIO, 0, SEEK_END);
}

void RestrictedPsdEncoder::WriteAdditionalChannelOfLayer(cv::Mat& data)
{
    if (0 == m_fileIO)
    {
        return;
    }

    fseek(m_fileIO, 0, SEEK_END);

    int height = data.rows;
    int width = data.cols;
    std::vector<ushort> rowDataLengths(height);
    int rowsDataTotalSize = 0;

    // Write the RLE compression identifier.
    Write16BitUnsigned(RLE_COMPRESSION_INDEX);

    // Write the data lengths of the rows preliminarily.
    WritePadding(2 * height);

    // Write RLE compression rows data.
    for (int rowIndex = 0; rowIndex < height; rowIndex++)
    {
        auto row = data.row(rowIndex);
        std::vector<uchar> vecRow(row.begin<uchar>(), row.end<uchar>());

        int maxLength = RleCompression::GetMaxLengthOfCompressedRow(width);
        std::vector<uchar> buffer(maxLength);
        int dataLength = m_rleCompression.ArrangeRlePatternOfRow(buffer, vecRow);

        rowDataLengths[rowIndex] = static_cast<ushort>(dataLength);

        // Write RLE row data.
        WriteData(buffer.data(), dataLength);

        rowsDataTotalSize += dataLength;
    }

    //  Write actual values of the row data lengths.
    fseek(m_fileIO, - (rowsDataTotalSize + (2 * height)), SEEK_END);
    for (int rowIndex = 0; rowIndex < height; rowIndex++)
    {
        Write16BitUnsigned(rowDataLengths[rowIndex]);
    }

    int totalChannelData = 2 + 2 * height + rowsDataTotalSize;

    // Write the channel data total length.
    fseek(m_fileIO, GetCurrentChannelDataOffset(), SEEK_SET);
    Write32BitSigned(totalChannelData);

    // Update writing status.
    fseek(m_fileIO, 0, SEEK_END);
    m_writtenChannelCount++;
    m_writtenSize += totalChannelData;
}

void RestrictedPsdEncoder::WriteAdditionalSingleValueChannel(uchar value, int height, int width)
{
    if (0 == m_fileIO)
    {
        return;
    }

    fseek(m_fileIO, 0, SEEK_END);

    int rowsDataTotalSize = 0;

    ushort rleLength = static_cast<ushort>(RleCompression::GetLengthOfCompressedSingleValueRow(width));
    std::vector<uchar> buffer(rleLength);
    RleCompression::ArrangeRlePatternOfSingleValueRow(buffer, value, width);

    // Write the RLE compression identifier.
    Write16BitUnsigned(RLE_COMPRESSION_INDEX);

    // Write the data lengths of the rows preliminarily.
    for (int rowIndex = 0; rowIndex < height; rowIndex++)
    {
        Write16BitUnsigned(rleLength);
    }

    // Write RLE compression rows data.
    for (int rowIndex = 0; rowIndex < height; rowIndex++)
    {
        // Write RLE row data.
        WriteData(buffer.data(), rleLength);

        rowsDataTotalSize += rleLength;
    }

    int totalChannelData = 2 + 2 * height + rowsDataTotalSize;

    // Write the channel data total length.
    fseek(m_fileIO, GetCurrentChannelDataOffset(), SEEK_SET);
    Write32BitSigned(totalChannelData);

    // Update writing status.
    fseek(m_fileIO, 0, SEEK_END);
    m_writtenChannelCount++;
    m_writtenSize += totalChannelData;
}

void RestrictedPsdEncoder::WriteLayersInformationSize()
{
    if (0 == m_fileIO)
    {
        return;
    }

    int layerInformationSize = static_cast<int>(m_writtenSize - (POSITION_OF_LAYERS_INFORMATION_SIZE + 4));

    // Adjust the length of the layer section to a multiple of 4. 
    int remainder = layerInformationSize % 4;

    if (0 < remainder)
    {
        int fillingLength = 4 - remainder;

        // Write padding.
        WritePadding(fillingLength);

        layerInformationSize += fillingLength;
        m_writtenSize += fillingLength;
    }

    // Write 
    fseek(m_fileIO, POSITION_OF_LAYERS_INFORMATION_SIZE, SEEK_SET);
    Write32BitSigned(layerInformationSize);
    fseek(m_fileIO, 0, SEEK_END);
}

void RestrictedPsdEncoder::WriteEmptyMaskData()
{
    if (0 == m_fileIO)
    {
        return;
    }

    // Write the mask data size. (Use "0".)
    Write32BitSigned(0);
    m_writtenSize += 4;
}

void RestrictedPsdEncoder::WriteInformationSectionSize()
{
    if (0 == m_fileIO)
    {
        return;
    }

    int sectionTotalSize = static_cast<int>(m_writtenSize - (POSITION_OF_INFORMATION_SECTION_SIZE + 4));

    // Write 
    fseek(m_fileIO, POSITION_OF_INFORMATION_SECTION_SIZE, SEEK_SET);
    Write32BitSigned(sectionTotalSize);
    fseek(m_fileIO, 0, SEEK_END);
}

void RestrictedPsdEncoder::WriteCompositeImage(cv::Mat& compositeImage)
{
    if (0 == m_fileIO)
    {
        return;
    }

    fseek(m_fileIO, 0, SEEK_END);

    int height = compositeImage.rows;
    int countOfAllRows = 4 * height;
    int width = compositeImage.cols;
    std::vector<ushort> rowDataLengths(countOfAllRows);
    int rowsDataTotalSize = 0;

    // Write the RLE compression identifier.
    Write16BitUnsigned(RLE_COMPRESSION_INDEX);

    // Write the area of the row lengths preliminarily.
    WritePadding(2 * countOfAllRows);

    // Write RLE compression rows in color channel.
    cv::Mat matColorChannel;
    for (int i = 0; i < 3; i++)
    {
        cv::extractChannel(compositeImage, matColorChannel, (2 - i));

        for (int rowIndex = 0; rowIndex < height; rowIndex++)
        {
            auto row = matColorChannel.row(rowIndex);
            std::vector<uchar> vecRow(row.begin<uchar>(), row.end<uchar>());

            int maxLength = RleCompression::GetMaxLengthOfCompressedRow(width);
            std::vector<uchar> buffer(maxLength);
            int dataLength = m_rleCompression.ArrangeRlePatternOfRow(buffer, vecRow);

            rowDataLengths[rowIndex + (i * height)] = static_cast<ushort>(dataLength);

            // Write RLE row data.
            WriteData(buffer.data(), dataLength);

            rowsDataTotalSize += dataLength;
        }
    }

    // Write RLE compression rows in alpha channel.
    ushort rleLength = static_cast<ushort>(RleCompression::GetLengthOfCompressedSingleValueRow(width));
    std::vector<uchar> rleDataBuffer(rleLength);
    RleCompression::ArrangeRlePatternOfSingleValueRow(rleDataBuffer, 255, width);
    for (int rowIndex = 0; rowIndex < height; rowIndex++)
    {
        rowDataLengths[rowIndex + (3 * height)] = static_cast<ushort>(rleLength);

        // Write RLE row data.
        WriteData(rleDataBuffer.data(), rleLength);
        rowsDataTotalSize += rleLength;
    }

    //  Write actual values of the row data lengths.
    fseek(m_fileIO, -(rowsDataTotalSize + (2 * countOfAllRows)), SEEK_END);
    for (int i = 0; i < countOfAllRows; i++)
    {
        Write16BitUnsigned(rowDataLengths[i]);
    }

    int totalChannelData = 2 + 2 * countOfAllRows + rowsDataTotalSize;
    m_writtenSize += totalChannelData;
}

void RestrictedPsdEncoder::CloseFile()
{
    if (m_fileIO)
    {
        fclose((FILE*)m_fileIO);
        m_fileIO = 0;
    }
}
