using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.All_Scenes.Games.FullQuizGame.QuizGame
{
    public class BirdSoundPlayButton : MonoBehaviour
    {
        [SerializeField] private AudioWaveform WaveFormDesigner;

        public void OnClick()
        {
           WaveFormDesigner.PlaySound();
        }
    }
}
