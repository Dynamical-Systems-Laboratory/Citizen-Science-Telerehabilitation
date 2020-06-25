using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//MonoBehavior required for unity to read (also script attatched to game object)
public class HomeScreen : MonoBehaviour
{
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
        if (MakeWordBank.inHomeScreen)
        {
            homeCamera.SetActive(true);
            mainCamera.SetActive(false);
            UICamera.SetActive(false);
            //videoCamera.SetActive(false);
        }

        //continue conditions
        canContinue = MakeWordBank.inHomeScreen;
        if (MakeWordBank.inTutorial)
        {
            canContinue = false;
        }

        Debug.Log("Home Screening: " + MakeWordBank.inHomeScreen.ToString() + ", Doing:" + canContinue.ToString());

        //main loop
        if (canContinue)
        {
            foreach (GameObject button in buttons)
            {
                if(button != null)
                {
                    Debug.Log(button.name + ": " + ((button.transform.position-stateModifier) - (state.getCursorPosition() * StateManager.cursorPosMod)) + ", " + (button.transform.position-stateModifier) + ", " + (button.transform.position - (state.getCursorPosition()*StateManager.cursorPosMod)).magnitude);
                }
            }
            //Clicking
            if (Input.GetKey(KeyCode.B))
            {
                float dist = 100000000;
                GameObject obj = buttons[0];
                foreach (GameObject button in buttons)
                {
                    if (button != null)
                    {
                        Vector3 newDist = (button.transform.position - stateModifier) - (state.getCursorPosition() * StateManager.cursorPosMod * 3/2);
                        if (newDist.magnitude < dist)
                        {
                            dist = newDist.magnitude;
                            obj = button;
                        }
                    }
                }
                Debug.Log("Closest Button: " + obj.name + ", " + ClickAction.buttonClose2(obj.transform.position));

                //if (ClickAction.buttonClose(startGameButton.transform.position))
                //{
                //    homeCamera.SetActive(false);
                //    mainCamera.SetActive(true);
                //    UICamera.SetActive(true);
                //    MakeWordBank.inHomeScreen = false;
                //}
                //else if (ClickAction.buttonClose(profileButton.transform.position))
                //{

                //}
                //else if (ClickAction.buttonClose(calibrateButton.transform.position))
                //{

                //}
                //else if (ClickAction.buttonClose(aboutButton.transform.position))
                //{

                //}
                //else if (ClickAction.buttonClose(quitButton.transform.position))
                //{
                //    QuitGameScript.TaskOnClick();
                //}
            }


        }
    }

}
