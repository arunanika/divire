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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using divire.Models;
using divire.Protocols;

namespace divire.ViewModels
{
    /// <summary>
    /// Main Window View Model
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        //================================//
        //==    Static Resources        ==//
        //================================//

        private static readonly byte MinimumCountOfRegions = 2;
        private static readonly byte MaximumCountOfRegions = 252;

        //================================//
        //==    Fields                  ==//
        //================================//

        private FrontendConfiguration frontendConfiguration;

        private DelegateCommand commandUpdateMiddleColors;
        private DelegateCommand commandExport;

        private string permittedPathExpression;
        private string imageFileFilter;
        private string suffix;
        private OperationProtocol.ExportDestination exportDestination;
        private string specifiedDestination;
        private OperationProtocol.ExportStyle exportStyle;
        private bool isAutoRenameEnabled;

        private bool hasLayerOfPeriphery;
        private bool hasLayersOfRegions;
        private bool hasLayerOfMiscellaneous;
        private bool hasLayerOfBackground;
        private byte maxOfRegionLayers;
        private byte minOfRegionLayers;
        private byte countOfRegionLayers;
        private OperationProtocol.BackgroundStyle backgroundStyle;
        private int colorValueOfPeriphery;
        private int colorValueOfRegionFirst;
        private int colorValueOfRegionLast;
        private int colorValueOfMiscellaneous;
        private OperationProtocol.ColorCalculation colorCalculation;
        private OperationProtocol.OrderOfRegions orderOfRegions;
        private byte opacityOfLayers;
        private OperationProtocol.BlendMode blendMode;

        private byte thresholdOfLines;
        private OperationProtocol.CornerPoint referenceCorner;
        private bool excludesPeriphery;
        private bool laysColorOverLines;
        private int maxThicknessOfLines;
        private bool isOpeningEnabled;

        private double windowTop;
        private double windowLeft;
        private double windowHeight;
        private double windowWidth;

        private string sourceFilePath;
        private string exportFileName;
        private string exportFilePath;
        private string droppedItem;
        private bool canExport;
        private IEnumerable<int> paddingColors;
        private IEnumerable<object> middleColors;
        private int thresholdSampleColor;

        private bool isBusy;

        private List<int> allColorsOfRegions;

        //================================//
        //==    Constructor             ==//
        //================================//

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindowViewModel()
        {
            // Load property values from configuration file.
            frontendConfiguration = new FrontendConfiguration();
            LoadValuesFromConfiguration();

            // Initialize with fixed values.
            permittedPathExpression = @"^[!#\$%&'\(\)\+\-=@\[\]\^_`\{\}~0-9A-Za-z]+$";
            minOfRegionLayers = MinimumCountOfRegions;

            // Initialize properties for indicators.
            if(null== paddingColors)
            {
                UpdateMiddleColors();
            }
            else
            {
                if ((countOfRegionLayers - 2) > paddingColors.Count())
                {
                    UpdateMiddleColors();
                }
                else
                {
                    MiddleColors = PaddingColors.Cast<object>();
                }
            }
            thresholdSampleColor = GetGrayValueColor(thresholdOfLines);

            // Initialize the application status
            sourceFilePath = "No file selected";
            exportFileName = string.Empty;
            exportFilePath = string.Empty;
            canExport = false;
            isBusy = false;
            allColorsOfRegions = new List<int>();
        }

        //================================//
        //==    Finalizer               ==//
        //================================//

        /// <summary>
        /// Finalizer
        /// </summary>
        ~MainWindowViewModel()
        {
            SaveConfiguration();
        }

        //================================//
        //==    Properties (Commands)   ==//
        //================================//

        /// <summary>
        /// Command updates middle colors.
        /// </summary>
        public DelegateCommand CommandUpdateMiddleColors
        {
            get
            {
                if (null == commandUpdateMiddleColors)
                {
                    commandUpdateMiddleColors = new DelegateCommand(UpdateMiddleColors, (() => { return true; }));
                }

                return commandUpdateMiddleColors;
            }
        }

