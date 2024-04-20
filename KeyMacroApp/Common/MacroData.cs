using KeyMacroApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyMacroApp.Common
{
    internal static class MacroData
    {
        private static MacroGroup? _macroAllData = null;
        public static MacroGroup MacroAllData
        {
            get
            {
                if (_macroAllData == null)
                {
                    Directory.CreateDirectory(@".\Macros");
                    _macroAllData = new MacroGroup("Macros");
                }
                return _macroAllData;
            }
        }
    }
}
