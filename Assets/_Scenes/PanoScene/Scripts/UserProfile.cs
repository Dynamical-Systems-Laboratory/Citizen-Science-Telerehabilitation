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
   
    //info to be updated
    public static Text dateJoined;
    public static Text imagesCompleted;
    public static Text tagsPlaced;
    public static Text timeCompleted;
    public static Text sessionsLogged;
    public static Text progress;
    public static Text difficulty;
    public static List<Text> texts = new List<Text>();

    //other
    public static InputField userName;
    public static Slider progressBar;
    public static Slider difficultyMeter;

    public static Vector3 stateModifier2 = new Vector3(-100.2f + 2.8f + 380f, 500f, 66.6f); //offset of button positions relative to makewordbank
    public static float scale = 1.72f;//3/2;

    //button colors
    public static float colorFactor = 183f / 255f;
    public static Color unhighlighted = new Color(colorFactor, colorFactor, colorFactor, 1f);
    public static Color highlighted = new Color(1f, 1f, 1f, 1f);

    private static string letters = "abcdefghijklmnopqrstuvwxyzACBDEFGHIJKLMNOPQRSTUVWXYZ";//allowed characters for username

    bool isTyping = false;

    public static bool isDisplaced = false;

    public static GameObject mainCanv;

    void Awake()
    {
        state = GameObject.Find("Canvas").GetComponent<StateManager>();

        eventListener = GameObject.Find("Canvas").GetComponent<ClickAction>();

        homeButton = GameObject.Find("HomeButton2");
        userName = GameObject.Find("UserName").GetComponent<InputField>() as InputField;

        dateJoined = GameObject.Find("DateJoined").GetComponent<Text>() as Text;
        imagesCompleted = GameObject.Find("ImagesCompleted").GetComponent<Text>() as Text;
        tagsPlaced = GameObject.Find("TagsPlaced").GetComponent<Text>() as Text;
        timeCompleted = GameObject.Find("TimeCompleted").GetComponent<Text>() as Text;
        sessionsLogged = GameObject.Find("SessionsLogged").GetComponent<Text>() as Text;
        progress = GameObject.Find("ProgressPercent").GetComponent<Text>() as Text;
        progressBar = GameObject.Find("ProgressBar").GetComponent<Slider>() as Slider;

        difficulty = GameObject.Find("DifficultyNumber").GetComponent<Text>() as Text;
        difficultyMeter = GameObject.Find("DifficultySlider").GetComponent<Slider>() as Slider;

        texts.Add(dateJoined);
        texts.Add(imagesCompleted);
        texts.Add(timeCompleted);
        texts.Add(sessionsLogged);
        texts.Add(progress);
        texts.Add(difficulty);

        mainCanv = GameObject.Find("ProfileCanvas");
    }

    public static void screenIsActive(bool isActive)
    {
        //if (!isActive && !isDisplaced)
        //{
        //    /*foreach (Text words in texts)
        //    {
        //        words.color = Color.clear;
        //    }*/
        //    GameObject.Find("UserInfoPanel").transform.position += new Vector3(100f, 0f, 0f);
        //    GameObject.Find("ProgressPanel").transform.position += new Vector3(100f, 0f, 0f);
        //    GameObject.Find("HomePanel2").transform.position += new Vector3(100f, 0f, 0f);
        //    GameObject.Find("DifficultyPanel").transform.position += new Vector3(100f, 0f, 0f);
        //    GameObject.Find("BackgroundPanel").transform.position += new Vector3(100f, 0f, 0f);
        //    isDisplaced = true;
        //}
        //else if (isActive && isDisplaced)
        //{
        //    /*foreach (Text words in texts)
        //    {
        //        words.color = HomeScreen.nyuPurple;
        //    }*/
        //    GameObject.Find("UserInfoPanel").transform.position -= new Vector3(100f, 0f, 0f);
        //    GameObject.Find("ProgressPanel").transform.position -= new Vector3(100f, 0f, 0f);
        //    GameObject.Find("HomePanel2").transform.position -= new Vector3(100f, 0f, 0f);
        //    GameObject.Find("DifficultyPanel").transform.position -= new Vector3(100f, 0f, 0f);
        //    GameObject.Find("BackgroundPanel").transform.position -= new Vector3(100f, 0f, 0f);
        //    isDisplaced = false;
        //}
        ////else if (isActive &&)
        ////mainCanv.SetActive(isActive);
    }

    void Update()
    {
        if (state.getState() == 3)
        {
            //Text replacement
            int[] textData = state.user.getCompletionData();
            if (state.user.hasName())
            {
                userName.text = state.user.getName();
            }
            else
            {
                userName.text = "";
            }
            dateJoined.text = "Date Joined: " + state.user.getDateJoined();
            imagesCompleted.text = "# Images Completed: " + textData[1];
            tagsPlaced.text = "# Tags Placed: " + textData[2];
            sessionsLogged.text = "Sessions Logged: " + textData[3];
            timeCompleted.text = "Time Logged: " + state.user.getTimeLogged();
            progress.text = state.user.getProgress() + "%";
            if (state.user.getProgress() > 100)
            {
                progressBar.value = 100;
            }
            else
            {
                progressBar.value = state.user.getProgress();
            }
            //TODO: figure out horizontal transformation that coorelateds to scaler (-11.2 = 50%?)
            difficultyMeter.value = state.user.getSettingData()[0];
            difficulty.text = difficultyMeter.value.ToString();

            //difficulty = diffucultyMeter.value;

            if (isTyping)
            {
                if (MakeWordBank.skip())
                {
                    userName.DeactivateInputField();
                    isTyping = false;
                }
                else if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
                {
                    state.user.popName();
                }
                else if (Input.anyKeyDown && letters.Contains(Input.inputString))
                {
                    if (!state.user.hasName())
                    {
                        state.user.setName("");
                    }
                    state.user.addName(Input.inputString);
                }
                
                Debug.Log("Editing Username Mode...");
            }
            //Clicking Things (buttons)
            else if (Input.GetKeyDown(KeyCode.B))
            {
                Vector3 homeDist = getScaledDist(homeButton.transform.position);
                if (homeDist.x <= 118 && homeDist.x >= 32.5 && homeDist.y <= 17.8 && homeDist.y >= -18.5)
                {
                    state.setState(1);
                }
            }
            else if (Input.GetKey(KeyCode.B)) //clicking
            {
                Vector3 sliderDist = getScaledDist(difficultyMeter.transform.position);
                Vector3 nameDist = getScaledDist(userName.transform.position);
                if (sliderDist.x <= 142.5 && sliderDist.x > -5 &&sliderDist.y <= 9 && sliderDist.y >= -12)
                {
                    //Debug.Log("Using Slider");
                    //[142.5 <--> -5] / 10 => 14.75*
                    float slideNum = Mathf.Floor((142.5f - sliderDist.x) / 14.75f);
                    difficultyMeter.value = slideNum + 1;
                    difficulty.text = difficultyMeter.value.ToString(); //change text
                    state.user.updateDifficulty(difficultyMeter.value); //change user settings
                }
                else if (nameDist.x <= 117.5 && nameDist.x >= 10.5 && nameDist.y <= 14.8 && nameDist.y >= -7.6)
                {
                    Debug.Log("Editing Name");
                    userName.ActivateInputField();
                    isTyping = true;
                }
                else
                {
                    Debug.Log("~No Button To Press~");
                    userName.DeactivateInputField();
                }
            }
            else //if not clicking
            {
                userName.DeactivateInputField();
            }

        }
    }

    private Vector3 getScaledDist(Vector3 dist) //helper
    {
        return ((dist - UserProfile.stateModifier2) - (state.getCursorPosition() * StateManager.cursorPosMod * HomeScreen.scale));
    }

}