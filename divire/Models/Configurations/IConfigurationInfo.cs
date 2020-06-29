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

namespace divire.Models
{
    /// <summary>
    /// Interface of configuration classes. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConfigurationInfo<T>
    {
        /// <summary>
        /// Set property values from same type object.
        /// </summary>
        /// <param name="source">The same type object containing reference property values</param>
        void SetProperties(T source);
    }
}
