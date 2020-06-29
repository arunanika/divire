/**
divire

Copyright (C) 2020 Aru Nanika

This program is released under the MIT License.
https://opensource.org/licenses/MIT
*/

#include "stdafx.h"
#include "DividingOperation.h"
#include <opencv2/imgcodecs.hpp>
#include <opencv2/imgproc.hpp>

//================================//
//==    Constructor             ==//
//================================//

DividingOperation::DividingOperation()
{
}

//================================//
//==    Destructor              ==//
//================================//

DividingOperation::~DividingOperation()
{
}

//================================//
//==    Setters                 ==//
//================================//

void DividingOperation::SetDivisionScheme(BOOL hasPeriphery, BOOL hasRegions, BOOL hasMisc)
{
    getInstance().m_hasPeriphery = hasPeriphery;
    getInstance().m_hasRegions = hasRegions;
    getInstance().m_hasMisc = hasMisc;
}

void DividingOperation::SetColorsForDivision(int* regionColors, int regionsCount, int peripheryColor, int miscColor)
{
    getInstance().m_peripheryColor = peripheryColor;
    getInstance().m_miscColor = miscColor;
    getInstance().m_regionsCount = regionsCount;

    int* colors = getInstance().m_regionColors;

    int count = std::min(MAX_REGIONS, regionsCount);

    // Set colors from parameter array. 
    for (int i = 0; i < count; i++)
    {
        colors[i] = regionColors[i];
    }

    // Reset colors out of range.
    for (int i = count; i < MAX_REGIONS; i++)
    {
        colors[i] = 0;
    }
}

void DividingOperation::SetOrderOfRegions(BOOL hasOrderOfArea, BOOL isDescendingAreaOrder)
{
    getInstance().m_hasOrderOfArea = hasOrderOfArea;
    getInstance().m_isDescendingAreaOrder = isDescendingAreaOrder;
}

void DividingOperation::SetBackgroundStyle(BOOL hasBackground, BOOL duplicatesSource)
{
    getInstance().m_hasBackground = hasBackground;
    getInstance().m_duplicatesSource = duplicatesSource;
}

void DividingOperation::SetLayerOptions(std::string blendMode, uchar opacity)
{
    getInstance().m_blendMode = blendMode;
    getInstance().m_layersOpacity = opacity;
}

void DividingOperation::SetThresholdForDivision(int threshold)
{
    getInstance().m_threshold = threshold;
}

void DividingOperation::SetReferenceOfPeriphery(BOOL refersToRowEnd, BOOL refersToColumnEnd)
{
    getInstance().m_refersToRowEnd = refersToRowEnd;
    getInstance().m_refersToColumnEnd = refersToColumnEnd;
}

void DividingOperation::SetPeripheryManagement(BOOL excludesPeriphery)
{
    getInstance().m_excludesPeriphery = excludesPeriphery;
}

void DividingOperation::SetColorOverwhelming(BOOL isOverwhelming, int maxThickness, BOOL isOpeningEnabled)
{
    getInstance().m_isOverwhelming = isOverwhelming;
    getInstance().m_maxThicknessOfLines = maxThickness;
    getInstance().m_isOpeningEnabled = isOpeningEnabled;
}

//================================//
//==    Instance methods        ==//
//================================//

// [Construction of the palette]
//
// N: the number of regions.
//
// [0] - the color of "Perifery"
// [1] - the color of "Region 1"
// [2] - the color of "Region 2"
//  |                      |
//  |                      |
// [N] - the color of "Region N"
// [N+1] - the color of "Misc."

/**
    Brief:
        prepare Colors for opencv operation.

    Parameters:
        palette     Collection of the colors(cv::Vec3b).
*/
void  DividingOperation::SetupPalette(std::vector<int>& palette)
{
    // The number of required colors.（Periphry + Regions + Misc.）
    int specifiedColorsNumber = m_regionsCount + 2;

    std::vector<int> vecPalette(specifiedColorsNumber);

    vecPalette[0] = m_peripheryColor;
    vecPalette[specifiedColorsNumber - 1] = m_miscColor;

    const int* colorValues = m_regionColors;
    for (int i = 1; i < (specifiedColorsNumber - 1); i++)
    {
        vecPalette[i] = colorValues[(i - 1)];
    }

    palette = vecPalette;
}

void DividingOperation::SetupPalette3b(std::vector<cv::Vec3b>& palette)
{
    std::vector<int> basePalette;
    SetupPalette(basePalette);
    size_t length = basePalette.size();

    std::vector<cv::Vec3b> palette3b(length);

    for (int i = 0; i < length; i++)
    {
        palette3b[i] = GetOpencvColor3b(basePalette[i]);
    }

    palette = palette3b;
}

void DividingOperation::SetupPalette4b(std::vector<cv::Vec4b>& palette)
{
    std::vector<int> basePalette;
    SetupPalette(basePalette);
    size_t length = basePalette.size();

    std::vector<cv::Vec4b> palette4b(length);

    for (int i = 0; i < length; i++)
    {
        palette4b[i] = GetOpencvColor4b(basePalette[i]);
    }

    palette = palette4b;
}

