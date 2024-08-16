using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class Pool :MonoBehaviour
    {
        private Queue<GameObject> _pool = new Queue<GameObject>();
        [SerializeField] private GameObject _prefab;
        
        public GameObject Get(Vector2 position, Transform parent = null)
        {
            if (_pool.Count == 0)
            {
                return Instantiate(_prefab, position, Quaternion.identity, parent);
            }
            var obj = _pool.Dequeue();
            obj.transform.position = position;
            obj.transform.SetParent(parent);
            obj.SetActive(true);
            return obj;
        }
        
        public void Return(GameObject obj)
        {
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }
    }
}