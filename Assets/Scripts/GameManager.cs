using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    private int score1, score2;         //The scores for each player

    [Header("Main Menu UI")]                            //The menu UI elements
    public Text titleText;
    public Text creditsText;
    public Text startText;
    public Text controlsText1, controlsText2;

    [Header("In-Game UI")]
    public Text score1Box;                              //The text boxes for both players' scores
    public Text score2Box; 
    public Text victoryTextBack, victoryTextFront;      //The victory text boxes

    [Header("Player Colours")]
    public Color playerOneColour;           //The colour of each player
    public Color playerTwoColour;

    [Header("Particle Effects")]
    public ParticleSystem goalParticles;    //The particle effects for the game
    public ParticleSystem hitParticles;

    [Header("Difficulties")]
    public GameObject mediumMode;           //The game objects containing the difficulty elements
    public GameObject hardMode;
    public GameObject ultraMode;

    private bool showMainMenu;              //Should the main menu be shown

    //Create a singleton
    public static GameManager instance;
    void Awake(){
        if(instance == null) instance = this;
        else Destroy(this);
        showMainMenu = true;            //Enable the main menu
    }

    void Update(){
        if(showMainMenu){
            //Enable menu UI
            titleText.enabled = true;
            creditsText.enabled = true;
            startText.enabled = true;
            controlsText1.enabled = true;
            controlsText2.enabled = true;

            //Disable game UI
            score1Box.enabled = score2Box.enabled = false;
            victoryTextBack.enabled = victoryTextFront.enabled = false;

            //Disable all mode-related objects
            mediumMode.SetActive(false);
            hardMode.SetActive(false);
            ultraMode.SetActive(false);

            //Check for input to determine what mode to play
            if(Input.GetKeyDown(KeyCode.Alpha1)) SetMode("Easy");
            else if(Input.GetKeyDown(KeyCode.Alpha2)) SetMode("Medium");
            else if(Input.GetKeyDown(KeyCode.Alpha3)) SetMode("Hard");
            else if(Input.GetKeyDown(KeyCode.Alpha4)) SetMode("Ultra");
        } else{
            //Enable game UI
            score1Box.enabled = score2Box.enabled = true;

            //Disable menu UI
            titleText.enabled = false;
            creditsText.enabled = false;
            startText.enabled = false;
            controlsText1.enabled = false;
            controlsText2.enabled = false;

            //If the escape key is pressed while game playing
            if(Input.GetKeyDown(KeyCode.Escape)){
                showMainMenu = true;        //Show the main menu
                StartCoroutine(Reset());    //Reset the game
            } 
        }
    }

    private void SetMode(string mode){
        showMainMenu = false;   //Disable the main menu

        //Set the mode based on the string provided
        if(mode.Equals("Easy")) Debug.Log("Easy mode selected");
        else if(mode.Equals("Medium")) mediumMode.SetActive(true);
        else if(mode.Equals("Hard")) hardMode.SetActive(true);
        else if(mode.Equals("Ultra")){
            ultraMode.SetActive(true);
            MovingPlatformController.instance.BeginMoving();    //Begin moving the platforms for ultra mode
        }

        StartCoroutine(BallController.instance.Reset());        //Reset the ball
    }

    public void IncrementScore(int player){
        //Increment the score
        if(player == 1) score1++;
        else if(player == 2) score2++;
        else Debug.Log("Invalid score to increment");

        //Update the textboxes
        score1Box.text = score1 + "";
        score2Box.text = score2 + "";

        string victoryText = "";
        if(score1 >= 5 || score2 >= 5){ //If the score for any player is 5 or more
            victoryText = score1 >= 5 ? "PLAYER ONE WINS!" : "PLAYER TWO WINS!";    //Get the victory text
            victoryTextFront.color = score1 >= 5 ? playerOneColour : playerTwoColour;   //Set the victory text colour
            victoryTextBack.enabled = victoryTextFront.enabled = true;      //Enable the victory text
            victoryTextBack.text = victoryTextFront.text = victoryText;     //Set the victory text
            int winningPlayer = score1 >= 5 ? 1 : 2;    //Determine which player one
            PlayGoalParticles(winningPlayer);   //Play the goal scored particle effects
            StartCoroutine(Reset());            //Reset the game
        }
    }

    IEnumerator Reset(){
        BallController.instance.StopBall();     //Stop the ball
        yield return new WaitForSeconds(2.0f);  //Wait two seconds
        score1 = score2 = 0;                    //Reset the score
        showMainMenu = true;                    //Show the main menu
    }

    public void PlayGoalParticles(int player){
        Color colour = player == 1 ? playerOneColour : playerTwoColour; //Determine which colour to use
        goalParticles.startColor = colour;  //Set the colour
        goalParticles.Play();               //Play the particle effects
    }

    public void PlayHitParticles(Vector2 colPos, Vector2 ballPos, Color colour){
        hitParticles.transform.position = (colPos + ballPos) / 2; //Set the position to the middle of the collider and ball
        hitParticles.startColor = colour;   //Set the colour of the particles
        hitParticles.Play();                //Play the particle effect
    }

    //Public ints to get the scores
    public int GetPlayerOneScore(){
        return score1;
    }
    
    public int GetPlayerTwoScore(){
        return score2;
    }
}
