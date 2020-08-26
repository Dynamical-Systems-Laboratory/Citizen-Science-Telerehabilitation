using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//all data stored from user
public class UserInfo //not sure if : this() is necessary
{// no ": MonoBehaviour" to make the class consistently run 
    public UserInfo(string name = "ExampleName", string datejoined = "mm/dd/yyyy")
    {
        this.userName = name;
        this.dateJoined = datejoined;

        updateDifficulty(difficulty);
    }

    private struct TagInfo //all tag related info needed to reload and track progress
    {
        public TagInfo(string newName, Vector3 newLocation, int associatedImage) : this()
        {
            this.name = newName;
            this.location = newLocation;
            this.image = associatedImage;
        }
        public string name;
        public Vector3 location;
        public int image; //associated image index
        //public Vector3 headPos;
    }

    //data
    public void logTagData(List<GameObject> addTags, int addImage)
    {
        //TODO check locational data - if bad, use (0,0,0) nextCamera offset to correct
        foreach (GameObject newTag in addTags)
        {
            TagInfo tempTag = new TagInfo(newTag.name, newTag.transform.position, addImage);
            tags.Add(tempTag);
        }
        imagesCompleted.Add(addImage);
    }
    public void setNewImage(int newImage) { lastImage = newImage; }

    public void logJoin()
    {
        dateJoined = System.DateTime.Now.ToString("MM/dd/yyyy");
    }
    public void logTime(float toAdd) //UserInfo.logTime(Time.deltaTime);
    {
        timeLogged += toAdd;
    }
    public void addSession()
    { //can be only time but atm, is date and time (tracking frequency of patient doing excercises)
        sessionsLogged.Add(System.DateTime.Now.ToString());
        startTime = timeLogged;
    }
    public void addDuration()
    {
        sessionDuration.Add(timeLogged - startTime);
    }

    public void addMovementBounds(float[] moves, float[] times)
    {
        for (int i = 0; i < 5; i++)
        {
            Debug.Log("Adding Movement2, ex: " + moves[i] + ", " + times[i]);
            movementBounds[i] = moves[i];
            movementTime[i] = times[i];
        }
    }
    public void addMovement(float elapseTime, string systemTime, Transform head, Vector3 rightHandp, Vector3 rightHandr, Vector3 leftHandp, Vector3 leftHandr)
    {
        MovementData moves = new MovementData(elapseTime, systemTime, head.position, head.rotation.eulerAngles,
            rightHandp, rightHandr, leftHandp, leftHandr);
        //Debug.Log("MoveData: " + String.Join(",", moves.write()));
        movements.Add(moves);
    }
    public void moveDataConfirm()
    {
        if (movements.Count > 0)
        {
            Debug.Log("Last MoveData: " + String.Join(",", movements[movements.Count - 1].write()));
        }
    }
    private class MovementData
    {
        public struct UserPositions
        {
            Vector3 position;
            Vector3 rotation;
            public UserPositions(Vector3 p = new Vector3(), Vector3 r = new Vector3())
            {
                position = p;
                rotation = r;
            }
            public IEnumerable<String> write()
            {
                yield return position.x.ToString(decimalPlaces); //3 decimal places
                yield return position.y.ToString(decimalPlaces);
                yield return position.z.ToString(decimalPlaces);
                yield return rotation.x.ToString(decimalPlaces);
                yield return rotation.y.ToString(decimalPlaces);
                yield return rotation.z.ToString(decimalPlaces);
            }
        }
        UserPositions head;
        UserPositions leftHand;
        UserPositions rightHand;
        float elapseTime;
        string systemTime;
        public MovementData(float newElapseTime = 0f, string newSystemTime = "", Vector3 p1 = new Vector3(), Vector3 r1 = new Vector3(),
                      Vector3 p2 = new Vector3(), Vector3 r2 = new Vector3(),
                      Vector3 p3 = new Vector3(), Vector3 r3 = new Vector3() )
        {
            elapseTime = newElapseTime;
            systemTime = newSystemTime;
            head = new UserPositions(p1, r1);
            rightHand = new UserPositions(p2, r2);
            leftHand = new UserPositions(p3, r3);
        }
        public IEnumerable<String> write()
        {
            yield return elapseTime.ToString(decimalPlaces);
            yield return systemTime;
            //yield return head.write().SelectMany(x => string);
            foreach (String word in head.write())
            {
                yield return word;
            }
            foreach (String word in rightHand.write())
            {
                yield return word;
            }
            foreach (String word in leftHand.write())
            {
                yield return word;
            }
        }
    }

