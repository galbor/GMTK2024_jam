using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Tank : MonoBehaviour
{
    [SerializeField] private KeyCode _up;
    [SerializeField] private KeyCode _down;
    [SerializeField] private KeyCode _left;
    [SerializeField] private KeyCode _right;
    
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _rotationSpeed = 10f;
    
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        move();
        rotate();
    }


    private void move()
    {
        Transform t = transform;
        if (Input.GetKey(_up))
        {
            t.position += t.up * _speed * Time.deltaTime;
        }
        else if (Input.GetKey(_down))
        {
            t.position -= t.up * _speed * Time.deltaTime;
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
}
