using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//MonoBehavior required for unity to read (also script attatched to game object)
public class HomeScreen : MonoBehaviour
{
    //inHomeScreen = 1

    //general
    public StateManager state;
    public ClickAction eventListener;
    public PowerpointScript slides;

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

    //button colors
    public static float colorFactor = 195f / 255f;
    public static Color unhighlighted = new Color(colorFactor, colorFactor, colorFactor, 1f);
    public static Color highlighted = new Color(1f, 1f, 1f, 1f);

    void Awake()
    {
        
        state = GameObject.Find("Canvas").GetComponent<StateManager>();

        mainCamera = GameObject.Find("Main Camera");
        UICamera = GameObject.Find("UICamera");
        homeCamera = GameObject.Find("HomeCamera");

        eventListener = GameObject.Find("Canvas").GetComponent<ClickAction>();
        //homeListener = GameObject.Find("HomeCanvas").GetComponent<ClickAction>();
        slides = GameObject.Find("Canvas").GetComponent<PowerpointScript>();

        tutorialText = GameObject.FindGameObjectWithTag("TutorialText").GetComponent<Text>() as Text;

        startGameButton = GameObject.Find("StartGamePanel");
        profileButton = GameObject.Find("ProfilePanel");
        calibrateButton = GameObject.Find("CalibratePanel");
        tutorialButton = GameObject.Find("TutorialPanel");
        aboutButton = GameObject.Find("ProjectPanel");
        quitButton = GameObject.Find("QuitPanel");

        buttons[0] = startGameButton;
        buttons[1] = profileButton;
        buttons[2] = calibrateButton;
        buttons[3] = tutorialButton;
        buttons[4] = aboutButton;
        buttons[5] = quitButton;
    }
    

    void Update()
    {
        if (state.getState() == 1)
        {
            float dist = 100000000; //find closest object
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
            //Debug.Log("Closest Button: " + obj.name + ", " + ClickAction.buttonClose2(obj.transform.position));
            if (ClickAction.buttonClose2(obj.transform.position)) //highlight color
            {
                obj.GetComponent<Image>().color = highlighted;
            }
            else
            {
                obj.GetComponent<Image>().color = unhighlighted;
            }

            if (Input.GetKey(KeyCode.B)) //clicking
            {
                if (ClickAction.buttonClose2(obj.transform.position))
                {
                    obj.GetComponent<Image>().color = unhighlighted;
                    if (obj.name == startGameButton.name)
                    {
                        StateManager.makeCamReset = true;
                        homeCamera.SetActive(false);
                        if (state.hasPracticed)
                        {
                            state.setState(2);
                        }
                        else
                        {
                            state.setState(7);
                        }
                        //reload last level & tags (read data stuff here)
                    }
                    else if (obj.name == profileButton.name)
                    {
                        //make UserProfile.cs
                        state.setState(3);
                    }
                    else if (obj.name == calibrateButton.name)
                    {
                        state.setState(4);
                    } 
                    else if (obj.name == tutorialButton.name)
                    {
                        state.setState(5);
                    }
                    else if (obj.name == aboutButton.name)
                    {
                        state.setState(6);
                        StateManager.makeCursReset = true;
                        //GameObject.FindGameObjectWithTag("Slides").SetActive(true);
                        GameObject.Find("Canvas").GetComponent<PowerpointScript>().enabled = true;
                    }
                    else if (obj.name == quitButton.name)
                    {
                        state.setState(0);
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
