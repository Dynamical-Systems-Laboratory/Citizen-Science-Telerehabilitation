using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OVRTouchSample;
using System;
using UnityEngine.UI;
using UnityEditor.Build.Content;

public class VRUser : MonoBehaviour
{
    //main stuff
    public StateManager state;

    //main controls
    public GameObject VRPerson;
    public OVRInput.Controller playerController;
    //public OVRCameraRig playerHead;
    public GameObject playerHead;
    public GameObject playerArms;
    public UserBounds playerPos;

    public static Color nothing = new Color(1, 1, 1, 0);
    public static Color selectedColor;
    public static Color highlightColor = new Color(1, 253/255, 126/255, 1);

    public static GameObject cursorPos;
    public static Vector3 controllerOffset;
    public static Color cursorHighlight = Color.green;
    public static Color cursorHighlight2 = Color.red;
    public static Color tagColor = new Color(1, 1, 1, 101 / 255);
    public static Color binColor = new Color(134 / 255, 150 / 255, 167 / 255, 186 / 255);

    public static GameObject trueCursor;

    public static GameObject centerer; //centering cursor
    public static GameObject farUp; //suplimental coordinate system for controllers
    public static GameObject farRight;
    public static GameObject farForward;

    public static bool extraControls = true;

    public static List<GameObject> interactables = new List<GameObject>();

    public static Vector3 uiButtonOffset = new Vector3(0f, 27f, 0f); //offset needed for button accuracy with uiButton methods within clickaction

    //tag sorting helpers
    public static Vector3 pos1;
    public static Vector3 pos2;
    public static Vector3 pos3;
    public static Vector3 pos4;

    /*  TODO!!
     * prefect tracking of headset and controllers
     * recreate cursor movement based on controller positions/rotations
     * rework cameras:
     * * UI camera menu stuck to headset - controlled via highlighting UI elements (thumbstick & button/trigger selection)
     * * * selection of tags indicated by tag floating along cursor sphere OR controller replaced with scaled down tag
     * * Home Screen, Video Camera, and Profile similar to UI except float in space in front of user with current image greyed out
     * * * work with survey stuff later
     * * cursor camera moved along invis sphere with r = 95% of imageSphere.r
     * * * controlled via arm movements (tbd)
     * 
     * * * cursor reset idea
     */

