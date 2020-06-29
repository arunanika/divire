//
//  divire
//
//  Copyright (C) 2020 Aru Nanika
//
//  This program is released under the MIT License.
//  https://opensource.org/licenses/MIT
//
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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace divire.Models
{
    public class FrontendConfiguration : IConfigurationInfo<FrontendConfiguration>
    {
        //================================//
        //==    Fields                  ==//
        //================================//

        private string configurationFileName;

        //================================//
        //==    Constructor             ==//
        //================================//

        public FrontendConfiguration()
        {
            FileScheme = new FileScheme();
            ConstructionScheme = new ConstructionScheme();
            ProcessingScheme = new ProcessingScheme();
            ExtraInformation = new ExtraInformation();

            SetupConfigurationFileName();
        }

        //================================//
        //==    Properties              ==//
        //================================//

        public FileScheme FileScheme { get; set; }

        public ConstructionScheme ConstructionScheme { get; set; }

        public ProcessingScheme ProcessingScheme { get; set; }

        public ExtraInformation ExtraInformation { get; set; }

        //================================//
        //==    Methods                 ==//
        //================================//

        public void SetProperties(FrontendConfiguration source)
        {
            FileScheme.SetProperties(source.FileScheme);
            ConstructionScheme.SetProperties(source.ConstructionScheme);
            ProcessingScheme.SetProperties(source.ProcessingScheme);
            ExtraInformation.SetProperties(source.ExtraInformation);
        }

        public void Load()
        {
            ConfigurationXmlUtility<FrontendConfiguration>.Load(this, configurationFileName);
        }

        public void Save()
        {
            ConfigurationXmlUtility<FrontendConfiguration>.Save(this, configurationFileName);
        }

        private void SetupConfigurationFileName()
        {
            // The same directory as the execution file.
            var assembly = Assembly.GetEntryAssembly();
            var location = assembly.Location;
            var directory = Path.GetDirectoryName(location);

            configurationFileName = $"{directory}\\{Path.GetFileNameWithoutExtension(location)}.xml";
        }
    }
}