/**
    Brief:
        Execute opencv labelling and create labelled matrix.

    Parameters:
        grayImage           The 8-bit single-channel image to be labeled.
        binaryImage         The binarized image.
        labelledMatrix      Destination of labeled matrix.
        labellingInfo       Statistics for each label.
        labelsCount         The total number of labels.
        hasNoComponents     Flag indicating whether the image has components or not.
        hasNoLines          Flag indicating whether the image has lines or not.
*/
void DividingOperation::FillMatrixWithLabels(cv::Mat& grayImage, cv::Mat& binaryImage, cv::Mat& labelledMatrix, cv::Mat& labellingInfo, int& labelsCount, BOOL& hasNoComponents, BOOL& hasNoLines)
{
    // Create binarized image.
    cv::threshold(grayImage, binaryImage, m_threshold, 255, cv::THRESH_BINARY);

    // Validate the binarized image.
    double minValue, maxValue;
    cv::minMaxLoc(binaryImage, &minValue, &maxValue);

    hasNoComponents = FALSE;
    hasNoLines = FALSE;

    if (minValue == maxValue)
    {
        if (maxValue == 0.0)
        {
            hasNoComponents = TRUE;
        }
        else
        {
            hasNoLines = TRUE;
        }
    }

    if (hasNoComponents)
    {
        labelsCount = 1;
        labelledMatrix.setTo(0);
    }
    else if (hasNoLines)
    {
        labelsCount = 1;
        labelledMatrix.setTo(1);
    }
    else
    {
        cv::Mat matCentroids;
        labelsCount = cv::connectedComponentsWithStats(binaryImage, labelledMatrix, labellingInfo, matCentroids, 8);
    }
}

BOOL DividingOperation::GetLabelOfPeriphery(cv::Mat& labelledMatrix, int& labelOfPeriphery)
{
    int imageHeight = labelledMatrix.rows;
    int imageWidth = labelledMatrix.cols;

    // Get index of the periphery
    int referenceRow = (m_refersToRowEnd) ? (imageWidth - 1) : 0;
    int referenceColumn = (m_refersToColumnEnd) ? (imageHeight - 1) : 0;
    labelOfPeriphery = labelledMatrix.at<int>(referenceColumn, referenceRow);

    BOOL isPeripheryValid = (0 != labelOfPeriphery);

    return isPeripheryValid;
}

void DividingOperation::SetupLabelsColorReference(std::vector<int>& reference, std::vector<int>& componentsAreaOrder, BOOL isPeripheryValid, int labelOfPeriphery)
{
    size_t labelsCount = componentsAreaOrder.size();

    // The number of required colors.（Periphry + Regions + Misc.）
    int specifiedColorsNumber = m_regionsCount + 2;

    // Get order of the Periphery.
    int peripheryAreaOrder = componentsAreaOrder[labelOfPeriphery];

    // Whether periphery needs to be inserted.
    BOOL insertsPeriphery = ((isPeripheryValid && (m_hasPeriphery || m_excludesPeriphery)) && (peripheryAreaOrder < m_regionsCount));

    // color reference for components.
    std::vector<int> vecColorReference(labelsCount);

    // 0(: The Lines) is excluded.
    vecColorReference[0] = -1;

    int referenceOutOfRange = (m_hasMisc) ? (specifiedColorsNumber - 1) : -1;

    int specifiedRange = 0;
    if (m_hasRegions)
    {
        specifiedRange = m_regionsCount;

        if (insertsPeriphery)
        {
            specifiedRange++;
        }
    }

    for (int i = 1; i < labelsCount; i++)
    {
        int areaOrder = componentsAreaOrder[i];
        vecColorReference[i] = referenceOutOfRange;

        if (areaOrder < specifiedRange)
        {
            if (insertsPeriphery)
            {
                if (areaOrder < peripheryAreaOrder)
                {
                    vecColorReference[i] = areaOrder + 1;
                }
                else if (areaOrder > peripheryAreaOrder)
                {
                    vecColorReference[i] = areaOrder;
                }
            }
            else
            {
                vecColorReference[i] = areaOrder + 1;
            }
        }
    }

    if (isPeripheryValid)
    {
        if (m_hasPeriphery)
        {
            vecColorReference[labelOfPeriphery] = 0;
        }
        else
        {
            if (m_excludesPeriphery)
            {
                vecColorReference[labelOfPeriphery] = -1;
            }
        }
    }

    reference = vecColorReference;
}

void DividingOperation::SetupColorsActivation(std::vector<BOOL>& colorsActivation, int& countOfEnabledColors, std::vector<int>& labelsColorReference)
{
    auto length = labelsColorReference.size();
    int miscColorIndex = m_regionsCount + 1;
    BOOL detectMisc = FALSE;

    countOfEnabledColors = 0;
    std::vector<BOOL> vecColorsActivation((m_regionsCount + 2), FALSE);

    for (size_t i = 0; i < length; i++)
    {
        int reference = labelsColorReference[i];

        if (0 <= reference)
        {
            if (reference == miscColorIndex)
            {
                if (!detectMisc)
                {
                    detectMisc = TRUE;
                    vecColorsActivation[reference] = TRUE;
                    countOfEnabledColors++;
                }
            }
            else
            {
                vecColorsActivation[reference] = TRUE;
                countOfEnabledColors++;
            }
        }
    }

    colorsActivation = vecColorsActivation;
}

