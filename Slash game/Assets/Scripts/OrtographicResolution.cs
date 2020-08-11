using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrtographicResolution : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprite;


    // Start is called before the first frame update
    void Start()
    {
        //float orthoSize = sprite.bounds.size.x * Screen.height / Screen.width * 0.5f;
        //Camera.main.orthographicSize = orthoSize;

        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = sprite.bounds.size.x / sprite.bounds.size.y;

        if(screenRatio >= targetRatio)
        {
            Camera.main.orthographicSize = sprite.bounds.size.y / 2;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            Camera.main.orthographicSize = sprite.bounds.size.y / 2 * differenceInSize;
        }
    }
}
