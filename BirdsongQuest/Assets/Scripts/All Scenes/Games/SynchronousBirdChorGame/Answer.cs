using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.All_Scenes.Games.BirdChorGame
{
    public class Answer : MonoBehaviour
    {
        
        [SerializeField] public Toggle checkbox;
        [SerializeField] private TextMeshProUGUI birdName;
        [SerializeField] private Image birdImage;


        private Bird _bird;
        public bool IsSelected => checkbox.isOn;
        public string BirdId { get; private set; }

        public void Init(Bird bird)
        {
            _bird = bird;
            BirdId = bird.id;
            birdName.text = bird.name;
            birdImage.sprite = bird.image;
            checkbox.isOn = false;
        }
        public void AddFoundCounter()
        {
            _bird.AddCorrect();
        }

        public void SetSelectedSilently(bool value)
        {
            checkbox.SetIsOnWithoutNotify(value);
        }

        public void SetInteractable(bool value)
        {
            checkbox.interactable = value;
        }
    }
}