void DividingOperation::StoreApplicableCoordinates(std::vector<cv::Rect>& coordinates, std::vector<int>& colorReference, cv::Mat& labellingStatistics)
{
    auto length = colorReference.size();
    int miscColorIndex = m_regionsCount + 1;

    std::vector<std::pair<int, int>> vecLabelAndColor;

    for (int i = 0; i < length; i++)
    {
        int reference = colorReference[i];

        if ((0 <= reference) && (reference != miscColorIndex))
        {
            vecLabelAndColor.push_back(std::make_pair(i, reference));
        }
    }

    sort(vecLabelAndColor.begin(), vecLabelAndColor.end(),
        [](const std::pair<int, int>& x, const std::pair<int, int>& y) { return x.second > y.second; });

    auto countOfAvailableColors = vecLabelAndColor.size();
    std::vector<cv::Rect> vecCoordinates(countOfAvailableColors);

    for (size_t i = 0; i < countOfAvailableColors; i++)
    {
        int label = vecLabelAndColor[i].first;
        int ref = vecLabelAndColor[i].second;

        int *param = labellingStatistics.ptr<int>(label);

        int top = param[cv::ConnectedComponentsTypes::CC_STAT_TOP];
        int left = param[cv::ConnectedComponentsTypes::CC_STAT_LEFT];
        int height = param[cv::ConnectedComponentsTypes::CC_STAT_HEIGHT];
        int width = param[cv::ConnectedComponentsTypes::CC_STAT_WIDTH];

        vecCoordinates[i] = cv::Rect(left, top, width, height);
    }

    coordinates = vecCoordinates;
}

void DividingOperation::CreateImageFile(std::string destinationPath, std::string sourcePath, cv::Mat& labelledMatrix, std::vector<int>& labelsColorReference)
{
    // Prepare colors for the operation. 
    std::vector<cv::Vec3b> vecSpecifiedColors;
    SetupPalette3b(vecSpecifiedColors);

    cv::Mat matDestination;
    if (m_duplicatesSource)
    {
        cv::Mat matSource = cv::imread(sourcePath, cv::IMREAD_UNCHANGED);
        auto numberOfChannels = matSource.channels();

        if (3 < numberOfChannels)
        {
            ConvertTransparentToWhite(matSource);
            cv::cvtColor(matSource, matDestination, cv::COLOR_RGBA2RGB);
        }
        else if (3 == numberOfChannels)
        {
            matDestination = matSource;
        }
        else if (1 == numberOfChannels)
        {
            cv::cvtColor(matSource, matDestination, cv::COLOR_GRAY2RGB);
        }
    }

    if (matDestination.empty())
    {
        matDestination = cv::Mat(labelledMatrix.size(), CV_8UC3, cv::Scalar(255, 255, 255));
    }

    // Draw result of the labelling.
    for (int y = 0; y < matDestination.rows; ++y)
    {
        for (int x = 0; x < matDestination.cols; ++x)
        {
            int componentsIndex = labelledMatrix.at<int>(y, x);
            int colorReference = labelsColorReference[componentsIndex];

            if (0 <= colorReference)
            {
                cv::Vec3b &pixel = matDestination.at<cv::Vec3b>(y, x);
                pixel = vecSpecifiedColors[colorReference];
            }
        }
    }

    // Output result as image file.
    cv::imwrite(destinationPath, matDestination);
}

void DividingOperation::CreateImageFileRGBA(std::string destinationPath, std::string sourcePath, cv::Mat& labelledMatrix, std::vector<int>& labelsColorReference)
{
    // Prepare colors for the operation. 
    std::vector<cv::Vec4b> vecSpecifiedColors;
    SetupPalette4b(vecSpecifiedColors);

    cv::Mat matDestination;
    ReadSourceImageWithAlphaChannel(matDestination, sourcePath);

    // Draw result of the labelling.
    for (int y = 0; y < matDestination.rows; ++y)
    {
        for (int x = 0; x < matDestination.cols; ++x)
        {
            int componentsIndex = labelledMatrix.at<int>(y, x);
            int colorReference = labelsColorReference[componentsIndex];

            if (0 <= colorReference)
            {
                cv::Vec4b &pixel = matDestination.at<cv::Vec4b>(y, x);
                pixel = vecSpecifiedColors[colorReference];
            }
        }
    }

    // Output result as image file.
    cv::imwrite(destinationPath, matDestination);
}

void DividingOperation::SetupTotalLayersCount(int countOfEnabledColors)
{
    int totalLayersCount = countOfEnabledColors;

    if (m_hasBackground)
    {
        totalLayersCount++;
    }

    m_totalLayersCount = totalLayersCount;
}

