using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OVRTouchSample;
using System;
using UnityEngine.UI;
using System.Runtime.Remoting.Messaging;
//using UnityEditor.Build.Content;

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
    public static Color cursorHighlight = Color.green;
    public static Color cursorHighlight2 = Color.red;
    public static Color cursorHighlight3 = Color.magenta;
    public static Color tagColor = new Color(1, 1, 1, 101 / 255);
    public static Color binColor = new Color(134 / 255, 150 / 255, 167 / 255, 186 / 255);
    public static Color binColor2 = new Color(167 / 255, 134 / 255, 143 / 255, 186 / 255);
    public static Color binUnhighlight = new Color(167 / 255, 167 / 255, 167 / 255, 167 / 255);

    public static GameObject trueCursor;

    public static GameObject centerer; //centering cursor
    public static GameObject farUp; //suplimental coordinate system for controllers
    public static GameObject farRight;
    public static GameObject farForward;

    public static bool extraControls = true; //for keyboard controls and other developer stuff

    public static List<GameObject> interactables = new List<GameObject>();
    public static Vector3 uiButtonOffset = new Vector3(0f, 27f, 0f); //offset needed for button accuracy with uiButton methods within clickaction

    public static Vector3 change; //modified change of controller movement

    public static bool showMoveStats = false; //bool to debug.log calibration stats

    public static float moveThreshold1 = 0.09f; //percentages for (1)reading movement & (2)displaying movement (+haptics)
    public static float moveThreshold2 = 0.70f;
    public static float moveThreshold3;

    public static float baseZCalibration = 1f; //var that signifies how far the user is supposed to reach (z) given no calibration data

    public float totalTime = 0f;

    public static float controllerVibration = 0f;
    /*  TODO!!
     * rework cameras:
     * * * work with survey stuff later
     * * cursor camera moved along invis sphere with r = 95% of imageSphere.r
     */

    //TODO: maybe fix floating feeling with flatform at user feet (make camera lower, put platform right under, set to floor lvl instead of eye lvl)
    // Start is called before the first frame update
    private void Awake()
    { //Ctrl+K+C = comment (+K+U = uncomment)
        cursorHighlight.a = 105f / 255f;
        cursorHighlight2.a = 75f / 255f;
        //cursorHighlight3 = HomeScreen.nyuPurple;
        cursorHighlight3.a = 80f / 255f;
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

        moveThreshold1 += 0.025f * (state.user.getSettingData()[0] - 5); //mod by difficulty
        moveThreshold2 += 0.02f * (state.user.getSettingData()[0] - 5);
        if (moveThreshold1 < 0) { moveThreshold1 = 0; }
        if (moveThreshold2 < 0) { moveThreshold2 = 0; }
        moveThreshold3 = (moveThreshold1  + moveThreshold2) / 2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (state.getState() != 0)
        {
            OVRInput.Update();
            OVRInput.FixedUpdate();

            totalTime += Time.deltaTime;
            //Debug.Log("Total Elapsed Time: " + totalTime + ", System: " + System.DateTime.Now.ToString("hh:mm:ss"));
            state.user.addMovement(totalTime, System.DateTime.Now.ToString("hh:mm:ss"), playerHead.transform,
                OVRInput.GetLocalControllerPosition(OVRInput.Controller.RHand), OVRInput.GetLocalControllerRotation(OVRInput.Controller.RHand).eulerAngles,
                OVRInput.GetLocalControllerPosition(OVRInput.Controller.RHand), OVRInput.GetLocalControllerRotation(OVRInput.Controller.RHand).eulerAngles);
            state.user.moveDataConfirm();

            //vrInfo();
            state.user.showMoveBounds(); //Move Data Ex: (29.0044, -29.00861, 13.51058, -14.32116, 0), (3.651338, 6.311625, 4.176139, 7.143209, 0)
            Debug.Log("Hand Tracking: " + handTracking(true) + ", Offset By: " + playerPos.arms + ", unfactored: " + handTracking(false));
            Debug.Log("Cont. Clicking: " + state.userIsClicking + ", Clicking: " + state.userClick); //continuous, noncontinuous clicking

            /* movement correction ideas:
             * correct local rotation by normal rotation
             * transform positions/orientations of parent relative to headset roll
             */
            //arms position editing
            //armsFixes(); //testing
            //playerArms.transform.localRotation = Quaternion.Euler( 0f, 0f, -playerHead.transform.rotation.z); //override parent transform rotation?
            //GameObject.Find("headsetForward").transform.position = new Vector3(GameObject.Find("headsetForward").transform.position.x, 0f, GameObject.Find("headsetForward").transform.position.z);
            //GameObject.Find("headsetRight").transform.position -= new Vector3(GameObject.Find("headsetRight").transform.position.x, 0f, GameObject.Find("headsetRight").transform.position.z);
            //GameObject.Find("headsetUp").transform.position -= new Vector3(0f, GameObject.Find("headsetUp").transform.position.y, 0f);

            controllerVibration = 0f;

            //FORCE QUIT
            if (Input.GetKey(KeyCode.Q))
            {
                state.setState(0);
            }

            //UI HIGHLIGHTING
            if (state.isGaming())
            {
                int converted = buttonConversion();
                switch (converted)
                {
                    case 0:
                        interactables[0].GetComponent<Image>().color = new Color(1, 1, 1, 1); //next
                        interactables[1].GetComponent<Image>().color = new Color(1, 1, 1, 1); //home
                        interactables[2].GetComponent<Image>().color = tagColor; //tags
                        interactables[3].GetComponent<Image>().color = tagColor;
                        interactables[4].GetComponent<Image>().color = tagColor;
                        interactables[5].GetComponent<Image>().color = tagColor;
                        //GameObject.Find("Trash image").GetComponent<Image>().color = binUnhighlight; //trash
                        interactables[6].GetComponent<Image>().color = binColor;
                        break;
                    case 1:
                        interactables[0].GetComponent<Image>().color = highlightColor;
                        break;
                    case 6:
                        interactables[1].GetComponent<Image>().color = highlightColor;
                        break;
                    case 7:
                        //GameObject.Find("Trash image").GetComponent<Image>().color = binColor2;
                        interactables[6].GetComponent<Image>().color = highlightColor;
                        break;
                    default:
                        //tags (converted 2-5)
                        interactables[converted].GetComponent<Image>().color = highlightColor;
                        break;
                }
            }

            /*  RESET Mechanic:
             *  Cursor starts at the center (cursorCenter) position and cannot move until...
             *  the user presses the isResetting() hand triggers and the user's head/hands pos is taken
             *  the user then can move the cursor relative to the saved vals
             *  the only exception is when the user changes states or the user presses the hand triggers
             * */
            if (cursorRelock() || StateManager.makeCursReset)
            {
                trueCursor.transform.position = centerer.transform.position;
                state.userControlActive = false;
                StateManager.makeCursReset = false;
                ClickAction.dropObject();
                controllerVibration += .4f;
            }
            if (isResetting()) //user resets cursor via hand triggers
            {
                //sets arm bounds to location of hands
                //playerArms.transform.position = handPos; 
                playerArms.transform.position = handTracking(false);

                //stores coords of arms relative to headset bounds
                //playerPos.arms = new Vector3((farRight.transform.position - handPos).magnitude, (farUp.transform.position - handPos).magnitude, (farForward.transform.position - handPos).magnitude);
                playerPos.arms = handTracking(true);

                //stores present position of head
                playerPos.head = playerHead.transform.position;
                //activates user control
                state.userControlActive = true;
                //resets position of cursor to the center of the user's vision
                trueCursor.transform.position = centerer.transform.position;
                controllerVibration += .25f; //feedback to tell user is being reset
                state.userIsClicking = false;
                state.userClick = false;
            }

            //EXTRA (thumbstick) cursor control
            Vector2 cursorMove = new Vector2(0f, 0f); //var to add stick positional stuff to cursor movement
            if (extraControls)
            {
                //add stick controls
                cursorMove = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Touch) + OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.Touch);
            }

            //adding hand controls
            if (state.userControlActive && !isResetting())
            {
                change = playerPos.arms - handTracking(true);
                change -= playerPos.head - playerHead.transform.position; //compensatory movement
                Debug.Log("Compensatory Offset: " + (playerPos.head - playerHead.transform.position));
            }
            else
            {
                change = new Vector3(0f, 0f, 0f);
            }
            Debug.Log("Move Change: " + change + ", Threshold: " + state.user.formattedMoveBounds() + ", " + state.user.formattedMoveBounds(moveThreshold1));
            Debug.Log("Thresholds(1/2/3): " + moveThreshold1 + ", " + moveThreshold2 + ", " + moveThreshold3);

            //set add controller stuff to cursorMove
            if (state.user.getPracticeLevelState()[0]) //has started practice level, aka has finished calibration
            { //Note: with the factored handpos, all axis' are reversed* (signs changed)
                //lowerbound threshold - controls when the cursor can move based on a % of the calibration values (x&y)
                if (state.cursorXMove && change.x < (state.user.getMovementBounds(1) * moveThreshold1)) //(-x,x,-y,y,z) (1,2,3,4,5)
                {
                    cursorMove += new Vector2(change.x - (state.user.getMovementBounds(1) * moveThreshold1 / 2f), 0);
                    //cursorMove += new Vector2(change.x, 0);
                    Debug.Log("MoveChange:Left");
                }
                else if (state.cursorXMove && change.x > (state.user.getMovementBounds(2) * moveThreshold1))
                {
                    cursorMove += new Vector2(change.x - (state.user.getMovementBounds(2) * moveThreshold1 / 2f), 0);
                    //cursorMove += new Vector2(change.x, 0);
                    Debug.Log("MoveChange:Right");
                }
                else
                {
                    Debug.Log("MoveChange:XNothing");
                }

                if (state.cursorYMove && change.y < (state.user.getMovementBounds(3) * moveThreshold1))
                {
                    cursorMove += new Vector2(0, change.y - (state.user.getMovementBounds(3) * moveThreshold1 / 2f));
                    //cursorMove += new Vector2(0, change.y);
                    Debug.Log("MoveChange:Down");
                }
                else if (state.cursorYMove && change.y > (state.user.getMovementBounds(4) * moveThreshold1))
                {
                    cursorMove += new Vector2(0, change.y - (state.user.getMovementBounds(4) * moveThreshold1 / 2f));
                    //cursorMove += new Vector2(0, change.y);
                    Debug.Log("MoveChange:Up");
                }
                else
                {
                    Debug.Log("MoveChange:YNothing");
                }

                //upperbound haptics - tells user when they are close to their max range xy&z
                float addHapt = 0f;
                if (state.cursorXMove && change.x < (state.user.getMovementBounds(1) * moveThreshold2)) //x
                {
                    addHapt += (change.x / (state.user.getMovementBounds(1) * moveThreshold2)) / 2;
                }
                else if (change.x > (state.user.getMovementBounds(2) * moveThreshold2))
                {
                    addHapt += (change.x / (state.user.getMovementBounds(2) * moveThreshold2)) / 2;
                }
                if (state.cursorYMove && change.y < (state.user.getMovementBounds(3) * moveThreshold2)) //y
                {
                    addHapt += (change.y / (state.user.getMovementBounds(3) * moveThreshold2)) / 2;
                }
                else if (change.y > (state.user.getMovementBounds(4) * moveThreshold2))
                {
                    addHapt += (change.y / (state.user.getMovementBounds(5) * moveThreshold2)) / 2;
                }
                if (change.z > (state.user.getMovementBounds(5) * moveThreshold2)) //z
                {
                    addHapt += (change.z / (state.user.getMovementBounds(5) * moveThreshold2)) / 2;
                }
                Debug.Log("MovementBounds Haptic Adding: " + addHapt);
                controllerVibration += addHapt;

                //clicking - click if user is a certain % of their max z range
                Debug.Log("Added Z Stuff: " + change.z + " vs. " + (state.user.getMovementBounds(5) * moveThreshold3));
                if ((change.z - (state.user.getMovementBounds(5) * moveThreshold3)) > 0) //TODO divide bounds by factor so user isnt always expected to go to their full range of motion
                {
                    state.userClick = true;
                    state.userIsClicking = true;
                }
                else if (change.z > state.user.getMovementBounds(5) * moveThreshold3)
                {
                    state.userIsClicking = true;
                    state.userClick = false;
                }
                else
                {
                    state.userIsClicking = false;
                    state.userClick = false;
                }
            }
            else //if no calibration data (assuming we're in calibration step)
            {
                if (state.cursorXMove) //give user access to x&y movements if these booleans say so
                {
                    cursorMove += new Vector2(change.x, 0);
                }
                if (state.cursorYMove)
                {
                    cursorMove += new Vector2(0, change.y);
                }

                //clicking - same clicking methodology but based on an easy-to-reach position instead of calibrated data
                if ((Math.Floor(change.z) - (baseZCalibration * moveThreshold3)) > 0)
                {
                    state.userClick = true;
                    state.userIsClicking = true;
                }
                else if (change.z > baseZCalibration * moveThreshold3)
                {
                    state.userIsClicking = true;
                    state.userClick = false;
                }
                else
                {
                    state.userIsClicking = false;
                    state.userClick = false;
                }
            }
            //failsafe for clicking
            if (extraControls)
            {
                if (userStickButton(false))
                {
                    state.userClick = true;
                }
                if (userStickButton(true))
                {
                    state.userIsClicking = true;
                }
            }

            //adds hand position stuff to cursor movements
            cursorMove += new Vector2(state.cursorAdd.x, state.cursorAdd.y);

            state.cursorAdd = new Vector3(0f, 0f, 0f); //resetting additive property

            //moves cursor by factor of all the above*****
            trueCursor.transform.position += ((1.4f + ((5 - state.user.getSettingData()[0]) / 10f)) * 1.7f * Time.deltaTime * ((trueCursor.transform.up * cursorMove.y * 1.25f) + (trueCursor.transform.right * cursorMove.x)));

            //Cursor cannot move past screen borders (bondaries) -- cursor bounds  y[-151,66], x[-90,88.4]
            if (trueCursor.transform.localPosition.x > 88)
            {
                trueCursor.transform.localPosition = new Vector3(88f, trueCursor.transform.localPosition.y, trueCursor.transform.localPosition.z);
            }
            else if (trueCursor.transform.localPosition.x < -90)
            {
                trueCursor.transform.localPosition = new Vector3(-90f, trueCursor.transform.localPosition.y, trueCursor.transform.localPosition.z);
            }
            if (trueCursor.transform.localPosition.y > 66)
            {
                trueCursor.transform.localPosition = new Vector3(trueCursor.transform.localPosition.x, 66f, trueCursor.transform.localPosition.z);
            }
            else if (trueCursor.transform.localPosition.y < -85)
            {
                trueCursor.transform.localPosition = new Vector3(trueCursor.transform.localPosition.x, -150, trueCursor.transform.localPosition.z);
            }

            //highlights the cursor based on certain actions
            if (isResetting(true) || isResetting(false)) //green = resetting
            {
                GameObject.Find("showClick").GetComponent<Image>().color = cursorHighlight;
            }
            else if (!state.userControlActive) //purple = locked
            {
                GameObject.Find("showClick").GetComponent<Image>().color = cursorHighlight3;
            }
            else if ((state.userIsClicking || state.userClick) && state.getState() != 5 && state.getState() != 6) //red = clicking
            {
                //Debug.Log("Red Clicking should be shown");
                GameObject.Find("showClick").GetComponent<Image>().color = cursorHighlight2;
            }
            else
            {
                GameObject.Find("showClick").GetComponent<Image>().color = nothing;
            }

            //extra haptics with thumbsticks
            float scaledVal = Math.Abs(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Touch).y + OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.Touch).y +
                                       OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Touch).x + OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.Touch).x) / 4f;
            //Debug.Log("scaled val: " + scaledVal);
            controllerVibration += scaledVal * 0.8f;

            //CAMERA CONTROL & CLICKING
            switch (state.getState()) //state camera control (positions of camera at various states)
            {
                case 0: //QUIT
                    VRPerson.SetActive(false);
                    break;
                case 1: //HOME
                    //VRPerson.transform.position = new Vector3(10f, 0f, 0f);
                    VRPerson.transform.position = new Vector3(0f, 0f, 0f);
                    break;
                case 2: //IN-GAME
                    VRPerson.transform.position = new Vector3(0f, 0f, 0f);
                    break;
                case 3: //PROFILE
                    //VRPerson.transform.position = new Vector3(20f, 0f, 0f);
                    VRPerson.transform.position = new Vector3(0f, 0f, 0f);
                    break;
                case 4: //CALIBRATE
                    //VRPerson.transform.position = new Vector3(0f, 10f, 0f);
                    VRPerson.transform.position = new Vector3(0f, 0f, 0f);
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
                    VRPerson.transform.position = new Vector3(-20f, 0f, 0f); //TODO: edit for diff survey canvas later
                    break;
                default:
                    Debug.Log("VR State Error");
                    break;
            }
            //raycasting attempts on hold for now...
            /*if (state.getSelected() != null)
            {
                Debug.Log("*Raycast starting up...");
                Ray cursorRay = new Ray(state.getCursorPosition(), (state.getCursorPosition() - playerHead.transform.position).normalized);
                RaycastHit[] hits = Physics.RaycastAll(cursorRay, (state.getCursorPosition() - playerHead.transform.position).magnitude);
                foreach (RaycastHit hit in hits)
                {
                    Debug.Log("**" + hit.collider.gameObject.name + ": " + hit.collider.gameObject.transform.position);
                    //RaycastHit hit = new RaycastHit();
                    if (Physics.Raycast(state.getCursorPosition(), (state.getCursorPosition() - playerHead.transform.position).normalized))
                    {
                        //Instantiate(state.getSelected(), hit.point, Quaternion.identity);
                        Debug.Log("*** raycast working???");
                    }
                }
            }*/

            //Set Haptics
            OVRInput.SetControllerVibration(controllerVibration, controllerVibration, OVRInput.Controller.RTouch); //set haptics
            OVRInput.SetControllerVibration(controllerVibration, controllerVibration, OVRInput.Controller.LTouch);
        }
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
        if (showMoveStats)
        {
            state.user.showMoveBounds();
        }
    }

    public static bool userContinue(bool isContinuous = false) //implemented for both hands
    {
        if (OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.RTouch) || OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.LTouch))
        {
            return false;
        }

        if (!isContinuous) 
        {
            return OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch) || OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch);
        }
        else
        {
            return OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch) || OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.LTouch);
        }
     }
    public static bool userSkip(bool isContinuous = false)
    {
        if (isContinuous)
        {
            return OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch) >= .99 || OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) >= .99;
        }
        else
        {
            return (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch) < .9 && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch) >= .15) ||
                (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) >= .9 && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) >= .15);
        }
    }
    public static bool userStickButton(bool isContinuous = false)
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
    public static bool cursorRelock(bool isContinuous = true)
    {
        /*if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch) && OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            return true;
        }
        else if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.LTouch) && OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.LTouch))
        {
            return true;
        }
        return false;*/
        return userSkip(isContinuous);
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
    public static bool isResetting(bool isContinuous = false)
    {
        if (!isContinuous)
        {
            return (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch) > .2 && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch) < 1.9) ||
            (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) > .2 && OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) < 1.9);
        }
        else
        {
            return (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch) > 1.9) ||
            (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) > 1.9);
        }
    }
    public static bool hasButton(bool isContinuous = false)
    {
        if (!isContinuous)
        {
            return OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch) || OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch)
                || OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch) || OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch);
        }
        else
        {
            return OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch) || OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.LTouch)
                || OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.RTouch) || OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.LTouch);
        }
    }
    public static bool isNotResetting()
    {
        return (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch) == 0 && OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) == 0);
    }

    public static Vector3 handTracking(bool factored = true)
    {
        if (!factored)
        {
            return (OVRInput.GetLocalControllerPosition(OVRInput.Controller.RHand) + OVRInput.GetLocalControllerPosition(OVRInput.Controller.LHand)) / 2f;
        }
        else //returns hand pos value factored for headset
        {
            Vector3 hand = (OVRInput.GetLocalControllerPosition(OVRInput.Controller.RHand) + OVRInput.GetLocalControllerPosition(OVRInput.Controller.LHand)) / 2f;
            Vector3 move = new Vector3((farRight.transform.position - hand).magnitude, (farUp.transform.position - hand).magnitude, (farForward.transform.position - hand).magnitude);
            return move * StateManager.cursorSpeed; //getSettingData()[2]
        }
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
        int i = ClickAction.tagClose();
        if (i != 0)
        {
            return (i + 1); //2-5
        }
        else if (ClickAction.uiButtonClose())
        {
            return 1;
        }
        else if (ClickAction.uiButtonClose2())
        {
            return 6;
        }
        else if (ClickAction.binClose())
        {
            return 7;
        }
        return 0;
    }

    public static void armsFixes()
    {
        GameObject.Find("headsetUp").transform.localPosition -= new Vector3(0f, GameObject.Find("headsetUp").transform.localPosition.y - 3.6f, 0f); //constant 3.6 y
        GameObject.Find("headsetRight").transform.localPosition -= new Vector3(0f, GameObject.Find("headsetRight").transform.localPosition.y, 0f); // constant 0 y
        GameObject.Find("headsetForward").transform.localPosition -= new Vector3(0f, GameObject.Find("headsetForward").transform.localPosition.y, 0f); // constant 0 y
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
