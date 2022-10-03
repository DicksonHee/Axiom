using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axiom.Core
{
    public class StaticClassInitializer : MonoBehaviour
    {
        public FlagScriptableObject flagSO;
        
        private void Start()
        {
            //set every flag to false
            foreach(string key in flagSO.flagKeys) 
            FlagSystem.AddDictEntry(key, false);
        }
    }
}