        /// <summary>
        /// Command export processed file.
        /// </summary>
        public DelegateCommand CommandExport
        {
            get
            {
                if (null == commandExport)
                {
                    commandExport = new DelegateCommand(Export, (() => { return CanExport; }));
                }

                return commandExport;
            }
        }

        //============================================//
        //==    Properties (File operations)        ==//
        //============================================//

        public string PermittedPathExpression
        {
            get { return permittedPathExpression; }
            set
            {
                if (value != permittedPathExpression)
                {
                    permittedPathExpression = value;
                    OnPropertyChanged();

                    UpdateExportFileName();
                }
            }
        }

        public string ImageFileFilter
        {
            get { return imageFileFilter; }
            set
            {
                if (value != imageFileFilter)
                {
                    imageFileFilter = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Suffix
        {
            get { return suffix; }
            set
            {
                if (value != suffix)
                {
                    suffix = value;
                    OnPropertyChanged();

                    UpdateExportFileName();
                }
            }
        }

        /// <summary>
        /// The selection of export destination
        /// </summary>
        public OperationProtocol.ExportDestination ExportDestination
        {
            get { return exportDestination; }
            set
            {
                if (value != exportDestination)
                {
                    exportDestination = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SpecifiedDestination
        {
            get { return specifiedDestination; }
            set
            {
                if (value != specifiedDestination)
                {
                    specifiedDestination = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The style of image export.
        /// </summary>
        public OperationProtocol.ExportStyle ExportStyle
        {
            get { return exportStyle; }
            set
            {
                if (value != exportStyle)
                {
                    exportStyle = value;
                    OnPropertyChanged();

                    UpdateExportFileName();
                }
            }
        }

        public bool IsAutoRenameEnabled
        {
            get { return isAutoRenameEnabled; }
            set
            {
                if (value != isAutoRenameEnabled)
                {
                    isAutoRenameEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        //============================================//
        //==    Properties (Layers construction)   ==//
        //============================================//

        /// <summary>
        /// Availability of periphery layer.
        /// </summary>
        public bool HasLayerOfPeriphery
        {
            get { return hasLayerOfPeriphery; }
            set
            {
                if (value != hasLayerOfPeriphery)
                {
                    hasLayerOfPeriphery = value;
                    OnPropertyChanged();

                    UpdateExportStatus();
                }
            }
        }

        /// <summary>
        /// Availability of region layers.
        /// </summary>
        public bool HasLayersOfRegions
        {
            get { return hasLayersOfRegions; }
            set
            {
                if (value != hasLayersOfRegions)
                {
                    hasLayersOfRegions = value;
                    OnPropertyChanged();

                    UpdateExportStatus();
                }
            }
        }

        /// <summary>
        /// Availability of miscellaneous layer.
        /// </summary>
        public bool HasLayerOfMiscellaneous
        {
            get { return hasLayerOfMiscellaneous; }
            set
            {
                if (value != hasLayerOfMiscellaneous)
                {
                    hasLayerOfMiscellaneous = value;
                    OnPropertyChanged();

                    UpdateExportStatus();
                }
            }
        }

        /// <summary>
        /// Availability of background layers.
        /// </summary>
        public bool HasLayerOfBackground
        {
            get { return hasLayerOfBackground; }
            set
            {
                if (value != hasLayerOfBackground)
                {
                    hasLayerOfBackground = value;
                    OnPropertyChanged();

                    UpdateExportStatus();
                }
            }
        }

        public byte MaxOfRegionLayers
        {
            get { return maxOfRegionLayers; }
            set
            {
                if (value != maxOfRegionLayers)
                {
                    maxOfRegionLayers = value;
                    OnPropertyChanged();
                }
            }
        }

        public byte MinOfRegionLayers
        {
            get { return minOfRegionLayers; }
            set
            {
                if (value != minOfRegionLayers)
                {
                    minOfRegionLayers = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The count of region layers.
        /// </summary>
        public byte CountOfRegionLayers
        {
            get { return countOfRegionLayers; }
            set
            {
                if (value != countOfRegionLayers)
                {
                    countOfRegionLayers = value;
                    OnPropertyChanged();

                    UpdateMiddleColors();
                }
            }
        }

        /// <summary>
        /// The style of the background layer.
        /// </summary>
        public OperationProtocol.BackgroundStyle BackgroundStyle
        {
            get { return backgroundStyle; }
            set
            {
                if (value != backgroundStyle)
                {
                    backgroundStyle = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Color value of periphery layer.
        /// </summary>
        public int ColorValueOfPeriphery
        {
            get { return colorValueOfPeriphery; }
            set
            {
                if (value != colorValueOfPeriphery)
                {
                    colorValueOfPeriphery = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Color value of first region layer.
        /// </summary>
        public int ColorValueOfRegionFirst
        {
            get { return colorValueOfRegionFirst; }
            set
            {
                if (value != colorValueOfRegionFirst)
                {
                    colorValueOfRegionFirst = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Color value of last region layer.
        /// </summary>
        public int ColorValueOfRegionLast
        {
            get { return colorValueOfRegionLast; }
            set
            {
                if (value != colorValueOfRegionLast)
                {
                    colorValueOfRegionLast = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Color valuey of last miscellaneous layer.
        /// </summary>
        public int ColorValueOfMiscellaneous
        {
            get { return colorValueOfMiscellaneous; }
            set
            {
                if (value != colorValueOfMiscellaneous)
                {
                    colorValueOfMiscellaneous = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Calculation method of padding colors.
        /// </summary>
        public OperationProtocol.ColorCalculation ColorCalculation
        {
            get { return colorCalculation; }
            set
            {
                if (value != colorCalculation)
                {
                    colorCalculation = value;
                    OnPropertyChanged();

                    UpdateMiddleColors();
                }
            }
        }

        public OperationProtocol.OrderOfRegions OrderOfRegions
        {
            get { return orderOfRegions; }
            set
            {
                if (value != orderOfRegions)
                {
                    orderOfRegions = value;
                    OnPropertyChanged();

                    UpdateMiddleColors();
                }
            }
        }

        public byte OpacityOfLayers
        {
            get { return opacityOfLayers; }
            set
            {
                if (value != opacityOfLayers)
                {
                    opacityOfLayers = value;
                    OnPropertyChanged();

                    UpdateMiddleColors();
                }
            }
        }

        public OperationProtocol.BlendMode BlendMode
        {
            get { return blendMode; }
            set
            {
                if (value != blendMode)
                {
                    blendMode = value;
                    OnPropertyChanged();

                    UpdateMiddleColors();
                }
            }
        }

        //============================================//
        //==    Properties (Image processing)       ==//
        //============================================//

        /// <summary>
        /// Threshold gray value of lines.
        /// </summary>
        public byte ThresholdOfLines
        {
            get { return thresholdOfLines; }
            set
            {
                if (value != thresholdOfLines)
                {
                    thresholdOfLines = value;
                    OnPropertyChanged();

                    ThresholdSampleColor = GetGrayValueColor(thresholdOfLines);
                }
            }
        }

        /// <summary>
        /// Reference corner point of periphery.
        /// </summary>
        public OperationProtocol.CornerPoint ReferenceCorner
        {
            get { return referenceCorner; }
            set
            {
                if (value != referenceCorner)
                {
                    referenceCorner = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ExcludesPeriphery
        {
            get { return excludesPeriphery; }
            set
            {
                if (value != excludesPeriphery)
                {
                    excludesPeriphery = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool LaysColorOverLines
        {
            get { return laysColorOverLines; }
            set
            {
                if (value != laysColorOverLines)
                {
                    laysColorOverLines = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MaxThicknessOfLines
        {
            get { return maxThicknessOfLines; }
            set
            {
                if (value != maxThicknessOfLines)
                {
                    maxThicknessOfLines = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsOpeningEnabled
        {
            get { return isOpeningEnabled; }
            set
            {
                if (value != isOpeningEnabled)
                {
                    isOpeningEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        //============================================//
        //==    Properties (Extra information)      ==//
        //============================================//

        public double WindowTop
        {
            get { return windowTop; }
            set
            {
                if (value != windowTop)
                {
                    windowTop = value;
                    OnPropertyChanged();
                }
            }
        }

        public double WindowLeft
        {
            get { return windowLeft; }
            set
            {
                if (value != windowLeft)
                {
                    windowLeft = value;
                    OnPropertyChanged();
                }
            }
        }

        public double WindowHeight
        {
            get { return windowHeight; }
            set
            {
                if (value != windowHeight)
                {
                    windowHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        public double WindowWidth
        {
            get { return windowWidth; }
            set
            {
                if (value != windowWidth)
                {
                    windowWidth = value;
                    OnPropertyChanged();
                }
            }
        }

        public double LastWindowTop { get; set; }

        public double LastWindowLeft { get; set; }

        public double LastWindowHeight { get; set; }

        public double LastWindowWidth { get; set; }

        //============================================//
        //==    Properties (Application state)      ==//
        //============================================//

        public string SourceFilePath
        {
            get { return sourceFilePath; }
            set
            {
                if (value != sourceFilePath)
                {
                    sourceFilePath = value;
                    OnPropertyChanged();

                    UpdateExportFileName();
                    UpdateExportStatus();
                }
            }
        }

        public string ExportFileName
        {
            get { return exportFileName; }
            set
            {
                if (value != exportFileName)
                {
                    exportFileName = value;
                    OnPropertyChanged();

                    UpdateExportFileName();
                }
            }
        }

        public string DroppedItem
        {
            get { return droppedItem; }
            set
            {
                if (value != droppedItem)
                {
                    droppedItem = value;
                    OnPropertyChanged();

                    ValidateDroppedItem();
                }
            }
        }

        public bool CanExport
        {
            get { return canExport; }
            set
            {
                if (value != canExport)
                {
                    canExport = value;
                    OnPropertyChanged();
                }
            }
        }

        private IEnumerable<int> PaddingColors
        {
            get { return paddingColors; }
            set
            {
                paddingColors = value;
                MiddleColors = paddingColors.Cast<object>();
            }
        }

        /// <summary>
        /// The middle colors between first region color and last.
        /// </summary>
        public IEnumerable<object> MiddleColors
        {
            get { return middleColors; }
            set
            {
                middleColors = value;
                OnPropertyChanged();
            }
        }

        public int ThresholdSampleColor
        {
            get { return thresholdSampleColor; }
            set
            {
                if (value != thresholdSampleColor)
                {
                    thresholdSampleColor = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (value != isBusy)
                {
                    isBusy = value;
                    OnPropertyChanged();
                }
            }
        }

        //================================//
        //==    Methods (Static)        ==//
        //================================//
        
        private static string GetImageFilesFilter(List<string> extList)
        {
            if (0 == extList.Count)
            {
                extList.AddRange(new List<string> { ".jpg", ".png", ".tif", });
            }

            var element1 = string.Empty;
            var element2 = string.Empty;

            foreach (var item in extList)
            {
                element1 += $"*{item}, ";
                element2 += $"*{item};";
            }

            var length1 = element1.Length;
            var length2 = element2.Length;

            element1 = element1.Substring(0, (length1 - 2));
            element2 = element2.Substring(0, (length2 - 1));

            var filter = $"Image Files ({element1})|{element2}";

            return filter;
        }

        private static int GetGrayValueColor(byte GrayValue)
        {
            int value = GrayValue;

            return value | (value << 8) | (value << 16);
        }

        //================================//
        //==    Methods (Instance)      ==//
        //================================//

        /// <summary>
        /// Load property values from configuration file.
        /// </summary>
        private void LoadValuesFromConfiguration()
        {
            if (null == frontendConfiguration)
            {
                return;
            }

            // Load from the XML.
            frontendConfiguration.Load();

            // The file operations scheme
            var extList = frontendConfiguration.FileScheme.SourceExtentionList;
            imageFileFilter = GetImageFilesFilter(extList);
            suffix = frontendConfiguration.FileScheme.Suffix;
            exportDestination = frontendConfiguration.FileScheme.ExportDestination;
            var destination = frontendConfiguration.FileScheme.ExportDirectoryPath;
            specifiedDestination = (string.Empty != destination) ? destination : Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            exportStyle = frontendConfiguration.FileScheme.ExportStyle;
            isAutoRenameEnabled = frontendConfiguration.FileScheme.IsAutoRenameEnabled;

            // The layers construction scheme
            hasLayerOfPeriphery = frontendConfiguration.ConstructionScheme.HasLayerOfPeriphery;
            hasLayersOfRegions = frontendConfiguration.ConstructionScheme.HasLayersOfRegions;
            hasLayerOfMiscellaneous = frontendConfiguration.ConstructionScheme.HasLayerOfMiscellaneous;
            hasLayerOfBackground = frontendConfiguration.ConstructionScheme.HasLayerOfBackground;
            maxOfRegionLayers = Math.Min(MaximumCountOfRegions, Math.Max(MinimumCountOfRegions, frontendConfiguration.ConstructionScheme.MaxCountOfRegions));
            countOfRegionLayers = Math.Min(MaximumCountOfRegions, Math.Max(MinimumCountOfRegions, frontendConfiguration.ConstructionScheme.CountOfRegions));
            backgroundStyle = frontendConfiguration.ConstructionScheme.BackgroundStyle;
            colorValueOfPeriphery = frontendConfiguration.ConstructionScheme.ColorValueOfPeriphery;
            colorValueOfRegionFirst = frontendConfiguration.ConstructionScheme.ColorValueOfRegionFirst;
            colorValueOfRegionLast = frontendConfiguration.ConstructionScheme.ColorValueOfRegionLast;
            colorValueOfMiscellaneous = frontendConfiguration.ConstructionScheme.ColorValueOfMiscellaneous;
            colorCalculation = frontendConfiguration.ConstructionScheme.ColorCalculation;
            orderOfRegions = frontendConfiguration.ConstructionScheme.OrderOfRegions;
            opacityOfLayers = frontendConfiguration.ConstructionScheme.OpacityOfLayers;
            blendMode = frontendConfiguration.ConstructionScheme.BlendMode;

            // The image processing scheme
            thresholdOfLines = frontendConfiguration.ProcessingScheme.ThresholdOfLines;
            referenceCorner = frontendConfiguration.ProcessingScheme.ReferenceOfPeriphery;
            excludesPeriphery = frontendConfiguration.ProcessingScheme.ExcludesPeripheryFromRegions;
            laysColorOverLines = frontendConfiguration.ProcessingScheme.LaysColorOverLines;
            maxThicknessOfLines = frontendConfiguration.ProcessingScheme.MaxThicknessOfLines;
            isOpeningEnabled = frontendConfiguration.ProcessingScheme.IsOpeningEnabledAfterColoringOverLines;

            // Extra information
            windowTop = frontendConfiguration.ExtraInformation.WindowTop;
            windowLeft = frontendConfiguration.ExtraInformation.WindowLeft;
            windowHeight = frontendConfiguration.ExtraInformation.WindowHeight;
            windowWidth = frontendConfiguration.ExtraInformation.WindowWidth;
            var regionColors = frontendConfiguration.ExtraInformation.AllColorValuesOfRegions;
            if(2 < regionColors.Count)
            {
                paddingColors = frontendConfiguration.ExtraInformation.AllColorValuesOfRegions.GetRange(1, (regionColors.Count - 2));
            }
        }

        /// <summary>
        /// Save property values to configuration file.
        /// </summary>
        private void SaveConfiguration()
        {
            // The file operations scheme
            frontendConfiguration.FileScheme.Suffix = Suffix;
            frontendConfiguration.FileScheme.ExportDestination = ExportDestination;
            frontendConfiguration.FileScheme.ExportDirectoryPath = SpecifiedDestination;
            frontendConfiguration.FileScheme.ExportStyle = ExportStyle;
            frontendConfiguration.FileScheme.IsAutoRenameEnabled = IsAutoRenameEnabled;

            // The layers construction scheme
            frontendConfiguration.ConstructionScheme.HasLayerOfPeriphery = HasLayerOfPeriphery;
            frontendConfiguration.ConstructionScheme.HasLayersOfRegions = HasLayersOfRegions;
            frontendConfiguration.ConstructionScheme.HasLayerOfMiscellaneous = HasLayerOfMiscellaneous;
            frontendConfiguration.ConstructionScheme.HasLayerOfBackground = HasLayerOfBackground;
            frontendConfiguration.ConstructionScheme.MaxCountOfRegions = MaxOfRegionLayers;
            frontendConfiguration.ConstructionScheme.CountOfRegions = CountOfRegionLayers;
            frontendConfiguration.ConstructionScheme.BackgroundStyle = BackgroundStyle;
            frontendConfiguration.ConstructionScheme.ColorValueOfPeriphery = ColorValueOfPeriphery;
            frontendConfiguration.ConstructionScheme.ColorValueOfRegionFirst = ColorValueOfRegionFirst;
            frontendConfiguration.ConstructionScheme.ColorValueOfRegionLast = ColorValueOfRegionLast;
            frontendConfiguration.ConstructionScheme.ColorValueOfMiscellaneous = ColorValueOfMiscellaneous;
            frontendConfiguration.ConstructionScheme.ColorCalculation = ColorCalculation;
            frontendConfiguration.ConstructionScheme.OrderOfRegions = OrderOfRegions;
            frontendConfiguration.ConstructionScheme.OpacityOfLayers = OpacityOfLayers;
            frontendConfiguration.ConstructionScheme.BlendMode = BlendMode;

            // The image processing scheme
            frontendConfiguration.ProcessingScheme.ThresholdOfLines = ThresholdOfLines;
            frontendConfiguration.ProcessingScheme.ReferenceOfPeriphery = ReferenceCorner;
            frontendConfiguration.ProcessingScheme.ExcludesPeripheryFromRegions = ExcludesPeriphery;
            frontendConfiguration.ProcessingScheme.LaysColorOverLines = LaysColorOverLines;
            frontendConfiguration.ProcessingScheme.MaxThicknessOfLines = MaxThicknessOfLines;
            frontendConfiguration.ProcessingScheme.IsOpeningEnabledAfterColoringOverLines = IsOpeningEnabled;

            // Extra information
            frontendConfiguration.ExtraInformation.WindowTop = LastWindowTop;
            frontendConfiguration.ExtraInformation.WindowLeft = LastWindowLeft;
            frontendConfiguration.ExtraInformation.WindowHeight = LastWindowHeight;
            frontendConfiguration.ExtraInformation.WindowWidth = LastWindowWidth;
            IntegrateColorsOfRegions();
            frontendConfiguration.ExtraInformation.SetAllColorsOfRegions(allColorsOfRegions);

            // Save to XML.
            frontendConfiguration.Save();
        }

        /// <summary>
        /// Update middle colors of region layers.
        /// </summary>
        private void UpdateMiddleColors()
        {
            IEnumerable<int> colors;

            if (OperationProtocol.ColorCalculation.HueViaRed == ColorCalculation)
            {
                colors = ColorGeneration.CreateHueTransitionViaRed(ColorValueOfRegionFirst, ColorValueOfRegionLast, CountOfRegionLayers);
            }
            else if (OperationProtocol.ColorCalculation.Rgb == ColorCalculation)
            {
                colors = ColorGeneration.CreateRgbTransition(ColorValueOfRegionFirst, ColorValueOfRegionLast, CountOfRegionLayers);
            }
            else
            {
                colors = ColorGeneration.CreateHueTransition(ColorValueOfRegionFirst, ColorValueOfRegionLast, CountOfRegionLayers);
            }

            PaddingColors = colors;
        }

        /// <summary>
        /// Update permission of export.
        /// </summary>
        private void UpdateExportStatus()
        {
            var isValidSource = File.Exists(SourceFilePath);
            var status = isValidSource && (HasLayerOfPeriphery || HasLayersOfRegions || HasLayerOfMiscellaneous || HasLayerOfBackground);

            CanExport = status;
        }

        /// <summary>
        /// Update export file name for displaying.
        /// </summary>
        private void UpdateExportFileName()
        {
            if (!File.Exists(SourceFilePath))
            {
                return;
            }

            var baseName = Path.GetFileNameWithoutExtension(SourceFilePath) + Suffix + OperationProtocol.GetExtention(ExportStyle);
            ExportFileName = baseName;
        }

        private void ValidateDroppedItem()
        {
            if (IsBusy)
            {
                return;
            }

            if (File.Exists(DroppedItem))
            {
                var extention = Path.GetExtension(DroppedItem);

                if (frontendConfiguration.FileScheme.SourceExtentionList.Contains(extention.ToLower()))
                {
                    SourceFilePath = DroppedItem;
                }
            }
        }

        private void IntegrateColorsOfRegions()
        {
            allColorsOfRegions.Clear();
            allColorsOfRegions.Add(ColorValueOfRegionFirst);
            allColorsOfRegions.AddRange(PaddingColors);
            allColorsOfRegions.Add(ColorValueOfRegionLast);
        }

        private void FixExportFilePath()
        {
            string directoryName;

            if (OperationProtocol.ExportDestination.Specified == ExportDestination)
            {
                directoryName = SpecifiedDestination;
                if (Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
            }
            else
            {
                directoryName = Path.GetDirectoryName(SourceFilePath);
            }

            var filePath = directoryName + @"\" + ExportFileName;

            if (IsAutoRenameEnabled)
            {
                filePath = FileNameOperations.GetFileNameAvoidingCollision(filePath);
            }

            exportFilePath = filePath;
        }

        /// <summary>
        /// Execute image export.
        /// </summary>
        private void Export()
        {
            IsBusy = true;

            // Fix the export file name.
            FixExportFilePath();

            // Integrate colors of regions.
            IntegrateColorsOfRegions();

            ExportAsync();
        }

        /// <summary>
        /// Execute image export（asynchronous execution）.
        /// </summary>
        private async void ExportAsync()
        {
            await Task.Run(() =>
            {
                // Set operation settings (Native).
                SetDivisionScheme();

                // Execute operation (Native).
                ImageOperationsWrapper.ExecuteDividing(exportFilePath, sourceFilePath, exportStyle);

                // After native method execution.
                var currentdispatcher = Application.Current.Dispatcher;
                currentdispatcher.Invoke(() =>
                {
                    IsBusy = false;
                });
            });
        }

        private void SetDivisionScheme()
        {
            ImageOperationsWrapper.SetDivisionScheme(HasLayerOfPeriphery, HasLayersOfRegions, HasLayerOfMiscellaneous);
            ImageOperationsWrapper.SetColorsForDivision(allColorsOfRegions, ColorValueOfPeriphery, ColorValueOfMiscellaneous);
            ImageOperationsWrapper.SetOrderOfRegions(OrderOfRegions);
            ImageOperationsWrapper.SetBackgroundStyle(HasLayerOfBackground, BackgroundStyle);
            ImageOperationsWrapper.SetLayerOptions(BlendMode, OpacityOfLayers);
            ImageOperationsWrapper.SetThreshold(ThresholdOfLines);
            ImageOperationsWrapper.SetReferenceOfPeriphery(ReferenceCorner);
            ImageOperationsWrapper.SetPeripheryManagement(ExcludesPeriphery);
            ImageOperationsWrapper.SetColorOverwhelming(LaysColorOverLines, MaxThicknessOfLines, IsOpeningEnabled);
        }
    }
}
