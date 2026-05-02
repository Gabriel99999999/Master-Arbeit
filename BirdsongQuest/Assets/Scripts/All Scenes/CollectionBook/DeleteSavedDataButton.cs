using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.All_Scenes.CollectionBook
{
    public class DeleteSavedDataButton : MonoBehaviour
    {
        [SerializeField] private DeleteSavedDataPanel deleteSavedDataPanel;

        public void OnClick()
        {
            deleteSavedDataPanel.Show();
        }
    }
}
