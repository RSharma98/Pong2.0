using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleController : MonoBehaviour{

    public float moveSpeed;     //The speed at which the platform moves
    public string horizontalInputAxis, verticalInputAxis;   //The axes to be used for movement

    private Vector2 pos, vel;   //Position, velocity
    private Vector2 size;       //Size

    CameraController cameraController;

    void Start(){
        cameraController = CameraController.instance;   //Reference to camera controller

        size = GetComponent<BoxCollider2D>().size * transform.localScale;   //Get the size of the collider
        pos = transform.position;   //Get the position
        vel = Vector2.zero;         //Set the velocity to zero at start
    }

    void Update(){
        Vector2 input = new Vector2(Input.GetAxisRaw(horizontalInputAxis), Input.GetAxisRaw(verticalInputAxis));
        vel.y = input.y * moveSpeed * Time.deltaTime;
        GetComponent<Rigidbody2D>().velocity = vel;

        pos = transform.position;   //Get the position
        //Do not let the paddle go above or below the camera's view
        if(pos.y >= cameraController.GetSize().y / 2 - size.y / 2){
            pos.y = cameraController.GetSize().y / 2 - size.y / 2;
        } else if(pos.y <= -cameraController.GetSize().y / 2 + size.y / 2){
            pos.y = -cameraController.GetSize().y / 2 + size.y / 2;
        }
        transform.position = pos;   //Set the position
    }

    //Public function to get velocity of paddle
    public Vector2 GetVelocity(){
        return vel;
    }
}