    //TODO: maybe fix floating feeling with flatform at user feet (make camera lower, put platform right under, set to floor lvl instead of eye lvl)
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    { //Ctrl+K+C = comment (+K+U = uncomment)
        cursorHighlight.a = 105f / 255f;
        cursorHighlight2.a = 75f / 255f;
        //player head and controllers set within Unity Scene (VRPerson's children)
        VRPerson = GameObject.Find("VRPerson");
        playerHead = GameObject.Find("CenterEyeAnchor");
        state = GameObject.Find("Canvas").GetComponent<StateManager>();
        cursorPos = GameObject.Find("CursorSphere");
        trueCursor = GameObject.Find("exampleCursor");
        playerArms = GameObject.Find("arms");
        playerPos = new UserBounds(playerHead.transform.position - new Vector3(0f,3f,0f), playerHead.transform.position);

        centerer = GameObject.Find("cursorCenter");
        farUp = GameObject.Find("headsetUp");
        farRight = GameObject.Find("headsetRight");
        farForward = GameObject.Find("headsetForward");

        interactables.Add(GameObject.Find("NextButtonPanel"));
        interactables.Add(GameObject.Find("HomeButtonPanel"));
        foreach (GameObject tag in GameObject.FindGameObjectsWithTag("Tag"))
        {
            interactables.Add(tag);
        }
        interactables.Add(GameObject.Find("Bin"));
        tagColor = interactables[2].GetComponent<Image>().color; //precausion
        binColor = interactables[6].GetComponent<Image>().color;

        pos1 = interactables[2].transform.localPosition;
        pos2 = interactables[3].transform.localPosition;
        pos3 = interactables[4].transform.localPosition;
        pos4 = interactables[5].transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        OVRInput.Update();
        OVRInput.FixedUpdate();
        //vrInfo();

        //FORCE QUIT
        //if(OVRInput.Get(OVRInput.Touch.PrimaryThumbRest, OVRInput.Controller.Touch) || OVRInput.Get(OVRInput.Touch.SecondaryThumbRest, OVRInput.Controller.Touch))
        //{
        //    state.setState(0);
        //}

        //RESETS
        if (userSkip(true) || StateManager.makeCursReset)
        {
            trueCursor.transform.position = centerer.transform.position;
            state.userControlActive = false;
            StateManager.makeCursReset = false;
        }

        if (state.isGaming())
        {
            //ui highlighting
            foreach (GameObject obj in interactables)
            {
                if (obj.tag == "Tag")
                {
                    //Debug.Log("Tag Close " + obj.name + ": " + ClickAction.tagClose(obj.transform.localPosition));
                }
                //Debug.Log(obj.name + ": " + (obj.transform.localPosition - trueCursor.transform.localPosition) + ", Mag: " + (obj.transform.localPosition - trueCursor.transform.localPosition).magnitude);
                if (obj.tag == "Tag" && ClickAction.tagClose(obj.transform.localPosition) == 1 && obj.transform.localPosition == pos1)
                {
                    obj.GetComponent<Image>().color = highlightColor;
                }
                else if (obj.tag == "Tag" && ClickAction.tagClose(obj.transform.localPosition) == 2 && obj.transform.localPosition == pos2)
                {
                    obj.GetComponent<Image>().color = highlightColor;
                }
                else if (obj.tag == "Tag" && ClickAction.tagClose(obj.transform.localPosition) == 3 && obj.transform.localPosition == pos3)
                {
                    obj.GetComponent<Image>().color = highlightColor;
                }
                else if (obj.tag == "Tag" && ClickAction.tagClose(obj.transform.localPosition) == 4 && obj.transform.localPosition == pos4)
                {
                    obj.GetComponent<Image>().color = highlightColor;
                }
                /*if (obj.tag == "Tag" && ClickAction.tagClose(obj.transform.localPosition) != 0)
                {
                    obj.GetComponent<Image>().color = highlightColor;
                }*/

                else if (obj.name == "Bin" && ClickAction.binClose(obj.transform.localPosition))
                {
                    obj.GetComponent<Image>().color = highlightColor;
                    //GameObject.Find("Trash image").GetComponent<Image>().color = highlightColor;
                }
                else if (obj.name == "NextButtonPanel" && ClickAction.uiButtonClose(obj.transform.position))
                {
                    obj.GetComponent<Image>().color = highlightColor;
                }
                else if (obj.name == "HomeButtonPanel" && ClickAction.uiButtonClose2(obj.transform.position))
                {
                    obj.GetComponent<Image>().color = highlightColor;
                }
                else
                {
                    if (obj.name == "Bin")
                    {
                        //GameObject.Find("Trash image").GetComponent<Image>().color = binColor;
                        obj.GetComponent<Image>().color = binColor;
                    }
                    else if (obj.name == "NextButtonPanel" || obj.name == "HomeButtonPanel")
                    {
                        obj.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    }
                    else if (obj.tag == "Tag")
                    {
                        obj.GetComponent<Image>().color = tagColor;
                    }
                    else
                    {
                        Debug.Log("Interactables error: " + obj.name);
                    }
                }
            }
        }

        Vector3 handPos = (OVRInput.GetLocalControllerPosition(OVRInput.Controller.RHand) + OVRInput.GetLocalControllerPosition(OVRInput.Controller.LHand)) / 2f;
        Vector3 movementVal = new Vector3(0f,0f,0f);
        /*  Reset Mechanic:
         *  Cursor starts at the center (cursorCenter) position and cannot move until...
         *  the user presses the isResetting() hand triggers and the user's head/hands pos is taken
         *  the user then can move the cursor relative to the saved vals
         *  the only exception is when the user changes states or the user presses the hand triggers
         * */
        if (isResetting())
        {
            playerArms.transform.position = handPos;
            playerPos.arms = new Vector3((farRight.transform.position - handPos).magnitude, (farUp.transform.position - handPos).magnitude, (farForward.transform.position - handPos).magnitude);
            playerPos.head = playerHead.transform.position;
            state.userControlActive = true;
            trueCursor.transform.position = centerer.transform.position; //center
        }

        if (state.userControlActive)
        {
            if (!isResetting())
            {
                controllerOffset = playerPos.arms;
                movementVal = new Vector3( (farRight.transform.position - handPos).magnitude, (farUp.transform.position - handPos).magnitude, (farForward.transform.position - handPos).magnitude );
                //movementVal += playerHead.transform.position - playerPos.head;
            }
        }
        else
        {
            controllerOffset = new Vector3(0f, 0f, 0f);
        }

        //extra control
        Vector2 cursorMove = new Vector2(0f, 0f);
        if (extraControls)
        {
            cursorMove = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Touch) + OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.Touch);
        }

        Vector3 change = controllerOffset - movementVal; // handPos - controllerOffset;
        change *= StateManager.cursorSpeed;
        if (state.cursorXMove)
        {
            cursorMove += new Vector2(change.x, 0);
        }
        if (state.cursorYMove)
        {
            cursorMove += new Vector2(0, change.y);
        }
        trueCursor.transform.position += (3f * Time.deltaTime * ((trueCursor.transform.up * cursorMove.y) + (trueCursor.transform.right * cursorMove.x)));

        //Cursor cannot move past screen borders (bondaries) -- cursor bounds  y[-151,66], x[-90,88.4]
        if (trueCursor.transform.localPosition.x > 88.4)
        {
            trueCursor.transform.localPosition = new Vector3(88.4f, trueCursor.transform.localPosition.y, trueCursor.transform.localPosition.z);
        }
        else if (trueCursor.transform.localPosition.x < -90)
        {
            trueCursor.transform.localPosition = new Vector3(-90f, trueCursor.transform.localPosition.y, trueCursor.transform.localPosition.z);
        }
        if (trueCursor.transform.localPosition.y > 66)
        {
            trueCursor.transform.localPosition = new Vector3(trueCursor.transform.localPosition.x, 66f, trueCursor.transform.localPosition.z);
        }
        else if (trueCursor.transform.localPosition.x < -151)
        {
            trueCursor.transform.localPosition = new Vector3(trueCursor.transform.localPosition.x, -151, trueCursor.transform.localPosition.z);
        }

        if (isClicking())
        {
            GameObject.Find("showClick").GetComponent<Image>().color = cursorHighlight2;
        }
        else if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) >= .2 || OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch) >= .2)
        {
            GameObject.Find("showClick").GetComponent<Image>().color = cursorHighlight;
        }
        else
        {
            GameObject.Find("showClick").GetComponent<Image>().color = nothing;
        }

        //CAMERA CONTROL & CLICKING
        switch (state.getState()) //state camera control
        {
            case 0: //QUIT
                VRPerson.SetActive(false);
                break;
            case 1: //HOME
                VRPerson.transform.position = new Vector3(10f, 0f, 0f);
                break;
            case 2: //IN-GAME
                VRPerson.transform.position = new Vector3(0f, 0f, 0f);
                break;
            case 3: //PROFILE
                VRPerson.transform.position = new Vector3(20f, 0f, 0f);
                break;
            case 4: //CALIBRATE
                VRPerson.transform.position = new Vector3(0f, 10f, 0f);
                break;
            case 5: //TUTORIAL
                VRPerson.transform.position = new Vector3(0f, 0f, 0f);
                break;
            case 6: //ABOUT PROJECT
                VRPerson.transform.position = new Vector3(-10f, 0f, 0f);
                break;
            case 7: //PRACTICE LVL
                VRPerson.transform.position = new Vector3(0f, 0f, 0f);
                break;
            case 8: //SURVEY
                VRPerson.transform.position = new Vector3(-20f, 0f, 0f);
                break;
            default:
                Debug.Log("VR State Error");
                break;
        }
        //TODO: bring back welcome panel
        
        //Debug.Log("Clicking: " + isClicking());
    }
    
    public void vrInfo()
    {
        //pos & rotation = [0,1], triggers = [0,1], sticks = [-10,10]
        Debug.Log("Player Pos: " + playerPos.arms + ", " + playerPos.head); ;
        Debug.Log("rightPos: " + OVRInput.GetLocalControllerPosition(OVRInput.Controller.RHand) + ", leftPos: " + OVRInput.GetLocalControllerPosition(OVRInput.Controller.LHand));
        Debug.Log("rightRot: " + OVRInput.GetLocalControllerRotation(OVRInput.Controller.RHand).eulerAngles + ", leftRot: " + OVRInput.GetLocalControllerRotation(OVRInput.Controller.LHand).eulerAngles);

        // returns a float of the Hand Trigger’s current state on the Left Oculus Touch controller.
        Debug.Log("LHandTrigger: " + OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch) + ", RHandTrigger: " + OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch));
        Debug.Log("LIndTrigger: " + OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch) + ", RIndTrigger: " + OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch));

        Debug.Log("LStick: " + OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Touch) + ", RStick: " + OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.Touch));
        Debug.Log("LStickP: " + OVRInput.Get(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.Touch) + ", RStickP: " + OVRInput.Get(OVRInput.Button.SecondaryThumbstick, OVRInput.Controller.Touch));
    }

    public static bool userContinue(bool isContinuous = false) //implemented for both hands
    {
        if (!isContinuous) 
        {
            return OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch) || OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch);
        }
        else
        {
            return OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch) || OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch);
        }
     }
    public static bool userSkip(bool isContinuous = false)
    {
        if (!isContinuous)
        {
            return OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.Touch) || OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick, OVRInput.Controller.Touch);
        }
        else
        {
            return OVRInput.Get(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.Touch) || OVRInput.Get(OVRInput.Button.SecondaryThumbstick, OVRInput.Controller.Touch);
        }
    }
    public static bool isClicking(bool isContinuous = false) //getbutton
    {
        /*Debug.Log("isClicking: " + (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch) >= .9 || OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) >= .9) + ", " 
            + ((OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch) < .9 && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch) >= .15) ||
                (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) < .9 && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) >= .15)));*/
        if (!isContinuous)
        {
            return OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch) >= .9 || OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) >= .9;
        }
        else
        {
            return (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch) < .9 && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch) >= .15) ||
                (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) >= .9 && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) >= .15);
        }
    }
    public static bool clickDown() //getbuttondown
    {
        if ((OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch) > .2 && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch) < .9)
            || (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) > .2 && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) < .9))
        {
            return true;
        }
        return false;
    }
    public static bool isResetting()
    {
        return (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch) > .2 && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch) < 1.9) && 
            (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) > .2 && OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) < 1.9);
    }

    public static int getStickState()
    {
        Vector2 stick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Touch) + OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.Touch);
        if (stick.x > .25 && stick.x < .8) //large range of stick vals but not fully pressed
        {
            return 2; //left
        }
        else if (stick.x < -.25 && stick.x > -.8)
        {
            return -2; //right
        }
        else if (stick.y > .25 && stick.y < .8) //large range of stick vals but not fully pressed
        {
            return 1; //up
        }
        else if (stick.y < -.25 && stick.y > -.8)
        {
            return -1; //down
        }
        
        else { return 0; }
    }

    public static int buttonConversion() //for MakeWordBank
    {
        foreach (GameObject obj in interactables)
        {
            if (obj.tag == "Tag" && ClickAction.tagClose(obj.transform.localPosition) == 1 && obj.transform.localPosition == pos1)
            {
                return 2;
            }
            else if (obj.tag == "Tag" && ClickAction.tagClose(obj.transform.localPosition) == 2 && obj.transform.localPosition == pos2)
            {
                return 3;
            }
            else if (obj.tag == "Tag" && ClickAction.tagClose(obj.transform.localPosition) == 3 && obj.transform.localPosition == pos3)
            {
                return 4;
            }
            else if (obj.tag == "Tag" && ClickAction.tagClose(obj.transform.localPosition) == 4 && obj.transform.localPosition == pos4)
            {
                return 5;
            }
            else if (obj.name == "Bin" && ClickAction.binClose(obj.transform.localPosition))
            {
                return 7;
            }
            else if (obj.name == "NextButtonPanel" && ClickAction.uiButtonClose(obj.transform.position))
            {
                return 1;
            }
            else if (obj.name == "HomeButtonPanel" && ClickAction.uiButtonClose2(obj.transform.position))
            {
                return 6;
            }
        }
        return 0;
    }
    public class UserBounds //used for later instances of compensatory motion tracking
    {
        public Vector3 head;
        public Vector3 arms;
        public UserBounds(Vector3 armsPos, Vector3 headPos)
        {
            head = headPos;
            arms = armsPos;
        }
    }
}
