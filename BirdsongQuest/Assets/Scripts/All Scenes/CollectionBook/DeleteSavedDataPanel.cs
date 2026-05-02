using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.All_Scenes.CollectionBook
{
    public class DeleteSavedDataPanel : MonoBehaviour
    {
        public void Show()
        {
            this.gameObject.SetActive(true);
        }
        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        public void OnYes()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();


            #if UNITY_EDITOR
                        EditorApplication.isPlaying = false;
            #else
                    Application.Quit();
            #endif
        }
    
    }
}
