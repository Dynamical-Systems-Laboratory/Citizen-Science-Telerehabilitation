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
        public override string ToString()
        {
            //return base.ToString();
            return name + "[" + image + "]";// (" + location + ")";
        }
    }

    //data
    public void logTag(GameObject addTag)
    {
        //TODO check locational data - if bad, use (0,0,0) nextCamera offset to correct
        string tagName = addTag.name;
        string toDelete = "(Clone)"; // ex: "Sky(Clone)"
        if (tagName.Length > toDelete.Length && tagName.Substring(tagName.Length - toDelete.Length) == toDelete)
        {
            tagName = tagName.Substring(0,tagName.Length - toDelete.Length);
        }
        Debug.Log("Tag attempted log " + tagName);
        TagInfo tempTag = new TagInfo(tagName, addTag.transform.position, getSession().end_im);
        tags.Add(tempTag);
    }
    public void logImageDone(int addImage)
    {
        Debug.Log("Image attempted log " + addImage);
        imagesCompleted.Add(addImage);
        //sessions[0].end_im = -1; //saftey?
    }
    public void logJoin()
    {
        dateJoined = System.DateTime.Now.ToString("MM/dd/yyyy");
    }
    public void logTime(float toAdd) //UserInfo.logTime(Time.deltaTime);
    {
        timeLogged += toAdd;
    }

    public struct SessionData
    {
        public SessionData(string nowDate = "", int startingImage = 0, bool startedPracticeLevel = false, bool endedPracticeLevel = false, int newDifficulty = 5)
        {
            date = nowDate;
            duration = 0;
            start_im = startingImage;
            end_im = start_im;
            started_pl = startedPracticeLevel;
            ended_pl = endedPracticeLevel;
            difficulty = newDifficulty;
            moveBounds = new float[] { 0f, 0f, 0f, 0f, 0f };
            moveTimes = new float[] { 0f, 0f, 0f, 0f, 0f };
        }

        //helpers for accessing session stuff
       public void logEndData(int end, bool startedPracticeLevel = false, bool endedPracticeLevel = false, int newDifficulty = 5)
        {
            end_im = end;
            started_pl = startedPracticeLevel;
            ended_pl = endedPracticeLevel;
            difficulty = newDifficulty;
        }
        public void setDifficulty(int newDiff)
        {
            difficulty = newDiff;
        }
        public void setBoundaries(float[] moves, float[] times)
        {
            for (int i = 0; i < 5; i++)
            {
                Debug.Log("Adding Movement2, ex: " + moves[i] + ", " + times[i]);
                moveBounds[i] = moves[i];
                moveTimes[i] = times[i];
            }
        }
        public void setLevel(bool start, bool end)
        {
            started_pl = start;
            ended_pl = end;
        }

        public IEnumerable<string> write()
        {
            yield return date;
            yield return duration.ToString();
            yield return start_im.ToString();
            yield return end_im.ToString();
            yield return boolToString(started_pl);
            yield return boolToString(ended_pl);
            yield return difficulty.ToString();
            for(int i = 0; i < 4; i++)
            {
                yield return moveBounds[i].ToString();
                yield return moveTimes[i].ToString();
            }
        }

        public string date;
        public float duration;
        public int start_im;
        public int end_im;
        public bool started_pl;
        public bool ended_pl;
        public int difficulty;
        public float[] moveBounds; //-x, x,-y, y, z
        public float[] moveTimes; //can also be new float[5]?
    }
    private SessionData getSession()
    {
        if (sessions.Count != 0)
        {
            return sessions[sessions.Count - 1];
        }
        else
        {
            Debug.Log("***Bad session access");
            return new SessionData(); //saftey?
        }
    }

    public void logSessionEnd(int end, bool startedPracticeLevel = false, bool endedPracticeLevel = false, int newDifficulty = 5)
    {
        getSession().logEndData(end, startedPracticeLevel, endedPracticeLevel, newDifficulty);
    }

    public void addSession()
    { //can be only time but atm, is date and time (tracking frequency of patient doing excercises)
        SessionData sesh = new SessionData(System.DateTime.Now.ToString());
        sessions.Add(sesh);
        startTime = timeLogged;
    }

    public void addMovementBounds(float[] moves, float[] times)
    {
        getSession().setBoundaries(moves, times);
    }
    public void addMovement(float elapseTime, string systemTime, bool isMoving, Transform head, Vector3 rightHandp, Vector3 rightHandr, Vector3 leftHandp, Vector3 leftHandr)
    {
        MovementData moves = new MovementData(elapseTime, systemTime, isMoving, head.position, head.rotation.eulerAngles,
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
            public IEnumerable<string> write()
            {
                yield return position.x.ToString(decimalPlaces); //3 decimal places
                yield return position.y.ToString(decimalPlaces);
                yield return position.z.ToString(decimalPlaces);
                yield return rotation.x.ToString(decimalPlaces);
                yield return rotation.y.ToString(decimalPlaces);
                yield return rotation.z.ToString(decimalPlaces);
            }
        }
        
        public MovementData(float newElapseTime = 0f, string newSystemTime = "", bool isMoving = false, Vector3 p1 = new Vector3(), Vector3 r1 = new Vector3(),
                      Vector3 p2 = new Vector3(), Vector3 r2 = new Vector3(),
                      Vector3 p3 = new Vector3(), Vector3 r3 = new Vector3() )
        {
            elapseTime = newElapseTime;
            systemTime = newSystemTime;
            isMovingCursor = isMoving;
            head = new UserPositions(p1, r1);
            rightHand = new UserPositions(p2, r2);
            leftHand = new UserPositions(p3, r3);
        }
        public IEnumerable<string> write()
        {
            yield return elapseTime.ToString(decimalPlaces);
            yield return systemTime;
            if (isMovingCursor) //boolToString
            {
                yield return "1";
            }
            else
            {
                yield return "0";
            }
            //yield return head.write().SelectMany(x => string);
            foreach (string word in head.write())
            {
                yield return word;
            }
            foreach (string word in rightHand.write())
            {
                yield return word;
            }
            foreach (string word in leftHand.write())
            {
                yield return word;
            }
        }

        //(private) vars
        UserPositions head;
        UserPositions leftHand;
        UserPositions rightHand;
        float elapseTime;
        string systemTime;
        bool isMovingCursor;
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
        getSession().setLevel(started, finished);
    }
    public int getProgress()//outputs a %/100 of progress based on user info 
    {
        //TODO: add joycon tracking
        float progress = ((imagesCompleted.Count*65) / MakeWordBank.imageMaterials.Length); //70% relies on image completion
        //assuming 4ish tags are ideally placed per image
        progress += ((tags.Count*25f) / (MakeWordBank.imageMaterials.Length * ((getSession().difficulty+ 4)/2))); //25% relies on number of tags placed
        //10% relies on doing tutorials and practice lvl
        if (getSession().started_pl) //ppt + tutorial
        {
            progress += 7;
        }
        if (getSession().ended_pl) // practice level
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
        getSession().setDifficulty((int)newDiff);
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
        if (sessions.Count == 0)
        {
            return 0;
        }

        float avg = 0;
        foreach(SessionData sess in sessions)
        {
            avg += sess.duration;
        }
        avg /= sessions.Count;
        return (int)avg;
    }
    public float getMovementBounds(int index)
    {
        switch (index)
        {
            case 1:
                return getSession().moveBounds[0];
            case 2:
                return getSession().moveBounds[1];
            case 3:
                return getSession().moveBounds[2];
            case 4:
                return getSession().moveBounds[3];
            case 5:
                return getSession().moveBounds[4];
            case 6:
                return getSession().moveTimes[0];
            case 7:
                return getSession().moveTimes[1];
            case 8:
                return getSession().moveTimes[2];
            case 9:
                return getSession().moveTimes[3];
            case 10:
                return getSession().moveTimes[4];
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
            outStr += "x:[" + (getSession().moveBounds[0] * modifier).ToString("0.00") + "," + (getSession().moveBounds[1] * modifier).ToString("0.00") + "], ";
            outStr += "y:[" + (getSession().moveBounds[2] * modifier).ToString("0.00") + "," + (getSession().moveBounds[3] * modifier).ToString("0.00") + "], ";
            outStr += "z: [" + (getSession().moveBounds[4] * modifier).ToString("0.00") + "]";
        }
        else
        {
            outStr += "xT:[" + (getSession().moveTimes[0] * modifier).ToString("0.00") + "," + (getSession().moveTimes[1] * modifier).ToString("0.00") + "], ";
            outStr += "yT:[" + (getSession().moveTimes[2] * modifier).ToString("0.00") + "," + (getSession().moveTimes[3] * modifier).ToString("0.00") + "], ";
            outStr += "z: [" + (getSession().moveTimes[4] * modifier).ToString("0.00") + "]";
        }
        return outStr;
    }

    public int getLastImage() { return getSession().end_im; }
    public int[] getCompletionData()
    {
        return new int[] { getSession().end_im, imagesCompleted.Count, tags.Count, sessions.Count };
    }
    public float[] getSettingData()
    {
        return new float[] { getSession().difficulty, cameraSpeed, cursorSpeed, cursorSize };
    }
    public bool[] getPracticeLevelState()
    {
        return new bool[] { getSession().started_pl, getSession().ended_pl };
    }
    
    public IEnumerable<GameObject> getTags(int image)
    { //informational gameobject
        foreach (TagInfo tagInform in tags)
        {
            if (tagInform.image == image)
            {
                GameObject tag = new GameObject(tagInform.name);
                tag.transform.position = tagInform.location;
                //tag.transform.localScale -= new Vector3(.88f, .88f, .88f);
                //TODO: make it look like a normal tag (cleanup with MakeWordBank as well)
                yield return tag;
            }
        }
        //yield return null;
    }

    public void show()
    {
        Debug.Log("*User: " + userName + ", Time: " + getTimeLogged() + ", Date Joined: " + dateJoined);
        Debug.Log("*LIm: " + getSession().end_im + ", Ims: " + imagesCompleted.Count + ", Tags: [" + string.Join(",", tags) + "], Sessions: " + sessions.Count +
            ", SPract: " + getSession().started_pl + ", FPract: " + getSession().ended_pl); //progress data
        //Debug.Log("*Settings: " + getSettingData()[0] + " " + getSettingData()[1] + " " + getSettingData()[2] + " " + getSettingData()[3] +
            //", PractState: " + boolToString(getPracticeLevelState()[0]) + " " + boolToString(getPracticeLevelState()[1]) + ", AvgSess: " + getAvgSessionDuration());
    }
    public void showTagStuff()
    {
        Debug.Log("*[" + imagesCompleted.Count + "]Images(" + getSession().end_im + "): (" + string.Join(",", imagesCompleted) + ")\n" +
            "*[" + tags.Count + "]Tags: (" + string.Join(",", tags) + ")");
    }
    public void showMoveBounds()
    {
        Debug.Log("Move Bounds: (" + string.Join(", ", getSession().moveBounds) + "), (" + string.Join(", ", getSession().moveTimes) + ")");
    }

    //data usage (reading/writing)
    public IEnumerable<string> writeMainData()
    {
        yield return userName + "'s Basic Data:\n";
        yield return "User_Name,Date_Joined,Time_Logged\n"; //formatting
        yield return userName;
        yield return dateJoined;
        yield return ((int)timeLogged).ToString(); //TODO: check if this int yields error

        yield return "\nSession Data:\n";

        yield return "Date_Time,Duration,First_Image,Last_Image,Started_PL,Finished_PL,Difficulty," +
            "Offset_XLeft,Time_XLeft,Offset_XRight,Time_XRight,Offset_YDown,Time_YDown,Offset_YUp,Time_YUp,Offset_ZForward,Time_ZForward\n";
        foreach (SessionData sesh in sessions)
        {
            foreach (string toWrite in sesh.write())
            {
                yield return toWrite;
            }
        }

        yield return "\nImages Completed:\n";
        /*for(int i = 0; i < imagesCompleted.Count; i++) //formatting
        {
            yield return "Image#";
        }
        yield return "\n";*/
        foreach (int image in imagesCompleted)
        {
            yield return image.ToString();
        }

        yield return "\nTag_Name,TagX,TagY,TagZ,Tag_Image#\n"; //formatting
        //yield return "\n";
        foreach (TagInfo tag in tags)
        {
            yield return tag.name;
            yield return tag.location.x.ToString(decimalPlaces);
            yield return tag.location.y.ToString(decimalPlaces);
            yield return tag.location.z.ToString(decimalPlaces);
            yield return tag.image.ToString() + "\n";
        }

        
        yield return "\nfinish"; //end marker
    }
    public IEnumerable<string> writeMovementData()
    {
        yield return userName + "'s Movement Data:\n";
        yield return "Elapsed_Time,System_Time";
        yield return "Cursor_Moving"; //whether or not user intends to move cursor

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
        //yield return "\n";

        foreach (MovementData move in movements)
        {
            foreach (String moveData in move.write()) //9
            {
                yield return moveData;
            }
            yield return "\n";
        }
        yield return "finish"; //end marker
    }

    public bool readData(string[] data) //reading main data
    { //TODO: fix for spacing
        if (data.Length < 10) //if no data then assume default vals
        {
            return false;
        }
        //general info
        userName = data[0];
        dateJoined = data[1];
        timeLogged = float.Parse(data[2]);
        /*startedPracticeLevel = stringToBool(data[3]);
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
        movements.Clear(); //saftey
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

    private static string boolToString(bool b)
    {
        if (b)
        {
            return "1";
        }
        else
        {
            return "0";
        }
        //return b ? "1" : "0"; //just use ternary
    }
    private static bool stringToBool(string s)
    {
        if (s == "1")
        {
            return true;
        }
        else if (s == "0")
        {
            return false;
        }
        throw new Exception("String To Bool Input Error: " + s);
    }

    //(private) variables
    private string userName;
    private string dateJoined;
    private float timeLogged = 0f; //time spent in interface
    public List<SessionData> sessions = new List<SessionData>();
    private float startTime = 0f; //helper ^

    private List<int> imagesCompleted = new List<int>(); //list of images by index - last index'd image is most recent/present

    private List<TagInfo> tags = new List<TagInfo>(); //in case the user wants to access their past tagged images we save all the tag infos

    private float cameraSpeed; //personalized settings 
    private float cursorSpeed;
    private float cursorSize;

    private List<MovementData> movements = new List<MovementData>();
    //compulsory movement tracker?

    private static string decimalPlaces = "0.00000";//data collection significant figures (5 atm)
}
