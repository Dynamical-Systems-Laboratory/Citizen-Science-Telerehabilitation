using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//MonoBehavior required for unity to read (also script attatched to game object)
public class UserProfile : MonoBehaviour
{
    //inHomeScreen = 1

    //general
    public StateManager state;
    public ClickAction eventListener;
    public PowerpointScript slides;

    //buttons
    public static GameObject homeButton; //main buttons
    public static GameObject userName;
    public static GameObject difficultyMeter;

    //info to be updated
    public static Text dateJoined;
    public static Text imagesCompleted;
    public static Text tagsPlaced;
    public static Text timeCompleted;
    public static Text sessionsLogged;
    public static Text progress;
    public static Text difficulty;
    public static Slider progressBar;

    public static Vector3 stateModifier2 = new Vector3(-100.2f + 2.8f + 380f, 500f, 66.6f); //offset of button positions relative to makewordbank
    public static float scale = 1.72f;//3/2;

    //button colors
    public static float colorFactor = 183f / 255f;
    public static Color unhighlighted = new Color(colorFactor, colorFactor, colorFactor, 1f);
    public static Color highlighted = new Color(1f, 1f, 1f, 1f);

    void Awake()
    {
        state = GameObject.Find("Canvas").GetComponent<StateManager>();

        eventListener = GameObject.Find("Canvas").GetComponent<ClickAction>();

        homeButton = GameObject.Find("HomeButton2");
        userName = GameObject.Find("UserName");

        dateJoined = GameObject.Find("DateJoined").GetComponent<Text>() as Text as Text; ;
        imagesCompleted = GameObject.Find("ImagesCompleted").GetComponent<Text>() as Text;
        tagsPlaced = GameObject.Find("TagsPlaced").GetComponent<Text>() as Text;
        timeCompleted = GameObject.Find("TimeCompleted").GetComponent<Text>() as Text;
        sessionsLogged = GameObject.Find("SessionsLogged").GetComponent<Text>() as Text;
        progress = GameObject.Find("ProgressPercent").GetComponent<Text>() as Text;
        progressBar = GameObject.Find("ProgressBar").GetComponent<Slider>() as Slider;

        difficulty = GameObject.Find("DifficultyNumber").GetComponent<Text>() as Text;
        difficultyMeter = GameObject.Find("DifficultySlider");
    }
    //TODO: change userName from text box to editable text field
    void Update()
    {
        if (state.getState() == 3)
        {
            //Text replacement
            int[] textData = state.user.getProgressData();
            //Debug.Log("TextData:" + textData[0] + "," + textData[1] + "," + textData[]);
            userName.GetComponent<Text>().text = "User: " + state.user.getName();
            dateJoined.text = "Date Joined: " + state.user.getDateJoined();
            imagesCompleted.text = "# Images Completed: " + textData[1];
            tagsPlaced.text = "# Tags Placed: " + textData[2];
            sessionsLogged.text = "Sessions Logged: " + textData[3];
            timeCompleted.text = "Time Logged: " + state.user.getTimeLogged();
            progress.text = state.user.getProgress() + "%";
            progressBar.value = state.user.getProgress();
            //TODO: figure out horizontal transformation that coorelateds to scaler (-11.2 = 50%?)

            //difficulty = diffucultyMeter.value;

            //Clicking Things (buttons)
            Debug.Log("Home: " + ((homeButton.transform.position - UserProfile.stateModifier2) - (state.getCursorPosition() * StateManager.cursorPosMod * HomeScreen.scale))
                + ", " + ((homeButton.transform.position - UserProfile.stateModifier2) - (state.getCursorPosition() * StateManager.cursorPosMod * HomeScreen.scale)).magnitude);
            Debug.Log("Slider: " + ((difficultyMeter.transform.position - UserProfile.stateModifier2) - (state.getCursorPosition() * StateManager.cursorPosMod * HomeScreen.scale))
                + ", " + ((difficultyMeter.transform.position - UserProfile.stateModifier2) - (state.getCursorPosition() * StateManager.cursorPosMod * HomeScreen.scale)).magnitude);
            if (Input.GetKeyDown(KeyCode.B)) //clicking
            {
                Vector3 homeDist = ((homeButton.transform.position - UserProfile.stateModifier2) - (state.getCursorPosition() * StateManager.cursorPosMod * HomeScreen.scale));
                Vector3 sliderDist = ((difficultyMeter.transform.position - UserProfile.stateModifier2) - (state.getCursorPosition() * StateManager.cursorPosMod * HomeScreen.scale));
                if (homeDist.x <= 118 && homeDist.x >= 32 && homeDist.y <= 17.8 && homeDist.y >= -18.5)
                {
                    //state.setState(1);
                    Debug.Log("Going Home");
                }
                else if (sliderDist.x <= 142.5 && sliderDist.x >= -5 &&sliderDist.y <= 9 && sliderDist.y >= -12)
                {
                    Debug.Log("Sliding into the dm's");
                }
                else
                {
                    Debug.Log("~No Button To Press~");
                }
            }

        }
    }

}