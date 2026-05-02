using System;
using UnityEngine;

namespace Assets.Scripts.Timer
{
    public abstract class BaseTimer : ScriptableObject
    {
        protected float time;
        protected bool IsRunning { get; set; }
        
        public event Action<float> OnTimeChanged;



        protected void NotifyTimeChanged()
        {
            OnTimeChanged?.Invoke(time);
        }
        public virtual void StartTimer()
        {
            time = 0f;
            IsRunning = true;
        }
        public virtual void Reset()
        {
            time = 0f;
            IsRunning = false;
            OnTimeChanged?.Invoke(time);
        }

        public virtual void Continue() => IsRunning = true;
        public virtual void Stop() => IsRunning = false;

        public virtual float GetTime() => time;


        public abstract void Tick(float deltaTime);
        
    }
}
