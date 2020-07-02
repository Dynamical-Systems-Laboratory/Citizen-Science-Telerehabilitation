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
    public static Vector3[] buttons = new Vector3[3];

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

        buttons[0] = homeButton.transform.position;
        buttons[1] = userInfo.Title.transform.position;
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

public class UserInfo
{
    //general
    public string userName = "User #00001";
    private string dateJoined = "mm/dd/yyyy";
    private float timeLogged;

    //data
    public void logData(int numTags, bool addImage)
    {
        tagsPlaced += numTags;
        if (addImage)
        {
            ++imagesCompleted;
        }
    }
    public void logJoin()
    {
        dateJoined = System.DateTime.Now.ToString();
    }
    public void logTime(float toAdd) //UserInfo.logTime(Time.Delta);
    {
        timeLogged += toAdd;
    }

    //progression
    private int imagesCompleted = 0;
    private int tagsPlaced = 0;
    private int sessionsLogged = 1;
    
    public float getProgress()//outputs a %/100 of progress based on user info 
    {
        return 0f;
    }

    //access
    public string getTimeLogged()
    {
        string time = "";
        time += Mathf.Floor(timeLogged / 360) + "h ";
        time += Mathf.Floor((timeLogged%360) / 60) + "m ";
        time += (timeLogged % 60) + "s";
        return time;
    }
    public int[] getProgressData()
    {
        return new int[] {imagesCompleted, tagsPlaced, sessionsLogged};
    }
}
