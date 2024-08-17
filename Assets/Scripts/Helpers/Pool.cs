using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class Pool :MonoBehaviour
    {
        private Queue<GameObject> _pool = new Queue<GameObject>();
        [SerializeField] private GameObject _prefab;
        
        public GameObject Get(Vector2 position, float scale = 1f)
        {
            GameObject obj;
            if (_pool.Count == 0)
            {
                obj = Instantiate(_prefab, position, Quaternion.identity, transform);
            }
            else
            {
                obj = _pool.Dequeue();
                obj.transform.position = position;
                obj.transform.SetParent(transform);
                obj.SetActive(true);
            }
            obj.transform.localScale = Vector3.one * scale;
            return obj;
        }
        
        public void Return(GameObject obj)
        {
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }
    }
}