using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour{   

    Camera cam;

    private float orthoSize;        //Orthographic size
    private float width, height;    //Width and height of camera (in world units)

    //Create a singleton
    public static CameraController instance;
    void Awake(){
        if(instance == null) instance = this;
        else Destroy(this);
    }
    
    void Start(){
        cam = GetComponent<Camera>();       //Get the camera
        float aspectRatio = (float)Screen.width / (float)Screen.height;   //Get the aspect ratio
        
        orthoSize = cam.orthographicSize;   //Get the orthographic size of the camera
        height = orthoSize * 2;             //Set the height
        width = height * aspectRatio;       //Set the width
    }

    //Coroutine for camera shake
    public IEnumerator Shake(float timer){
        while(timer > 0){
            cam.orthographicSize = orthoSize + Random.Range(-0.1f, 0.1f);   //Set the orthographic size to a random value in range
            timer -= Time.deltaTime;    //Decrease the timer
            yield return null;          //Wait for the next frame
        }
        cam.orthographicSize = orthoSize;   //Reset the orthographic size
    }

    //Get the size of the camera view (in world units)
    public Vector2 GetSize(){
        return new Vector2(width, height);
    }
}
