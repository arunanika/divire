/**
divire

Copyright (C) 2020 Aru Nanika

This program is released under the MIT License.
https://opensource.org/licenses/MIT
*/

#pragma once
#include "RleCompression.h"
#include <opencv2/core.hpp>

//----------------------------------------------------------------
// Class which encodes PSD file in fixed format.
//----------------------------------------------------------------
class RestrictedPsdEncoder
{

//--------------------------------
// Definitions (Private)
//--------------------------------

private:
    // PSD file signature "8BPS"
    static constexpr uchar PSD_SIGNATURE[] = { 0x38, 0x42, 0x50, 0x53 };

    // Information tag "8BIM"
    static constexpr uchar PSD_INFORMATION_TAG[] = { 0x38, 0x42, 0x49, 0x4D };

    static const int POSITION_OF_INFORMATION_SECTION_SIZE = 0x22;
    static const int POSITION_OF_LAYERS_INFORMATION_SIZE = 0x26;

    static const int LENGTH_BEFORE_LAYERS_INFORMATION = 44;

    static constexpr uchar LAYERS_INFORMATION_BASIC_PART_FRAME[] = {
        0x00,   // 00 | The top of the layer.
        0x00,   // 01 |
        0x00,   // 02 |
        0x00,   // 03 |

        0x00,   // 04 | The left of the layer.
        0x00,   // 05 |
        0x00,   // 06 |
        0x00,   // 07 |

        0x00,   // 08 | The bottom of the layer.
        0x00,   // 09 |
        0x00,   // 10 |
        0x00,   // 11 |

        0x00,   // 12 | The right of the layer.
        0x00,   // 13 |
        0x00,   // 14 |
        0x00,   // 15 |

        0x00,   // 16 | The number of channels.
        0x04,   // 17 | In this class, Use 4 as the fixed value.

        0xFF,   // 18 | The ID of the alpha channel.
        0xFF,   // 19 | "-1"

        0x00,   // 20 | The data length of the alpha channel.
        0x00,   // 21 |
        0x00,   // 22 |
        0x00,   // 23 |

        0x00,   // 24 | The ID of the red channel.
        0x00,   // 25 | "0"

        0x00,   // 26 | The data length of the red channel.
        0x00,   // 27 |
        0x00,   // 28 |
        0x00,   // 29 |

        0x00,   // 30 | The ID of the green channel.
        0x01,   // 31 | "1"

        0x00,   // 32 | The data length of the green channel.
        0x00,   // 33 |
        0x00,   // 34 |
        0x00,   // 35 |

        0x00,   // 36 | The ID of the blue channel.
        0x02,   // 37 | "2"

        0x00,   // 38 | The data length of the blue channel.
        0x00,   // 39 |
        0x00,   // 40 |
        0x00,   // 41 |

        0x38,   // 42 | The tag "8BIM".
        0x42,   // 43 |
        0x49,   // 44 |
        0x4D,   // 45 |

        0x6E,   // 46 | The layer blending mode.
        0x6F,   // 47 | Use "norm" as the fixed value.
        0x72,   // 48 |
        0x6D,   // 49 |

        0xFF,   // 50 | The opacity. Use the maximum value fixedly.

        0x00,   // 51 | The clipping.

        0x00,   // 52 | Various flags.

        0x00,   // 53 | Padding.
    };

    static const int LENGTH_OF_INFORMATION_BASIC_PART = 54;
    static const int OFFSET_OF_CHANNEL_LENGTH_ALPHA = 20;
    static const int OFFSET_OF_CHANNEL_LENGTH_RED = 26;
    static const int OFFSET_OF_CHANNEL_LENGTH_GREEN = 32;
    static const int OFFSET_OF_CHANNEL_LENGTH_BLUE = 38;
    static const int OFFSET_OF_BLEND_MODE = 46;
    static const int OFFSET_OF_OPACITY = 50;
    static const int OFFSET_OF_LAYER_NAME_LENGTH = LENGTH_OF_INFORMATION_BASIC_PART + 12;
    static const int OFFSET_OF_LAYER_NAME = OFFSET_OF_LAYER_NAME_LENGTH + 1;

    // Unicode information tag "luni"
    static constexpr uchar TAG_ELEMENT_UNICODE[] = { 0x6C, 0x75, 0x6E, 0x69 };

    // Identifier of RLE compression
    static constexpr ushort RLE_COMPRESSION_INDEX = 0x01;

    static const int CHANNELS_COUNT = 4;
    static const int CHANNEL_RED = 1;
    static const int CHANNEL_GREEN = 2;
    static const int CHANNEL_BLUE = 3;

//--------------------------------
// Fields
//--------------------------------
private:
    FILE* m_fileIO = 0;

    RleCompression m_rleCompression;

    size_t m_writtenSize;
    int m_writtenChannelCount;

    int m_imageWidth;
    int m_imageHeight;
    int m_layersCount;

    std::vector<std::string> m_layerNames;
    std::vector<long> m_positionsOfLayerInformation;

//--------------------------------
// Constructor & Destructor
//--------------------------------
public:
    RestrictedPsdEncoder();
    ~RestrictedPsdEncoder();

//--------------------------------
// Methods (Private)
//--------------------------------
private:
    BOOL WriteData(const uchar* dataBytes, size_t length);
    const std::array<uchar, 2>& GetBytesOf16BitUnsigned(ushort number);
    const std::array<uchar, 4>& GetBytesOf32BitSigned(int number);
    BOOL Write16BitUnsigned(ushort number);
    BOOL Write32BitSigned(int number);
    BOOL WritePadding(size_t length);
    static int GetFixedLayerInformationLength(int nameLength);
    long GetCurrentChannelDataOffset();

//--------------------------------
// Methods (Public)
//--------------------------------
public:
    BOOL OpenFile(std::string destinationPath);
    void Initialize(int width, int height, int layersCount);
    void WriteBeforeLayersBriefInformation();
    void SetLayerNames(std::vector<std::string>& names);
    void WriteLayersBriefInformation(std::string blendMode, uchar opacity, BOOL hasSolidBackground);

    void WriteLayerCoordinates(int layerIndex, int top, int left, int height, int width);
    void WriteAdditionalChannelOfLayer(cv::Mat& data);
    void WriteAdditionalSingleValueChannel(uchar value, int height, int width);

    void WriteLayersInformationSize();
    void WriteEmptyMaskData();
    void WriteInformationSectionSize();

    void WriteCompositeImage(cv::Mat& compositeImage);

    void CloseFile();

};