void DividingOperation::SetupEnabledLayerColors(std::vector<int>& colors, std::unordered_map<int, int>& layerOrder, std::vector<BOOL>& colorsActivation)
{
    std::vector<int> vecColors;

    int length = m_regionsCount + 2;
    vecColors.reserve(length);

    std::vector<int> palette;
    SetupPalette(palette);

    int layerOrderValue = 0;

    for (int i = 0; i < length; i++)
    {
        int index = (length - 1) - i;

        if (colorsActivation[index])
        {
            vecColors.push_back(palette[index]);
            layerOrder[index] = layerOrderValue;
            layerOrderValue++;
        }
    }

    colors = vecColors;
}

void DividingOperation::ComposeLayeredImageBase(cv::Mat& layeredImageBase, cv::Mat& labelledMatrix, std::vector<int>& labelsColorReference, std::unordered_map<int, int>& colorsLayerOrder)
{
    layeredImageBase = cv::Mat(labelledMatrix.size(), CV_8UC1, cv::Scalar(0));

    for (int y = 0; y < labelledMatrix.rows; ++y)
    {
        for (int x = 0; x < labelledMatrix.cols; ++x)
        {
            int componentsIndex = labelledMatrix.at<int>(y, x);
            int colorReference = labelsColorReference[componentsIndex];

            if (0 <= colorReference)
            {
                int layerOrder = colorsLayerOrder[colorReference];

                if (0 <= layerOrder)
                {
                    uchar &pixel = layeredImageBase.at<uchar>(y, x);

                    int ref = colorReference;
                    int order = layerOrder;
                    int grayTEST = 0xff - layerOrder;

                    pixel = 0xff - layerOrder;
                }
            }
        }
    }
}

void DividingOperation::SetupLayerNames(std::vector<std::string>& names, std::vector<BOOL>& colorsActivation)
{
    std::vector<std::string> vecLayerNames(m_totalLayersCount);

    int nameIndex = 0;

    if (m_hasBackground)
    {
        vecLayerNames[nameIndex] = "Background";

        nameIndex++;
    }

    int miscIndex = m_regionsCount + 1;
    int length = m_regionsCount + 2;

    for (int i = 0; i < length; i++)
    {
        int index = (length - 1) - i;

        if (colorsActivation[index])
        {
            if (index == miscIndex)
            {
                vecLayerNames[nameIndex] = "Misc.";
            }
            else if (index == 0)
            {
                vecLayerNames[nameIndex] = "Periphery";
            }
            else
            {
                char digitsBuf[4];
                sprintf_s(digitsBuf, "%03d", index);
                std::string digits(digitsBuf);
                std::string name = "Region_" + digits;

                vecLayerNames[nameIndex] = name;
            }

            nameIndex++;
        }
    }

    names = vecLayerNames;
}

void DividingOperation::CreateLayeredImageFile(BOOL isColorSpilledImage, BOOL isLayerdFormat, std::string destinationPath, std::string sourcePath, cv::Mat& labelledMatrix, cv::Mat& binaryImage, std::vector<int>& labelsColorReference, std::vector<cv::Rect>& availableCoordinates)
{
    std::vector<BOOL> vecColorsActivation;
    int countOfEnabledColors = 0;
    SetupColorsActivation(vecColorsActivation, countOfEnabledColors, labelsColorReference);

    // Setup the total number of layers.
    SetupTotalLayersCount(countOfEnabledColors);

    // Setup colors for layers.
    std::vector<int> vecEnabledLayerColors;
    std::unordered_map<int, int> mapColorsLayerOrder;
    SetupEnabledLayerColors(vecEnabledLayerColors, mapColorsLayerOrder, vecColorsActivation);

    // Visualize the layered image base.
    cv::Mat matLayeredImageBase;
    ComposeLayeredImageBase(matLayeredImageBase, labelledMatrix, labelsColorReference, mapColorsLayerOrder);

    // Open the target file.
    if (isLayerdFormat)
    {
        m_psdEncoder.OpenFile(destinationPath);
    }

    int imageHeight = matLayeredImageBase.rows;
    int imageWidth = matLayeredImageBase.cols;

    // Initialize PSD encoder with basic information of image.
    m_psdEncoder.Initialize(imageWidth, imageHeight, m_totalLayersCount);

     // Write before layers brief information.
    m_psdEncoder.WriteBeforeLayersBriefInformation();

    // Setup layer names.
    std::vector<std::string> names;
    SetupLayerNames(names, vecColorsActivation);
    m_psdEncoder.SetLayerNames(names);

    // Write layers brief information.
    m_psdEncoder.WriteLayersBriefInformation(m_blendMode, m_layersOpacity, m_hasBackground);

    // Mat for the composite image.
    cv::Mat matCompositeImage(labelledMatrix.size(), CV_8UC4, cv::Scalar(0, 0, 0, 0));

    // The current layer index.
    int layerIndex = 0;

    // Write background layer's channels data.
    WriteBackgroundLayerChannelsData(matCompositeImage, sourcePath, layerIndex);

    // Write color layers channels data.
    BOOL existsMiscLayer = vecColorsActivation[m_regionsCount + 1];
    if (isColorSpilledImage)
    {
        WriteColorSpilledLayerChannelsData(matLayeredImageBase, matCompositeImage, binaryImage, layerIndex, countOfEnabledColors, existsMiscLayer, availableCoordinates, vecEnabledLayerColors);
    }
    else
    {
        WriteColorSettledLayerChannelsData(matLayeredImageBase, matCompositeImage, layerIndex, countOfEnabledColors, existsMiscLayer, availableCoordinates, vecEnabledLayerColors);
    }

    // Write before the composite image data. 
    m_psdEncoder.WriteLayersInformationSize();
    m_psdEncoder.WriteEmptyMaskData();
    m_psdEncoder.WriteInformationSectionSize();

    // Write the composite image.
    m_psdEncoder.WriteCompositeImage(matCompositeImage);

    // Close the file.
    m_psdEncoder.CloseFile();

    // If the destination is not ".psd".
    if (!isLayerdFormat)
    {
        cv::imwrite(destinationPath, matCompositeImage);
    }
}

