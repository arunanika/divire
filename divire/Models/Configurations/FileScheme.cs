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
    /// The file operations scheme of the application.
    /// </summary>
    public class FileScheme : IConfigurationInfo<FileScheme>
    {
        //================================//
        //==    Constructor             ==//
        //================================//

        public FileScheme()
        {
            SourceExtentionList = new List<string>();
            Suffix = "_R";
            ExportDestination = OperationProtocol.ExportDestination.SameAsSource;
            ExportDirectoryPath = string.Empty;
            ExportStyle = OperationProtocol.ExportStyle.Jpg;
            IsAutoRenameEnabled = true;
        }

        //================================//
        //==    Properties              ==//
        //================================//

        [XmlArrayItem(typeof(string), ElementName = "FileExtention")]
        public List<string> SourceExtentionList { get; }

        public string Suffix { get; set; }

        public OperationProtocol.ExportDestination ExportDestination { get; set; }

        public string ExportDirectoryPath { get; set; }

        public OperationProtocol.ExportStyle ExportStyle { get; set; }

        public bool IsAutoRenameEnabled { get; set; }

        //================================//
        //==    Methods                 ==//
        //================================//

        public void SetProperties(FileScheme source)
        {
            SourceExtentionList.Clear();
            if (0 < source.SourceExtentionList.Count())
            {
                SourceExtentionList.AddRange(source.SourceExtentionList);
            }

            Suffix = source.Suffix;
            ExportDestination = source.ExportDestination;
            ExportDirectoryPath = source.ExportDirectoryPath;
            ExportStyle = source.ExportStyle;
            IsAutoRenameEnabled = source.IsAutoRenameEnabled;
        }
    }
}
