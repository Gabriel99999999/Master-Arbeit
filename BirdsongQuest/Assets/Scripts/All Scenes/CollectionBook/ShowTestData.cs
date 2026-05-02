using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Diagnostics;

namespace Assets.Scripts.All_Scenes.CollectionBook
{
    public class ShowTestData : MonoBehaviour
    {
        [SerializeField] private GameObject BackToLevelSceneBtn;

        public void OnClick()
        {
            OpenLogFolder();
            BackToLevelSceneBtn.SetActive(true);
        }

        private static void OpenLogFolder()
        {
            // gleicher Pfad wie in deinem Logger
            var folder = Application.persistentDataPath;


            #if UNITY_STANDALONE_WIN
                // Ordner öffnen
                System.Diagnostics.Process.Start("explorer.exe", folder.Replace("/", "\\"));
            #elif UNITY_STANDALONE_OSX
                System.Diagnostics.Process.Start("open", folder);
            #elif UNITY_STANDALONE_LINUX
                System.Diagnostics.Process.Start("xdg-open", folder);
            #else
                Debug.LogWarning("OpenLogFolder wird auf dieser Plattform nicht unterstützt: " + Application.platform);
            #endif
            
        }
    }
}
