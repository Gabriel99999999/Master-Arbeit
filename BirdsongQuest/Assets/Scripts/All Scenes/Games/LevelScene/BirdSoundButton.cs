using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Assets.Scripts.All_Scenes.Games.LevelScene
{
    public class BirdSoundButton : MonoBehaviour
    {
        private AudioClip _clip;

        public void Awake()
        {
            SoundFXManager.Instance.SetBirdSoundMode(false);
        }
        
        public void PlaySound()
        {
            UIEvents.RequestBirdSound(_clip, transform, 1f);
        }

        public void SetUp(AudioClip clip)
        {
            _clip = clip;
        }

    }
}
