using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLevel : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    // Start is called before the first frame update
    void Start()
    {
        _particleSystem = gameObject.GetComponent<ParticleSystem>();
        EventManagerScript.Instance.StartListening(EventManagerScript.ENDREACHEDEVENT, FinishLevelReached);
    }

    private void FinishLevelReached(object obj)
    {
        _particleSystem.Play();
    }
}
