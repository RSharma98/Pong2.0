using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

    CameraController camController;     //Reference to the camera controller
    public TrailRenderer trail;         //The trail renderer attached to the ball

    public float minMoveSpeed, maxMoveSpeed;    //The minimum and maximum move speed for the ball
    private Vector2 pos, vel, size;             //Position, velocity and size
    private bool canMove;                       //Can this ball move?

    //Create a singleton
    public static BallController instance;
    void Awake(){
        if(instance == null) instance = this;
        else Destroy(this.gameObject);
    }

    void Start(){
        size = GetComponent<BoxCollider2D>().size * transform.localScale;   //Get the size of the ball
        camController = CameraController.instance;      //Get the camera controller instance

        canMove = false;    //By default the ball cannot move
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);   //Set the ball to be transparent
    }

    void Update(){
        if(canMove){
            pos = transform.position;   //Get the position
            //If the position is at the top or bottom of the screen, flip the y velocity
            if((pos.y >= (camController.GetSize().y / 2) - (size.y / 2) && vel.y > 0) || (pos.y <= (-camController.GetSize().y / 2) + (size.y / 2) && vel.y < 0)){
                vel.y *= -1;
            }

            if(pos.x <= -camController.GetSize().x / 2){            //If the ball touches the left edge of the screen
                GameManager.instance.IncrementScore(2);                 //Increment the score for player two
                GameManager.instance.PlayGoalParticles(2);              //Play the goal particle effect for player two
                StartCoroutine(camController.Shake(0.2f));              //Shake the camera
                StartCoroutine(Reset());                                //Reset the ball
            } else if(pos.x >= camController.GetSize().x / 2){      //Otherwise if the ball touches the right edge of screen
                GameManager.instance.IncrementScore(1);                 //Increment the score for player one
                GameManager.instance.PlayGoalParticles(1);              //Play the goal particle effect for player one
                StartCoroutine(camController.Shake(0.2f));              //Shake the camera
                StartCoroutine(Reset());                                //Reset the ball
            }

            GetComponent<Rigidbody2D>().velocity = vel;             //Set the velocity
        }
    }

    void OnCollisionEnter2D(Collision2D col){
        //The position and size of the collision objects
        Vector2 colPos = col.gameObject.transform.position;
        Vector2 colSize = col.gameObject.GetComponent<BoxCollider2D>().size * col.gameObject.transform.localScale;

        //If the ball hit the top or bottom of the object, flip the y velocity. Otherwise, flip the x velocity
        if(pos.y + size.y / 2 <= colPos.y - colSize.y / 2 || pos.y - size.y / 2 >= colPos.y + colSize.y / 2){
            if(pos.x + size.x / 2 >= colPos.x - colSize.x / 2 && pos.x - size.x / 2 <= colPos.x + colSize.x / 2) vel.y *= -1.05f;
            else vel.x *= -1.05f;
        } else vel.x *= -1.05f;

        Color colour = col.gameObject.GetComponent<SpriteRenderer>().color;     //Get the colour of the collision object
        GetComponent<SpriteRenderer>().color = colour;                          //Set the ball colour to the colour of the collision object
        trail.startColor = colour;                                              //Set the trail colour
        GameManager.instance.PlayHitParticles(col.transform.position, this.transform.position, colour); //Play the hit particles
        StartCoroutine(camController.Shake(0.1f));                              //Shake the camera

        if(col.gameObject.name.Contains("Paddle")){     //If the ball hit a paddle
            PaddleController paddleController = col.gameObject.GetComponent<PaddleController>();    //Get the paddle controller
            if(paddleController != null){   //Make the ball move up or down depending on if the paddle had a positive or negative velocity
                if(paddleController.GetVelocity().y > 0) vel.y = Mathf.Abs(vel.y);
                else if(paddleController.GetVelocity().y < 0) vel.y = Mathf.Abs(vel.y) * -1;
            }
        } 
    }

    //Coroutine to reset the ball
    public IEnumerator Reset(){
        canMove = false;    //While resetting, the ball cannot move
        transform.position = Vector2.zero;      //Set the position to be the center of the screen
        vel = Vector2.zero;                     //Set the velocity to zero

        GetComponent<SpriteRenderer>().color = Color.white;     //Set the colour to white
        trail.startColor = Color.white;                         //Set the trail colour to white
        trail.enabled = false;                                  //Disable the trail
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);   //Set the sprite to be transparent
        if(GameManager.instance.GetPlayerOneScore() < 5 && GameManager.instance.GetPlayerTwoScore() < 5){   //If the score for both players is below 5
            float alpha = 0;        //Fade in the alpha of the sprite
            while(alpha < 1){
                alpha += Time.deltaTime * 2;
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
                yield return null;
            }
            StartCoroutine(Launch());   //Begin the launch coroutine
        }
    }
    
    IEnumerator Launch(){
        canMove = false;    //Do not move the ball
        yield return new WaitForSeconds(1.0f);  //Wait for 1 second
        Vector2 launchDir = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));  //Pick a random launch direction
        launchDir.x = launchDir.x < 0 ? -1 : 1;                     //Ensure the launch velocity is not to olow
        launchDir.y = launchDir.y < 0 ? -1 : 1;
        vel = Random.Range(minMoveSpeed, maxMoveSpeed) * launchDir; //Calculate the launch velocity
        GetComponent<Rigidbody2D>().velocity = vel;                 //Set the launch velocity
        GetComponent<SpriteRenderer>().color = Color.white;         //Set the sprite colour to white
        trail.enabled = true;                                       //Enable the trail renderer
        canMove = true;                                             //The ball can now move
    }

    public void StopBall(){
        canMove = false;    //Do not allow the ball to move
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);   //Set the ball to be transparent
        trail.enabled = false;      //Disable the trail renderer
        transform.position = Vector2.zero;                      //Set the position to the centre
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;    //Set the velocity to zero
    }
}
