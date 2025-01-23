using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotater : MonoBehaviour
{
    public bool doRotate = true;
    public bool xActive = true;
    public bool yActive = true;
    public bool zActive = true;

    public float speedH = 2.0f;
    public float speedV = 2.0f;

    public float speedZ = 2.0f;
    
    public float yaw = 0.0f;
    public float pitch = 0.0f;
    public float zRot = 0.0f;
    
    public float x = 0.0f;
    public  float y = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update () {
        if(!doRotate) {return;}
        
        yaw = 0.0f;
        pitch = 0.0f;
        zRot = 0.0f;

        if(xActive)
        {
            x = Mathf.Clamp((Input.mousePosition.x / Screen.width) * 2 - 1, -1.0F, 1.0F);
            yaw = speedH * x;
        }

        if(yActive)
        {
            y = Mathf.Clamp((Input.mousePosition.y / Screen.height) * 2 - 1, -1.0F, 1.0F);
            pitch = -1 * speedV * y;
        }

        if(zActive)
        {
            zRot = speedZ;
        }

        transform.Rotate(pitch * Time.deltaTime, yaw * Time.deltaTime, zRot * Time.deltaTime);
    }
}
