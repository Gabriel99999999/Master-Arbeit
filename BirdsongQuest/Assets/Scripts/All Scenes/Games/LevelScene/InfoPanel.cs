using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.All_Scenes.Games.LevelScene
{
    public class InfoPanel : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text titleText;
        [SerializeField] private TMPro.TMP_Text bestzeit;

        public Action OnStartClicked;

        public void Awake()
        {
            
        }
        public void OnButtonClick()
        {
            Debug.Log("InfoPanel: Start Button Clicked");
            OnStartClicked.Invoke();

        }

        public void Show(bool isActive)
        {
            Debug.Log($"InfoPanel: Show({isActive})");
            gameObject.SetActive(isActive);
        }

        public void SetInfo(string title, string bestTime)
        {
            titleText.text = title;
            bestzeit.text = $"Bestzeit: {bestTime}";
        }


    }
}
