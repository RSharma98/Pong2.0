using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : MonoBehaviour{

    Vector2 pos, size;      //The position and size of the platform
    Vector2 startPos;       //The starting position of the platform

    public float speed;         //The speed at which the platform should move
    private float moveSpeed;    //The speed at which the 

    public GameObject platform1, platform2;

    //Create a singleton
    public static MovingPlatformController instance;
    void Awake(){
        if(instance == null) instance = this;
        else Destroy(this);

        startPos = platform1.transform.position;    //Set the starting position
    }

    public void BeginMoving(){
        //Set the platform positions
        platform1.transform.position = startPos;
        platform2.transform.position = startPos * -1;

        //Get the position and size of the platform
        pos = platform1.transform.position; 
        size = platform1.GetComponent<BoxCollider2D>().size * platform1.transform.localScale;
        
        StartCoroutine(MovePlatform()); //Call the coroutine to begin moving the platforms
    }

    private IEnumerator MovePlatform(){
        CameraController cameraController = CameraController.instance;  //A reference to the camera controller script
        moveSpeed = speed;  //Set the movement speed

        //While no player has got 5 points
        while(GameManager.instance.GetPlayerOneScore() < 5 && GameManager.instance.GetPlayerTwoScore() < 5){
            if(pos.y - size.y / 2 <= -cameraController.GetSize().y / 2) moveSpeed = speed;          //Move up if touching the bottom part of screen
            else if(pos.y + size.y / 2 >= cameraController.GetSize().y / 2) moveSpeed = -speed;     //Move down if touching top part of screeen
            pos.y += moveSpeed * Time.deltaTime;        //Update the y position
            platform1.transform.position = pos;         //Set the first platform position
            platform2.transform.position = pos * -1;    //Set the second platform position to be the opposite of the first platform
            yield return null;
        }
    }
}
