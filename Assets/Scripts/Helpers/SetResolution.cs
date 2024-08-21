using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetResolutionScript : MonoBehaviour
{
    private const float width = 1920f;
    private const float height = 1080f;

    private float curheight = 0;
    
    private const float landscapeRatio =  width / height;

    // Start is called before the first frame update
    void update()
    {
        if (Screen.height != curheight)
        {
            SetRatio();
            curheight = Screen.height;
        }
    }

    private void SetRatio()
    {
        Debug.Log("Resolution, width: " + Screen.width + ", height: " + Screen.height);

        // Get the real ratio
        float ratio = (float)Screen.width / (float)Screen.height;

        // Cammera settings to landscape
        if (ratio >= landscapeRatio)
        {
            Camera.main.orthographicSize = height/ 200f;
        }
        else
        {
            float scaledHeight = width / ratio;
            Camera.main.orthographicSize = scaledHeight / 200f;
        }
    }
}