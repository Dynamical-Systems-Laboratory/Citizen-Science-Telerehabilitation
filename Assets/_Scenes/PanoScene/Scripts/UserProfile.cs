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
    public static List<Text> texts = new List<Text>(); //list of things to update

    //other
    public static Slider progressBar;
    public static Slider difficultyMeter; //moveable slider
    public static InputField userName; //interactable input
    bool isTyping = false; //keeps track of state of input field

    //button colors
    public static float colorFactor = 183f / 255f;
    public static Color unhighlighted = new Color(colorFactor, colorFactor, colorFactor, 1f);
    public static Color highlighted = new Color(1f, 1f, 1f, 1f);

    private static string letters = "abcdefghijklmnopqrstuvwxyzACBDEFGHIJKLMNOPQRSTUVWXYZ";//allowed characters for username

    public static GameObject mainCanv;

    void Awake()
    {
        state = GameObject.Find("Canvas").GetComponent<StateManager>();
        eventListener = GameObject.Find("Canvas").GetComponent<ClickAction>();

        homeButton = GameObject.Find("HomePanel2");
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
            
            difficultyMeter.value = state.user.getSettingData()[0];
            difficulty.text = difficultyMeter.value.ToString();
            //difficulty = diffucultyMeter.value;

            if (isTyping)
            {
                if (MakeWordBank.skip()) //trigger
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
            else
            {
                int buttonNum = ClickAction.profileButtonClose();
                //Debug.Log("Profile Button: " + buttonNum);
                switch (buttonNum)
                {
                    case 1: //user name
                        ColorBlock cb1 = userName.colors;
                        cb1.normalColor = VRUser.highlightColor;
                        userName.colors = cb1;
                        break;
                    case 2: //home
                        homeButton.GetComponent<Image>().color = VRUser.highlightColor;
                        break;
                    case 3: //difficulty meter
                        ColorBlock cb2 = difficultyMeter.colors;
                        cb2.normalColor = VRUser.highlightColor;
                        difficultyMeter.colors = cb2;
                        break;
                    default: //0
                        //GameObject.Find("Placeholder").GetComponent<Text>().color = new Color(50 / 255, 50 / 255, 50 / 255, 128 / 255);
                        userName.colors = ColorBlock.defaultColorBlock;
                        homeButton.GetComponent<Image>().color = unhighlighted;
                        //GameObject.Find("Handle").GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        difficultyMeter.colors = ColorBlock.defaultColorBlock;
                        break;
                }
                //Clicking Things (buttons)
                if (Input.GetKeyDown(KeyCode.B) || state.userIsClicking)
                {
                    switch (buttonNum)
                    {
                        case 1: //name feild
                            Debug.Log("Editing Name");
                            userName.ActivateInputField();
                            isTyping = true;
                            break;
                        case 2: //home 
                            state.setState(1);
                            break;
                        case 3: //difficulty meter (TODO: make on button down)
                            //[-12.5,33] -- 45.5 len - (4.55) interval
                            int slideNum = (int)((state.getCursorPosition().x + 12.5f) / 45.5f * 10f); //percentage 10.0 = 100&
                            //Debug.Log("Slide Num: " + slideNum);
                            difficultyMeter.value = slideNum + 1;
                            difficulty.text = difficultyMeter.value.ToString(); //change text
                            state.user.updateDifficulty(difficultyMeter.value); //change user settings
                            break;
                        default:
                            Debug.Log("~No Button To Press~");
                            userName.DeactivateInputField();
                            break;
                    }
                }
                else //if not clicking
                {
                    userName.DeactivateInputField();
                }
            }
        }
    }
}