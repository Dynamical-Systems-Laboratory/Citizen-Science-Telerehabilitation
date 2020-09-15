using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

using System.IO;
using System.Linq;
using UnityEngine.SocialPlatforms;

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

    public UserInfo user = new UserInfo(); //main user to store info
    public VRUser complexUser; //user's main body

    private GameObject selected = null; // The current tag being selected
    private GameObject falconCursor; // The cursor being manipulated by the falcon

    private Camera falconCamera;

    public static bool[] falconButtons = new bool[4];

    //private int buttons; // bit 1 is the pan button, bit 2 is the click button, bit 3 is the top button of the falcon

    public static bool kinectReady = false;

    //private float pushCoolDown = 0f;

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

   // private float speedInitializeTime = 0f;

    //private float avgDistance_x = 0f;
    //private float avgDistance_y = 0f;

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

    public bool makeCursReset = false; // resets the position of the cursor to the middle of the screen
    public static bool makeCamReset = false; // resets the position of the camera to the middle of the image/starting pt.
    public static bool allSystemsGo = false; // variable that enables all movement functionality for camera and cursor upon update()
    public Vector3 cursorAdd = new Vector3(0f, 0f, 0f);
    //public static Vector3 cameraAdd = new Vector3(0f, 0f, 0f);

    public static float cursorPosMod = 239.36f; //modifier for sphere to ui translation

    public static GameObject mainCamera;
    public bool cameraMoving;
    public static Vector3 absRotation; //rotation of camera without lowest abs val conversion

    public float xOffset = 25.9f; //factor that sets tags to dissapear after being a certain dist away from camera's center

    public static float camSpeed = 2.3f; //factor that speeds up the camera's movement
    public static float cursorSpeed = 3.75f; //factor that speeds up cursor's movement
    public static float cursorSize = -0.418f; //factor that makes cursor bigger or smaller

    //reimplementation of cursor movement controlling for VR
    public bool cursorXMove = true;
    public bool cursorYMove = true;

    //TODO: Make tag count update as game goes on rather than after image is completed
    public List<GameObject> tagsPlaced = new List<GameObject>();
    //public List<InvisTag> invisTags;

    public static bool newUser = true; // whether or not data was read
    //TODO: makeNewUser = false; --> when ready to fix reading data issues
    public bool makeNewUser = true; // used to bypass reading data if you want to create another save file (true for testing)
    private string path;
    private string path2;
    private static string dataName = "main_data"; //name of the file
    private static string data2Name = "move_data";
    public static string[] dataRead = new string[] { "no data" };

    public bool reloading = false; //covers edge case with reloading tags

    public static GameObject trueCursor;
    public static GameObject cursorOffset;

    private static int gameCull;
    private static int tutorialCull;
    private static int homeCull;
    private static int profileCull;
    //public static int cursorCull;

    public bool userControlActive = false; //bool that controls the reset mechanic for the cursor, aka cursorNotLocked

    public bool userIsClicking = false; //getbutton - isClicking(true)
    public bool userClick = false; //getbuttondown

    private int userState = 6;
    /* 0 = Quit
     * 1 = Home
     * 2 = In-Game
     * 3 = Profile
     * 4 = Calibration
     * 5 = Button Tutorial
     * 6 = About Project
     * 7 = Practice Level
     * 8 = Survey
     */
     //Mock order: 6,4,5,7,1,0,1,7,2,1,3,1,2,1,0
     //distance from baseline of head anchors affecting the amount of change rather than the difference...
    public int getState()
    {
        return userState;
    }
    public void setState(int newState)
    {
        if (isGaming(true)) //if current state is in game, destroy tags
        {
            ClickAction.destroyTags();
        }
        userState = newState;
        updateState();
        makeCursReset = true;
        makeCamReset = true;
    }
    public void updateState()
    {
        //defaults*
        MakeWordBank.cursorCamera.SetActive(true);
        MakeWordBank.UICamera.SetActive(true);
        MakeWordBank.videoCamera.SetActive(false);
        MakeWordBank.mainCamera.SetActive(false);

        MakeWordBank.UICamera.GetComponent<Camera>().cullingMask = gameCull;
        //GameObject.Find("SimpleTutorialCanvas").SetActive(true);
        //complexUser.VRPerson.SetActive(true);

        userControlActive = false;
        makeCursReset = true;

        if (isGaming(true))
        {
            //GameObject.Find("tagCanvas").GetComponent<Canvas>().overrideSorting = true;  //failsafe for seeing tags?
            loadTags(); //load in tags
        }

        switch (userState)
        {
            case 0: //QUIT
                //complexUser.VRPerson.SetActive(false);
                MakeWordBank.cursorCamera.SetActive(false);
                MakeWordBank.UICamera.SetActive(false);
                user.updateSettings();
                user.addDuration();

                //RePathing (maybe make a folder)
                string nowStamp = "_" + System.DateTime.Now.ToString("MM-dd-y_HH.mm.ss"); //"/" & ":" not allowed in address
                path = Application.dataPath + "/" + dataName + nowStamp + ".csv"; //"_" + System.DateTime.Now.ToString("MM/dd/y_HH:mm:ss")
                path2 = Application.dataPath + "/" + data2Name + nowStamp + ".csv";

                //WRITING DATA CODE
                StreamWriter writer;
                StreamWriter writer2;
                if (newUser) //if new user or no data detected
                {
                    writer = System.IO.File.CreateText(path); //create new user_data file
                    Debug.Log("File Created at " + path);
                    //writer.WriteLine("UserName,DateJoined,TimeLogged,StartedPL,FinishedPL,Difficulty,LastImage,ImageData,,TagData,,SessionData,,finish");
                }
                else
                {
                    writer = new StreamWriter(path, false);// overwrites insead of append = false
                    //TODO: convert to try catch
                }
                writer2 = System.IO.File.CreateText(path2);
                Debug.Log("File Created at " + path2);

                Debug.Log("Data Writting...");
                foreach (string data in user.writeMainData()) //or String.Join(",", enum)
                {
                    writer.Write(data + ","); //comma separated value file = csv
                    //Debug.Log("Data: " + data);
                }
                foreach (string data in user.writeMovementData())
                {
                    writer2.Write(data + ",");
                    //Debug.Log("Data2: " + data);
                }
                writer.Write("\n"); //indent for new data (delete previous data in another step)
                Debug.Log("Data Finished Writting...");
                writer.Close();
                writer2.Close();

                //ClickAction.destroyTags();
                //UnityEditor.EditorApplication.isPlaying = false; //for editing only
                Application.Quit();
                break;

            case 1: //HOME
                MakeWordBank.UICamera.GetComponent<Camera>().cullingMask = homeCull;
                break;
            case 2: //GAME
                MakeWordBank.nextImage(MakeWordBank.imageIndex);
                //GameObject.Find("gameText").GetComponent<Text>().text = "Game Screen";
                break;
            case 3: //PROFILE
                MakeWordBank.UICamera.GetComponent<Camera>().cullingMask = profileCull;
                break;
            case 4: //CALIBRATE (simpletutorial)
                MakeWordBank.mainCamera.SetActive(true); //videos?
                MakeWordBank.UICamera.GetComponent<Camera>().cullingMask = tutorialCull;
                break;
            case 5: //TUTORIAL
                //tutorial tags
                break;
            case 6: //ABOUT PROJECT
                MakeWordBank.cursorCamera.SetActive(false);
                MakeWordBank.UICamera.SetActive(false);
                break;
            case 7: //PRACTICE LEVEL
                //GameObject.Find("gameText").GetComponent<Text>().text = "Practice Screen";
                break;
            case 8: //Survey
                break;
            default: //STATE ERROR
                //complexUser.VRPerson.SetActive(false);
                break;
        }
    }

    public GameObject getSelected()
    {
        return selected;
    }

    public void setSelected(GameObject g)
    {
        selected = g;
    }

    public Vector3 getCursorPosition(bool isLocal = true)
    {
        if (isLocal)
        {
            return trueCursor.transform.localPosition; //return cursorPos;
        }
        else
        {
            return trueCursor.transform.position;
        }
        
    }

    public Vector3 getCameraPosition()
    {
        return cameraPos; //GameObject.Find("CenterEyeAnchor").transform.rotation.eulerAngles;
    }

    /*public int getButtons()
    {
        return buttons;
    }*/

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

    public bool isGaming(bool restricted = false)
    {
        if (!restricted)
        {
            return userState == 2 || userState == 5 || userState == 7;
        }
        else
        {
            return userState == 2 || userState == 7;
        }
    }

    public void loadTags(int image = -1) //loadTags(user.getLastImage())
    {
        if (image < 0)
        {
            image = user.getLastImage();
        }
        foreach (GameObject tag in user.getTags(image))
        {
            Debug.Log("Importing tag: " + tag.name);
            GameObject newTag = Instantiate(GameObject.Find("tagRef"), ClickAction.canvas.transform);
            newTag.GetComponentInChildren<Text>().color = Color.blue;
            newTag.name = tag.name;
            newTag.tag = "Untagged";
            newTag.transform.transform.SetParent(GameObject.Find("tagCanvas").transform); //saftey
            newTag.layer = 16;//4;
            newTag.transform.position = tag.transform.position;
            newTag.transform.localScale = ClickAction.tagDownScale;
            tagsPlaced.Add(newTag);
        }
    }

    public int numTagsToPlace()
    {
        switch (user.getSettingData()[0])
        {
            case 1:
                return 0;
            case 2:
                return 0;
            case 3:
                return 1;
            case 4:
                return 1;
            case 5:
                return 2;
            case 6:
                return 2;
            case 7:
                return 3;
            case 8:
                return 3;
            case 9:
                return 4;
            case 10:
                return 5;
            default:
                return 0;
        }
    }

    void Awake()
    {
        //READING DATA CODE
        path = Application.dataPath + dataName + ".csv"; //MainData
        //path = Application.dataPath + dataName + "_" + System.DateTime.Now.ToString() + ".csv";
        //path2 = Application.dataPath + data2Name + "_" + System.DateTime.Now.ToString() + ".csv";
        if (System.IO.File.Exists(path) && !makeNewUser)
        {
            StreamReader reader = new StreamReader(path);
            dataRead = reader.ReadLine().Split(','); //array
            newUser = !user.readData(dataRead);
            reloading = newUser;
        }

        /* List <--> Array conversions
         * List<string> list = new List<string>(arr);
         * string[] arr = list.ToArray()
         */

        falconButtons = new bool[4] { false, false, false, false };
        speeds = new List<Tuple<float, float, float, float>>();
        mainCamera = GameObject.Find("Main Camera");
        trueCursor = GameObject.Find("exampleCursor");
        cursorOffset = GameObject.Find("cursorCenter");

        if (user.getPracticeLevelState()[0])
        {//necessary for camera stuff
            //userState = 1;
            setState(1);
            //TODO: disable canvases (welcome screen, help text, and simple tutorial stuff)
            GameObject.Find("SimpleTutorialCanvas").SetActive(false);
            MakeWordBank.stepOfTutorial = 24;
            MakeWordBank.welcomeScreen.SetActive(false);
        }

        cursorPos = GameObject.Find("exampleCursor").transform.position;

        //culling masks
        gameCull = GameObject.Find("UICamera").GetComponent<Camera>().cullingMask; //present culling mask with tag/trash/ui/bin/visibleTags
        tutorialCull = (1 << LayerMask.NameToLayer("Tutorial"));
        homeCull = (1 << LayerMask.NameToLayer("Home"));
        profileCull = (1 << LayerMask.NameToLayer("Profile"));
        //cursorCull = GameObject.Find("CursorCamera").GetComponent<Camera>().cullingMask;
    }
    /*private IEnumerator Start()
    {
        yield return new WaitForSeconds(5f);
        this.falconCursor = GameObject.Find("CursorSphere");
        this.falconCamera = GameObject.Find("CursorCamera").GetComponent<Camera>();
        //mainCamera = GameObject.Find("Main Camera");
        //mainCamera = MakeWordBank.mainCamera;
    }*/

    private static bool stateInit = false;
    private void Update()
    {
        if (userState == 0)
        {
            Debug.Log("****************************Game Should Be Closed***************************");
        }
        else
        {
            if (!stateInit)
            {
                updateState();
                stateInit = true;
            }
            //updateState(); //testing...**
            Debug.Log("isNewUser: " + newUser.ToString() + ", data: " + dataRead[0]);
            switch (userState)
            {
                case 0:
                    Debug.Log("State: Quit");
                    break;
                case 1:
                    Debug.Log("State: Home");
                    break;
                case 2:
                    Debug.Log("State: Game");
                    break;
                case 3:
                    Debug.Log("State: Profile");
                    break;
                case 4:
                    Debug.Log("State: Calibrating");
                    break;
                case 5:
                    Debug.Log("State: Button Tutorial");
                    break;
                case 6:
                    Debug.Log("State: About Project");
                    break;
                case 7:
                    Debug.Log("State: Practice Level");
                    break;
                case 8: //Survey
                    Debug.Log("State: Survey");
                    break;
                default:
                    Debug.Log("State: Error " + userState);
                    break;
            }

            /*if (allSystemsGo)
            {
                moveCursorL = true; // cursors
                moveCursorR = true;
                moveCursorU = true;
                moveCursorD = true;
                moveCameraL = true; // cameras
                moveCameraR = true;
                moveCameraU = true;
                moveCameraD = true;
                //makeCursReset = false;
                //makeCamReset = false;
            }*/

            kinectReady = true;
            falconButtons[1] = false;
            /*rodClicked = false;
            if ((Input.GetKey(KeyCode.V) && VRUser.extraControls) || VRUser.isClicking()) //new update - forwards and backwards
            {
                rodClicked = true;
            }*/

            cameraL = false;
            cameraR = false;
            cameraU = false;
            cameraD = false;

            cameraUpCoolDown += Time.deltaTime;
            cameraDownCoolDown += Time.deltaTime;

            //TODO: Set cameraPos to orientation of oculus
            cameraPos = GameObject.Find("CenterEyeAnchor").transform.rotation.eulerAngles;
            nextCameraPos = cameraPos;

            if (makeCamReset) //cam reset method
            {
                //nextCameraPos = new Vector3(0f, 0f, 0f);
                absRotation = nextCameraPos;
                makeCamReset = false;
            }

            /*foreach (TagPlaced obj in tagsPlaced)
            {
                obj.tag.transform.position -= (cameraPos - obj.headPos);
            }*/

            //nextCameraPos.y = getLowestAngle(nextCameraPos.y); //reset x pos if goes too far
            /*if (cameraMoving)
            {
                foreach (GameObject obj in tagsPlaced)
                {
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
            }*/
            Debug.Log("Camera Info: " + getCameraPosition() + ", abs: " + absRotation + ", speed: " + camSpeed);
            cameraPos = nextCameraPos;

            cursorL = false;
            cursorR = false;
            cursorU = false;
            cursorD = false;

            //float keyspeed = 0.07f * cursorSpeed; //x
            //float keyspeed2 = .00016f * cursorSpeed; //y
            nextCursorPos = cursorPos;

            if (moveCursorL)
            {
                //nextCursorPos = new Vector3(cursorPos.x + Input.GetAxis("Horizontal")* keyspeed, cursorPos.y, 0.418f);
                //cursorL = true;
                if (Input.GetKey(KeyCode.LeftArrow) && VRUser.extraControls)
                {
                    nextCursorPos += new Vector3(-.06f * cursorSpeed * Time.deltaTime, 0f, 0f);
                    cursorL = true;
                }
            }

            if (moveCursorR)
            {
                if (Input.GetKey(KeyCode.RightArrow) && VRUser.extraControls)
                {
                    nextCursorPos += new Vector3(.06f * cursorSpeed * Time.deltaTime, 0f, 0f);
                    cursorR = true;
                }
            }

            if (moveCursorU)
            {
                if (Input.GetKey(KeyCode.UpArrow) && VRUser.extraControls)
                {
                    nextCursorPos += new Vector3(0f, .048f * cursorSpeed * Time.deltaTime, 0f);
                    cursorU = true;
                }
            }

            if (moveCursorD)
            {
                if (Input.GetKey(KeyCode.DownArrow) && VRUser.extraControls)
                {
                    nextCursorPos += new Vector3(0f, -.048f * cursorSpeed * Time.deltaTime, 0f);
                    cursorD = true;
                }
            }

            /*if (moveCursorL || moveCursorR || moveCursorU || moveCursorD || cursorAdd != new Vector3(0f, 0f, 0f))
            {
                nextCursorPos += cursorAdd;
            }
            cursorAdd = new Vector3(0f,0f,0f);*/

            //nextCursorPos.z = -cursorSize;
            //originally where cursor bounds were... - migrated to VRUser
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
            Debug.Log("Cursor Info: " + getCursorPosition() + ", Unfiltered: " + getCursorPosition(false) + ", Size: " + -cursorSize);

            //Debug.Log("LRUD Cursor: " + moveCursorL + "/" + moveCursorR + "/" + moveCursorU + "/" + moveCursorD); // log info on what can and cannot move
            //Debug.Log("LRUD Camera: " + moveCameraL + "/" + moveCameraR + "/" + moveCameraU + "/" + moveCameraD);
        }
    }
}
/*public struct TagPlaced
{
    public GameObject tag;
    public Vector3 headPos;
    public TagPlaced(GameObject obj, Vector3 pos)
    {
        tag = obj;
        headPos = pos;
    }
}*/