using Assets.Scripts.Timer;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Timer/Stopwatch")]
public class Stopwatch : BaseTimer
{
    public override void Tick(float deltaTime)
    {
        if (IsRunning)
        {
            time += deltaTime;
            NotifyTimeChanged(); // Event in der Basisklasse auslösen
        }
    }
}
