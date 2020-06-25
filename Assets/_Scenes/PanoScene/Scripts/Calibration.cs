using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calibration : MonoBehaviour
{
    //main
    public static bool inCalibrating = false;

    //camera control
    public static GameObject mainCamera;
    public static GameObject UICamera;
    public static GameObject videoCamera;
    public static GameObject homeCamera;
    public static GameObject cursorCamera;

    //Texts
    public static Text tutorialText;
    public static GameObject helpTextContainer;
    public static GameObject helpTextPanel;
    public static Text welcomeText;


    void Awake()
    {
        mainCamera = GameObject.Find("MainCamera");
        UICamera = GameObject.Find("UICamera");
        videoCamera = GameObject.Find("VideoCamera");
        homeCamera = GameObject.Find("HomeCamera");
        cursorCamera = GameObject.Find("CursorCamera");

        tutorialText = GameObject.FindGameObjectWithTag("TutorialText").GetComponent<Text>() as Text;
        tutorialText.text = ""; //Blank for now since welcome screen must come first
        helpTextContainer = GameObject.Find("HelpTextContainer");
        helpTextContainer.SetActive(false);
        welcomeText = GameObject.FindGameObjectWithTag("WelcomeText").GetComponent<Text>() as Text;
    }

    // Update is called once per frame
    void Update()
    {
        if (inCalibrating)
        {
            //...
            inCalibrating = false;
        }
    }
}
