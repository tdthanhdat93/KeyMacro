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
        [DllImport("user32.dll")]
        public static extern int MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        public static extern int GetKeyNameTextW(int lParam, [MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder lpString, int nSize);

        private const uint MAPVK_VK_TO_VSC = 0x00;
        private const uint MAPVK_VSC_TO_VK = 0x01;
        private const uint MAPVK_VK_TO_CHAR = 0x02;
        private const uint MAPVK_VSC_TO_VK_EX = 0x03;
        private const uint MAPVK_VK_TO_VSC_EX = 0x04;

        private static Dictionary<uint, string> _extendedKeys = new Dictionary<uint, string>()
        {
            { 0x2D, "Insert"    },
            { 0x24, "Home"      },
            { 0x21, "PgUp"      },
            { 0x2E, "Delete"    },
            { 0x23, "End"       },
            { 0x22, "PgDn"      },
            { 0x26, "↑"         },
            { 0x25, "←"         },
            { 0x28, "↓"         },
            { 0x27, "→"         },
            { 0x13, "Pause"     },
            { 0x91, "Scroll"    },
            { 0x2c, "Print"    },
        };


        public static uint HexStringToUInt(string s)
        {
            uint value = 0;
            const string PREFIX_HEX = "0x";
            try
            {
                if (s?.StartsWith(PREFIX_HEX) ?? false)
                {
                    uint.TryParse(s.Substring(PREFIX_HEX.Length), NumberStyles.HexNumber, null, out value);
                }
                else
                {
                    uint.TryParse(s, out value);
                }
            }
            catch { }

            return value;
        }

        public static string GetKeyName(uint keyCode)
        {
            if (_extendedKeys.ContainsKey(keyCode))
            {
                return _extendedKeys[keyCode];
            }

            var scanCode = MapVirtualKey(keyCode, uMapType: MAPVK_VK_TO_VSC_EX);
            StringBuilder buffer = new StringBuilder(128);
            int lengthName = GetKeyNameTextW(scanCode << 16, buffer, buffer.Capacity);

            if (lengthName > 0)
            {
                return buffer.ToString();
            }
            else
            {
                return "Unknown";
            }
        }

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
