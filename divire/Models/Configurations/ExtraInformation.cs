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

namespace divire.Models
{
    public class ExtraInformation : IConfigurationInfo<ExtraInformation>
    {
        //================================//
        //==    Constructor             ==//
        //================================//

        public ExtraInformation()
        {
            WindowTop = 0.0;
            WindowLeft = 0.0;
            WindowHeight = 908.0;
            WindowWidth = 420.0;

            AllColorValuesOfRegions = new List<int>();
            AllColorsOfRegions = new List<string>();
        }

        //================================//
        //==    Properties              ==//
        //================================//

        public double WindowTop { get; set; }

        public double WindowLeft { get; set; }

        public double WindowHeight { get; set; }

        public double WindowWidth { get; set; }

        [XmlIgnore]
        public List<int> AllColorValuesOfRegions { get; }

        [XmlArrayItem(typeof(string), ElementName = "ColorValue")]
        public List<string> AllColorsOfRegions { get; }

        //================================//
        //==    Methods                 ==//
        //================================//

        public void SetProperties(ExtraInformation source)
        {
            WindowTop = source.WindowTop;
            WindowLeft = source.WindowLeft;
            WindowHeight = source.WindowHeight;
            WindowWidth = source.WindowWidth;

            AllColorValuesOfRegions.Clear();
            if (0 < source.AllColorsOfRegions.Count())
            {
                AllColorValuesOfRegions.AddRange(source.AllColorsOfRegions.Select(e => Convert.ToInt32(e, 16)));
            }

            AllColorsOfRegions.Clear();
            if (0 < source.AllColorsOfRegions.Count())
            {
                AllColorsOfRegions.AddRange(source.AllColorValuesOfRegions.Select(e => e.ToString("X6")));
            }
        }

        public void SetAllColorsOfRegions(IEnumerable<int> colors)
        {
            AllColorValuesOfRegions.Clear();
            AllColorValuesOfRegions.AddRange(colors);

            AllColorsOfRegions.Clear();
            AllColorsOfRegions.AddRange(colors.Select(e => e.ToString("X6")));
        }
    }
}
