using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 30f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(destroyBullet());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += _speed * Time.deltaTime * transform.up;
    }
    
    
    IEnumerator destroyBullet()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
