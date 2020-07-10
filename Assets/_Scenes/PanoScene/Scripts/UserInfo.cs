using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//all data stored from user
public class UserInfo //not sure if : this() is necessary
{// no ": MonoBehaviour" to make the class consistently run 
    public UserInfo(string name = "Example Name", string datejoined = "mm/dd/yyyy")
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
    }

    //data
    public void logTagData(List<GameObject> addTags, int addImage)
    {
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
        return userName != "Example Name";
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
                tag.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                //TODO: make it look like a normal tag
                yield return tag;
            }
        }
        //yield return null;
    }

    //other
    public void show()
    {
        Debug.Log("*User: " + userName + ", Time: " + getTimeLogged() + ", Date Joined: " + dateJoined);
        Debug.Log("*LIm: " + lastImage + ", Ims: " + imagesCompleted.Count + ", Tags: " + tags.Count + ", Sessions: " + sessionsLogged.Count +
            ", SPract: " + startedPracticeLevel + ", FPract: " + finishedPracticeLevel); //progress data
        Debug.Log("*Settings: " + getSettingData()[0] + " " + getSettingData()[1] + " " + getSettingData()[2] + " " + getSettingData()[3] +
            ", PractState: " + boolToString(getPracticeLevelState()[0]) + " " + boolToString(getPracticeLevelState()[1]) + ", AvgSess: " + getAvgSessionDuration());
    }

    //data usage (reading/writing)
    public IEnumerable<string> writeData()
    {
        yield return userName;
        yield return dateJoined;
        yield return timeLogged.ToString();
        yield return boolToString(startedPracticeLevel);
        yield return boolToString(finishedPracticeLevel);
        yield return difficulty.ToString();
        yield return lastImage.ToString();
        foreach (int image in imagesCompleted)
        {
            yield return image.ToString();
        }
        yield return "tag"; //marker to collect tag info
        foreach (TagInfo tag in tags)
        {
            yield return tag.name;
            yield return tag.location.x.ToString();
            yield return tag.location.y.ToString();
            yield return tag.location.z.ToString();
            yield return tag.image.ToString();
        }
        yield return "session";
        for(int i = 0; i < sessionsLogged.Count; i++)
        {
            yield return sessionsLogged[i];
            yield return sessionDuration[i].ToString();
        }
        yield return "finish"; //end marker
    }
    public void readData(List<string> data)
    {
        if (data.Count < 10) //if no data then assume default vals
        {
            return;
        }
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
        { //adding tags
            TagInfo tag = new TagInfo( data[counter], new Vector3(float.Parse(data[counter + 1]),
                float.Parse(data[counter + 2]), float.Parse(data[counter + 3])), int.Parse(data[counter + 4]));
            tags.Add(tag);
            counter += 5;
        }
        ++counter; //after "session"

        sessionsLogged.Clear();
        sessionDuration.Clear();
        while (data[counter] != "finish")
        { //adding sessions
            sessionsLogged.Add(data[counter]);
            sessionDuration.Add(float.Parse(data[counter + 1]));
            counter += 2;
        }
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
}
