using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.All_Scenes.Games.LevelScene
{
    public class LevelButtonGui : MonoBehaviour
    {
        [SerializeField] private GameObject _active;
        [SerializeField] private GameObject _inactive;
        public void SetActive(bool isActive)
        {

            _active.SetActive(isActive);
            _inactive.SetActive(!isActive);
        }
    }
}