void DividingOperation::WriteBackgroundLayerChannelsData(cv::Mat& compositeImage, std::string sourcePath, int& layerIndex)
{
    int imageHeight = compositeImage.rows;
    int imageWidth = compositeImage.cols;

    // If Background is required.
    if (m_hasBackground)
    {
        // Write coordinates.
        m_psdEncoder.WriteLayerCoordinates(layerIndex, 0, 0, imageHeight, imageWidth);

        if (m_duplicatesSource)
        {
            // Read source image
            cv::Mat matBackground;

            matBackground = cv::imread(sourcePath, cv::IMREAD_UNCHANGED);
            auto numberOfChannels = matBackground.channels();

            // Write alpha channel.
            if (3 < numberOfChannels)
            {
                cv::Mat matAlphaChannel;
                cv::extractChannel(matBackground, matAlphaChannel, 3);
                m_psdEncoder.WriteAdditionalChannelOfLayer(matAlphaChannel);

                compositeImage = matBackground.clone();
            }
            else
            {
                m_psdEncoder.WriteAdditionalSingleValueChannel(0xff, imageHeight, imageWidth);

                cv::cvtColor(matBackground, compositeImage, cv::COLOR_RGB2RGBA);
            }

            cv::Mat matColorChannel;
            int channelIndex;
            for (int i = 0; i < 3; i++)
            {
                channelIndex = (3 == numberOfChannels) ? (2 - i) : 0;
                cv::extractChannel(matBackground, matColorChannel, channelIndex);

                // Write color channel.
                m_psdEncoder.WriteAdditionalChannelOfLayer(matColorChannel);
            }
        }
        else
        {
            // Set blank image
            compositeImage.setTo(cv::Scalar(0xff, 0xff, 0xff, 0xff));
            
            // Write alpha channel.
            m_psdEncoder.WriteAdditionalSingleValueChannel(0xff, imageHeight, imageWidth);

            for (int i = 0; i < 3; i++)
            {
                // Write color channel.
                m_psdEncoder.WriteAdditionalSingleValueChannel(0xff, imageHeight, imageWidth);
            }
        }

        layerIndex++;
    }
}

void DividingOperation::WriteColorSettledLayerChannelsData(cv::Mat& layeredImageBase, cv::Mat& compositeImage, int& layerIndex, int countOfEnabledColors, BOOL existsMiscLayer, std::vector<cv::Rect>& availableCoordinates, std::vector<int>& enabledColors)
{
    // Mat which truncated superior layers information
    cv::Mat matTruncated = layeredImageBase.clone();

    // Mat for buffer
    cv::Mat matBuffer(layeredImageBase.size(), CV_8UC1);

    for (int colorLayerOrder = 0; colorLayerOrder < countOfEnabledColors; colorLayerOrder++)
    {
        int targetValue = 0xff - colorLayerOrder;

        // Truncate superior layers information.
        cv::threshold(layeredImageBase, matTruncated, targetValue, 0xff, cv::THRESH_TOZERO_INV);

        // ROI
        cv::Rect roiRect;

        if (existsMiscLayer && (0 == colorLayerOrder))
        {
            cv::threshold(layeredImageBase, matBuffer, (targetValue - 1), 0xff, cv::THRESH_BINARY);
            roiRect = cv::boundingRect(matBuffer);
        }
        else
        {
            int coordinatesIndex = (existsMiscLayer) ? (colorLayerOrder - 1) : colorLayerOrder;

            roiRect = availableCoordinates[coordinatesIndex];
        }

        int roiHeight = roiRect.height;
        int roiWidth = roiRect.width;

        cv::Mat matRoiSource = matTruncated(roiRect);
        cv::Mat matRoiTarget = matBuffer(roiRect);

        // Assign 0xff to target pixels.
        cv::threshold(matRoiSource, matRoiTarget, (targetValue - 1), 0xff, cv::THRESH_BINARY);

        // Write coordinates.
        m_psdEncoder.WriteLayerCoordinates(layerIndex, roiRect.y, roiRect.x, roiHeight, roiWidth);

        // Write alpha channel.
        m_psdEncoder.WriteAdditionalChannelOfLayer(matRoiTarget);

        // Write color channels.
        auto color = GetOpencvColor4b(enabledColors[colorLayerOrder]);
        for (int i = 0; i < 3; i++)
        {
            // Write color channel.
            m_psdEncoder.WriteAdditionalSingleValueChannel(color[2 - i], roiHeight, roiWidth);
        }

        // Paint the composite image.
        PaintWithFloatingAlphaData(compositeImage, matRoiTarget, roiRect.x, roiRect.y, color);

        layerIndex++;
    }
}

