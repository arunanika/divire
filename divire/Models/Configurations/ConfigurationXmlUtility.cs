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
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace divire.Models
{
    /// <summary>
    /// Provides XML serialization and deserialization for configuration classes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ConfigurationXmlUtility<T> where T : IConfigurationInfo<T>
    {
        public static void Load(T configuration, string sourceFilePath)
        {
            if (!File.Exists(sourceFilePath))
            {
                return;
            }

            var serializer = new XmlSerializer(typeof(T));

            using (var streamreader = new StreamReader(sourceFilePath))
            {
                try
                {
                    var source = (T)serializer.Deserialize(streamreader);

                    configuration.SetProperties(source);
                }
                catch (InvalidOperationException ex)
                {
                    throw ex;
                }
            }
        }

        public static void Save(T configuration, string destination)
        {
            // Create if the parent directory doesn't exist.
            var dir = Path.GetDirectoryName(destination);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var serializer = new XmlSerializer(typeof(T));

            using (var streamwriter = new StreamWriter(destination, false))
            {
                serializer.Serialize(streamwriter, configuration);
            }
        }
    }
}