    public void setName(string newName)
    {
        userName = newName;
    }
    public void addName(string newName)
    {
        userName += newName;
    }
    public bool hasName()
    {
        return userName != "ExampleName";
    }
    public void popName()
    {
        userName = userName.Remove(userName.Length-1);
    }

    //progression
    public void setLevelProgress(bool started, bool finished = false)
    {
        startedPracticeLevel = started;
        finishedPracticeLevel = finished;
    }
    public int getProgress()//outputs a %/100 of progress based on user info 
    {
        //TODO: add joycon tracking
        float progress = ((imagesCompleted.Count*65) / MakeWordBank.imageMaterials.Length); //70% relies on image completion
        //assuming 4ish tags are ideally placed per image
        progress += ((tags.Count*25f) / (MakeWordBank.imageMaterials.Length * ((difficulty+4)/2))); //25% relies on number of tags placed
        //10% relies on doing tutorials and practice lvl
        if (startedPracticeLevel) //ppt + tutorial
        {
            progress += 7;
        }
        if (finishedPracticeLevel) // practice level
        {
            progress += 3;
        }
        return (int)progress;
    }

    //user settings
    public void updateSettings()
    {
        cameraSpeed = StateManager.camSpeed;
        cursorSpeed = StateManager.cursorSpeed;
        cursorSize = StateManager.cursorSize;
    }
    public void updateDifficulty(float newDiff)
    {
        difficulty = newDiff;
        newDiff = (7-newDiff) / 9;
        StateManager.camSpeed = 2.3f * (1 + newDiff);
        StateManager.cursorSpeed = 3.75f * (1 + newDiff);
        updateSettings();
    }

    //accessors
    public string getName() { return userName; }
    public string getDateJoined() { return dateJoined; }
    public bool hasJoined() { return dateJoined != "mm/dd/yyyy"; }
    public string getTimeLogged()
    {
        string time = "";
        time += Mathf.Floor(timeLogged / 360) + "h ";
        time += Mathf.Floor((timeLogged % 360) / 60) + "m ";
        time += Mathf.Floor(timeLogged % 60) + "s";
        return time;
    }
    public int getAvgSessionDuration()
    {
        if (sessionDuration.Count == 0)
        {
            return 0;
        }

        float avg = 0;
        foreach(float sess in sessionDuration)
        {
            avg += sess;
        }
        avg /= sessionDuration.Count;
        return (int)avg;
    }
    public float getMovementBounds(int index)
    {
        switch (index)
        {
            case 1:
                return movementBounds[0];
            case 2:
                return movementBounds[1];
            case 3:
                return movementBounds[2];
            case 4:
                return movementBounds[3];
            case 5:
                return movementBounds[4];
            case 6:
                return movementTime[0];
            case 7:
                return movementTime[1];
            case 8:
                return movementTime[2];
            case 9:
                return movementTime[3];
            case 10:
                return movementTime[4];
            default:
                Debug.LogError("movement bounds error");
                return 0;
        }
    }
    public string formattedMoveBounds(float modifier = 1f, bool isNotTime = true)
    {
        string outStr = "";
        if (isNotTime)
        {
            outStr += "x:[" + (movementBounds[0] * modifier).ToString("0.00") + "," + (movementBounds[1] * modifier).ToString("0.00") + "], ";
            outStr += "y:[" + (movementBounds[2] * modifier).ToString("0.00") + "," + (movementBounds[3] * modifier).ToString("0.00") + "], ";
            outStr += "z: [" + (movementBounds[4] * modifier).ToString("0.00") + "]";
        }
        else
        {
            outStr += "xT:[" + (movementTime[0] * modifier).ToString("0.00") + "," + (movementTime[1] * modifier).ToString("0.00") + "], ";
            outStr += "yT:[" + (movementTime[2] * modifier).ToString("0.00") + "," + (movementTime[3] * modifier).ToString("0.00") + "], ";
            outStr += "z: [" + (movementTime[4] * modifier).ToString("0.00") + "]";
        }
        return outStr;
    }

    public int getLastImage() { return lastImage; }
    public int[] getCompletionData()
    {
        return new int[] { lastImage, imagesCompleted.Count, tags.Count, sessionsLogged.Count };
    }
    public float[] getSettingData()
    {
        return new float[] { difficulty, cameraSpeed, cursorSpeed, cursorSize };
    }
    public bool[] getPracticeLevelState()
    {
        return new bool[] { startedPracticeLevel, finishedPracticeLevel };
    }
    
