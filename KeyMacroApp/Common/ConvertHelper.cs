using KeyMacroApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace KeyMacroApp.Common
{
    public static class ConvertHelper
    {
        public static List<INPUT_Managed>? ToListInputManaged(IEnumerable<KeyHook>? listKeyHooks)
        {
            if (listKeyHooks == null)
            {
                return null;
            }
            List<INPUT_Managed> keyInputs = (from keyInfo in listKeyHooks
                                             let flag = (uint)(keyInfo.State == KeyHook.KeyState.Up ? 0x2 : 0)
                                             let item = new INPUT_Managed(keyInfo.KeyCode, flag)
                                             select item).ToList();
            return keyInputs;
        }
    }
}
