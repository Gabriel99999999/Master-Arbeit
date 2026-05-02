using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.All_Scenes.Games.LevelScene
{
    public class LevelSelectController : MonoBehaviour
    {
        [SerializeField] private List<LevelButton> _levelButtons;

        private void Awake()
        {
            foreach (var button in _levelButtons)
            {
                button.OnClicked += HandleButtonClicked;
            }
        }

        private void OnDestroy()
        {
            foreach (var button in _levelButtons)
            {
                button.OnClicked -= HandleButtonClicked;
            }
        }

        private void HandleButtonClicked(LevelButton clickedButton)
        {
            // Zuerst alle Panels schließen
            foreach (var btn in _levelButtons)
                btn.ClosePanel();

            // Dann nur das angeklickte Panel öffnen
            clickedButton.OpenPanel();
        }
    }
}
