using Assets.Scripts.All_Scenes.Games.FullQuizGame.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.All_Scenes.Games.FullQuizGame
{
    [CreateAssetMenu(menuName = "Game/Quiz/QuizSettingsSO")]
    public class QuizSettingsAnchor : ScriptableObject
    {
        [System.NonSerialized] public QuizSettings Item;

        public bool IsSet => Item != null;

        public void Clear() => Item = null;

    }
}
