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

    public static Vector3 stateModifier = new Vector3(-160f + 64f, 500f, 66.6f); //offset of button positions relative to makewordbank
    public static float scale = 1.72f;//3/2;

    //button colors
    public static float colorFactor = 183f / 255f;
    public static Color unhighlighted = new Color(colorFactor, colorFactor, colorFactor, 1f);
    public static Color highlighted = new Color(1f, 1f, 1f, 1f);

    void Awake()
    {
        state = GameObject.Find("Canvas").GetComponent<StateManager>();

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
                        //homeCamera.SetActive(false);
                        if (state.user.getPracticeLevelState()[1])
                        {
                            state.setState(2);
                        }
                        else
                        {
                            state.setState(7);
                        }
                        //TODO: reload last level & tags (read data stuff here)
                    }
                    else if (obj.name == profileButton.name)
                    {
                        state.setState(3);
                    }
                    else if (obj.name == calibrateButton.name)
                    {
                        SimpleTutorial.initialized = false;
                        StateManager.makeCursReset = true;
                        state.setState(4);
                    } 
                    else if (obj.name == tutorialButton.name)
                    {
                        MakeWordBank.stepOfTutorial = 13;
                        MakeWordBank.focusor.SetActive(true);

                        for (int i = 0; i < MakeWordBank.tags.Length; i++)
                        {
                            MakeWordBank.tags[i].setText(MakeWordBank.tutorialWords[MakeWordBank.tutorialWordsIndex]);
                            MakeWordBank.tutorialWordsIndex = (MakeWordBank.tutorialWordsIndex + 1) % 14;

                        }

                        state.setState(5);
                    }
                    else if (obj.name == aboutButton.name)
                    {
                        state.setState(6);
                    }
                    else if (obj.name == quitButton.name)
                    {
                        state.setState(0);
                        //QuitGameScript.TaskOnClick();
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
