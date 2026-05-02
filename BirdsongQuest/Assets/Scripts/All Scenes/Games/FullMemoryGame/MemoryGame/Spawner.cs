using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.All_Scenes.Games.FullMemoryGame.MemoryGame
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;

        public GameObject SpawnPrefab(Transform parent)
        {
            return Instantiate(prefab, parent, false);
        }
    }
}
