using System;

namespace Runtime.Interfaces
{
    public interface IHitable
    {
        event Action<IHitable, IHitable> OnHit;

        void Hit();
    }
}