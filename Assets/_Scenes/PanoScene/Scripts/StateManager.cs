using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
/*
 StateManager is an object used for accessing and setting the state of the program 
 across the various scripts that manipulate the state.

 Currently it is being implemented as a bunch of fields with getters and setters,
 but eventually, it would be nice to have an object (like in js) containing them.
     */

public class StateManager : MonoBehaviour {

    /* Computer Keyboard - Keymap
     * Cursor Movement = (arrow keys)
     * Camera Movement = wasd
     * Rod Click = space bar
     * Rod Movement/Speed = n/m = forward/back
     */

    private GameObject selected = null; // The current tag being selected
    private GameObject falconCursor; // The cursor being manipulated by the falcon

    private Camera falconCamera;

    public static bool[] falconButtons = new bool[4];

    private int buttons; // bit 1 is the pan button, bit 2 is the click button, bit 3 is the top button of the falcon

    public static bool kinectReady = false;

    private float pushCoolDown = 0f;

    private float prevLHand_z = Kinect.LHandPos.z;
    private float prevRHand_z = Kinect.RHandPos.z;
    private float prevLElbow_y = Kinect.LElbowPos.y;
    private float prevRElbow_y = Kinect.RElbowPos.y;

    public static Vector3 cursorPos = new Vector3(0f, 0f, 0.418f);
    public static Vector3 cameraPos = Vector3.zero;

    public static Vector3 nextCursorPos;
    public static Vector3 nextCameraPos;

    //private float smallestLHdiff = 999f;
    //private float smallestRHdiff = 999f;
    //private float largestLEdiff = 0f;
    //private float largestREdiff = 0f;

    public static float LHandSpeed = 0f;
    public static float RHandSpeed = 0f;
    public static float LElbowSpeed = 0f;
    public static float RElbowSpeed = 0f;

    private List<Tuple<float, float, float, float>> speeds;

    private float speedInitializeTime = 0f;

    private float avgDistance_x = 0f;
    private float avgDistance_y = 0f;

    public static bool cameraL = false;
    public static bool cameraR = false;
    public static bool cameraU = false;
    public static bool cameraD = false;

    public static bool cursorL = false;
    public static bool cursorR = false;
    public static bool cursorU = false;
    public static bool cursorD = false;

    //Limit cursor and camera movement in tutorials
    public static bool moveCameraU = true;
    public static bool moveCameraD = true;
    public static bool moveCameraL = true;
    public static bool moveCameraR = true;
    public static bool moveCursorU = true;
    public static bool moveCursorD = true;
    public static bool moveCursorL = true;
    public static bool moveCursorR = true;
    public static bool rodClicked = false;

    public static float cameraUpCoolDown = 0f;
    public static float cameraDownCoolDown = 0f;

    public static bool makeCursReset = false; // resets the position of the cursor to the middle of the screen
    public static bool makeCamReset = false; // resets the position of the camera to the middle of the image/starting pt.
    public static bool allSystemsGo = false; // variable that enables all movement functionality for camera and cursor upon update()
    public static Vector3 cursorAdd = new Vector3(0f, 0f, 0f);
    public static Vector3 cameraAdd = new Vector3(0f, 0f, 0f);

    public static float cursorPosMod = 239.36f; //modifier for sphere to ui translation

    public static GameObject mainCamera;
    public bool cameraMoving;
    public static Vector3 absRotation; //rotation of camera without lowest abs val conversion

    public float xOffset = 25.9f; //factor that sets tags to dissapear after being a certain dist away from camera's center

    public static float camSpeed = 1.6f; //factor that speeds up the camera's movement
    public static float cursorSpeed = 2.5f; //factor that speeds up cursor's movement
    public static float cursorSize = -0.4f; //factor that makes cursor bigger or smaller


    public List<GameObject> tagsPlaced;

    //public List<InvisTag> invisTags;

    public GameObject getSelected()
    {
        return selected;
    }

    public void setSelected(GameObject g)
    {
        selected = g;
    }

    public Vector3 getCursorPosition()
    {
        return cursorPos;
    }

    public Vector3 getCameraPosition()
    {
        return cameraPos;
    }

    public int getButtons()
    {
        return buttons;
    }

    public bool[] getFalconButtons()
    {
        return falconButtons;
    }

