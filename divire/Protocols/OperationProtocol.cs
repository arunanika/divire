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

namespace divire.Protocols
{
    /// <summary>
    /// Various protocols.
    /// </summary>
    public static class OperationProtocol
    {
        //================================//
        //==    Definitions             ==//
        //================================//

        /// <summary>
        /// Styles of background layer.
        /// </summary>
        public enum BackgroundStyle
        {
            Unspecified,

            SourceImage,
            Blank,
        }

        /// <summary>
        /// Calculation method of padding colors.
        /// </summary>
        public enum ColorCalculation
        {
            Unspecified,

            Hue,
            HueViaRed,
            Rgb,
        }

        /// <summary>
        /// Corner points.
        /// </summary>
        public enum CornerPoint
        {
            Unspecified,

            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
        }

        /// <summary>
        /// Styles of image export.
        /// </summary>
        public enum ExportStyle
        {
            Unspecified,

            Jpg,
            Png,
            Psd,
        }

        /// <summary>
        /// Selection of export destination.
        /// </summary>
        public enum ExportDestination
        {
            Unspecified,

            SameAsSource,
            Specified,
        }

        /// <summary>
        /// Order of Regions
        /// </summary>
        public enum OrderOfRegions
        {
            Unspecified,

            AreaDescending,
            AreaAscending,
            FromUpperLeftToLowerRight,
        }

        /// <summary>
        /// Layer blend modes.
        /// </summary>
        public enum BlendMode
        {
            Unspecified,

            Normal,
            Dissolve,

            Darken,
            Multiply,
            ColorBurn,
            LinearBurn,
            DarkerColor,

            Lighten,
            Screen,
            ColorDodge,
            LinearDodge,
            LighterColor,

            Overlay,
            SoftLight,
            HardLight,
            VividLight,
            LinearLight,
            PinLight,
            HardMix,

            Difference,
            Exclusion,
            Subtract,
            Divide,

            Hue,
            Saturation,
            Color,
            Luminosity,
        }

        //================================//
        //==    Fields                  ==//
        //================================//

        private static readonly Dictionary<ExportStyle, string> ExtentionDictionary = new Dictionary<ExportStyle, string>()
        {
            { ExportStyle.Jpg,    ".jpg" },
            { ExportStyle.Png,    ".png" },
            { ExportStyle.Psd,    ".psd" },
        };

        private static readonly IReadOnlyDictionary<string, ExportStyle> ExportStyleDictionary = ExtentionDictionary.GetInvertedReference();

        private static readonly Dictionary<BlendMode, string> BlendModeDetails = new Dictionary<BlendMode, string>()
        {
            { BlendMode.Normal,         "Normal"        },
            { BlendMode.Dissolve,       "Dissolve"      },

            { BlendMode.Darken,         "Darken"        },
            { BlendMode.Multiply,       "Multiply"      },
            { BlendMode.ColorBurn,      "Color Burn"    },
            { BlendMode.LinearBurn,     "Linear Burn"   },
            { BlendMode.DarkerColor,    "Darker Color"  },

            { BlendMode.Lighten,        "Lighten"       },
            { BlendMode.Screen,         "Screen"        },
            { BlendMode.ColorDodge,     "Color Dodge"   },
            { BlendMode.LinearDodge,    "Linear Dodge"  },
            { BlendMode.LighterColor,   "Lighter Color" },

            { BlendMode.Overlay,        "Overlay"       },
            { BlendMode.SoftLight,      "Soft Light"    },
            { BlendMode.HardLight,      "Hard Light"    },
            { BlendMode.VividLight,     "Vivid Light"   },
            { BlendMode.LinearLight,    "Linear Light"  },
            { BlendMode.PinLight,       "Pin Light"     },
            { BlendMode.HardMix,        "Hard Mix"      },

            { BlendMode.Difference,     "Difference"    },
            { BlendMode.Exclusion,      "Exclusion"     },
            { BlendMode.Subtract,       "Subtract"      },
            { BlendMode.Divide,         "Divide"        },

            { BlendMode.Hue,            "Hue"           },
            { BlendMode.Saturation,     "Saturation"    },
            { BlendMode.Color,          "Color"         },
            { BlendMode.Luminosity,     "Luminosity"    },
        };

        private static readonly IReadOnlyDictionary<string, BlendMode> BlendModeDictionary = BlendModeDetails.GetInvertedReference();

        private static readonly Dictionary<BlendMode, string> BlendModeIdentifiers = new Dictionary<BlendMode, string>()
        {
            { BlendMode.Normal,         "norm"  },
            { BlendMode.Dissolve,       "diss"  },

            { BlendMode.Darken,         "dark"  },
            { BlendMode.Multiply,       "mul "  },
            { BlendMode.ColorBurn,      "idiv"  },
            { BlendMode.LinearBurn,     "lbrn"  },
            { BlendMode.DarkerColor,    "dkCl"  },

            { BlendMode.Lighten,        "lite"  },
            { BlendMode.Screen,         "scrn"  },
            { BlendMode.ColorDodge,     "div "  },
            { BlendMode.LinearDodge,    "lddg"  },
            { BlendMode.LighterColor,   "lgCl"  },

            { BlendMode.Overlay,        "over"  },
            { BlendMode.SoftLight,      "sLit"  },
            { BlendMode.HardLight,      "hLit"  },
            { BlendMode.VividLight,     "vLit"  },
            { BlendMode.LinearLight,    "lLit"  },
            { BlendMode.PinLight,       "pLit"  },
            { BlendMode.HardMix,        "hMix"  },

            { BlendMode.Difference,     "diff"  },
            { BlendMode.Exclusion,      "smud"  },
            { BlendMode.Subtract,       "fsub"  },
            { BlendMode.Divide,         "fdiv"  },

            { BlendMode.Hue,            "hue "  },
            { BlendMode.Saturation,     "sat "  },
            { BlendMode.Color,          "colr"  },
            { BlendMode.Luminosity,     "lum "  },
        };

        //================================//
        //==    Methods                 ==//
        //================================//

        public static IReadOnlyDictionary<TValue, TKey> GetInvertedReference<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> table)
        {
            return table.Select(pair => pair.Value).Distinct().ToDictionary(value => value, value => table.FirstOrDefault(pair => pair.Value.Equals(value)).Key);
        }

        public static string GetExtention(ExportStyle style)
        {
            var extention = (style != ExportStyle.Unspecified) ? style : ExportStyle.Jpg;

            return ExtentionDictionary[extention];
        }

        public static string GetBlendModeDetail(BlendMode mode)
        {
            var blendMode = (mode != BlendMode.Unspecified) ? mode : BlendMode.Normal;

            return BlendModeDetails[blendMode];
        }

        public static BlendMode GetBlendMode(string detail)
        {
            var mode = BlendMode.Normal;

            BlendModeDictionary.TryGetValue(detail, out mode);

            return mode;
        }

        public static string GetBlendModeIdentifier(BlendMode mode)
        {
            var blendMode = (mode != BlendMode.Unspecified) ? mode : BlendMode.Normal;

            return BlendModeIdentifiers[blendMode];
        }
    }
}
