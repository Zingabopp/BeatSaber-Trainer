using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BeatSaberTrainer.Plugin;

namespace BeatSaberTrainer
{
    public static class Util
    {
        public enum MSGLEVEL
        {
            DISABLED = 0,
            MESSAGE = 1, // Always show unless disabled
            ERROR = 2,
            WARNING = 3,
            INFO = 4,
            DEBUG = 5
        }

        public static MSGLEVEL LEMsgLevel = MSGLEVEL.DEBUG;
        /// <summary>
        /// Writes a message to the console if the message's MSGLEVEL is >= the set message level.
        /// </summary>
        /// <param name="msg">Message to write</param>
        /// <param name="level">Verbosity level of the message</param>
        /// <returns>The string that would be outputted to the console</returns>
        public static string WriteMsg(string msg, MSGLEVEL level = MSGLEVEL.MESSAGE)
        {
            string toWrite = "";
            switch (level)
            {
                case MSGLEVEL.ERROR:
                    {
                        toWrite = toWrite + "ERROR: ";
                        break;
                    }

                case MSGLEVEL.WARNING:
                    {
                        toWrite = toWrite + "WARNING: ";
                        break;
                    }

                case MSGLEVEL.INFO:
                    {
                        toWrite = toWrite + "INFO: ";
                        break;
                    }
            }
            toWrite = $"[{Plugin.PluginName}]: " + toWrite + msg;
            if (LEMsgLevel >= level)
                Console.WriteLine(toWrite);
            return toWrite;
        }

        public static string DebugMessage(string msg)
        {
            return WriteMsg(msg, MSGLEVEL.DEBUG);
        }

        public static string BinaryToString(int val)
        {
            string binary = "";
            string retVal = "";
            const int mask = 1;
            if (val >= 0)
            {
                while (val > 0)
                {
                    binary = (val & mask) + binary;
                    val = val >> 1;
                }
                binary = binary.PadLeft(32, '0');
            }
            else
            {
                binary = Convert.ToString(val & 4294967295, 2);
            }

            for (int i = 0; i < 32; i += 4)
            {
                retVal = retVal + binary.Substring(i, 4) + " ";
            }
            return retVal.Trim();
        }
    }
}