void DividingOperation::WriteColorSpilledLayerChannelsData(cv::Mat& layeredImageBase, cv::Mat& compositeImage, cv::Mat& binaryImage, int& layerIndex, int countOfEnabledColors, BOOL existsMiscLayer, std::vector<cv::Rect>& availableCoordinates, std::vector<int>& enabledColors)
{
    int imageHeight = compositeImage.rows;
    int imageWidth = compositeImage.cols;

    // Get lines component.
    cv::Mat matLines;
    cv::bitwise_not(binaryImage, matLines);

    // Kernel for the morphology.
    int kernelLength = (0 == (m_maxThicknessOfLines % 2)) ? (m_maxThicknessOfLines + 1) : m_maxThicknessOfLines;
    auto matKernel = cv::getStructuringElement(cv::MORPH_ELLIPSE, cv::Size(kernelLength, kernelLength));
    int KernelLengthForOpening = m_maxThicknessOfLines / 2 + 1;
    auto matKernelForOpening = cv::getStructuringElement(cv::MORPH_ELLIPSE, cv::Size(KernelLengthForOpening, KernelLengthForOpening));

    // Mat which truncated superior layers information
    cv::Mat matTruncated = layeredImageBase.clone();

    // Mat for buffer
    cv::Mat matBufferForComponent(layeredImageBase.size(), CV_8UC1);
    cv::Mat matBuffer01(layeredImageBase.size(), CV_8UC1);
    cv::Mat matBuffer02(layeredImageBase.size(), CV_8UC1);

    for (int colorLayerOrder = 0; colorLayerOrder < countOfEnabledColors; colorLayerOrder++)
    {
        int targetValue = 0xff - colorLayerOrder;

        // Truncate superior layers information.
        cv::threshold(layeredImageBase, matTruncated, targetValue, 0xff, cv::THRESH_TOZERO_INV);

        cv::Rect baseRect;

        if (existsMiscLayer && (0 == colorLayerOrder))
        {
            cv::threshold(layeredImageBase, matBuffer01, (targetValue - 1), 0xff, cv::THRESH_BINARY);
            baseRect = cv::boundingRect(matBuffer01);
        }
        else
        {
            int coordinatesIndex = (existsMiscLayer) ? (colorLayerOrder - 1) : colorLayerOrder;

            baseRect = availableCoordinates[coordinatesIndex];
        }

        // Make ROI rectangles.
        cv::Rect roiRect = GetExpandedRectangle(baseRect, m_maxThicknessOfLines, imageWidth, imageHeight);
        cv::Rect expandedRect = GetExpandedRectangle(baseRect, (m_maxThicknessOfLines * 2), imageWidth, imageHeight);

        // Prepare morphology work area.
        cv::Mat matWorkAreaOfComponent = matBufferForComponent(expandedRect);
        cv::Mat matWorkAreaOfBuffer01 = matBuffer01(expandedRect);
        cv::Mat matWorkAreaOfBuffer02 = matBuffer02(expandedRect);
        matWorkAreaOfComponent.setTo(0);
        matWorkAreaOfBuffer01.setTo(0);
        matWorkAreaOfBuffer02.setTo(0);

        // Prepare source image of the target component.
        cv::Mat matSource = matTruncated(baseRect);

        // Assign 0xff to pixels of the target component.
        cv::Mat matTargetComponent = matBufferForComponent(baseRect);
        cv::threshold(matSource, matTargetComponent, (targetValue - 1), 0xff, cv::THRESH_BINARY);

        // Dilate pixels of the target component.
        cv::dilate(matWorkAreaOfComponent, matWorkAreaOfBuffer01, matKernel);

        // Prepare image of the lines.
        cv::Mat matLinesToConsider = matLines(expandedRect);

        // Get intersection of dilated component and the lines.
        cv::bitwise_and(matWorkAreaOfBuffer01, matLinesToConsider, matWorkAreaOfBuffer02);

        // That intersection + target component
        cv::bitwise_or(matWorkAreaOfBuffer02, matWorkAreaOfComponent, matWorkAreaOfBuffer01);

        // Result of layer image.
        cv::Mat matRoiResult = matBuffer01(roiRect);

        // If opening is enabled..
        if (m_isOpeningEnabled)
        {
            cv::morphologyEx(matWorkAreaOfBuffer01, matWorkAreaOfBuffer02, cv::MORPH_OPEN, matKernelForOpening);
            matRoiResult = matBuffer02(roiRect);
        }

        // Write coordinates.
        m_psdEncoder.WriteLayerCoordinates(layerIndex, roiRect.y, roiRect.x, roiRect.height, roiRect.width);

        // Write alpha channel.
        m_psdEncoder.WriteAdditionalChannelOfLayer(matRoiResult);

        // Write color channels.
        auto color = GetOpencvColor4b(enabledColors[colorLayerOrder]);
        for (int i = 0; i < 3; i++)
        {
            // Write color channel.
            m_psdEncoder.WriteAdditionalSingleValueChannel(color[2 - i], roiRect.height, roiRect.width);
        }

        // Paint the composite image.
        PaintWithFloatingAlphaData(compositeImage, matRoiResult, roiRect.x, roiRect.y, color);

        layerIndex++;
    }
}

