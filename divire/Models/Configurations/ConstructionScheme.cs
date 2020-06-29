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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using divire.Protocols;

namespace divire.Models
{
    /// <summary>
    /// The layers construction scheme
    /// </summary>
    public class ConstructionScheme : IConfigurationInfo<ConstructionScheme>
    {
        //================================//
        //==    Constructor             ==//
        //================================//

        public ConstructionScheme()
        {
            HasLayerOfPeriphery = true;
            HasLayersOfRegions = true;
            HasLayerOfMiscellaneous = true;
            HasLayerOfBackground = true;
            MaxCountOfRegions = 99;
            CountOfRegions = 12;
            BackgroundStyle = OperationProtocol.BackgroundStyle.SourceImage;
            ColorValueOfPeriphery = 0x4d4c49;
            ColorValueOfRegionFirst = 0xfa8ea4;
            ColorValueOfRegionLast = 0x6cb4dd;
            ColorValueOfMiscellaneous = 0xe2e6d3;
            ColorCalculation = OperationProtocol.ColorCalculation.Hue;
            OrderOfRegions = OperationProtocol.OrderOfRegions.AreaDescending;
            OpacityOfLayers = 0xff;
            BlendMode = OperationProtocol.BlendMode.Normal;
        }

        //================================//
        //==    Properties              ==//
        //================================//

        public bool HasLayerOfPeriphery { get; set; }

        public bool HasLayersOfRegions { get; set; }

        public bool HasLayerOfMiscellaneous { get; set; }

        public bool HasLayerOfBackground { get; set; }

        public byte MaxCountOfRegions { get; set; }

        public byte CountOfRegions { get; set; }

        public OperationProtocol.BackgroundStyle BackgroundStyle { get; set; }

        [XmlIgnore]
        public int ColorValueOfPeriphery { get; set; }

        [XmlElement(ElementName = "ColorValueOfPeriphery")]
        public string HexColorValueOfPeriphery
        {
            get { return ColorValueOfPeriphery.ToString("X6"); }
            set { ColorValueOfPeriphery = Convert.ToInt32(value, 16); ; }
        }

        [XmlIgnore]
        public int ColorValueOfRegionFirst { get; set; }

        [XmlElement(ElementName = "ColorValueOfRegionFirst")]
        public string HexColorValueOfRegionFirst
        {
            get { return ColorValueOfRegionFirst.ToString("X6"); }
            set { ColorValueOfRegionFirst = Convert.ToInt32(value, 16); ; }
        }

        [XmlIgnore]
        public int ColorValueOfRegionLast { get; set; }

        [XmlElement(ElementName = "ColorValueOfRegionLast")]
        public string HexColorValueOfRegionLast
        {
            get { return ColorValueOfRegionLast.ToString("X6"); }
            set { ColorValueOfRegionLast = Convert.ToInt32(value, 16); ; }
        }

        [XmlIgnore]
        public int ColorValueOfMiscellaneous { get; set; }

        [XmlElement(ElementName = "ColorValueOfMiscellaneous")]
        public string HexColorValueOfMiscellaneous
        {
            get { return ColorValueOfMiscellaneous.ToString("X6"); }
            set { ColorValueOfMiscellaneous = Convert.ToInt32(value, 16); ; }
        }

        public OperationProtocol.ColorCalculation ColorCalculation { get; set; }

        public OperationProtocol.OrderOfRegions OrderOfRegions { get; set; }

        public byte OpacityOfLayers { get; set; }

        public OperationProtocol.BlendMode BlendMode { get; set; }

        //================================//
        //==    Methods                 ==//
        //================================//

        public void SetProperties(ConstructionScheme source)
        {
            HasLayerOfPeriphery = source.HasLayerOfPeriphery;
            HasLayersOfRegions = source.HasLayersOfRegions;
            HasLayerOfMiscellaneous = source.HasLayerOfMiscellaneous;
            HasLayerOfBackground = source.HasLayerOfBackground;
            MaxCountOfRegions = source.MaxCountOfRegions;
            CountOfRegions = source.CountOfRegions;
            BackgroundStyle = source.BackgroundStyle;
            ColorValueOfPeriphery = source.ColorValueOfPeriphery;
            ColorValueOfRegionFirst = source.ColorValueOfRegionFirst;
            ColorValueOfRegionLast = source.ColorValueOfRegionLast;
            ColorValueOfMiscellaneous = source.ColorValueOfMiscellaneous;
            ColorCalculation = source.ColorCalculation;
            OrderOfRegions = source.OrderOfRegions;
            OpacityOfLayers = source.OpacityOfLayers;
            BlendMode = source.BlendMode;
        }
    }
}