    public IEnumerable<GameObject> getTags(int image)
    {
        foreach (TagInfo tagInform in tags)
        {
            if (tagInform.image == image)
            {
                GameObject tag = new GameObject(tagInform.name);
                tag.transform.position = tagInform.location;
                tag.transform.localScale -= new Vector3(.88f, .88f, .88f);
                //TODO: make it look like a normal tag (cleanup with MakeWordBank as well)
                yield return tag;
            }
        }
        //yield return null;
    }

    public void show()
    {
        Debug.Log("*User: " + userName + ", Time: " + getTimeLogged() + ", Date Joined: " + dateJoined);
        Debug.Log("*LIm: " + lastImage + ", Ims: " + imagesCompleted.Count + ", Tags: " + tags.Count + ", Sessions: " + sessionsLogged.Count +
            ", SPract: " + startedPracticeLevel + ", FPract: " + finishedPracticeLevel); //progress data
        //Debug.Log("*Settings: " + getSettingData()[0] + " " + getSettingData()[1] + " " + getSettingData()[2] + " " + getSettingData()[3] +
            //", PractState: " + boolToString(getPracticeLevelState()[0]) + " " + boolToString(getPracticeLevelState()[1]) + ", AvgSess: " + getAvgSessionDuration());
    }
    public void showMoveBounds()
    {
        Debug.Log("Move Bounds: (" + string.Join(", ", movementBounds) + "), (" + string.Join(", ", movementTime) + ")");
    }

    //data usage (reading/writing)
    public IEnumerable<string> writeMainData()
    {
        yield return "*Basic_Data*\n";
        yield return "User_Name,Date_Joined,Time_Logged,Started_PL, Finshed_PL,Difficulty,Last_Image\n"; //formatting
        yield return userName;
        yield return dateJoined;
        yield return ((int)timeLogged).ToString(); //TODO: check if this int yields error
        yield return boolToString(startedPracticeLevel);
        yield return boolToString(finishedPracticeLevel);
        yield return difficulty.ToString();
        yield return lastImage.ToString();

        yield return "\n*Completed_Images*\n";
        for(int i = 0; i < imagesCompleted.Count; i++) //formatting
        {
            yield return "Image#";
        }
        yield return "\n";
        foreach (int image in imagesCompleted)
        {
            yield return image.ToString();
        }

        yield return "\n*Movement_Bounds*\n";
        yield return "Offset_XLeft,Time_XLeft,Offset_XRight,Time_XRight,Offset_YDown,Time_YDown,Offset_YUp,Time_YUp,Offset_ZForward,Time_ZForward\n"; //formatting
        for (int i = 0; i < movementBounds.Length; i++) //always 8 entries
        {
            yield return movementBounds[i].ToString(decimalPlaces);
            yield return movementTime[i].ToString(decimalPlaces);
        }

        yield return "\n*Placed_Tags*\n"; //marker to collect tag info
        yield return "Tag_Name,TagX,TagY,TagZ,Tag_Image#\n"; //formatting
        foreach (TagInfo tag in tags)
        {
            yield return tag.name;
            yield return tag.location.x.ToString(decimalPlaces);
            yield return tag.location.y.ToString(decimalPlaces);
            yield return tag.location.z.ToString(decimalPlaces);
            yield return tag.image.ToString() + "\n";
        }

        yield return "\n*Sessions*\n";
        yield return "Date_Time,Duration\n";
        for (int i = 0; i < sessionsLogged.Count; i++)
        {
            yield return sessionsLogged[i];
            yield return sessionDuration[i].ToString(decimalPlaces) + "\n";
        }
        yield return "**finish**"; //end marker
    }
    public IEnumerable<string> writeMovementData()
    {
        yield return "*" + userName + "'s Movement Data*\n";
        yield return "Elapsed_Time,System_Time";

        string limb = "Head";
        while (limb != "done") //formatting
        {
            yield return limb + "_PosX," + limb + "_PosY," + limb + "_PosZ," + limb + "_RotX," + limb + "_RotY," + limb + "_RotZ";
            if (limb == "Head")
            {
                limb = "RHand";
            }
            else if (limb == "RHand")
            {
                limb = "LHand";
            }
            else if (limb == "LHand")
            {
                limb = "done";
            }
        }
        yield return "\n";

        foreach (MovementData move in movements)
        {
            foreach (String moveData in move.write()) //9
            {
                yield return moveData;
            }
            yield return "\n";
        }
        yield return "**finish**"; //end marker
    }

