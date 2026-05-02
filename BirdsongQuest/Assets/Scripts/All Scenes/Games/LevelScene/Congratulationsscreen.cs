using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.All_Scenes.Games.LevelScene
{
    public class Congratulationsscreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _birdName;
        [SerializeField] private Image _birdImage;
        [SerializeField] private GameObject _songs;
        [SerializeField] private GameObject _birdSoundButton;

        public void Activate(Bird bird)
        {
            _birdName.text = bird.displayName;
            _birdImage.sprite = bird.image;
            foreach (var clip in bird.calls)
            {
                var soundButton = Instantiate(_birdSoundButton, _songs.transform);
                var soundButtonComponent = soundButton.GetComponent<BirdSoundButton>();
                soundButtonComponent.SetUp(clip);
            }

            this.gameObject.SetActive(true);
        }

        public void OnWeiter()
        {
            SoundFXManager.Instance.StopAllBirdSounds();
            this.gameObject.SetActive(false);
            foreach (BirdSoundButton song in _songs.GetComponentsInChildren<BirdSoundButton>(true))
            {
                Destroy(song.gameObject);
            }
            
        }

    }
}
