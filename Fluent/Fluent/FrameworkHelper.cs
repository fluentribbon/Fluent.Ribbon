using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace Fluent
{
    /// <summary>
    /// Represents class to determine .NET Framework version difference
    /// </summary>
    public static class FrameworkHelper
    {
        /// <summary>
        /// Version of WPF
        /// </summary>
        public static readonly Version PresentationFrameworkVersion = Assembly.GetAssembly(typeof(Window)).GetName().Version;
    }
}