    public bool isKinectReady()
    {
        return kinectReady;
    }
    float Kinect_Angle(float x1, float y1, float standard_x, float standard_y)
    { //Calculate rod rotation angle about the z axis
        return (Mathf.Atan2((y1 - standard_y), (x1 - standard_x))) * (180 / Mathf.PI);
    }

    public float getLowestAngle(float num) //resets positional rotation to be the abs lowest possible representation
    {
        num %= 360;
        if (num > 180)
        {
            return (num - 360);
        }
        else if(num < -180)
        {
            return (360 + num);
        }
        return num;
    }

    void Awake()
    {
        falconButtons = new bool[4] { false, false, false, false };
        speeds = new List<Tuple<float, float, float, float>>();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(5f);
        this.falconCursor = GameObject.Find("CursorSphere");
        this.falconCamera = GameObject.Find("CursorCamera").GetComponent<Camera>();
        mainCamera = GameObject.Find("Main Camera");
    }

    private void Update()
    {
        if (allSystemsGo)
        {
            //moveCursorL = true; // cursors
            //moveCursorR = true;
            //moveCursorU = true;
            //moveCursorD = true;
            //moveCameraL = true; // cameras
            //moveCameraR = true;
            //moveCameraU = true;
            //moveCameraD = true;
            makeCursReset = false;
            makeCamReset = false;
        }
        //if (Kinect.LHandPos.x != 0 || Kinect.LHandPos.y != 0 || Kinect.LHandPos.z != 0)
        //{
        //    kinectReady = true;
        //}
        kinectReady = true;
        /*
        if (Math.Abs(Kinect.LHandPos.z - Kinect.LShoulderPos.z) < smallestLHdiff) {
            smallestLHdiff = Math.Abs(Kinect.LHandPos.z - Kinect.LShoulderPos.z);
        }
        
        if (Math.Abs(Kinect.RHandPos.z - Kinect.RShoulderPos.z) < smallestRHdiff) {
            smallestRHdiff = Math.Abs(Kinect.RHandPos.z - Kinect.RShoulderPos.z);
        }
        
        if (Math.Abs(Kinect.LElbowPos.y - Kinect.LShoulderPos.y) > largestLEdiff) {
            largestLEdiff = Math.Abs(Kinect.LElbowPos.y - Kinect.LShoulderPos.y);
        }
        
        if (Math.Abs(Kinect.RElbowPos.y - Kinect.RShoulderPos.y) > largestREdiff) {
            largestREdiff = Math.Abs(Kinect.RElbowPos.y - Kinect.RShoulderPos.y);
        }

        if (Math.Abs(Kinect.LHandPos.z - Kinect.LShoulderPos.z) > (smallestLHdiff / 0.8f) 
            && Math.Abs(Kinect.RHandPos.z - Kinect.RShoulderPos.z) > (smallestRHdiff / 0.8f)
            && Math.Abs(Kinect.LElbowPos.y - Kinect.LShoulderPos.y) < (largestLEdiff * 0.6f)
            && Math.Abs(Kinect.RElbowPos.y - Kinect.RShoulderPos.y) < (largestREdiff * 0.6f)) {
            
            falconButtons[1] = true;
            
            smallestLHdiff = 999f; 
            smallestRHdiff = 999f;
            largestLEdiff = 0f;
            largestREdiff = 0f;
            
        }
        else {
            falconButtons[1] = false;
        }
        */

        falconButtons[1] = false;
        rodClicked = false;

        //LHandSpeed = (Kinect.LHandPos.z - prevLHand_z) / Time.deltaTime;
        //RHandSpeed = (Kinect.RHandPos.z - prevRHand_z) / Time.deltaTime;
        //LElbowSpeed = (Kinect.LElbowPos.y - prevLElbow_y) / Time.deltaTime;
        //RElbowSpeed = (Kinect.RElbowPos.y - prevRElbow_y) / Time.deltaTime;

        // If the user push the rod and then pull the rod OR pull the rod and then push the rod at a certain speed within 3.5 seconds, register a click
        //Change for calibration
        //if (speedInitializeTime < 3.5f)
        //{
        //    speedInitializeTime += Time.deltaTime;
        //    Tuple<float, float, float, float> speed = new Tuple<float, float, float, float>(LHandSpeed, RHandSpeed, LElbowSpeed, RElbowSpeed);
        //    speeds.Add(speed);
        //}
        //else
        //{
        //    for (int i = 0; i < speeds.Count; i++)
        //    {
        //        if ((Kinect.LShoulderPos.y > Kinect.LElbowPos.y) && (Kinect.LElbowPos.y < Kinect.LHandPos.y) 
        //            && (Kinect.RShoulderPos.y > Kinect.RElbowPos.y) && (Kinect.RElbowPos.y < Kinect.RHandPos.y)) 
        //        {
        //            if (speeds[i].Item1 > 0.45 && LHandSpeed < -0.45 && speeds[i].Item2 > 0.45 && RHandSpeed < -0.45
        //            && speeds[i].Item3 < -0.09 && LElbowSpeed > 0.09 && speeds[i].Item4 < -0.09 && RElbowSpeed > 0.09)
        //            {
        //                falconButtons[1] = true;
        //                rodClicked = true;
        //            }
        //            else if (speeds[i].Item1 < -0.45 && LHandSpeed > 0.45 && speeds[i].Item2 < -0.45 && RHandSpeed > 0.45
        //                && speeds[i].Item3 > 0.09 && LElbowSpeed < -0.09 && speeds[i].Item4 > 0.09 && RElbowSpeed < -0.09)
        //            {
        //                falconButtons[1] = true;
        //                rodClicked = true;
        //            }
        //        }
        //    }

        //    for (int i = 0; i < speeds.Count - 1; i++)
        //    {
        //        speeds[i] = speeds[i + 1];
        //    }

        //    Tuple<float, float, float, float> new_speed = new Tuple<float, float, float, float>(LHandSpeed, RHandSpeed, LElbowSpeed, RElbowSpeed);

        //    speeds[speeds.Count - 1] = new_speed;
        //}
        if (Input.GetKey(KeyCode.V)) //new update - forwards and backwards
        {
            rodClicked = true;
        }

        /*
        if (((Kinect.LHandPos.z - prevLHand_z) / Time.deltaTime) < -0.45 && ((Kinect.RHandPos.z - prevRHand_z) / Time.deltaTime) < -0.45 &&
         ((Kinect.LElbowPos.y - prevLElbow_y) / Time.deltaTime) > 0.09 && ((Kinect.RElbowPos.y - prevRElbow_y) / Time.deltaTime) > 0.09 && pushCoolDown >= 1f)
        {
            falconButtons[1] = true;
            pushCoolDown = 0f;
        }
        else
        {
            falconButtons[1] = false;
        }

        pushCoolDown += Time.deltaTime;
        */

        //prevLElbow_y = Kinect.LElbowPos.y;
        //prevRElbow_y = Kinect.RElbowPos.y;
        //prevLHand_z = Kinect.LHandPos.z;
        //prevRHand_z = Kinect.RHandPos.z;

        cameraL = false;
        cameraR = false;
        cameraU = false;
        cameraD = false;

        cameraUpCoolDown += Time.deltaTime;
        cameraDownCoolDown += Time.deltaTime;

        //if (!SimpleTutorial.inSimpleTutorial)
        //{ //Allow camera to rotate in SimpleTutorial
        //    falconButtons[0] = false ;
        //}

        nextCameraPos = cameraPos;

        if (moveCameraL)
        {
            //if (Kinect.RHandPos.y > Kinect.LHandPos.y && Kinect_Angle(Kinect.RHandPos.x, Kinect.RHandPos.y, (Kinect.LHandPos.x + Kinect.RHandPos.x) / 2, (Kinect.LHandPos.y + Kinect.RHandPos.y) / 2) > (SimpleTutorial.angleLeftAverage * 0.4f))
            //{
            //    nextCameraPos = new Vector3(cameraPos.x, (cameraPos.y - 0.5f), 0f);
            //    falconButtons[0] = true;
            //    cameraL = true;
            //}
            if (Input.GetKey(KeyCode.A))
            {
                nextCameraPos += new Vector3(0f, -20f * camSpeed * Time.deltaTime, 0f);
                absRotation += new Vector3(0f, -20f * camSpeed * Time.deltaTime, 0f);
                cameraL = true;
            }
        }

        if (moveCameraR)
        {
            //if (Kinect.RHandPos.y < Kinect.LHandPos.y && Kinect_Angle(Kinect.LHandPos.x, Kinect.LHandPos.y, (Kinect.LHandPos.x + Kinect.RHandPos.x) / 2, (Kinect.LHandPos.y + Kinect.RHandPos.y) / 2) < (SimpleTutorial.angleRightAverage * 2.5f)
            //    && Kinect_Angle(Kinect.LHandPos.x, Kinect.LHandPos.y, (Kinect.LHandPos.x + Kinect.RHandPos.x) / 2, (Kinect.LHandPos.y + Kinect.RHandPos.y) / 2) > 0)
            //{
            //    nextCameraPos = new Vector3(cameraPos.x, (cameraPos.y + 0.5f), 0f);
            //    falconButtons[0] = true;
            //    cameraR = true;
            //}
            if (Input.GetKey(KeyCode.D))
            {
                nextCameraPos += new Vector3(0f, 20f * camSpeed * Time.deltaTime, 0f);
                absRotation += new Vector3(0f, 20f * camSpeed * Time.deltaTime, 0f);
                cameraR = true;
            }
        }

        if (moveCameraU)
        {
            //if (IMU.Gyro_Pitch > (SimpleTutorial.speedUpAverage * 0.4f) && cameraUpCoolDown > 1.5f)
            //{
            //    nextCameraPos = new Vector3((cameraPos.x - 2.5f), cameraPos.y, 0f);
            //    falconButtons[0] = true;
            //    cameraU = true;
            //    cameraDownCoolDown = 0f;
            //}
            if (Input.GetKey(KeyCode.W))
            {
                nextCameraPos += new Vector3(-14f * camSpeed * Time.deltaTime, 0f, 0f);
                absRotation += new Vector3(-14f * camSpeed * Time.deltaTime, 0f, 0f);
                cameraU = true;
            }
        }

        if (moveCameraD)
        {
            //if (IMU.Gyro_Pitch < (SimpleTutorial.speedDownAverage * 0.4f) && cameraDownCoolDown > 1.5f)
            //{
            //    nextCameraPos = new Vector3((cameraPos.x + 2.5f), cameraPos.y, 0f);
            //    falconButtons[0] = true;
            //    cameraD = true;
            //    cameraUpCoolDown = 0f;
            //    Debug.Log(IMU.Gyro_Pitch);
            //}
            if (Input.GetKey(KeyCode.S))
            {
                nextCameraPos += new Vector3(14f * camSpeed * Time.deltaTime, 0f, 0f);
                absRotation += new Vector3(14f * camSpeed * Time.deltaTime, 0f, 0f);
                cameraD = true;
            }
        }

        if (moveCameraL || moveCameraR || moveCameraU || moveCameraD || cameraAdd != new Vector3(0f, 0f, 0f))
        {
            nextCameraPos -= cameraAdd;
        }
        cameraAdd = new Vector3(0f, 0f, 0f);

        // Enforce a boundary on rotating up/down
        //if (nextCameraPos.x > 270f && nextCameraPos.x < 280f)
        //{
        //    nextCameraPos.x = 280f;
        //}
        //else if (nextCameraPos.x > 35f && nextCameraPos.x < 90f)
        //{
        //    nextCameraPos.x = 35f;
        //}
        //old boundaries [-16.8,17.4]

        if (nextCameraPos.x < MakeWordBank.camBot) //[-90,90]
        {
            nextCameraPos.x = MakeWordBank.camBot;
        }
        else if (nextCameraPos.x > MakeWordBank.camTop)
        {
            nextCameraPos.x = MakeWordBank.camTop;
        }

        if (absRotation.x < MakeWordBank.camBot)
        {
            absRotation.x = MakeWordBank.camBot;
        }
        else if (absRotation.x > MakeWordBank.camTop)
        {
            absRotation.x = MakeWordBank.camTop;
        }

        //if (Input.GetKey(KeyCode.Y))
        //{
        //    camSpeed += .01f;
        //}

        if (makeCamReset) //cam reset method
        {
            nextCameraPos = new Vector3(0f, 0f, 0f);
            absRotation = nextCameraPos;
            makeCamReset = false;
        }
        else
        {
            nextCameraPos.y = getLowestAngle(nextCameraPos.y); //reset x pos if goes too far
            Debug.Log("Camera Info: (" + nextCameraPos.y + ", " + nextCameraPos.x + ", " + nextCameraPos.z + "), abs: " + absRotation + ", speed: " + camSpeed);
            if (cameraMoving)
            {
                //quarterion rotations ***
                Quaternion qRotation = Quaternion.Euler(nextCameraPos);// * Time.deltaTime);
                mainCamera.transform.rotation = qRotation;

                //tags movement
                Vector3 change = (nextCameraPos-cameraPos); //take the amount that the camera moves and displace all placed tags by it
                foreach (GameObject obj in tagsPlaced) //8
                {
                    obj.transform.position -= new Vector3(change.y * 3.565f, -change.x * 3.58f, 0f); //**

                    //float offset = (obj.transform.position.x - nextCameraPos.y);
                    //Debug.Log("Object " + obj.name + ": " + obj.transform.position + ", offset: " + (obj.transform.position - nextCameraPos) + ", " + offset);

                    Color newColor = obj.GetComponent<Image>().color;
                    if (obj.transform.position.x > 15 && obj.transform.position.x < 101)//offset > xOffset) disapear after a certain x (factoring for full rotations of 180 degrees)
                    {
                        obj.GetComponentInChildren<Text>().color = Color.clear; //text color change
                        newColor.a = 0;
                    }
                    else //reapear
                    {
                        obj.GetComponentInChildren<Text>().color = Color.blue;
                        newColor.a = .391f; // (100/255) = (.39/1) - transfer to 0->1 scale
                    }
                    obj.GetComponent<Image>().color = newColor;
                }
            }
        }

        cameraPos = nextCameraPos;

        //avgDistance_x = Mathf.Abs(((Kinect.LHandPos.x - Kinect.LShoulderPos.x) + (Kinect.RHandPos.x - Kinect.RShoulderPos.x)) / 2);
        //avgDistance_y = Mathf.Abs(((Kinect.LHandPos.y - Kinect.LShoulderPos.y) + (Kinect.RHandPos.y - Kinect.RShoulderPos.y)) / 2);

        cursorL = false;
        cursorR = false;
        cursorU = false;
        cursorD = false;

        //float keyspeed = 0.07f * cursorSpeed; //x
        //float keyspeed2 = .00016f * cursorSpeed; //y
        nextCursorPos = cursorPos;
        if (moveCursorL)
        {
            //if ((Kinect.LHandPos.x - Kinect.LShoulderPos.x) < (SimpleTutorial.LHandLeftAverage * 0.4f) && (Kinect.RHandPos.x - Kinect.RShoulderPos.x) < (SimpleTutorial.RHandLeftAverage * 0.4f))
            //{
            //    nextCursorPos = new Vector3((cursorPos.x - (0.0005f + 0.02f * avgDistance_x)), cursorPos.y, 0.418f);
            //    cursorL = true;
            //}

            //nextCursorPos = new Vector3(cursorPos.x + Input.GetAxis("Horizontal")* keyspeed, cursorPos.y, 0.418f);
            //cursorL = true;
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                nextCursorPos += new Vector3(-.06f * cursorSpeed * Time.deltaTime, 0f, 0f);
                cursorL = true;
            }
        }

