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
    /// The image processing scheme
    /// </summary>
    public class ProcessingScheme : IConfigurationInfo<ProcessingScheme>
    {
        //================================//
        //==    Constructor             ==//
        //================================//

        public ProcessingScheme()
        {
            ThresholdOfLines = 128;
            ReferenceOfPeriphery = OperationProtocol.CornerPoint.TopLeft;
            ExcludesPeripheryFromRegions = true;
            LaysColorOverLines = false;
            MaxThicknessOfLines = 11;
            IsOpeningEnabledAfterColoringOverLines = false;
        }

        //================================//
        //==    Properties              ==//
        //================================//

        public byte ThresholdOfLines { get; set; }

        public OperationProtocol.CornerPoint ReferenceOfPeriphery { get; set; }

        public bool ExcludesPeripheryFromRegions { get; set; }

        public bool LaysColorOverLines { get; set; }

        public int MaxThicknessOfLines { get; set; }

        public bool IsOpeningEnabledAfterColoringOverLines { get; set; }

        //================================//
        //==    Methods                 ==//
        //================================//

        public void SetProperties(ProcessingScheme source)
        {
            ThresholdOfLines = source.ThresholdOfLines;
            ReferenceOfPeriphery = source.ReferenceOfPeriphery;
            ExcludesPeripheryFromRegions = source.ExcludesPeripheryFromRegions;
            LaysColorOverLines = source.LaysColorOverLines;
            MaxThicknessOfLines = source.MaxThicknessOfLines;
            IsOpeningEnabledAfterColoringOverLines = source.IsOpeningEnabledAfterColoringOverLines;
        }
    }
}
