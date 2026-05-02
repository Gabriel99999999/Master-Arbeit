using Assets.Scripts.All_Scenes.Games.FullMemoryGame.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.All_Scenes.Games.BirdChorGame
{
    [CreateAssetMenu(menuName = "Game/BirdChor/BirdChorSettingsSO")]
    public class BirdChorSettingsAnchor : ScriptableObject
    {
        [System.NonSerialized] public BirdChorSettings Item;

        public bool IsSet => Item != null;

        public void Clear() => Item = null;
    }
}
