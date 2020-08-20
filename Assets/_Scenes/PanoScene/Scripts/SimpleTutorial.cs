using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Video;

public class SimpleTutorial : MonoBehaviour //for all intensive purposes can be considered Calibrate.cs
{
    private static MovementBounds userMovement = new MovementBounds();

    public StateManager state;
    public static bool inSimpleTutorial = false;
    public static GameObject mainCamera;
    public static GameObject UICamera;
    public static GameObject videoCamera;
    public static GameObject canvas;
    public static Camera cam;

    public static GameObject lockPanel;

    public static GameObject crossUp;
    public static GameObject textPanel;
    public static GameObject circle;
    public static Text text;
    public static bool circleWhite = true;

    public static bool initialized = false;

    public static int step = 0;
    public static float timer = 0f;
    public static float cameraTimer = 0f;
    public static bool cameraMove = true;
    public static bool addTimer = true;
    public static bool resetted = false;
    public static float shortInterval = 3.5f;
    public static float longInterval = shortInterval * 2f;
    public static float miniPause = 0.6f;

    public static float cursor_x = 0f;
    public static float cursor_y = 0f;
    public static float camera_x = 0f;
    public static float camera_y = 0f;

    public static int counter = 0;
    public static bool logged = false;
    public static bool moved = false;

    public static bool prevClicked = false;

    //Videoplayers
    public static GameObject VP1;
    public static GameObject VP2;
    public static GameObject VP3;
    public static GameObject VP4;
    public static GameObject VP5;

    public static UnityEngine.Video.VideoPlayer VPA1;
    public static UnityEngine.Video.VideoPlayer VPA2;
    public static UnityEngine.Video.VideoPlayer VPA3;
    public static UnityEngine.Video.VideoPlayer VPA4;
    public static UnityEngine.Video.VideoPlayer VPA5;

    public static bool startedPlaying = false;
    //public static bool calibrationEdgeCase = false;
    private static bool hasCompleted = false;

    public static GameObject vert;
    public static GameObject horiz;
    //public static Text lockText;

    public static string continueText = "press* A* or *X* to continue";

    public static GameObject cursor;
    public static GameObject cursorCam;
    //private static int beginngingCull;
    public static Vector3 savedCursorScale;

    public static Vector3 handPos1; //helper vars for hand tracking
    public static Vector3 handPos2;
    public static float[] movementAvg = new float[5];
    public static float[] timerAvg = new float[5];

    // Start is called before the first frame update
    void Start()
    {
        state = GameObject.Find("Canvas").GetComponent<StateManager>(); //state of game**
        mainCamera = GameObject.Find("Main Camera");
        UICamera = GameObject.Find("UICamera");
        videoCamera = GameObject.Find("VideoCamera");
        cam = mainCamera.GetComponent<Camera>();
        textPanel = GameObject.Find("TextPanel");
        //text = GameObject.Find("Text").GetComponent<Text>() as Text;
        text = GameObject.FindGameObjectWithTag("SimpleTutorialText").GetComponent<Text>() as Text;
        canvas = GameObject.Find("SimpleTutorialCanvas");

        circle = GameObject.Find("Circle");
        vert = GameObject.Find("Vertical"); //extra
        horiz = GameObject.Find("Horizontal");

        lockPanel = GameObject.Find("lockPanel");

        cursor = GameObject.Find("exampleCursor");
        cursorCam = GameObject.Find("CursorCamera");
        //beginngingCull = cursorCam.GetComponent<Camera>().cullingMask;

        //video players
        VP1 = GameObject.Find("VPlayer1"); //cursor unlocking
        VP2 = GameObject.Find("VPlayer2"); //cursor moving left/right
        VP3 = GameObject.Find("VPlayer3"); //cursor moving up/down
        VP4 = GameObject.Find("VPlayer4");
        VP5 = GameObject.Find("VPlayer5");
        //video player activators
        VPA1 = VP1.GetComponent<UnityEngine.Video.VideoPlayer>();
        VPA2 = VP2.GetComponent<UnityEngine.Video.VideoPlayer>();
        VPA3 = VP3.GetComponent<UnityEngine.Video.VideoPlayer>();
        VPA4 = VP4.GetComponent<UnityEngine.Video.VideoPlayer>();
        VPA5 = VP5.GetComponent<UnityEngine.Video.VideoPlayer>();
        // to play a video, activate the associated player and call activator.Play(), then ask compiler activator.isPlaying?, deactivate if not playing

        //lockText = GameObject.Find("lockText").GetComponent<Text>() as Text;
    }

