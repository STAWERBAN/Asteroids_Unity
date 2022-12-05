using System.Collections.Generic;

using Runtime.Services.Pool;

using Unity.VisualScripting;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Runtime.Pool
{
    public class PoolService<T> where T: Object, IPoolItem<T>
    {
        private readonly T _poolItem;

        private Queue<T> _poolingItems = new();
        private Transform _parent;

        public PoolService(T poolItem)
        {
            _poolItem = poolItem;

            _parent = new GameObject().transform;
            _parent.name = poolItem.name + "Root";
        }

        public T GetItem()
        {
            if (_poolingItems.Count > 0)
            {
                var item = _poolingItems.Dequeue();

                item.OnReturn += ReturnToPool;

                return item;
            }

            var newItem = Object.Instantiate(_poolItem, _parent);

            newItem.OnReturn += ReturnToPool;

            return newItem;
        }

        public void ClearPool()
        {
            foreach (var poolingItem in _poolingItems)
            {
                poolingItem.OnReturn -= ReturnToPool;
            }
            
            Object.Destroy(_parent.gameObject);
        }

        private void ReturnToPool(T item)
        {
            item.GameObject().SetActive(false);
            item.OnReturn -= ReturnToPool;
            
            _poolingItems.Enqueue(item);
        }
    }
}