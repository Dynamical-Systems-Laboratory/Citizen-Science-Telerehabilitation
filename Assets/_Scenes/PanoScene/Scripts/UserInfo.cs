using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//all data stored from user
public class UserInfo //not sure if : this() is necessary
{// no ": MonoBehaviour" to make the class consistently run 
    public UserInfo(string name = "Example", string datejoined = "mm/dd/yyyy")
    {
        this.userName = name;
        this.dateJoined = datejoined;

        updateSettings();
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
    public void logData(List<GameObject> addTags, int addImage)
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
    public void changeName(string newName)
    {
        userName = newName;
    }
    public void addSession() { ++sessionsLogged; }

    //progression
    public void setLevelProgress(bool started, bool finished = false)
    {
        startedPracticeLevel = started;
        finishedPracticeLevel = finished;
    }
    public float getProgress()//outputs a %/100 of progress based on user info 
    {
        //TODO: add joycon tracking
        return (imagesCompleted.Count / MakeWordBank.imageMaterials.Length)*100f + 10f;
    }

    //user settings
    public void updateSettings()
    {
        cameraSpeed = StateManager.camSpeed;
        cursorSpeed = StateManager.cursorSpeed;
        cursorSize = StateManager.cursorSize;
    }

    //accessors
    public string getName() { return userName; }
    public string getDateJoined() { return dateJoined; }
    public string getTimeLogged()
    {
        string time = "";
        time += Mathf.Floor(timeLogged / 360) + "h ";
        time += Mathf.Floor((timeLogged % 360) / 60) + "m ";
        time += Mathf.Floor(timeLogged % 60) + "s";
        return time;
    }
    public int getLastImage() { return lastImage; }
    public int[] getProgressData()
    {
        return new int[] { lastImage, imagesCompleted.Count, tags.Count, sessionsLogged };
    }
    public float[] getSettingData()
    {
        return new float[] { cameraSpeed, cursorSpeed, cursorSize };
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
        Debug.Log("*Im: " + lastImage + ", Ims: " + imagesCompleted.Count + ", Tags: " + tags.Count + ", Sessions: " + sessionsLogged +
            ", SPract: " + startedPracticeLevel + ", Pract: " + finishedPracticeLevel); //progress data
        //Debug.Log("*Settings: " + getSettingData().ToString() + ", PractState: " + getPracticeLevelState().ToString());
    }

    //(private) variables
    private string userName;
    private string dateJoined;
    private float timeLogged = 0f;

    private List<int> imagesCompleted = new List<int>(); //list of images by index - last index'd image is most recent/present
    private int lastImage = 0; //current image the user is editing
    private List<TagInfo> tags = new List<TagInfo>();
    private int sessionsLogged = 0;

    private bool startedPracticeLevel = false; //tracks basic progress
    private bool finishedPracticeLevel = false;

    private float cameraSpeed; //personalized settings 
    private float cursorSpeed;
    private float cursorSize;
}
