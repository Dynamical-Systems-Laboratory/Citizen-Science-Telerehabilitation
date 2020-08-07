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
    public MovementData UserMovement = new MovementData();

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
    public static float shortInterval = 2.5f;
    public static float longInterval = shortInterval * 2f;
    public static float miniPause = 0.6f;

    public static float cursor_x = 0f;
    public static float cursor_y = 0f;
    public static float camera_x = 0f;
    public static float camera_y = 0f;

    public static int counter = 5;

    public static float maxLDistance = 0f;
    public static float maxRDistance = 0f;
    public static float LHandLeftTotal = 0f;
    public static float LHandLeftAverage = -0.275f;
    public static float RHandLeftTotal = 0f;
    public static float RHandLeftAverage = -0.225f;
    public static float LHandRightTotal = 0f;
    public static float LHandRightAverage = 0.225f;
    public static float RHandRightTotal = 0f;
    public static float RHandRightAverage = 0.275f;
    public static float LHandUpTotal = 0f;
    public static float LHandUpAverage = 0.225f;
    public static float RHandUpTotal = 0f;
    public static float RHandUpAverage = 0.275f;
    public static float LHandDownTotal = 0f;
    public static float LHandDownAverage = -0.225f;
    public static float RHandDownTotal = 0f;
    public static float RHandDownAverage = -0.275f;

    public static float maxAngle = 0f;
    public static float angleLeftTotal = 0f;
    public static float angleLeftAverage = 75f;
    public static float angleRightTotal = 0f;
    public static float angleRightAverage = 60f;

    public static float maxSpeed = 0f;
    public static float speedUpTotal = 0f;
    public static float speedUpAverage = 50f;
    public static float speedDownTotal = 0f;
    public static float speedDownAverage = -50f;

    public static float LHSmin = 0f;
    public static float LHSmax = 0f;
    public static float RHSmin = 0f;
    public static float RHSmax = 0f;
    public static float LESmin = 0f;
    public static float LESmax = 0f;
    public static float RESmin = 0f;
    public static float RESmax = 0f;

    public static float LHandSpeedForwardTotal = 0f;
    public static float LHandSpeedForwardAverage = 0f;
    public static float RHandSpeedForwardTotal = 0f;
    public static float RHandSpeedForwardAverage = 0f;
    public static float LElbowSpeedForwardTotal = 0f;
    public static float LElbowSpeedForwardAverage = 0f;
    public static float RElbowSpeedForwardTotal = 0f;
    public static float RElbowSpeedForwardAverage = 0f;
    public static float LHandSpeedBackwardTotal = 0f;
    public static float LHandSpeedBackwardAverage = 0f;
    public static float RHandSpeedBackwardTotal = 0f;
    public static float RHandSpeedBackwardAverage = 0f;
    public static float LElbowSpeedBackwardTotal = 0f;
    public static float LElbowSpeedBackwardAverage = 0f;
    public static float RElbowSpeedBackwardTotal = 0f;
    public static float RElbowSpeedBackwardAverage = 0f;

    public static float prevCameraAngle = 0f;
    public static bool logged = false;
    public static bool moved = false;

    public static bool prevClicked = false;

    //Videoplayers
    public static GameObject VP1;
    public static GameObject VP2;
    public static GameObject VP3;
    public static GameObject VP4;
    public static GameObject VP5;

    public static UnityEngine.Video.VideoPlayer cameraLRVP;
    public static UnityEngine.Video.VideoPlayer cameraUDVP;
    public static UnityEngine.Video.VideoPlayer cursorLRVP;
    public static UnityEngine.Video.VideoPlayer cursorUDVP;
    public static UnityEngine.Video.VideoPlayer clickVP;

    public static bool startedPlaying = false;
    //public static bool calibrationEdgeCase = false;
    private static bool hasCompleted = false;

    public static GameObject vert;
    public static GameObject horiz;

    public static Text lockText;

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

        VP1 = GameObject.Find("VPlayer1");
        VP2 = GameObject.Find("VPlayer2");
        VP3 = GameObject.Find("VPlayer3");
        VP4 = GameObject.Find("VPlayer4");
        VP5 = GameObject.Find("VPlayer5");

        cameraLRVP = VP1.GetComponent<UnityEngine.Video.VideoPlayer>();
        cameraUDVP = VP2.GetComponent<UnityEngine.Video.VideoPlayer>();
        cursorLRVP = VP3.GetComponent<UnityEngine.Video.VideoPlayer>();
        cursorUDVP = VP4.GetComponent<UnityEngine.Video.VideoPlayer>();
        clickVP = VP5.GetComponent<UnityEngine.Video.VideoPlayer>();

        lockText = GameObject.Find("lockText").GetComponent<Text>() as Text;
    }

    /* Simple Tutorial aka Calibration Breakdown
     * (1) lock/unluck cursor
     * (2) move cursor
     * 
     * (4) User is moved to Button Tutorial
     * (5) clicking, placing tags, trashing,
     * (6) other ui (home/next image)
     * 
     */

    // Update is called once per frame
    void Update()
    {
        if (state.getState() == 4)
        {
            //canvas.SetActive(true);
            if (!initialized)
            {
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

            if (MakeWordBank.skip())
            {
                step = 35;//changed from 35
            }

            //saftey for user seeing when cursor is locked or not
            if (state.userControlActive && step > 0)
            { //StateManager.makeCursReset = false;
                lockPanel.GetComponent<Image>().color = new Color(1,1,1,220/255);
                lockText.text = "The cursor is currently in locked mode,\n center your hands and squeeze the *hand triggers* to unlock the cursor";
            }
            else
            {
                lockText.text = "";
                lockPanel.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }

            Debug.Log("isLocked: " + !state.userControlActive);

            //start of steps
            if (step == 0) //introduces calibration
            {
                if (MakeWordBank.moveOn() && !MakeWordBank.skip())
                {
                    StateManager.makeCursReset = true;
                    text.text = "Read the notification above then press *A* or *X* to continue to a video";
                    step++;
                }
            }
            else if (step == 1)
            {
                //add video of someone centering their hands
                
                if (MakeWordBank.moveOn() && !MakeWordBank.skip())
                {
                    text.text = "watching video";
                    step++;
                }
            }
            else if (step == 2)
            {
                timer += Time.deltaTime;
                if (timer > 2) //if video done
                {
                    text.text = "Now try unlocking the cursor yourself..";
                    if (state.userControlActive)
                    {
                        StateManager.makeCursReset = true;
                        state.userControlActive = false;
                        text.text = "Good, unlock it one more time.";
                        if (state.userControlActive)
                        {
                            lockPanel.transform.localScale -= new Vector3(0.3f, 0.3f, 0.3f);
                            StateManager.makeCursReset = true;
                            state.userControlActive = false;
                            timer = 0;
                            step++;
                        }
                    }
                }
            }
            else if (step == 3) //cursor moves left
            { //Show user what movements can be done
                text.text = "Excellent! Now you'll start to move the cursor.\n Watch the on-screen and video examples then try it on your own..." +
                    "\n When your ready to see the movements in action, press *A* or *X* to continue";
                if (MakeWordBank.moveOn() && !MakeWordBank.skip())
                {
                    step++;
                }
            }
            else if (step == 4)
            {
                text.text = "The cursor can be moved left and right...";
                StateManager.cursorAdd = new Vector3(-.2f * Time.deltaTime, 0f, 0f);
                timer += Time.deltaTime;

                if (timer > shortInterval)
                {
                    timer = 0f;
                    step++;
                }
            }
            else if (step == 5) //pause
            {
                timer += Time.deltaTime;
                if (timer > miniPause)
                {
                    timer = 0f;
                    step++;
                }
            }
            else if (step == 6) //cursor moves right
            {
                StateManager.cursorAdd = new Vector3(.2f * Time.deltaTime, 0f, 0f);
                timer += Time.deltaTime;

                if (timer > longInterval)
                {
                    timer = 0f;
                    step++;
                }
            }
            else if (step == 7) //pause
            {
                timer += Time.deltaTime;
                if (timer > miniPause)
                {
                    StateManager.makeCursReset = true;
                    timer = 0f;
                    step++;
                }
            }
            else if (step == 5)
            {
                text.text = "The cursor can be moved upward and downward...";
                StateManager.cursorAdd = new Vector3(0f, .15f * Time.deltaTime, 0f);
                timer += Time.deltaTime;

                if (timer > shortInterval)
                {
                    timer = 0f;
                    step++;
                }
            }
            else if (step == 6)
            {
                timer += Time.deltaTime;
                if (timer > miniPause)
                {
                    timer = 0f;
                    step++;
                }
            }
            else if (step == 7)
            {
                StateManager.cursorAdd = new Vector3(0f, -.15f * Time.deltaTime, 0f);
                timer += Time.deltaTime;

                if (timer > longInterval)
                {
                    timer = 0f;
                    step++;
                }
            }
            else if (step == 8)
            {
                timer += Time.deltaTime;
                if (timer > miniPause)
                {
                    StateManager.makeCursReset = true;
                    timer = 0f;
                    step++;
                }
            }
            else if (step == 17)
            {
                text.text = "Now it's your turn!\n In between each section you'll see a video of the motion\n" +
                    "After each video, you will be prompted to try the exercises on your own.";

                if (MakeWordBank.moveOn() && !MakeWordBank.skip())
                {
                    mainCamera.SetActive(false);
                    videoCamera.SetActive(true);
                    MakeWordBank.cursorCamera.SetActive(false);
                    VP3.SetActive(true);
                    cursorLRVP.Play();
                    step++;
                }
            }
            else if (step == 18)
            {
                if (cursorLRVP.isPlaying)
                {
                    startedPlaying = true;
                }

                if (startedPlaying && (!cursorLRVP.isPlaying))
                {
                    mainCamera.SetActive(true);
                    videoCamera.SetActive(false);
                    MakeWordBank.cursorCamera.SetActive(true);
                    VP3.SetActive(false);
                    StateManager.moveCursorL = true;

                    if (counter == 5)
                    {
                        text.text = "Move the cursor to the left by moving the rod" + "\n"
                            + "with both your hands to the left as far as you can" + "\n"
                            + "Keep your shoulders and torso in place, and the rod parallel to the ground";

                        if ((Kinect.LHandPos.x - Kinect.LShoulderPos.x) < maxLDistance)
                        {
                            maxLDistance = Kinect.LHandPos.x - Kinect.LShoulderPos.x;
                        }

                        if ((Kinect.RHandPos.x - Kinect.RShoulderPos.x) < maxRDistance)
                        {
                            maxRDistance = Kinect.RHandPos.x - Kinect.RShoulderPos.x;
                        }

                        if (StateManager.nextCursorPos.x <= MakeWordBank.leftBound) //original = -0.425f
                        { //Cursor moved to the border of the screen
                            LHandLeftTotal += maxLDistance;
                            RHandLeftTotal += maxRDistance;
                            maxLDistance = 0f;
                            maxRDistance = 0f;
                            StateManager.makeCursReset = true;
                            counter--;
                        }
                    }
                    else if (counter > 0)
                    {
                        if (counter > 1)
                        {
                            text.text = "Good job! Now return to the neutral position" + "\n"
                                + "And repeat this movement for " + counter.ToString() + " more times" + "\n"
                                + "Move your hands as far as you can every time";
                        }
                        else
                        {
                            text.text = "Good job! Now return to the neutral position" + "\n"
                                + "And repeat this movement for 1 more time" + "\n"
                                + "Move your hands as far as you can every time";
                        }

                        if (addTimer)
                        {
                            timer += Time.deltaTime;
                        }

                        if (timer > 1f) //&& !StateManager.cursorL
                        { //Proceed if user returns to the neutural position
                            //StateManager.nextCursorPos = new Vector3(0f, 0f, 0.418f);
                            timer = 0f;
                            addTimer = false;
                            resetted = true;
                        }

                        if (resetted)
                        {
                            if ((Kinect.LHandPos.x - Kinect.LShoulderPos.x) < maxLDistance)
                            {
                                maxLDistance = Kinect.LHandPos.x - Kinect.LShoulderPos.x;
                            }

                            if ((Kinect.RHandPos.x - Kinect.RShoulderPos.x) < maxRDistance)
                            {
                                maxRDistance = Kinect.RHandPos.x - Kinect.RShoulderPos.x;
                            }

                            if (StateManager.nextCursorPos.x <= MakeWordBank.leftBound)
                            {
                                LHandLeftTotal += maxLDistance;
                                RHandLeftTotal += maxRDistance;
                                maxLDistance = 0f;
                                maxRDistance = 0f;
                                addTimer = true;
                                resetted = false;
                                StateManager.makeCursReset = true;
                                counter--;
                            }
                        }
                    }
                    else
                    {
                        text.text = "Good job!" + "\n"
                                   + "Now return to the neutral position";
                        if (true)//(!StateManager.cursorL)
                        {
                            LHandLeftAverage = LHandLeftTotal / 5;
                            RHandLeftAverage = RHandLeftTotal / 5;
                            maxLDistance = 0f;
                            maxRDistance = 0f;
                            counter = 5;
                            startedPlaying = false;
                            step++;
                        }
                    }
                }
            }
            else if (step == 19)
            {
                StateManager.nextCursorPos = new Vector3(0f, 0f, 0.418f);
                step++;
            }
            else if (step == 20)
            {
                StateManager.moveCursorL = false;
                StateManager.moveCursorR = true;

                if (counter == 5)
                {
                    text.text = "Move the cursor to the right by moving the rod" + "\n"
                        + "with both your hands to the right as far as you can" + "\n"
                        + "Keep your shoulders and torso in place, and the rod parallel to the ground";

                    if ((Kinect.LHandPos.x - Kinect.LShoulderPos.x) > maxLDistance)
                    {
                        maxLDistance = Kinect.LHandPos.x - Kinect.LShoulderPos.x;
                    }

                    if ((Kinect.RHandPos.x - Kinect.RShoulderPos.x) > maxRDistance)
                    {
                        maxRDistance = Kinect.RHandPos.x - Kinect.RShoulderPos.x;
                    }

                    if (StateManager.nextCursorPos.x >= MakeWordBank.rightBound) //.415f?
                    {
                        LHandRightTotal += maxLDistance;
                        RHandRightTotal += maxRDistance;
                        maxLDistance = 0f;
                        maxRDistance = 0f;
                        StateManager.makeCursReset = true;
                        counter--;
                    }
                }
                else if (counter > 0)
                {
                    if (counter > 1)
                    {
                        text.text = "Good job! Now return to the neutral position" + "\n"
                            + "And repeat this movement for " + counter.ToString() + " more times" + "\n"
                            + "Move your hands as far as you can every time";
                    }
                    else
                    {
                        text.text = "Good job! Now return to the neutral position" + "\n"
                            + "And repeat this movement for 1 more time" + "\n"
                            + "Move your hands as far as you can every time";
                    }

                    if (addTimer)
                    {
                        timer += Time.deltaTime;
                    }

                    if (timer > 1f)// && !StateManager.cursorR)
                    {
                        //StateManager.nextCursorPos = new Vector3(0f, 0f, 0.418f);
                        timer = 0f;
                        addTimer = false;
                        resetted = true;
                    }

                    if (resetted)
                    {
                        if ((Kinect.LHandPos.x - Kinect.LShoulderPos.x) > maxLDistance)
                        {
                            maxLDistance = Kinect.LHandPos.x - Kinect.LShoulderPos.x;
                        }

                        if ((Kinect.RHandPos.x - Kinect.RShoulderPos.x) > maxRDistance)
                        {
                            maxRDistance = Kinect.RHandPos.x - Kinect.RShoulderPos.x;
                        }

                        if (StateManager.nextCursorPos.x >= MakeWordBank.rightBound)
                        {
                            LHandRightTotal += maxLDistance;
                            RHandRightTotal += maxRDistance;
                            maxLDistance = 0f;
                            maxRDistance = 0f;
                            addTimer = true;
                            resetted = false;
                            StateManager.nextCursorPos = new Vector3(0f, 0f, 0.418f);
                            StateManager.makeCursReset = true;
                            counter--;
                        }
                    }
                }
                else
                {
                    text.text = "Good job!" + "\n"
                                + "Now return to the neutral position";
                    if (true)//!StateManager.cursorR)
                    {
                        LHandRightAverage = LHandRightTotal / 5;
                        RHandRightAverage = RHandRightTotal / 5;
                        maxLDistance = 0f;
                        maxRDistance = 0f;
                        counter = 5;
                        step++;
                    }
                }            
            }
            else if (step == 21)
            {
                mainCamera.SetActive(false);
                videoCamera.SetActive(true);
                MakeWordBank.cursorCamera.SetActive(false);
                VP4.SetActive(true);
                cursorUDVP.Play();
                //StateManager.nextCursorPos = new Vector3(0f, 0f, 0.418f);
                StateManager.makeCursReset = true;
                step++;
            }
            else if (step == 22)
            {
                if (cursorUDVP.isPlaying)
                {
                    startedPlaying = true;
                }

                if (startedPlaying && (!cursorUDVP.isPlaying))
                {
                    mainCamera.SetActive(true);
                    videoCamera.SetActive(false);
                    MakeWordBank.cursorCamera.SetActive(true);
                    VP4.SetActive(false);
                    StateManager.moveCursorR = false;
                    StateManager.moveCursorU = true;

                    if (counter == 5)
                    {
                        text.text = "Move the cursor upward by lifting the rod vertically as far as you can" + "\n"
                            + "Keep your hands at the same height and keep the rod parallel to the ground";

                        if ((Kinect.LHandPos.y - Kinect.LShoulderPos.y) > maxLDistance)
                        {
                            maxLDistance = Kinect.LHandPos.y - Kinect.LShoulderPos.y;
                        }

                        if ((Kinect.RHandPos.y - Kinect.RShoulderPos.y) > maxRDistance)
                        {
                            maxRDistance = Kinect.RHandPos.y - Kinect.RShoulderPos.y;
                        }

                        if (StateManager.nextCursorPos.y >= MakeWordBank.upperBound)
                        {
                            LHandUpTotal += maxLDistance;
                            RHandUpTotal += maxRDistance;
                            maxLDistance = 0f;
                            maxRDistance = 0f;
                            StateManager.makeCursReset = true;
                            counter--;
                        }
                    }
                    else if (counter > 0)
                    {
                        if (counter > 1)
                        {
                            text.text = "Good job! Now return to the neutral position" + "\n"
                                + "And repeat this movement for " + counter.ToString() + " more times" + "\n"
                                + "Move your hands as far as you can every time";
                        }
                        else
                        {
                            text.text = "Good job! Now return to the neutral position" + "\n"
                                + "And repeat this movement for 1 more time" + "\n"
                                + "Move your hands as far as you can every time";
                        }

                        if (addTimer)
                        {
                            timer += Time.deltaTime;
                        }

                        if (timer > 1f)//&& !StateManager.cursorU)
                        {
                            //StateManager.nextCursorPos = new Vector3(0f, 0f, 0.418f);
                            timer = 0f;
                            addTimer = false;
                            resetted = true;
                        }

                        if (resetted)
                        {
                            if ((Kinect.LHandPos.y - Kinect.LShoulderPos.y) > maxLDistance)
                            {
                                maxLDistance = Kinect.LHandPos.y - Kinect.LShoulderPos.y;
                            }

                            if ((Kinect.RHandPos.y - Kinect.RShoulderPos.y) > maxRDistance)
                            {
                                maxRDistance = Kinect.RHandPos.y - Kinect.RShoulderPos.y;
                            }

                            if (StateManager.nextCursorPos.y >= MakeWordBank.upperBound) // 0.238f
                            {
                                LHandUpTotal += maxLDistance;
                                RHandUpTotal += maxRDistance;
                                maxLDistance = 0f;
                                maxRDistance = 0f;
                                addTimer = true;
                                resetted = false;
                                StateManager.makeCursReset = true;
                                counter--;
                            }
                        }
                    }
                    else
                    {
                        text.text = "Good job!" + "\n"
                                   + "Now return to the neutral position";
                        if (true)//!StateManager.cursorU)
                        {
                            LHandUpAverage = LHandUpTotal / 5;
                            RHandUpAverage = RHandUpTotal / 5;
                            maxLDistance = 0f;
                            maxRDistance = 0f;
                            counter = 5;
                            startedPlaying = false;
                            step++;
                        }
                    }
                }
            }
            else if (step == 23)
            {
                StateManager.nextCursorPos = new Vector3(0f, 0f, 0.418f);
                step++;
            }
            else if (step == 24)
            {
                StateManager.moveCursorU = false;
                StateManager.moveCursorD = true;

                if (counter == 5)
                {
                    text.text = "Move the cursor downward by lowering the rod vertically as far as you can" + "\n"
                        + "Keep your hands at the same height and keep the rod parallel to the ground";

                    if ((Kinect.LHandPos.y - Kinect.LShoulderPos.y) < maxLDistance)
                    {
                        maxLDistance = Kinect.LHandPos.y - Kinect.LShoulderPos.y;
                    }

                    if ((Kinect.RHandPos.y - Kinect.RShoulderPos.y) < maxRDistance)
                    {
                        maxRDistance = Kinect.RHandPos.y - Kinect.RShoulderPos.y;
                    }

                    if (StateManager.nextCursorPos.y <= MakeWordBank.lowerBound) //-.215
                    {
                        LHandDownTotal += maxLDistance;
                        RHandDownTotal += maxRDistance;
                        maxLDistance = 0f;
                        maxRDistance = 0f;
                        StateManager.makeCursReset = true;
                        counter--;
                    }

                }
                else if (counter > 0)
                {
                    if (counter > 1)
                    {
                        text.text = "Good job! Now return to the neutral position" + "\n"
                            + "And repeat this movement for " + counter.ToString() + " more times" + "\n"
                            + "Move your hands as far as you can every time";
                    }
                    else
                    {
                        text.text = "Good job! Now return to the neutral position" + "\n"
                            + "And repeat this movement for 1 more time" + "\n"
                            + "Move your hands as far as you can every time";
                    }

                    if (addTimer)
                    {
                        timer += Time.deltaTime;
                    }

                    if (timer > 1f)// && !StateManager.cursorD)
                    {
                        //StateManager.nextCursorPos = new Vector3(0f, 0f, 0.418f);
                        timer = 0f;
                        addTimer = false;
                        resetted = true;
                    }

                    if (resetted)
                    {
                        if ((Kinect.LHandPos.y - Kinect.LShoulderPos.y) < maxLDistance)
                        {
                            maxLDistance = Kinect.LHandPos.y - Kinect.LShoulderPos.y;
                        }

                        if ((Kinect.RHandPos.y - Kinect.RShoulderPos.y) < maxRDistance)
                        {
                            maxRDistance = Kinect.RHandPos.y - Kinect.RShoulderPos.y;
                        }

                        if (StateManager.nextCursorPos.y <= MakeWordBank.lowerBound)
                        {
                            LHandDownTotal += maxLDistance;
                            RHandDownTotal += maxRDistance;
                            maxLDistance = 0f;
                            maxRDistance = 0f;
                            addTimer = true;
                            resetted = false;
                            StateManager.makeCursReset = true;
                            counter--;
                        }
                    }
                }
                else
                {
                    text.text = "Good job!" + "\n"
                           + "Now return to the neutral position";
                    if (true)//!StateManager.cursorD)
                    {
                        LHandDownAverage = LHandDownTotal / 5;
                        RHandDownAverage = RHandDownTotal / 5;
                        maxLDistance = 0f;
                        maxRDistance = 0f;
                        counter = 5;
                        step++;
                    }
                }
            }
            else if (step == 25)
            {
                VP1.SetActive(true);
                cameraLRVP.Play();
                StateManager.moveCursorD = false; //all cursor movement disabled by this pt.
                mainCamera.SetActive(false);
                videoCamera.SetActive(true);
                MakeWordBank.cursorCamera.SetActive(false);
                StateManager.makeCursReset = true;
                step++;
            }
            else if (step == 26)
            {
                if (cameraLRVP.isPlaying)
                {
                    startedPlaying = true;
                }
                if (startedPlaying && (!cameraLRVP.isPlaying))
                {
                    mainCamera.SetActive(true);
                    videoCamera.SetActive(false);
                    MakeWordBank.cursorCamera.SetActive(true);
                    VP1.SetActive(false);     
                    StateManager.moveCameraL = true;

                    if (counter == 5)
                    {
                        text.text = "Pan the image to the left by rotating the rod counter-clockwise" + "\n"
                            + "with your right hand moving upward and your left hand moving downward";

                        //if (Kinect_Angle(Kinect.RHandPos.x, Kinect.RHandPos.y, (Kinect.LHandPos.x + Kinect.RHandPos.x) / 2, (Kinect.LHandPos.y + Kinect.RHandPos.y) / 2) > maxAngle)
                        //{
                        //    maxAngle = Kinect_Angle(Kinect.RHandPos.x, Kinect.RHandPos.y, (Kinect.LHandPos.x + Kinect.RHandPos.x) / 2, (Kinect.LHandPos.y + Kinect.RHandPos.y) / 2);
                        //}

                        if (StateManager.absRotation.y <= MakeWordBank.camHorizontalBound) //originally -16f
                        {
                            angleLeftTotal += maxAngle;
                            maxAngle = 0f;
                            timer = 0f;
                            //prevCameraAngle = StateManager.nextCameraPos.y;
                            counter--;
                        }
                    }
                    else if (counter > 0)
                    {
                        if (counter > 1)
                        {
                            text.text = "Good job!" + "\n"
                                + "Now return to the neutral position" + "\n"
                                + "And repeat this movement for " + counter.ToString() + " more times";
                        }
                        else
                        {
                            text.text = "Good job!" + "\n"
                                + "Now return to the neutral position" + "\n"
                                + "And repeat this movement for 1 more time";
                        }

                        if (!logged && !StateManager.cameraL)
                        {
                            prevCameraAngle = StateManager.absRotation.y; //cameraPos?
                            resetted = true;
                            logged = true;
                        }

                        if (resetted)
                        {
                            if (Kinect_Angle(Kinect.RHandPos.x, Kinect.RHandPos.y, (Kinect.LHandPos.x + Kinect.RHandPos.x) / 2, (Kinect.LHandPos.y + Kinect.RHandPos.y) / 2) > maxAngle)
                            {
                                maxAngle = Kinect_Angle(Kinect.RHandPos.x, Kinect.RHandPos.y, (Kinect.LHandPos.x + Kinect.RHandPos.x) / 2, (Kinect.LHandPos.y + Kinect.RHandPos.y) / 2);
                            }

                            if (StateManager.absRotation.y <= (prevCameraAngle + MakeWordBank.camHorizontalBound)) //originally -16f
                            { //The image can be panned around indefinitely so change in position is used
                                angleLeftTotal += maxAngle;
                                maxAngle = 0f;
                                logged = false;
                                resetted = false;
                                counter--;
                            }
                        }
                    }
                    else
                    {
                        text.text = "Good job!" + "\n"
                               + "Now return to the neutral position";
                        if (!StateManager.cameraL)
                        {
                            angleLeftAverage = angleLeftTotal / 5;
                            maxAngle = 1000f;
                            counter = 5;
                            startedPlaying = false;
                            step++;
                        }
                    }
                }
            }
            else if (step == 27)
            {
                prevCameraAngle = 0f;
                step++;
            }
            else if (step == 28)
            {
                StateManager.moveCameraL = false;
                StateManager.moveCameraR = true;

                //Debug.Log(Kinect_Angle(Kinect.LHandPos.x, Kinect.LHandPos.y, (Kinect.LHandPos.x + Kinect.RHandPos.x) / 2, (Kinect.LHandPos.y + Kinect.RHandPos.y) / 2));

                if (counter == 5)
                {
                    text.text = "Pan the image to the right by rotating the rod clockwise" + "\n"
                        + "with your right hand moving downward and your left hand moving upward";

                    if (Kinect_Angle(Kinect.LHandPos.x, Kinect.LHandPos.y, (Kinect.LHandPos.x + Kinect.RHandPos.x) / 2, (Kinect.LHandPos.y + Kinect.RHandPos.y) / 2) < maxAngle)
                    {
                        maxAngle = Kinect_Angle(Kinect.LHandPos.x, Kinect.LHandPos.y, (Kinect.LHandPos.x + Kinect.RHandPos.x) / 2, (Kinect.LHandPos.y + Kinect.RHandPos.y) / 2);
                        if (maxAngle < 0)
                        {
                            maxAngle = 1;
                        }
                    }

                    if (StateManager.absRotation.y >= -MakeWordBank.camHorizontalBound) //originally 16f
                    {
                        angleRightTotal += maxAngle;
                        maxAngle = 0f;
                        timer = 0f;
                        counter--;
                    }
                }
                else if (counter > 0)
                {
                    if (counter > 1)
                    {
                        text.text = "Good job!" + "\n"
                            + "Now return to the neutral position" + "\n"
                            + "And repeat this movement for " + counter.ToString() + " more times";
                    }
                    else
                    {
                        text.text = "Good job!" + "\n"
                            + "Now return to the neutral position" + "\n"
                            + "And repeat this movement for 1 more time";
                    }

                    if (!logged && !StateManager.cameraR)
                    {
                        prevCameraAngle = StateManager.absRotation.y;
                        resetted = true;
                        logged = true;
                    }

                    if (resetted)
                    {
                        if (Kinect_Angle(Kinect.LHandPos.x, Kinect.LHandPos.y, (Kinect.LHandPos.x + Kinect.RHandPos.x) / 2, (Kinect.LHandPos.y + Kinect.RHandPos.y) / 2) < maxAngle)
                        {
                            maxAngle = Kinect_Angle(Kinect.LHandPos.x, Kinect.LHandPos.y, (Kinect.LHandPos.x + Kinect.RHandPos.x) / 2, (Kinect.LHandPos.y + Kinect.RHandPos.y) / 2);
                            if (maxAngle < 0)
                            {
                                maxAngle = 1;
                            }
                        }

                        if (StateManager.absRotation.y >= (prevCameraAngle - MakeWordBank.camHorizontalBound)) //originally -20f
                        {
                            angleRightTotal += maxAngle;
                            maxAngle = 0f;
                            logged = false;
                            resetted = false;
                            counter--;
                        }
                    }
                }
                else
                {
                    text.text = "Good job!" + "\n"
                           + "Now return to the neutral position";
                    if (!StateManager.cameraR)
                    {
                        //angleRightAverage = angleRightTotal / 5;
                        maxAngle = 0f;
                        counter = 5;
                        step++;
                    }
                }
            }
            else if (step == 29)
            {
                mainCamera.SetActive(false);
                videoCamera.SetActive(true);
                MakeWordBank.cursorCamera.SetActive(false);
                VP2.SetActive(true);
                cameraUDVP.Play();
                StateManager.moveCameraR = false;
                StateManager.nextCursorPos = new Vector3(0f, 0f, 0.418f);
                prevCameraAngle = 0f;
                step++;
            }
            else if (step == 30)
            {
                if (cameraUDVP.isPlaying)
                {
                    startedPlaying = true;
                    Console.WriteLine(startedPlaying); //debug line
                }

                if (startedPlaying && (!cameraUDVP.isPlaying))
                {
                    mainCamera.SetActive(true);
                    videoCamera.SetActive(false);
                    MakeWordBank.cursorCamera.SetActive(true);
                    VP2.SetActive(false);       
                    StateManager.moveCameraU = true;

                    if (counter == 5)
                    {
                        text.text = "Pan the image upward by rotating both your wrists up" + "\n"
                            + "and then back to level as fast as you can";

                        if (StateManager.cameraU)
                        {
                            moved = true;
                        }
                        if (moved)
                        {
                            if (IMU.Gyro_Pitch > maxSpeed)
                            {
                                maxSpeed = IMU.Gyro_Pitch;
                            }

                            if (!StateManager.cameraU)
                            {
                                timer += Time.deltaTime;
                                if (timer > 0.5f)
                                {
                                    speedUpTotal += maxSpeed;
                                    maxSpeed = 0f;
                                    moved = false;
                                    counter--;
                                }
                            }
                        }
                    }
                    else if (counter > 0)
                    {
                        if (counter > 1)
                        {
                            text.text = "Good job! Now repeat this movement for " + counter.ToString() + " more times" + "\n"
                                 + "Move your hands as fast as you can every time";
                        }
                        else
                        {
                            text.text = "Good job! Now repeat this movement for 1 more time" + "\n"
                                 + "Move your hands as fast as you can every time";
                        }
                        if (StateManager.nextCameraPos.x >= MakeWordBank.camTop) //if goes over bounds than reset
                        {
                        }
                        if (StateManager.cameraU)
                        {
                            moved = true;
                        }
                        if (moved)
                        {
                            if (IMU.Gyro_Pitch > maxSpeed)
                            {
                                maxSpeed = IMU.Gyro_Pitch;
                            }

                            if (!StateManager.cameraU)
                            {
                                timer += Time.deltaTime;

                                if (timer > 0.5f)
                                {
                                    speedUpTotal += maxSpeed;
                                    moved = false;
                                    counter--;
                                }
                            }
                        }
                    }
                    else
                    {
                        speedUpAverage = speedUpTotal / 5;
                        maxSpeed = 0f;
                        counter = 5;
                        startedPlaying = false;
                        step++;

                    }
                }
            }
            else if (step == 31)
            {
                prevCameraAngle = 0f;// StateManager.cameraPos.x;
                step++;
            }
            else if (step == 32)
            {
                StateManager.moveCameraU = false;
                StateManager.moveCameraD = true;

                if (counter == 5)
                {
                    text.text = "Pan the image downward by rotating both your wrists down" + "\n"
                        + "and then back to level as fast as you can";
                    if (StateManager.cameraD)
                    {
                        moved = true;
                    }
                    if (moved)
                    {
                        if (IMU.Gyro_Pitch < maxSpeed)
                        {
                            maxSpeed = IMU.Gyro_Pitch;
                        }

                        if (!StateManager.cameraD)
                        {
                            timer += Time.deltaTime;

                            if (timer > 0.5f)
                            {
                                speedDownTotal += maxSpeed;
                                maxSpeed = 0f;
                                moved = false;
                                counter--;
                            }
                        }
                    }

                }
                else if (counter > 0)
                {
                    if (counter > 1)
                    {
                        text.text = "Good job! Now repeat this movement for " + counter.ToString() + " more times" + "\n"
                              + "Move your hands as fast as you can every time";
                    }
                    else
                    {
                        text.text = "Good job! Now repeat this movement for 1 more time" + "\n"
                             + "Move your hands as fast as you can every time";
                    }
                    if (StateManager.nextCameraPos.x <= MakeWordBank.camBot) //if goes over bounds than reset
                    {
                        //cam reset?
                    }
                    if (StateManager.cameraD)
                    {
                        moved = true;
                    }
                    if (moved)
                    {
                        if (IMU.Gyro_Pitch < maxSpeed)
                        {
                            maxSpeed = IMU.Gyro_Pitch;
                        }

                        if (!StateManager.cameraD)
                        {
                            timer += Time.deltaTime;

                            if (timer > 0.5f)
                            {
                                speedDownTotal += maxSpeed;
                                moved = false;
                                counter--;
                            }
                        }
                    }
                }
                else
                {
                    speedDownAverage = speedDownTotal / 5;
                    maxSpeed = 0f;
                    counter = 5;
                    step++;
                }
            }
            else if (step == 33)
            {
                mainCamera.SetActive(false);
                videoCamera.SetActive(true);
                MakeWordBank.cursorCamera.SetActive(false);
                VP5.SetActive(true);
                clickVP.Play();
                StateManager.moveCameraD = false; //cam's false too now
                step++;
            }
            else if (step == 34)
            {
                if (clickVP.isPlaying)
                {
                    startedPlaying = true;
                }

                if (startedPlaying && (!clickVP.isPlaying))
                {
                    mainCamera.SetActive(true);
                    videoCamera.SetActive(false);
                    MakeWordBank.cursorCamera.SetActive(true);
                    VP5.SetActive(false);
                    circle.SetActive(true);

                    float LHandSpeed = StateManager.LHandSpeed;
                    float RHandSpeed = StateManager.RHandSpeed;
                    float LElbowSpeed = StateManager.LElbowSpeed;
                    float RElbowSpeed = StateManager.RElbowSpeed;

                    if (counter == 5)
                    {
                        text.text = "To select an object, push the rod forward with both your hands as fast as you can" + "\n"
                            + "and then pull the rod backward as fast as you can";

                        if (LHandSpeed < LHSmin)
                        {
                            LHSmin = LHandSpeed;
                        }

                        if (LHandSpeed > LHSmax)
                        {
                            LHSmax = LHandSpeed;
                        }

                        if (RHandSpeed < RHSmin)
                        {
                            RHSmin = RHandSpeed;
                        }

                        if (RHandSpeed > RHSmax)
                        {
                            RHSmax = RHandSpeed;
                        }

                        if (LElbowSpeed < LESmin)
                        {
                            LESmin = LElbowSpeed;
                        }

                        if (LElbowSpeed > LESmax)
                        {
                            LESmax = LElbowSpeed;
                        }

                        if (RElbowSpeed < RESmin)
                        {
                            RESmin = RElbowSpeed;
                        }

                        if (RElbowSpeed > RESmax)
                        {
                            RESmax = RElbowSpeed;
                        }

                        if (!prevClicked && StateManager.rodClicked)
                        {
                            LHandSpeedForwardTotal += LHSmin;
                            LHandSpeedBackwardTotal += LHSmax;
                            RHandSpeedForwardTotal += RHSmin;
                            RHandSpeedBackwardTotal += RHSmax;
                            LElbowSpeedForwardTotal += LESmin;
                            LElbowSpeedBackwardTotal += LESmax;
                            RElbowSpeedForwardTotal += RESmin;
                            RElbowSpeedBackwardTotal += RESmax;

                            LHSmax = 0f;
                            LHSmin = 0f;
                            RHSmax = 0f;
                            RHSmin = 0f;
                            LESmax = 0f;
                            LESmin = 0f;
                            RESmax = 0f;
                            RESmin = 0f;

                            if (circleWhite)
                            {
                                circle.GetComponent<Image>().color = new Color32(200, 200, 200, 255);
                                circleWhite = false;
                            }
                            else
                            {
                                circle.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                                circleWhite = true;
                            }

                            counter--;
                            timer = 2f;
                        }
                    }
                    else if (counter > 0)
                    {
                        if (counter > 1)
                        {
                            text.text = "Good job! Now repeat this movement for " + counter.ToString() + " more times" + "\n"
                                  + "Move your hands as fast as you can every time";
                        }
                        else
                        {
                            text.text = "Good job! Now repeat this movement for 1 more time" + "\n"
                                 + "Move your hands as fast as you can every time";
                        }

                        if (LHandSpeed < LHSmin) //sets left/right's hand/elbow's max and min's to update
                        {
                            LHSmin = LHandSpeed;
                        }

                        if (LHandSpeed > LHSmax)
                        {
                            LHSmax = LHandSpeed;
                        }

                        if (RHandSpeed < RHSmin)
                        {
                            RHSmin = RHandSpeed;
                        }

                        if (RHandSpeed > RHSmax)
                        {
                            RHSmax = RHandSpeed;
                        }

                        if (LElbowSpeed < LESmin)
                        {
                            LESmin = LElbowSpeed;
                        }

                        if (LElbowSpeed > LESmax)
                        {
                            LESmax = LElbowSpeed;
                        }

                        if (RElbowSpeed < RESmin)
                        {
                            RESmin = RElbowSpeed;
                        }

                        if (RElbowSpeed > RESmax)
                        {
                            RESmax = RElbowSpeed;
                        }

                        timer += Time.deltaTime;

                        if (timer > 1f)
                        {
                            if (!prevClicked && StateManager.rodClicked) //there is a click in the rod?
                            {
                                LHandSpeedForwardTotal += LHSmin;
                                LHandSpeedBackwardTotal += LHSmax;
                                RHandSpeedForwardTotal += RHSmin;
                                RHandSpeedBackwardTotal += RHSmax;
                                LElbowSpeedForwardTotal += LESmin;
                                LElbowSpeedBackwardTotal += LESmax;
                                RElbowSpeedForwardTotal += RESmin;
                                RElbowSpeedBackwardTotal += RESmax;

                                LHSmax = 0f;
                                LHSmin = 0f;
                                RHSmax = 0f;
                                RHSmin = 0f;
                                LESmax = 0f;
                                LESmin = 0f;
                                RESmax = 0f;
                                RESmin = 0f;

                                if (circleWhite)
                                {
                                    circle.GetComponent<Image>().color = new Color32(200, 200, 200, 255);
                                    circleWhite = false;
                                }
                                else
                                {
                                    circle.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                                    circleWhite = true;
                                }

                                counter--;
                                timer = 0f;
                            }
                            //if (Input.GetKey(KeyCode.Space)) //visualization of method for keyboard in statemanager class
                            //{
                            //    counter--;
                            //    timer = 2f;
                            //}
                        }
                    }
                    else
                    {
                        LHandSpeedForwardAverage = LHandSpeedForwardTotal / 5;
                        LHandSpeedBackwardAverage = LHandSpeedBackwardTotal / 5;
                        RHandSpeedForwardAverage = RHandSpeedForwardTotal / 5;
                        RHandSpeedBackwardAverage = RHandSpeedBackwardTotal / 5;
                        LElbowSpeedForwardAverage = LElbowSpeedForwardTotal / 5;
                        LElbowSpeedBackwardAverage = LElbowSpeedBackwardTotal / 5;
                        RElbowSpeedForwardAverage = RElbowSpeedForwardTotal / 5;
                        RElbowSpeedBackwardAverage = RElbowSpeedBackwardTotal / 5;

                        LHSmax = 0f;
                        LHSmin = 0f;
                        RHSmax = 0f;
                        RHSmin = 0f;
                        LESmax = 0f;
                        LESmin = 0f;
                        RESmax = 0f;
                        RESmin = 0f;

                        counter = 5;
                        startedPlaying = false;
                        step++;
                    }
                }

                prevClicked = StateManager.rodClicked;
            }
            else if (step == 35)
            {
                canvas.SetActive(false);
                mainCamera.SetActive(true);
                UICamera.SetActive(true);
                videoCamera.SetActive(false);
                MakeWordBank.cursorCamera.SetActive(true);
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

                //MakeWordBank.inTutorial = true;
                inSimpleTutorial = false; //stops simple tutorial
                initialized = false;

                //import movement data into UserInfo;

                lockPanel.SetActive(false);

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
public class MovementData
{
    public float[] rangeOfMotion; //-x, x, -y, y bounds for user
    public float[] timeOfMotion; //the time it takes for the user to achieve max motion for each range

    public MovementData()
    {
        rangeOfMotion = new float[4];
        timeOfMotion = new float[4];
    }
}