using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.All_Scenes.Games
{
    [CreateAssetMenu(menuName = "Game/Events/GameFinishedEvent")]
    public class GameFinishedEventSO : ScriptableObject
    {
        public Action<bool> OnGameFinished; // nur gewonnen oder verloren
        public void Raise(bool won) => OnGameFinished?.Invoke(won);
    }
}
