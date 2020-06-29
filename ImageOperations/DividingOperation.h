/**
divire

Copyright (C) 2020 Aru Nanika

This program is released under the MIT License.
https://opensource.org/licenses/MIT
*/

#pragma once
#include <opencv2/core.hpp>
#include "RestrictedPsdEncoder.h"

//----------------------------------------------------------------
// Class which provides graphic operation of this program.
//----------------------------------------------------------------
class DividingOperation
{
//--------------------------------
// Definitions (Private)
//--------------------------------
private:
    static const int MAX_REGIONS = 256;

//--------------------------------
// Fields
//--------------------------------
private:
    BOOL m_hasPeriphery;
    BOOL m_hasRegions;
    BOOL m_hasMisc;

    int m_regionColors[MAX_REGIONS];
    int m_regionsCount;
    int m_peripheryColor;
    int m_miscColor;

    BOOL m_hasOrderOfArea;
    BOOL m_isDescendingAreaOrder;
    BOOL m_hasBackground;
    BOOL m_duplicatesSource;
    std::string m_blendMode;
    uchar m_layersOpacity;

    int m_threshold;
    BOOL m_refersToRowEnd;
    BOOL m_refersToColumnEnd;

    BOOL m_excludesPeriphery;

    BOOL m_isOverwhelming;
    int m_maxThicknessOfLines;
    BOOL m_isOpeningEnabled;

    RestrictedPsdEncoder m_psdEncoder;

    int m_totalLayersCount;

//--------------------------------
// Constructor & Destructor (Private)
//--------------------------------
private:
    DividingOperation();
    ~DividingOperation();

// Prohibit copy constrctor and assignment operator.
public:
    DividingOperation(const DividingOperation&) = delete;
    DividingOperation(DividingOperation&&) = delete;
    DividingOperation& operator=(const DividingOperation&) = delete;
    DividingOperation& operator=(DividingOperation&&) = delete;

// Get singleton instance. (Private)
private:
    static DividingOperation& getInstance()
    {
        static DividingOperation instance;
        return instance;
    }

//--------------------------------
// Methods (Instance)
//--------------------------------
private:
    void SetupPalette(std::vector<int>& palette);
    void SetupPalette3b(std::vector<cv::Vec3b>& palette);
    void SetupPalette4b(std::vector<cv::Vec4b>& palette);

    void FillMatrixWithLabels(cv::Mat& grayImage, cv::Mat& binaryImage, cv::Mat& labelledMatrix, cv::Mat& labellingInfo, int& labelsCount, BOOL& hasNoComponents, BOOL& hasNoLines);

    BOOL GetLabelOfPeriphery(cv::Mat& labelledMatrix, int& labelOfPeriphery);

    void SetupComponentsOrder(std::vector<int>& order, int labelsCount, cv::Mat& labellingStatistics);

    void SetupLabelsColorReference(std::vector<int>& reference, std::vector<int>& componentsAreaOrder, BOOL isPeripheryValid, int labelOfPeriphery);
    void SetupColorsActivation(std::vector<BOOL>& colorsActivation, int& countOfEnabledColors, std::vector<int>& labelsColorReference);
    void StoreApplicableCoordinates(std::vector<cv::Rect>& coordinates, std::vector<int>& colorReference, cv::Mat& labellingStatistics);

    void CreateImageFile(std::string destinationPath, std::string sourcePath, cv::Mat& labelledMatrix, std::vector<int>& labelsColorReference);
    void CreateImageFileRGBA(std::string destinationPath, std::string sourcePath, cv::Mat& labelledMatrix, std::vector<int>& labelsColorReference);

    void SetupTotalLayersCount(int countOfEnabledColors);
    void SetupEnabledLayerColors(std::vector<int>& colors, std::unordered_map<int, int>& layerOrder, std::vector<BOOL>& colorsActivation);
    void ComposeLayeredImageBase(cv::Mat& layeredImageBase, cv::Mat& labelledMatrix, std::vector<int>& labelsColorReference, std::unordered_map<int, int>& colorsLayerOrder);
    void SetupLayerNames(std::vector<std::string>& names, std::vector<BOOL>& colorsActivation);

    void CreateLayeredImageFile(BOOL isColorSpilledImage, BOOL isLayerdFormat, std::string destinationPath, std::string sourcePath, cv::Mat& labelledMatrix, cv::Mat& binaryImage, std::vector<int>& labelsColorReference, std::vector<cv::Rect>& availableCoordinates);

    void WriteBackgroundLayerChannelsData(cv::Mat& compositeImage, std::string sourcePath, int& layerIndex);
    void WriteColorSettledLayerChannelsData(cv::Mat& layeredImageBase, cv::Mat& compositeImage, int& layerIndex, int countOfEnabledColors, BOOL existsMiscLayer, std::vector<cv::Rect>& availableCoordinates, std::vector<int>& enabledColors);
    void WriteColorSpilledLayerChannelsData(cv::Mat& layeredImageBase, cv::Mat& compositeImage, cv::Mat& binaryImage, int& layerIndex, int countOfEnabledColors, BOOL existsMiscLayer, std::vector<cv::Rect>& availableCoordinates, std::vector<int>& enabledColors);

//--------------------------------
// Methods (private and static)
//--------------------------------
private:
    static void ConvertTransparentToWhite(cv::Mat& matWithAlpha);
    static const BOOL CreateGrayscaleSource(std::string sourcePath, cv::Mat& matGrayscale);
    static const cv::Vec3b GetOpencvColor3b(int rgbColor);
    static const cv::Vec4b GetOpencvColor4b(int rgbColor);
    static const cv::Rect GetExpandedRectangle(cv::Rect& baseRect, int expansion, int imageWidth, int imageHeight);
    static void ReadSourceImageWithAlphaChannel(cv::Mat& destination, std::string sourcePath);
    static void PaintWithFloatingAlphaData(cv::Mat& destination, cv::Mat& alphaData, int leftOfAlpha, int topOfAlpha, cv::Vec4b color);

//--------------------------------
// Methods (Public)
//--------------------------------
public:
    static void SetDivisionScheme(BOOL hasPeriphery, BOOL hasRegions, BOOL hasMisc);
    static void SetColorsForDivision(int* regionColors, int regionsCount, int peripheryColor, int miscColor);
    static void SetOrderOfRegions(BOOL hasOrderOfArea, BOOL isDescendingAreaOrder);
    static void SetBackgroundStyle(BOOL hasBackground, BOOL duplicatesSource);
    static void SetLayerOptions(std::string blendMode, uchar opacity);
    static void SetThresholdForDivision(int threshold);
    static void SetReferenceOfPeriphery(BOOL refersToRowEnd, BOOL refersToColumnEnd);
    static void SetPeripheryManagement(BOOL excludesPeriphery);
    static void SetColorOverwhelming(BOOL isOverwhelming, int maxThickness, BOOL isOpeningEnabled);

    static BOOL ExecuteDividing(std::string destinationPath, std::string sourcePath, BOOL isLayeredStyle, BOOL hasAlpha);

};
