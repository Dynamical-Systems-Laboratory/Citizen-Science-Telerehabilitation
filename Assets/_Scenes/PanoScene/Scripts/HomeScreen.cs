using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//MonoBehavior required for unity to read (also script attatched to game object)
public class HomeScreen : MonoBehaviour
{
    //main
    public static bool inHomeScreen;

    //general
    public StateManager state;
    public ClickAction eventListener;

    //cameras
    public static GameObject mainCamera;
    public static GameObject UICamera;
    public static GameObject homeCamera;

    //buttons
    public static GameObject[] buttons = new GameObject[6];

    public static GameObject startGameButton; //main buttons
    public static GameObject profileButton;
    public static GameObject calibrateButton;
    public static GameObject tutorialButton;
    public static GameObject aboutButton; //side buttons
    public static GameObject quitButton;

    //other
    public static Text welcomeText;
    public static Text tutorialText;

    private static bool canContinue = false;

    public static Vector3 stateModifier = new Vector3(-100.2f + 2.8f, 500f, 66.6f); //offset of button positions relative to makewordbank
    public static float scale = 1.72f;//3/2;
    void Awake()
    {
        
        state = GameObject.Find("Canvas").GetComponent<StateManager>();

        mainCamera = GameObject.Find("MainCamera");
        UICamera = GameObject.Find("UICamera");
        homeCamera = GameObject.Find("HomeCamera");

        eventListener = GameObject.Find("Canvas").GetComponent<ClickAction>();
        //homeListener = GameObject.Find("HomeCanvas").GetComponent<ClickAction>();

        tutorialText = GameObject.FindGameObjectWithTag("TutorialText").GetComponent<Text>() as Text;

        startGameButton = GameObject.Find("StartGameButton");
        profileButton = GameObject.Find("ProfileButton");
        calibrateButton = GameObject.Find("CalibrateButton");
        tutorialButton = GameObject.Find("TutorialButton");
        aboutButton = GameObject.Find("AboutProjectButton");
        quitButton = GameObject.Find("QuitButton");

        buttons[0] = startGameButton;
        buttons[1] = profileButton;
        buttons[2] = calibrateButton;
        buttons[3] = tutorialButton;
        buttons[4] = aboutButton;
        buttons[5] = quitButton;
    }
    

    void Update()
    {
        //making sure it stays open (uniturrupted)
        if (inHomeScreen)
        {
            homeCamera.SetActive(true);
            mainCamera.SetActive(false);
            UICamera.SetActive(false);
            //videoCamera.SetActive(false);
        }

        //continue conditions
        canContinue = inHomeScreen;
        if (MakeWordBank.inTutorial)
        {
            canContinue = false;
        }

        Debug.Log("Screens Home(" + inHomeScreen.ToString() + ") ProjectInfo(" + PowerpointScript.inSlides + ") Calibrate(" + Calibration.inCalibrating + ")");
        //Debug.Log("Scaled Cursor:" + (state.getCursorPosition() * StateManager.cursorPosMod * scale));
        //main loop
        if (canContinue)
        {
            //foreach (GameObject button in buttons)
            //{
            //    if(button != null)
            //    {
            //        Debug.Log(button.name + ": " + ((button.transform.position - stateModifier) - (state.getCursorPosition() * StateManager.cursorPosMod * scale)) + ", " + (button.transform.position - stateModifier));
            //    }
            //}
            //Clicking
            if (Input.GetKey(KeyCode.B))
            {
                float dist = 100000000;
                GameObject obj = buttons[0];
                foreach (GameObject button in buttons)
                {
                    if (button != null)
                    {
                        Vector3 newDist = (button.transform.position - stateModifier) - (state.getCursorPosition() * StateManager.cursorPosMod * scale);
                        if (newDist.magnitude < dist)
                        {
                            dist = newDist.magnitude;
                            obj = button;
                        }
                    }
                }
                Debug.Log("Closest Button: " + obj.name + ", " + ClickAction.buttonClose2(obj.transform.position));

                if (ClickAction.buttonClose2(obj.transform.position))
                {
                    if (obj.name == startGameButton.name)
                    {
                        homeCamera.SetActive(false);
                        mainCamera.SetActive(true);
                        UICamera.SetActive(true);
                        inHomeScreen = false;
                        //reload last level (read data stuff here)
                    }
                    else if (obj.name == profileButton.name)
                    {
                        //make UserProfile.cs
                    }
                    else if (obj.name == calibrateButton.name)
                    {
                        Calibration.inCalibrating = true;
                    }
                    else if (obj.name == tutorialButton.name)
                    {

                    }
                    else if (obj.name == aboutButton.name)
                    {
                        homeCamera.SetActive(false);
                        PowerpointScript.inSlides = true;
                    }
                    else if (obj.name == quitButton.name)
                    {
                        QuitGameScript.TaskOnClick();
                    }
                    else
                    {
                        Debug.Log("Unknown Gameobject Button Found: " + obj.name);
                    }
                }
            }


        }
    }

}
