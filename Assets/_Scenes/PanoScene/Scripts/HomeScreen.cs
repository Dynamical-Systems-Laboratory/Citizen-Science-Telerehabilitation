//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeScreen : MonoBehaviour
{
    //general
    public StateManager state;
    public StateManager homeState; //container

    public ClickAction eventListener;
    public ClickAction homeListener;

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

    private static bool canContinue = true;
    void Awake()
    {
        
        state = GameObject.Find("Canvas").GetComponent<StateManager>();
        homeState = GameObject.Find("HomeCanvas").GetComponent<StateManager>(); //implement

        mainCamera = GameObject.Find("Main Camera");
        UICamera = GameObject.Find("UICamera");
        homeCamera = GameObject.Find("HomeCamera"); //implement

        eventListener = GameObject.Find("Canvas").GetComponent<ClickAction>();
        homeListener = GameObject.Find("HomeCanvas").GetComponent<ClickAction>(); //implement

        tutorialText = GameObject.FindGameObjectWithTag("TutorialText").GetComponent<Text>() as Text;
        welcomeText = GameObject.FindGameObjectWithTag("WelcomeText").GetComponent<Text>() as Text; //implement

        startGameButton = GameObject.Find("StartGameButton");
        profileButton = GameObject.Find("ProfileButton");
        calibrateButton = GameObject.Find("CalibrateButton");
        tutorialButton = GameObject.Find("TutorialButton");
        aboutButton = GameObject.Find("AboutProjectButton");
        quitButton = GameObject.Find("QuitButton2");

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
            Debug.Log("Home Cursors: " + homeState.getCursorPosition() + ", original: " + state.getCursorPosition());

            foreach (GameObject button in buttons)
            {
                Debug.Log(button.name + ": " + button.transform.position + ", dist: " + (button.transform.position - homeState.getCursorPosition()).magnitude);
            }
            //Clicking
            if (Input.GetKey(KeyCode.B))
            {
                if (ClickAction.buttonClose(startGameButton.transform.position))
                {
                    homeCamera.SetActive(false);
                    mainCamera.SetActive(true);
                    UICamera.SetActive(true);
                    MakeWordBank.inHomeScreen = false;
                }
                else if (ClickAction.buttonClose(profileButton.transform.position))
                {

                }
                else if (ClickAction.buttonClose(calibrateButton.transform.position))
                {

                }
                else if (ClickAction.buttonClose(aboutButton.transform.position))
                {

                }
                else if (ClickAction.buttonClose(quitButton.transform.position))
                {
                    QuitGameScript.TaskOnClick();
                }
            }


        }
    }

}
