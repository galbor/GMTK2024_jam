using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private TextAsset[] _maps;
    [SerializeField] private MapDisplayer _mapDisplayer;
    
    [SerializeField] private KeyCode _nextLevelKey = KeyCode.E;
    [SerializeField] private KeyCode _previousLevelKey = KeyCode.Q;

    private int currentLevel = 0;
    
    private void Start()
    {
        _mapDisplayer.SetMap(_maps[currentLevel]);
    }
    
    public void NextLevel(bool previous = false)
    {
        currentLevel += previous ? -1 : 1;
        currentLevel = (currentLevel + _maps.Length) % _maps.Length;
        // if (currentLevel >= _mapPaths.Length)
        // {
        //     Debug.Log("No more levels");
        //     return;
        // }
        _mapDisplayer.SetMap(_maps[currentLevel]);
        KeyKeeper.Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(_nextLevelKey))
        {
            NextLevel();
        }
        else if (Input.GetKeyDown(_previousLevelKey))
        {
            NextLevel(true);
        }
    }
}