void DividingOperation::SetupComponentsOrder(std::vector<int>& order, int labelsCount, cv::Mat& labellingStatistics)
{
    // label indexes and area informations (The lines pixels is excluded.)
    std::vector<std::pair<int, int>> vecIndexAndArea(labelsCount - 1);

    // Get area informations. (The lines pixels is excluded.)
    for (int i = 1; i < labelsCount; ++i) {
        int *param = labellingStatistics.ptr<int>(i);
        vecIndexAndArea[(i - 1)] = (std::make_pair(i, param[cv::ConnectedComponentsTypes::CC_STAT_AREA]));
    }

    if (m_hasOrderOfArea)
    {
        if (m_isDescendingAreaOrder)
        {
            // Sort in descending order of area.
            sort(vecIndexAndArea.begin(), vecIndexAndArea.end(),
                [](const std::pair<int, int>& x, const std::pair<int, int>& y) { return x.second > y.second; });
        }
        else
        {
            // Sort in ascending order of area.
            sort(vecIndexAndArea.begin(), vecIndexAndArea.end(),
                [](const std::pair<int, int>& x, const std::pair<int, int>& y) { return x.second < y.second; });
        }
    }

    std::vector<int> vecComponentsAreaOrder(labelsCount);
    vecComponentsAreaOrder[0] = -1;
    for (int i = 0; i < (labelsCount - 1); i++)
    {
        int componentsIndex = vecIndexAndArea[i].first;
        vecComponentsAreaOrder[componentsIndex] = i;
    }

    order = vecComponentsAreaOrder;
}

//================================//
//==    Utility methods         ==//
//================================//

void DividingOperation::ConvertTransparentToWhite(cv::Mat& matWithAlpha)
{
    cv::Mat matAlphaChannel;
    cv::extractChannel(matWithAlpha, matAlphaChannel, 3);

    auto white = cv::Vec4b(255, 255, 255, 255);

    for (int y = 0; y < matAlphaChannel.rows; ++y)
    {
        for (int x = 0; x < matAlphaChannel.cols; ++x)
        {
            uchar alphaValue = matAlphaChannel.at<uchar>(y, x);

            if (0 == alphaValue)
            {
                cv::Vec4b &pixel = matWithAlpha.at<cv::Vec4b>(y, x);
                pixel = white;
            }
        }
    }
}

const BOOL DividingOperation::CreateGrayscaleSource(std::string sourcePath, cv::Mat& matGrayscale)
{
    BOOL bResult = FALSE;

    // Read source image
    cv::Mat matSource;

    matSource = cv::imread(sourcePath, cv::IMREAD_UNCHANGED);

    if (!(matSource.empty()))
    {
        bResult = TRUE;

        auto numberOfChannels = matSource.channels();

        if (1 < numberOfChannels)
        {
            if (3 < numberOfChannels)
            {
                ConvertTransparentToWhite(matSource);
            }

            cv::cvtColor(matSource, matGrayscale, cv::COLOR_BGR2GRAY);
        }
        else
        {
            matGrayscale = matSource;
        }
    }

    return bResult;
}

const cv::Vec3b DividingOperation::GetOpencvColor3b(int rgbColor)
{
    int r = ((rgbColor & 0x00FF0000) >> 16);
    int g = ((rgbColor & 0x0000FF00) >> 8);
    int b = (rgbColor & 0x000000FF);

    return cv::Vec3b(b, g, r);
}

const cv::Vec4b DividingOperation::GetOpencvColor4b(int rgbColor)
{
    int r = ((rgbColor & 0x00FF0000) >> 16);
    int g = ((rgbColor & 0x0000FF00) >> 8);
    int b = (rgbColor & 0x000000FF);

    return cv::Vec4b(b, g, r, 255);
}

const cv::Rect DividingOperation::GetExpandedRectangle(cv::Rect& baseRect, int expansion, int imageWidth, int imageHeight)
{
    int left = std::max(0, (baseRect.x - expansion));
    int top = std::max(0, (baseRect.y - expansion));
    int right = std::min(imageWidth, (baseRect.x + baseRect.width + expansion));
    int bottom = std::min(imageHeight, (baseRect.y + baseRect.height + expansion));

    return cv::Rect(left, top, (right - left), (bottom - top));
}

void DividingOperation::ReadSourceImageWithAlphaChannel(cv::Mat& destination, std::string sourcePath)
{
    // Read source image
    cv::Mat matSource;
    matSource = cv::imread(sourcePath, cv::IMREAD_UNCHANGED);
    auto numberOfChannels = matSource.channels();

    // Write alpha channel.
    if (4 == numberOfChannels)
    {
        destination = matSource.clone();
    }
    else
    {
        cv::cvtColor(matSource, destination, cv::COLOR_RGB2RGBA);
    }
}

