using Assets.Scripts.All_Scenes.Games.FullMemoryGame.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.All_Scenes.Games.FindeDieAmsel
{
    [CreateAssetMenu(menuName = "Game/FindeDieAmsel/FindeDieAmselSettingsSO")]
    public class FindeDieAmselSettingsAnchor : ScriptableObject
    {
        [System.NonSerialized] public FindeDieAmselSettings Item;

        public bool IsSet => Item != null;

        public void Clear() => Item = null;
        
    }
}
