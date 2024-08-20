using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restarter : MonoBehaviour
{
    [SerializeField] private KeyCode _restartKey = KeyCode.R;
    [SerializeField] private GameObject _startScreen;
    private bool restart;
    
    //restarts the game to the initial state
    
    void Awake() {
        // DontDestroyOnLoad(this.gameObject);
        // restart = false;
        // Debug.Log("Restarter Start");
    }

    void Update() {
        if (Input.GetKeyDown(_restartKey)) {
            // restart = true;  
            Debug.Log("Restarted");
            // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            MapDisplayer.Instance.ResetMap();
            KeyKeeper.Reset();
            _startScreen.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }
}