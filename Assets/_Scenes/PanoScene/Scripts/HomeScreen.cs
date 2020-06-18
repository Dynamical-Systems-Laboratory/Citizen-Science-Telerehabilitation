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
    public static GameObject startGameButton;
    public static GameObject profileButton;
    public static GameObject calibrateButton;
    public static GameObject aboutButton;
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
        aboutButton = GameObject.Find("AboutProjectButton");
        quitButton = GameObject.Find("QuitButton2");

    }
    void Update()
    {
        //continue conditions
        canContinue = MakeWordBank.inHomeScreen;
        if (MakeWordBank.inTutorial)
        {
            canContinue = false;
        }

        //main loop
        if (canContinue)
        {

            //Clicking
            if (Input.GetKey(KeyCode.B))
            {
                if (ClickAction.buttonClose(startGameButton.transform.position))
                {
                    homeCamera.SetActive(false);
                    mainCamera.SetActive(true);
                    UICamera.SetActive(true);
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
