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

namespace divire.Models
{
    public static class FileNameOperations
    {
        public static string GetFileNameAvoidingCollision(string baseName)
        {
            var result = baseName;

            if (File.Exists(baseName))
            {
                var directory = Path.GetDirectoryName(baseName);
                var withoutExtention = Path.GetFileNameWithoutExtension(baseName);
                var extention = Path.GetExtension(baseName);
                var fixedPart = directory + @"\" + withoutExtention;

                for (var collisionCount = 1; collisionCount < int.MaxValue; collisionCount++)
                {
                    var modified = fixedPart + $"[{(collisionCount + 1)}]" + extention;

                    if (!File.Exists(modified))
                    {
                        result = modified;
                        break;
                    }
                }
            }

            return result;
        }
    }
}