    /* Simple Tutorial aka Calibration Breakdown
     * (1) lock/unluck cursor
     * (2) move cursor
     * 
     * (4) User is moved to Button Tutorial
     * (5) clicking, placing tags, trashing,
     * (6) other ui (home/next image)
     */

    // Update is called once per frame
    void Update()
    {
        if (state.getState() == 4)
        {
            Debug.Log("Calibration Step: " + step);
            //canvas.SetActive(true);
            if (!initialized)
            {
                hasCompleted = state.user.getPracticeLevelState()[1]; //finished pract lvl
                circle.SetActive(false);
                cam.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                StateManager.kinectReady = true;
                StateManager.makeCursReset = true;
                VP1.SetActive(false);
                VP2.SetActive(false);
                VP3.SetActive(false);
                VP4.SetActive(false);
                VP5.SetActive(false);
                initialized = true;
                step = 0;

                state.cursorXMove = false;
                state.cursorYMove = false;
                state.userControlActive = false;
            }

            if (MakeWordBank.skip() && hasCompleted && timer > 1)
            {
                step = 35;//changed from 35
            }
            /*else if (MakeWordBank.skip() && step == 0 && timer > 1) //for testing purposes of z movements
            {
                step = 9;
                counter = 0;
                timer = 0;
                state.cursorXMove = false;
                state.cursorYMove = false;
                VP4.SetActive(true);
                VPA4.Play();
                savedCursorScale = cursor.transform.localScale;
            }*/

            //saftey for user seeing when cursor is locked or not
            /*if (state.userControlActive && step > 0 && step < 3)
            { //StateManager.makeCursReset = false;
                lockPanel.GetComponent<Image>().color = new Color(1,1,1,220/255);
                lockText.text = "The cursor is currently in locked mode,\n center your hands and squeeze the *hand triggers* to unlock the cursor";
            }
            else
            {
                lockText.text = "";
                lockPanel.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }*/
            //Debug.Log("isLocked: " + !state.userControlActive);


            //start of steps
            //TODO: maybe go over color coding?
            if (step == 0) //introduces calibration
            {
                timer += Time.deltaTime;
                if (MakeWordBank.moveOn() && !MakeWordBank.skip())
                {
                    StateManager.makeCursReset = true;
                    text.text = "The cursor is currently in locked mode (*purple*),\n center your hands and squeeze the *hand triggers* to unlock the cursor\n"
                        + "To see a demonstration of this, " + continueText + " to a video";
                    step++;
                    timer = 0;
                }
            }
            else if (step == 1)
            {
                if (MakeWordBank.moveOn() && !MakeWordBank.skip())
                {
                    text.text = "watching video:\n Model centering their hands and unlocking the cursor";
                    counter = 0;
                    step++;
                    //add video of someone centering their hands
                    VP1.SetActive(true);
                    VPA1.Play();
                }
            }
            else if (step == 2)
            {
                if (VPA1.isPlaying)
                {
                    //cursorCam.GetComponent<Camera>().cullingMask = (1 << LayerMask.NameToLayer("Nothing")); //can also use & and | to enable/disable parts of culling mask
                    startedPlaying = true;
                }
                //timer += Time.deltaTime;
                if (!VPA1.isPlaying && startedPlaying) //if video done
                {
                    VP1.SetActive(false);
                    Debug.Log("Step 2 Counter: " + counter);
                    if (counter == 0 && !VRUser.isResetting())
                    {
                        text.text = "(1)Now try unlocking the cursor yourself, the cursor will flash *green* if youve done it correctly...\n Remeber the unlock buttons are the two hand triggers.";
                    }
                    else if (counter == 0 && VRUser.isResetting()) //just skips cuz of getdown problems but solve later...
                    {
                        counter = 1;
                        StateManager.makeCursReset = true;
                        state.userControlActive = false;
                    }
                    else if (counter == 1 && VRUser.isNotResetting())
                    {
                        text.text = "Great! Try to unlock the cursor one more time...";
                        counter = 2;
                        state.userControlActive = false;
                    }
                    else if (counter == 2 && state.userControlActive)
                    {
                        //lockPanel.transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);
                        //lockPanel.GetComponentInChildren<Text>().fontSize -= 1;
                        StateManager.makeCursReset = true;
                        state.userControlActive = false;
                        timer = 0;
                        counter = 5; //default?
                        step++;
                        startedPlaying = false;
                    }
                }
            }
            else if (step == 3) //cursor moves left
            { //Show user what movements can be done
                text.text = "Excellent! Now you'll start to move the cursor.\n Watch the video examples then try it on your own..." +
                    "\n When your ready to see the movements in action, " + continueText;
                if (MakeWordBank.moveOn() && !MakeWordBank.skip())
                {
                    text.text = "The cursor can be moved left and right...";
                    VP2.SetActive(true);
                    VPA2.Play();
                    state.userControlActive = false;
                    step++;
                }
            }
            else if (step == 4) //play left/right video
            {
                if (VPA2.isPlaying)
                {
                    //cursorCam.GetComponent<Camera>().cullingMask = (1 << LayerMask.NameToLayer("Nothing"));
                    startedPlaying = true;
                }
                if (!VPA2.isPlaying && startedPlaying)
                {
                    VP2.SetActive(false);
                    //cursorCam.GetComponent<Camera>().cullingMask = beginngingCull;

                    timer += Time.deltaTime;
                    if (timer > miniPause)
                    {
                        timer = 0f;
                        step++;
                        startedPlaying = false;
                        counter = 0;
                        state.cursorXMove = true; //unlock x axis movement
                        StateManager.makeCursReset = true;
                        state.userControlActive = false;
                    }
                }
            }
            else if (step == 5)
            {
                timer += Time.deltaTime;
                if (counter == 0)
                {
                    text.text = "(2)We'll start with the *right* direction.\n Unlock the cursor and move your hands/arms to the right as far as you can stretch without moving your body\n" +
                    "it is important to stretch at this point without moving your shoulders as it will determine your exercise progress later on";
                }
                else if (counter > 0 && counter < 5)
                {
                    text.text = "Great! Now unlock the cursor and repeat that *right* movement " + (5 - counter) + " more times.\n" +
                        "Remember to stretch as far as you can.";
                }
                else if (counter == 5)
                {
                    float avg = movementAvg[0] + movementAvg[1] + movementAvg[2] + movementAvg[3] + movementAvg[4];
                    userMovement.rangeOfMotion[1] = avg / 5;
                    avg = timerAvg[0] + timerAvg[1] + timerAvg[2] + timerAvg[3] + timerAvg[4];
                    userMovement.timeOfMotion[1] = avg / 5;
                    step++;
                    counter = 0;
                    timer = 0;
                }

                if (state.getCursorPosition().x >= 87f && state.userControlActive)
                {
                    movementAvg[counter] = (VRUser.handTracking() - handPos1).x * StateManager.cursorSpeed; //mult by cursorspeed for factoring
                    timerAvg[counter] = timer;
                    counter += 1;
                    StateManager.makeCursReset = true;
                    state.userControlActive = false;
                }

                if (!state.userControlActive && VRUser.isResetting())
                {
                    handPos1 = VRUser.handTracking();
                    timer = 0;
                }
            }
            else if (step == 6)
            {
                timer += Time.deltaTime;
                if (counter == 0)
                {
                    text.text = "(3)Now we'll try the *left* direction.\n Unlock the cursor and move your hands/arms to the left as far as you can stretch without moving your body\n" +
                    "it is important to stretch at this point without moving your shoulders as it will determine your exercise progress later on";
                }
                else if (counter > 0 && counter < 5)
                {
                    text.text = "Great! Now unlock the cursor and repeat that *left* movement " + (5 - counter) + " more times.\n" +
                        "Remember to stretch as far as you can.";
                }
                else if (counter == 5)
                {
                    float avg = movementAvg[0] + movementAvg[1] + movementAvg[2] + movementAvg[3] + movementAvg[4];
                    userMovement.rangeOfMotion[0] = avg / 5;
                    avg = timerAvg[0] + timerAvg[1] + timerAvg[2] + timerAvg[3] + timerAvg[4];
                    userMovement.timeOfMotion[0] = avg / 5;
                    step++;
                    counter = 0;
                    timer = 0;
                    state.cursorXMove = false;
                    state.cursorYMove = true;
                    VP3.SetActive(true);
                    VPA3.Play();
                }

                if (state.getCursorPosition().x <= -89f && state.userControlActive)
                {
                    movementAvg[counter] = (VRUser.handTracking() - handPos1).x * StateManager.cursorSpeed;
                    timerAvg[counter] = timer;
                    counter += 1;
                    StateManager.makeCursReset = true;
                    state.userControlActive = false;
                }

                if (!state.userControlActive && VRUser.isResetting())
                {
                    handPos1 = VRUser.handTracking();
                    timer = 0;
                }
            }
            else if (step == 7)
            {
                if (VPA3.isPlaying)
                {
                    text.text = "The cursor can be moved upward and downward...";
                    startedPlaying = true;
                }
                if (startedPlaying == true)
                {
                    timer += Time.deltaTime;
                }
                if (!VPA3.isPlaying && startedPlaying)
                {
                    VP3.SetActive(false);
                    if (counter == 0)
                    {
                        text.text = "(4)Now we'll try the *up* direction.\n Unlock the cursor and move your hands/arms up as far as you can stretch without moving your body\n" +
                        "it is important to stretch at this point without moving your shoulders as it will determine your exercise progress later on";
                    }
                    else if (counter > 0 && counter < 5)
                    {
                        text.text = "Great! Now unlock the cursor and repeat that *up* movement " + (5 - counter) + " more times.\n" +
                            "Remember to stretch as far as you can. ";
                    }
                    else if (counter == 5)
                    {
                        float avg = movementAvg[0] + movementAvg[1] + movementAvg[2] + movementAvg[3] + movementAvg[4];
                        userMovement.rangeOfMotion[3] = avg / 5;
                        avg = timerAvg[0] + timerAvg[1] + timerAvg[2] + timerAvg[3] + timerAvg[4];
                        userMovement.timeOfMotion[3] = avg / 5;
                        step++;
                        startedPlaying = false;
                        counter = 0;
                        timer = 0;
                    }

                    if (state.getCursorPosition().y >= 65f && state.userControlActive)
                    {
                        movementAvg[counter] = (VRUser.handTracking() - handPos1).y * StateManager.cursorSpeed;
                        timerAvg[counter] = timer;
                        counter += 1;
                        StateManager.makeCursReset = true;
                        state.userControlActive = false;
                    }

                    if (!state.userControlActive && VRUser.isResetting())
                    {
                        handPos1 = VRUser.handTracking();
                        timer = 0;
                    }
                }
            }
            else if (step == 8)
            {
                timer += Time.deltaTime;
                if (counter == 0)
                {
                    text.text = "(5)Now we'll try the *down* direction.\n Unlock the cursor and move your hands/arms down as far as you can stretch without moving your body\n" +
                    "it is important to stretch at this point without moving your shoulders as it will determine your exercise progress later on";
                }
                else if (counter > 0 && counter < 5)
                {
                    text.text = "Great! Now unlock the cursor and repeat that *down* movement " + (5 - counter) + " more times.\n" +
                        "Remember to stretch as far as you can.";
                }
                else if (counter == 5)
                {
                    float avg = movementAvg[0] + movementAvg[1] + movementAvg[2] + movementAvg[3] + movementAvg[4];
                    userMovement.rangeOfMotion[2] = avg / 5;
                    avg = timerAvg[0] + timerAvg[1] + timerAvg[2] + timerAvg[3] + timerAvg[4];
                    userMovement.timeOfMotion[2] = avg / 5;
                    step++;
                    counter = 0;
                    timer = 0;

                    state.cursorXMove = false;
                    state.cursorYMove = false;
                    VP4.SetActive(true);
                    VPA4.Play();
                    savedCursorScale = cursor.transform.localScale;
                }

                if (state.getCursorPosition().y <= -84f && state.userControlActive)
                {
                    movementAvg[counter] = (VRUser.handTracking() - handPos1).y * StateManager.cursorSpeed;
                    timerAvg[counter] = timer;
                    counter += 1;
                    StateManager.makeCursReset = true;
                    state.userControlActive = false;
                }

                if (!state.userControlActive && VRUser.isResetting())
                {
                    handPos1 = VRUser.handTracking();
                    timer = 0;
                }
            }
            else if (step == 9)
            {
                if (VPA4.isPlaying)
                {
                    text.text = "The cursor can be clicked";
                    startedPlaying = true;
                }
                if (startedPlaying == true)
                {
                    timer += Time.deltaTime;
                }
                if (!VPA4.isPlaying && startedPlaying)
                {
                    VP4.SetActive(false);
                    /*if (state.userControlActive)
                    {
                        cursor.transform.localScale += new Vector3((VRUser.handTracking() - handPos1).z, (VRUser.handTracking() - handPos1).z, (VRUser.handTracking() - handPos1).z)/5f;
                    }*/
                    Debug.Log("ZCounter: " + counter);
                    if (counter == 0)
                    {
                        text.text = "(6)Now we'll try the *forward* direction.\n " +
                            "Unlock the cursor and move your hands/arms forward as far as you can stretch without moving your body\n" +
                        "THEN press any of the controller buttons (A/B/X/Y) to signify your at your maximum range and relock the cursor.";
                    }
                    else if (counter > 0 && counter < 5)
                    {
                        text.text = "Great! Now unlock the cursor and repeat that *forward* movement " + (5 - counter) + " more times.\n" +
                            "The cursor should appear *red* as you do the motion... Remember to stretch as far as you can.";
                    }
                    else if (counter == 5)
                    {
                        float avg = movementAvg[0] + movementAvg[1] + movementAvg[2] + movementAvg[3] + movementAvg[4];
                        userMovement.rangeOfMotion[4] = avg / 5;
                        avg = timerAvg[0] + timerAvg[1] + timerAvg[2] + timerAvg[3] + timerAvg[4];
                        userMovement.timeOfMotion[4] = avg / 5;
                        step++;
                        startedPlaying = false;
                        counter = 0;
                        timer = 0;

                        state.cursorXMove = true;
                        state.cursorYMove = true;
                    }

                    Debug.Log("Z Offset: " + (VRUser.handTracking() - handPos1) + ", Button: " + VRUser.hasButton(true).ToString()); //VRUser.cursorRelock()
                    if (Math.Abs((VRUser.handTracking() - handPos1).z) <= VRUser.baseZCalibration/2 && VRUser.hasButton(true) && state.userControlActive)
                    {
                        text.text = "Try to extend a bit farther forward to get a good calibration...";
                    }
                    else if (Math.Abs((VRUser.handTracking() - handPos1).z) > VRUser.baseZCalibration/2 && VRUser.hasButton(true) && state.userControlActive) //VRUser.change.z
                    {
                        movementAvg[counter] = (VRUser.handTracking() - handPos1).z * StateManager.cursorSpeed;
                        timerAvg[counter] = timer;
                        counter += 1;
                        StateManager.makeCursReset = true;
                        state.userControlActive = false;
                        //cursor.transform.localScale = savedCursorScale;
                    }

                    if (!state.userControlActive && VRUser.isResetting())
                    {
                        handPos1 = VRUser.handTracking();
                        timer = 0;
                        //cursor.transform.localScale = savedCursorScale;
                    }
                }
            }
            else if (step == 10)
            {
                text.text = "Nice job! Once in-game, if you want to lock the cursor again without needing to hold the hand triggers, \n" +
                    "press either index trigger. This will prevent the cursor from moving until you unlock it again.\n" +
                    "(7) As a final step, please lock the cursor once.";
                if (VRUser.cursorRelock())
                {
                    text.text = "Tutorial Completed";
                    step++;
                }
            }
            else if (step == 11) //cursor moves right
            {
                timer += Time.deltaTime;
                if (hasCompleted)
                {
                    text.text = "Finished Tutorial... Here's your data:\n (" + string.Join(", ", userMovement.rangeOfMotion) + "),\n (" +
                    string.Join(", ", userMovement.timeOfMotion) + ")\n " + continueText + " to the next section of the tutorial...";
                }
                else
                {
                    text.text = "Great, in a second you will move onto the game tutorial. In the meanwhile, here is your data:\n(" +
                        string.Join(", ", userMovement.rangeOfMotion) + "),\n (" + string.Join(", ", userMovement.timeOfMotion) + ")";
                }
                Debug.Log("***Move Data: (" + string.Join(", ", userMovement.rangeOfMotion) + "),\n (" + string.Join(", ", userMovement.timeOfMotion) + ")");
                
                if (timer > longInterval || MakeWordBank.moveOn())
                {
                    timer = 0f;
                    step = 35;
                    Debug.Log("Adding Movement0: (" + string.Join(", ", userMovement.rangeOfMotion) + "),\n (" + string.Join(", ", userMovement.timeOfMotion) + ")");
                }
            }
            else if (step == 35) //end
            {
                //import movement data into UserInfo;
                Debug.Log("Adding Movement1: (" + string.Join(", ", userMovement.rangeOfMotion) + "),\n (" + string.Join(", ", userMovement.timeOfMotion) + ")");
                state.user.addMovementBounds(userMovement.rangeOfMotion, userMovement.timeOfMotion);
                //Ex: Adding Movement1: (28.53447, -29.49958, 15.65605, -15.51243, -8.604665), (5.484325, 5.240921, 3.642139, 4.295701, 13.32272)
                VRUser.showMoveStats = true;

                cam.rect = new Rect(0.0f, 0.0f, 0.622f, 1.0f);
                StateManager.moveCameraU = true;
                StateManager.moveCameraD = true;
                StateManager.moveCameraL = true;
                StateManager.moveCameraR = true;

                StateManager.moveCursorU = true;
                StateManager.moveCursorD = true;
                StateManager.moveCursorL = true;
                StateManager.moveCursorR = true;

                StateManager.makeCursReset = true;
                state.cursorXMove = true;
                state.cursorYMove = true;

                //MakeWordBank.inTutorial = true;
                inSimpleTutorial = false; //stops simple tutorial
                initialized = false;

                //lockPanel.SetActive(false);
                if (hasCompleted)
                {
                    state.setState(1);
                }
                else
                {
                    state.setState(5);
                    hasCompleted = true;
                }
            }
        }
    }

    float Kinect_Angle(float x1, float y1, float standard_x, float standard_y)
    { //Calculate rod rotation about the z axis
        return (Mathf.Atan2((y1 - standard_y), (x1 - standard_x))) * (180 / Mathf.PI);
    }
}
public class MovementBounds
{
    public float[] rangeOfMotion; //-x, x, -y, y bounds for user
    public float[] timeOfMotion; //the time it takes for the user to achieve max motion for each range

    public MovementBounds()
    {
        rangeOfMotion = new float[] { 0f, 0f, 0f, 0f, 0f };
        timeOfMotion = new float[] { 0f, 0f, 0f, 0f, 0f };
    }
}