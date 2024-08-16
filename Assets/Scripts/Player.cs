using UnityEngine;

namespace DefaultNamespace
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private KeyCode _up;
        [SerializeField] private KeyCode _down;
        [SerializeField] private KeyCode _left;
        [SerializeField] private KeyCode _right;
        
        private void Update()
        {
            if (Input.GetKeyDown(_up))
            {
                MapDisplayer.Instance.MovePlayer(0, -1);
            }
            else if (Input.GetKeyDown(_down))
            {
                MapDisplayer.Instance.MovePlayer(0, 1);
            }
            else if (Input.GetKeyDown(_left))
            {
                MapDisplayer.Instance.MovePlayer(-1, 0);
            }
            else if (Input.GetKeyDown(_right))
            {
                MapDisplayer.Instance.MovePlayer(1, 0);
            }
        }
    }
}