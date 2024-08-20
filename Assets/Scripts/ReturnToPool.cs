using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class ReturnToPool : MonoBehaviour
{
    private Pool _pool;
    [SerializeField] private string[] _tags;
    [SerializeField] private int _requiredKeys;
    
    // Start is called before the first frame update
    void Start()
    {
        _pool = transform.parent.GetComponent<Pool>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (KeyKeeper.KeysAmt() < _requiredKeys) return;
        foreach (string tag in _tags)
        {
            if (other.CompareTag(tag))
            {
                _pool.Return(gameObject);
                return;
            }
        }
    }
}
