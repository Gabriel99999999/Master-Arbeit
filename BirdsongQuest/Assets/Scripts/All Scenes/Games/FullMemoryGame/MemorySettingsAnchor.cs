using Assets.Scripts.All_Scenes.Games.FullMemoryGame.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.All_Scenes.Games.FullMemoryGame
{
    [CreateAssetMenu(menuName = "Game/Memory/MemorySettingsSO")]
    public class MemorySettingsAnchor : ScriptableObject
    {
        [System.NonSerialized] public MemorySettings Item;

        public bool IsSet => Item != null;

        public void Clear() => Item = null;
    }
}
