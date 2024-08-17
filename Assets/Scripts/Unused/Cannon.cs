using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private KeyCode _left;
    [SerializeField] private KeyCode _right;
    [SerializeField] private KeyCode _shoot;
    
    [SerializeField] private GameObject _bulletPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotate();
        if (Input.GetKeyDown(_shoot))
        {
            shoot();
        }
    }
    
    
    private void rotate()
    {
        Transform t = transform;
        if (Input.GetKey(_left))
        {
            t.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(_right))
        {
            t.Rotate(Vector3.forward, -_rotationSpeed * Time.deltaTime);
        }
    }

    private void shoot()
    {
        Instantiate(_bulletPrefab, transform.position, transform.rotation);
    }
}
