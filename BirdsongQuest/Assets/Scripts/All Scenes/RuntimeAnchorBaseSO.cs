using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.All_Scenes
{
    public abstract class RuntimeAnchorBaseSO : ScriptableObject
    {
        public bool IsSet { get; protected set; }
        public abstract void Clear();
    }
}
