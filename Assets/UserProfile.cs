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
    public static GameObject[] buttons = new GameObject[3];

    public static GameObject homeButton; //main buttons
    public static GameObject userInfo;

    public static Vector3 stateModifier2 = new Vector3(-100.2f + 2.8f + 350f, 500f, 66.6f); //offset of button positions relative to makewordbank
    public static float scale = 1.72f;//3/2;

    //button colors
    public static float colorFactor = 183f / 255f;
    public static Color unhighlighted = new Color(colorFactor, colorFactor, colorFactor, 1f);
    public static Color highlighted = new Color(1f, 1f, 1f, 1f);

    void Awake()
    {
        state = GameObject.Find("Canvas").GetComponent<StateManager>();

        eventListener = GameObject.Find("Canvas").GetComponent<ClickAction>();

        homeButton = GameObject.Find("HomeButton2");
        userInfo = GameObject.Find("UserText");

        buttons[0] = homeButton;
        buttons[1] = userInfo;
        //buttons[2] = progressBar;
    }


    void Update()
    {
        if (state.getState() == 3)
        {
            float dist = 100000000; //find closest object
            GameObject obj = buttons[0];
            Vector3 newDist = new Vector3(0f,0f,0f);
            foreach (GameObject button in buttons)
            {
                if (button != null)
                {
                    newDist = (button.transform.position - stateModifier2) - (state.getCursorPosition() * StateManager.cursorPosMod * scale);
                    if (newDist.magnitude < dist)
                    {
                        dist = newDist.magnitude;
                        obj = button;
                    }
                }
            }
            Debug.Log("Closest Button: " + obj.name + ", " + ClickAction.buttonClose2(obj.transform.position) + ", " + newDist);

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
                    if (obj.name == userInfo.name)
                    {
                        //TODO: Implement editing of user profile and storing with user data
                        Debug.Log("Editing User Profile Data...");
                    }
                    else if (obj.name == homeButton.name)
                    {
                        state.setState(1);
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