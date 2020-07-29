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

    public static Color nothing = new Color(1, 1, 1, 0);
    public static Color selectedColor;
    public static Color highlightColor = new Color(1, 253/255, 126/255, 1);

    public static GameObject cursorPos;
    public static Vector3 controllerOffset;
    public static Color cursorHighlight = Color.green;
    public static Color cursorHighlight2 = Color.red;


    public static GameObject testCursor;

    public static GameObject centerer; //centering cursor
    public static GameObject farUp; //suplimental coordinate system for controllers
    public static GameObject farRight;
    public static GameObject farForward;

    public static bool extraControls = true;

    public static List<GameObject> interactables = new List<GameObject>();

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
        testCursor = GameObject.Find("exampleCursor");

        centerer = GameObject.Find("cursorCenter");
        farUp = GameObject.Find("headsetUp");
        farRight = GameObject.Find("headsetRight");
        farForward = GameObject.Find("headsetForward");

        interactables.Add(GameObject.Find("NextButtonPanel"));
        interactables.Add(GameObject.Find("HomeButtonPanel"));
        interactables.Add(GameObject.Find("Tag1"));
        interactables.Add(GameObject.Find("Tag2"));
        interactables.Add(GameObject.Find("Tag3"));
        interactables.Add(GameObject.Find("Tag4"));
        interactables.Add(GameObject.Find("Bin"));
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
        if (state.isGaming())
        {
            if (userSkip(true) || StateManager.makeCursReset)
            {
                testCursor.transform.position = centerer.transform.position;
            }

            foreach (GameObject obj in interactables)
            {
                Debug.Log(obj.name + ": " + (obj.transform.localPosition - testCursor.transform.localPosition));
                if (ClickAction.buttonClose(obj.transform.localPosition))
                {
                    obj.GetComponent<Image>().color = highlightColor;
                }
                else
                {
                    if (obj.name == "Bin")
                    {
                        obj.GetComponent<Image>().color = new Color(134 / 255, 150 / 255, 167 / 255, 186 / 255);
                    }
                    else if (obj.name == "NextButtonPanel" || obj.name == "HomeButtonPanel")
                    {
                        obj.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    }
                    else
                    {
                        obj.GetComponent<Image>().color = new Color(1, 1, 1, 100 / 255);
                    }
                }
            }
        }

        Vector3 handPos = (OVRInput.GetLocalControllerPosition(OVRInput.Controller.RHand) + OVRInput.GetLocalControllerPosition(OVRInput.Controller.LHand)) / 2f;
        Vector3 movementVal = new Vector3((farRight.transform.position - handPos).magnitude, (farUp.transform.position - handPos).magnitude, (farForward.transform.position - handPos).magnitude);

        //extra control
        Vector2 cursorMove = new Vector2(0f, 0f);
        if (extraControls)
        {
            cursorMove = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Touch) + OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.Touch);
        }

        if ((OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) >= .2 && OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) < .9)
            || (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch) >= .2 && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch) < .9))
        {
            //controllerOffset = handPos;
            controllerOffset = movementVal;
        }
        else if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) >= .9 || OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch) >= .9)
        {
            Vector3 change = controllerOffset - movementVal; // handPos - controllerOffset;
            change *= Time.deltaTime * 24f * StateManager.cursorSpeed;
            cursorMove += new Vector2(change.x, change.y);
            /*if (change.z > ...){
                //ispushing
            }*/
        }

        testCursor.transform.position += (3f * Time.deltaTime * ((testCursor.transform.up * cursorMove.y) + (testCursor.transform.right * cursorMove.x)));
        Debug.Log("Cursor Location: " + testCursor.transform.localPosition + ", Tag1: " + GameObject.Find("Tag1").transform.localPosition);
        Debug.Log("Difference: " + (testCursor.transform.localPosition - GameObject.Find("Tag1").transform.localPosition));

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

        //if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        //{
        //    state.setState(1); //home
        //}
        //else if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        //{
        //    state.setState(2); //game
        //}
        //else if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch))
        //{
        //    state.setState(3); //profile
        //}
        //else if (OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.LTouch))
        //{
        //    state.setState(2); //game
        //}
    }
    

    public static void vrInfo()
    {
        //pos & rotation = [0,1], triggers = [0,1], sticks = [-10,10]
        //Debug.Log("VRHead: " + playerHead.GetCameraPositionOrientation() + "EyePos?: " + playerHead.centerEyeAnchor.position);
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
    public static bool isClicking() //getbutton
    {
        return OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch) >= .8 || OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) >= .8;
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
    //public static void getUILocation(Transform object, Transform userPOV = playerHead.transform) //transform - gets pos and rot
    //{

    //}
}