    public bool readData(string[] data) //array version
    { //TODO: fix for spacing
        if (data.Length < 10) //if no data then assume default vals
        {
            return false;
        }
        //general info
        userName = data[0];
        dateJoined = data[1];
        timeLogged = float.Parse(data[2]);
        startedPracticeLevel = stringToBool(data[3]);
        finishedPracticeLevel = stringToBool(data[4]);
        difficulty = int.Parse(data[5]);
        lastImage = int.Parse(data[6]);

        int counter = 7;
        imagesCompleted.Clear(); //saftey
        while (data[counter] != "tag")
        { //adding images
            imagesCompleted.Add(int.Parse(data[counter]));
            ++counter;
        }
        ++counter; //after "tag"

        tags.Clear(); //saftey
        while (data[counter] != "session")
        {                                //tag data
            TagInfo tag = new TagInfo(
                data[counter],
                new Vector3(float.Parse(data[counter + 1]), float.Parse(data[counter + 2]), float.Parse(data[counter + 3])),
                int.Parse(data[counter + 4]));
            tags.Add(tag);
            counter += 5;
        }
        ++counter; //after "session"

        sessionsLogged.Clear(); //saftey
        sessionDuration.Clear(); //saftey
        while (data[counter] != "movement") //session data
        { //adding sessions
            sessionsLogged.Add(data[counter]);
            sessionDuration.Add(float.Parse(data[counter + 1]));
            counter += 2;
        }
        counter++;

        for (int i = counter; i < (counter + 4); i++) //movement bounds stuff
        {
            movementBounds[i - counter] = float.Parse(data[i]);
            movementTime[i - counter + 4] = float.Parse(data[i]);
        }
        counter += 8;
        //actual movement data
        /*movements.Clear(); //saftey
        while (data[counter] != "finish")
        {
            MovementData newMoveEntry = new MovementData( //alternative pos & rot, vector3's
                new Vector3(float.Parse(data[counter]), float.Parse(data[counter + 1]), float.Parse(data[counter + 2])),
                 new Vector3(float.Parse(data[counter + 3]), float.Parse(data[counter + 4]), float.Parse(data[counter + 5])),
                new Vector3(float.Parse(data[counter + 6]), float.Parse(data[counter + 7]), float.Parse(data[counter + 8])),
                 new Vector3(float.Parse(data[counter + 9]), float.Parse(data[counter + 10]), float.Parse(data[counter + 11])),
                new Vector3(float.Parse(data[counter + 12]), float.Parse(data[counter + 13]), float.Parse(data[counter + 14])),
                 new Vector3(float.Parse(data[counter + 15]), float.Parse(data[counter + 16]), float.Parse(data[counter + 17]))
                );
            movements.Add(newMoveEntry);
            counter += 18;
        }*/ //TODO: Fix for spacing in reading data
        //counter++;
        return true;
    }

    private string boolToString(bool b)
    {
        if (b)
        {
            return "1";
        }
        else
        {
            return "0";
        }
    }
    private bool stringToBool(string s)
    {
        if (s == "1")
        {
            return true;
        }
        else if (s == "0")
        {
            return false;
        }
        throw new Exception("String To Bool Input Error");
    }

    //(private) variables
    private string userName;
    private string dateJoined;
    private float timeLogged = 0f; //time spent in interface
    private List<string> sessionsLogged = new List<string>(); //dates of sessions joined
    private List<float> sessionDuration = new List<float>(); //duration of each session
    private float startTime = 0f; //helper ^

    private bool startedPracticeLevel = false; //tracks basic progress
    private bool finishedPracticeLevel = false;

    private List<int> imagesCompleted = new List<int>(); //list of images by index - last index'd image is most recent/present
    private int lastImage = 0; //current image the user is editing
    private List<TagInfo> tags = new List<TagInfo>(); //in case the user wants to access their past tagged images we save all the tag infos

    private float cameraSpeed; //personalized settings 
    private float cursorSpeed;
    private float cursorSize;
    private float difficulty = 5;

    private float[] movementBounds = new float[] { 0f, 0f, 0f, 0f, 0f }; //-x, x,-y, y, z
    private float[] movementTime = new float[] { 0f, 0f, 0f, 0f, 0f }; //can also be new float[5]?
    private List<MovementData> movements = new List<MovementData>();
    //compulsory movement tracker?

    private static string decimalPlaces = "0.00000";//data collection significant figures (5 atm)
}