void DividingOperation::PaintWithFloatingAlphaData(cv::Mat& destination, cv::Mat& alphaData, int leftOfAlpha, int topOfAlpha, cv::Vec4b color)
{
    for (int y = 0; y < alphaData.rows; ++y)
    {
        for (int x = 0; x < alphaData.cols; ++x)
        {
            uchar alphaValue = alphaData.at<uchar>(y, x);

            if (0 < alphaValue)
            {
                cv::Vec4b &pixel = destination.at<cv::Vec4b>((y + topOfAlpha), (x + leftOfAlpha));
                pixel = color;
            }
        }
    }
}

//==============================================//
//==    Method of the dividing operation      ==//
//==============================================//

BOOL DividingOperation::ExecuteDividing(std::string destinationPath, std::string sourcePath, BOOL isLayeredStyle, BOOL hasAlpha)
{
    BOOL bResult = TRUE;

    // Load image as gray scale.
    cv::Mat matGrayscaleSource;
    if (!CreateGrayscaleSource(sourcePath, matGrayscaleSource))
    {
        return FALSE;
    }

    //------------------------------------------------//
    // Check export image style.                      //
    //------------------------------------------------//
    BOOL isOverwhelming = getInstance().m_isOverwhelming;

    //------------------------------------------------//
    // Create labeled matrix.                         //
    //------------------------------------------------//
    cv::Mat matLabeledMatrix(matGrayscaleSource.size(), CV_32S);
    cv::Mat matBinary;
    cv::Mat matComponentsInfo;
    int labelsCount;
    BOOL hasNoComponents, hasNoLines;
    getInstance().FillMatrixWithLabels(matGrayscaleSource, matBinary, matLabeledMatrix, matComponentsInfo, labelsCount, hasNoComponents, hasNoLines);

    //------------------------------------------------//
    // Get information of components order.     //
    //------------------------------------------------//

    std::vector<int> vecComponentsOrder;
    getInstance().SetupComponentsOrder(vecComponentsOrder, labelsCount, matComponentsInfo);

    //------------------------------------------------//
    // Get information of "Periphery".                //
    //------------------------------------------------//
    int labelOfPeriphery;
    auto isPeripheryValid = getInstance().GetLabelOfPeriphery(matLabeledMatrix, labelOfPeriphery);

    //------------------------------------------------//
    // Get color reference of each label.             //
    //------------------------------------------------//
    std::vector<int> vecColorReference;

    if (hasNoComponents)
    {
        vecColorReference.push_back(-1);
    }
    else if (hasNoLines)
    {
        vecColorReference.push_back(-1);

        int paletteIndex = 0;
        if (!(getInstance().m_hasPeriphery))
        {
            paletteIndex = 1;
            if (!(getInstance().m_hasRegions))
            {
                paletteIndex = getInstance().m_regionsCount + 1;
                if (!(getInstance().m_hasMisc))
                {
                    paletteIndex = -1;
                }
            }
        }
        vecColorReference.push_back(paletteIndex);
    }
    else
    {
        getInstance().SetupLabelsColorReference(vecColorReference, vecComponentsOrder, isPeripheryValid, labelOfPeriphery);
    }

    //------------------------------------------------//
    // Store applicable coordinates of regions.       //
    //------------------------------------------------//
    std::vector<cv::Rect> vecApplicableCoordinates;
    if (hasNoComponents || hasNoLines)
    {
        vecApplicableCoordinates.push_back(cv::Rect(0, 0, matLabeledMatrix.cols, matLabeledMatrix.rows));
    }
    else
    {
        getInstance().StoreApplicableCoordinates(vecApplicableCoordinates, vecColorReference, matComponentsInfo);
    }

    //------------------------------------------------//
    // Consider appropriate encoding method.          //
    //------------------------------------------------//
    BOOL hasBackground = getInstance().m_hasBackground;
    BOOL hasBlankBackground = !(getInstance().m_duplicatesSource) && hasBackground;
    BOOL requiresRGBA = ((!hasBlankBackground) && hasAlpha);
    BOOL isColorsSpilledImage = (isOverwhelming && (!(hasNoComponents || hasNoLines)));
    BOOL excludesPeriphery = getInstance().m_excludesPeriphery;

    //------------------------------------------------//
    // Create image file.                             //
    //------------------------------------------------//
    if (isLayeredStyle || isColorsSpilledImage || (excludesPeriphery && (!hasBackground)))
    {
        getInstance().CreateLayeredImageFile(isColorsSpilledImage, isLayeredStyle, destinationPath, sourcePath, matLabeledMatrix, matBinary, vecColorReference, vecApplicableCoordinates);
    }
    else
    {
        if (requiresRGBA)
        {
            getInstance().CreateImageFileRGBA(destinationPath, sourcePath, matLabeledMatrix, vecColorReference);
        }
        else
        {
            getInstance().CreateImageFile(destinationPath, sourcePath, matLabeledMatrix, vecColorReference);
        }
    }

    return bResult;
}
