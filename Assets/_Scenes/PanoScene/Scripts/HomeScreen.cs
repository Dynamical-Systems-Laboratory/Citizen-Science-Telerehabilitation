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
    public static MeshRenderer background;

    //other
    public static Text welcomeText;
    public static Text tutorialText;

    public static Vector3 stateModifier = new Vector3(-160f + 64f, 500f, 66.6f); //offset of button positions relative to makewordbank
    public static float scale = 13;

    //button colors
    public static float colorFactor = 183f / 255f;
    public static Color unhighlighted = new Color(colorFactor, colorFactor, colorFactor, 1f);
    public static Color highlighted = new Color(1f, 1f, 1f, 1f);
    
    public static Color nyuPurple = new Color(88f, 6f, 140f, 1f);

    public static bool isDisplaced = false;

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
        background = GameObject.Find("BackgroundPanel").GetComponent<MeshRenderer>() as MeshRenderer;

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
            int buttonNum = ClickAction.homeButtonClose(); //representation of button if cursor hovers over
            if (buttonNum != 0)
            {
                GameObject obj = buttons[buttonNum - 1];

                obj.GetComponent<Image>().color = VRUser.highlightColor; //highlighted;
                for (int i = 0; i < buttons.Length; i++)
                {
                    if (i != (buttonNum - 1))
                    {
                        buttons[i].GetComponent<Image>().color = unhighlighted;
                    }
                }

                if (Input.GetKey(KeyCode.B)) //clicking
                {
                    obj.GetComponent<Image>().color = unhighlighted;
                    if (obj.name == startGameButton.name)
                    {
                        StateManager.makeCamReset = true;
                        if (state.user.getPracticeLevelState()[1])
                        {
                            state.setState(2);
                        }
                        else
                        {
                            MakeWordBank.practiceLevelText.SetActive(true);
                            MakeWordBank.helpTextContainer.SetActive(true);
                            state.setState(7);
                        }
                        //TODO: reload last level & tags (read data stuff here)
                        //MakeWordBank.nextImage(state.user.getLastImage());
                        //state.reloading = true;
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
                        //MakeWordBank.focusor.SetActive(true);

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
            else
            {
                foreach (GameObject button in buttons)
                {
                    button.GetComponent<Image>().color = unhighlighted;
                }
            }
        }
    }

}
