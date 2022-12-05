using System;

namespace Runtime.Services.Pool
{
    public interface IPoolItem<T>
    {
        public event Action<T> OnReturn; 
        public void ReturnToPool();
    }
}