        if (moveCursorR)
        {
            //if ((Kinect.LHandPos.x - Kinect.LShoulderPos.x) > (SimpleTutorial.LHandRightAverage * 0.4f) && (Kinect.RHandPos.x - Kinect.RShoulderPos.x) > (SimpleTutorial.RHandRightAverage * 0.4f))
            //{
            //    nextCursorPos = new Vector3((cursorPos.x + (0.0005f + 0.02f * avgDistance_x)), cursorPos.y, 0.418f);
            //    cursorR = true;
            //}

            //nextCursorPos = new Vector3(cursorPos.x + Input.GetAxis("Horizontal")* keyspeed, cursorPos.y, 0.418f);
            //cursorR = true;
            if (Input.GetKey(KeyCode.RightArrow))
            {
                nextCursorPos += new Vector3(.06f * cursorSpeed * Time.deltaTime, 0f, 0f);
                cursorR = true;
            }
        }

        if (moveCursorU)
        {
            //if ((Kinect.LHandPos.y - Kinect.LShoulderPos.y) > (SimpleTutorial.LHandUpAverage * 0.4f) && (Kinect.RHandPos.y - Kinect.RShoulderPos.y) > (SimpleTutorial.RHandUpAverage * 0.4f))
            //{
            //    nextCursorPos = new Vector3(cursorPos.x, (cursorPos.y + (0.0005f + 0.02f * avgDistance_y)), 0.418f);
            //    cursorU = true;
            //}

            //nextCursorPos = new Vector3(cursorPos.x, cursorPos.y + Input.GetAxis("Vertical") * keyspeed2, 0.418f);
            //cursorU = true;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                nextCursorPos += new Vector3(0f, .04f * cursorSpeed * Time.deltaTime, 0f);
                cursorU = true;
            }
        }

        if (moveCursorD)
        {
            //if ((Kinect.LHandPos.y - Kinect.LShoulderPos.y) < (SimpleTutorial.LHandDownAverage * 0.4f) && (Kinect.RHandPos.y - Kinect.RShoulderPos.y) < (SimpleTutorial.RHandDownAverage * 0.4f))
            //{
            //    nextCursorPos = new Vector3(cursorPos.x, (cursorPos.y - (0.0005f + 0.02f * avgDistance_y)), 0.418f);
            //    cursorD = true;
            //}

            //nextCursorPos = new Vector3(cursorPos.x, cursorPos.y + Input.GetAxis("Vertical") * keyspeed2, 0.418f);
            //cursorD = true;
            if (Input.GetKey(KeyCode.DownArrow))
            {
                nextCursorPos += new Vector3(0f, -.04f * cursorSpeed * Time.deltaTime, 0f);
                cursorD = true;
            }
        }
        Debug.Log("cursorAdd: " + cursorAdd);
        if (moveCursorL || moveCursorR || moveCursorU || moveCursorD)
        {
            nextCursorPos += cursorAdd;
        }
        cursorAdd = new Vector3(0f,0f,0f);

        nextCursorPos.z = -cursorSize;

        //Cursor cannot move past screen borders (bondaries)
        if (nextCursorPos.x > MakeWordBank.rightBound)
        {
            nextCursorPos.x = MakeWordBank.rightBound;
        }
        else if (nextCursorPos.x < MakeWordBank.leftBound)
        {
            nextCursorPos.x = MakeWordBank.leftBound;
        }
        else if (nextCursorPos.y > MakeWordBank.upperBound)
        {
            nextCursorPos.y = MakeWordBank.upperBound;
        }
        else if (nextCursorPos.y < MakeWordBank.lowerBound)
        {
            nextCursorPos.y = MakeWordBank.lowerBound;
        }

        //if (Input.GetKey(KeyCode.T))
        //{
        //    cursorSize += .0006f;
        //}
        //if (Input.GetKey(KeyCode.Y))
        //{
        //    cursorSize -= .0006f;
        //}

        if (makeCursReset)
        {
            cursorPos = new Vector3(0f, 0f, -cursorSize);
            makeCursReset = false;
        }
        else
        {
            cursorPos = nextCursorPos;
        }
        Vector3 outCursor = cursorPos * cursorPosMod; //modifier to match tag vals (was 180)
        Debug.Log("Cursor Info: " + cursorPos + ", *Mod: " + outCursor + ", Size: " + cursorSize);

        Debug.Log("LRUD Cursor: " + moveCursorL + "/" + moveCursorR + "/" + moveCursorU + "/" + moveCursorD); // log info on what can and cannot move
        Debug.Log("LRUD Camera: " + moveCameraL + "/" + moveCameraR + "/" + moveCameraU + "/" + moveCameraD);

        //buttons = 0;
        //if (kinectReady)
        //{
        //    buttons |= falconButtons[0] ? 1 : 0; // middle button
        //    buttons |= falconButtons[1] ? 2 : 0; // left button
        //    buttons |= falconButtons[2] ? 4 : 0; // top button
        //    buttons |= falconButtons[3] ? 2 : 0; // right button
        //}
        //else
        //{
        //    cursorPos = Input.mousePosition;
        //    buttons |= Input.GetMouseButton(1) ? 1 : 0; // right mouse button
        //    buttons |= Input.GetMouseButton(0) ? 2 : 0; // left mouse button
        //}
        //Debug.Log("Update() exit - StateManager");
    }

}

//struct InvisTag
//{
//    string name;
//    //color blue
//    Vector3 location;
//}