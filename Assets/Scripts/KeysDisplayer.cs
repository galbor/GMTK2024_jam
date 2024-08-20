using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class KeysDisplayer : MonoBehaviour
{
    private int _keysAmt;
    private Pool _keyPool;
    
    [SerializeField] private Vector3 _spacing;
    [SerializeField] private float _scale = 1f;

    void Start()
    {
        _keysAmt = KeyKeeper.KeysAmt();
        _keyPool = GetComponent<Pool>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_keysAmt == KeyKeeper.KeysAmt()) return;
        _keysAmt = KeyKeeper.KeysAmt();
        DisplayKeys();
    }
    
    private void DisplayKeys()
    {
        _keyPool.ReturnAll();
        for (int i = 0; i < _keysAmt; i++)
        {
            _keyPool.Get(transform.position + _spacing * i, _spacing.magnitude*_scale);
        }
    }
}
