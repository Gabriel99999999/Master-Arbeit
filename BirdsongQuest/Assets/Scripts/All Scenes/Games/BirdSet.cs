using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Birdsong/Bird Set", fileName = "BirdSet")]
public class BirdSet : ScriptableObject
{
    public List<Bird> birds = new();
}