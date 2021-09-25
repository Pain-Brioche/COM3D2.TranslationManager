using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Translation_Manager
{
    internal class TempTools
    {
        internal static void Add_HashValue()
        {
            foreach (KeyValuePair<string, LineInfos> keyValuePair in TranslationDatabase.database)
            {
                if (keyValuePair.Value.HashValue == null)
                {
                    keyValuePair.Value.HashValue = keyValuePair.Key;
                }
                if (string.IsNullOrEmpty(keyValuePair.Value.HashValue))
                {
                    keyValuePair.Value.HashValue = keyValuePair.Key;
                    Log.Write("Empty");
                }
                if (keyValuePair.Value.HashValue != keyValuePair.Key)
                {
                    keyValuePair.Value.HashValue = keyValuePair.Key;
                    Log.Write("mismatch");
                }
            }
        }

        internal static void DoStuff()
        {
            Thread.Sleep(10000);
        }
    }
}
