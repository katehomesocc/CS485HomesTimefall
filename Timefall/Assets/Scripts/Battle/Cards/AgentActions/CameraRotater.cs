using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotater : MonoBehaviour
{

    public float speedH = 2.0f;
    public float speedV = 2.0f;

    public float speedZ = 2.0f;
    
    public float yaw = 0.0f;
    public  float pitch = 0.0f;
    
    public float x = 0.0f;
    public  float y = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update () {
        x = Mathf.Clamp((Input.mousePosition.x / Screen.width) * 2 - 1, -1.0F, 1.0F);
        y = Mathf.Clamp((Input.mousePosition.y / Screen.height) * 2 - 1, -1.0F, 1.0F);

        yaw = speedH * x;
        pitch = -1 * speedV * y;

        transform.Rotate(pitch * Time.deltaTime, yaw * Time.deltaTime, speedZ * Time.deltaTime);
    }
}
