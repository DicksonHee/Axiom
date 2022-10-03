using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Axiom.Core
{
    public static class FlagSystem
    {
       // private static Dictionary<string, FlagValue<int[], bool>> flagDictionary = new Dictionary<string, FlagValue<int[], bool>>();
        private static Dictionary<string, bool> flagDict = new();
       /* public static int[] GetInts(string key)
        {
            flagDictionary.TryGetValue(key, out FlagValue<int[],bool> value);
            return value.GetInts();
        }
        public static bool GetBool(string key)
        {
            flagDictionary.TryGetValue(key, out FlagValue<int[],bool> value);
            return value.GetBool();
        }*/
        
        ///////
       public static bool GetBoolValue(string key)
        {
            flagDict.TryGetValue(key, out bool val);
            return val;
        }

        public static void SetBoolValue(string key, bool val)
        {
            if(flagDict.TryGetValue(key, out _)) 
            flagDict[key] = val;

            else AddDictEntry
            (key, val);
        }

        public static void AddDictEntry(string key, bool value)
        {
                flagDict.Add(key, value); 
        }
    }
}