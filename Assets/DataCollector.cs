using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using UnityEngine.UI;
using System.Linq;
using UnityGoogleDrive;
using System.Text;

public class DataCollector : MonoBehaviour
{

    static int sequenceIndex = 0;
    static int imageIndex = 0;
    static int numTagsRemaining = 5;
    static int userID = 0;

    static float elapsedTime; // Running time of the app
    static string timeLog;
    static string dateLog;

    static List<float> times; // Time of each logged frame
    static List<Vector3> cameraRot; // The angle of the camera at the logged frame

	static List<TagInfo> tagInfo;

    static GameObject cam;

    static string dataPath = "Data/";
	static string userPath = "";

    static bool writtenIMUDataColumnNames = false;
    static bool writtenKinectDataColumnNames = false;
    static bool writtenAnalysisColumnNames = false;
    static bool writtenScoresColumnNames = false;


    //SURVEY ANSWERS: (Data of survey will be logged when DataCollector is activated)
    public static string ageAnswer;
	public static string genderAnswer;
	public static string englishSpeakingAnswer;
	public static string[] sliderQuestions;
	public static int[] sliderAnswers;

    public static bool startAnalyzing = false;
    static bool finished = false;

    public static List<float> elapsedTimeList = new List<float>();
    public static List<string> systemTime = new List<string>();
    public static List<float> IMUAngle = new List<float>();
    public static List<float> IMUSpeed = new List<float>();
    public static List<float> LElbowAngle = new List<float>();
    public static List<float> LElbowSpeed = new List<float>();
    public static List<float> RElbowAngle = new List<float>();
    public static List<float> RElbowSpeed = new List<float>();
    public static List<float> LArmUDAngle = new List<float>();
    public static List<float> LArmUDSpeed = new List<float>();
    public static List<float> RArmUDAngle = new List<float>();
    public static List<float> RArmUDSpeed = new List<float>();
    public static List<float> LArmLRAngle = new List<float>();
    public static List<float> LArmLRSpeed = new List<float>();
    public static List<float> RArmLRAngle = new List<float>();
    public static List<float> RArmLRSpeed = new List<float>();

    public static float IMUMean;
    //public static float IMUMax;
    public static float LElbowMean;
    //public static float LElbowMax;
    public static float RElbowMean;
    //public static float RElbowMax;
    public static float LArmUDMean;
    //public static float LArmUDMax;
    public static float RArmUDMean;
    //public static float RArmUDMax;
    public static float LArmLRMean;
    //public static float LArmLRMax;
    public static float RArmLRMean;
    //public static float RArmLRMax;

    public static float prevShoulderAngle = 0f;
    public static float prevElbowAngle = 0f;

    public static string IMU_path;
    public static string Kinect_path;



    // Use this for initialization
    public static void MakeFolder() {
        Directory.CreateDirectory(dataPath);
        userID = Directory.GetDirectories(dataPath).Length;

        userPath = dataPath + "User-" + userID + '/';
        Directory.CreateDirectory(userPath);
    }

    void Start()
    {
        cam = GameObject.Find("MainCamera");
        cameraRot = new List<Vector3>();
        times = new List<float>();
        elapsedTime = 0f;
        tagInfo = new List<TagInfo> ();

		string path = userPath + "survey.csv";

        /*
		new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Write).Close();
		StreamWriter streamWriter = new StreamWriter(path, true, Encoding.ASCII);
		streamWriter.Write ("Questions:,Answers:\n");
		string line = "Age:," + ageAnswer + "\n";
		streamWriter.Write(line);
		line = "Gender:," + genderAnswer + "\n";
		streamWriter.Write(line);
		line = "English first language?:," + englishSpeakingAnswer + "\n";
		streamWriter.Write(line);
		//line = "(1 = strongly disagree) - (2 = disagree) - (3 = somewhat disagree), 4 = neutral," +
		//	" 5 = somewhat agree, 6 = agree, 7 = strongly agree";
		//Strongly disagree -> agree questions answers:

		for (int i = 0; i < sliderQuestions.Length; i++) {
			string agreement;
			int val = sliderAnswers[i];
			if (val == 0)
				agreement = "Didn't answer";
			else if (val == 1)
				agreement = "Strongly disagree";
			else if (val == 2)
				agreement = "Disagree";
			else if (val == 3)
				agreement = "Somewhat disagree";
			else if (val == 4)
				agreement = "Neutral";
			else if (val == 5)
				agreement = "Somewhat agree";
			else if (val == 6)
				agreement = "Agree";
			else
				agreement = "Strongly agree";
			line = sliderQuestions [i] + "," + agreement + "\n";
			streamWriter.Write(line);
		}

		streamWriter.Close();
        */
    }

    // Update is called once per frame
    void Update()
    {
        if (!finished)
        {
            if (!startAnalyzing)
            { //Record all raw data first
                elapsedTime += Time.deltaTime;
                //times.Add(elapsedTime);
                //cameraRot.Add(cam.transform.localEulerAngles);

                timeLog = DateTime.Now.ToString(" HH:mm:ss.fff");
                dateLog = DateTime.Now.ToString("yyyyMMdd");

                IMU_path = userPath + dateLog + "_IMU_Data.csv";

                new FileStream(IMU_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None).Close();
                StreamWriter IMUStreamWriter = new StreamWriter(IMU_path, true, Encoding.ASCII);

                if (!writtenIMUDataColumnNames)
                {
                    //Only write this once:
                    IMUStreamWriter.Write("Elapsed_Time, System_Time, Euler_Roll, Euler_Pitch, Euler_Yaw, Gyro_Roll, Gyro_Pitch, Gyro_Yaw, Accel_x, Accel_y, Accel_z\n");
                    writtenIMUDataColumnNames = true;
                }

                string IMU_line = elapsedTime.ToString() + "," + timeLog + "," +
                    IMU.Euler_Roll.ToString() + "," + IMU.Euler_Pitch.ToString() + "," + IMU.Euler_Yaw.ToString() + "," +
                    IMU.Gyro_Roll.ToString() + "," + IMU.Gyro_Pitch.ToString() + "," + IMU.Gyro_Yaw.ToString() + "," +
                    IMU.Accel_x.ToString() + "," + IMU.Accel_y.ToString() + "," + IMU.Accel_z.ToString() + "," + "\n";

                IMUStreamWriter.Write(IMU_line);
                IMUStreamWriter.Close();

                Kinect_path = userPath + dateLog + "_Kinect_Data.csv";

                new FileStream(Kinect_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None).Close();
                StreamWriter KinectStreamWriter = new StreamWriter(Kinect_path, true, Encoding.ASCII);

                if (!writtenKinectDataColumnNames)
                {
                    //Only write this once:
                    KinectStreamWriter.Write("Elapsed_Time, System_Time, CHip_x, CHip_y, CHip_z, Spine_x, Spine_y, Spine_z, CShoulder_x, CShoulder_y, CShoulder_z, Head_x, Head_y, Head_z," +
                        "LShoulder_x, LShoulder_y, LShoulder_z, LElbow_x, LElbow_y, LElbow_z, LWrist_x, LWrist_y, LWrist_z, LHand_x, LHand_y, LHand_z," +
                        "RShoulder_x, RShoulder_y, RShoulder_z, RElbow_x, RElbow_y, RElbow_z, RWrist_x, RWrist_y, RWrist_z, RHand_x, RHand_y, RHand_z," +
                        "LHip_x, LHip_y, LHip_z, LKnee_x, LKnee_y, LKnee_z, LAnkle_x, LAnkle_y, LAnkle_z, LFoot_x, LFoot_y, LFoot_z," +
                        "RHip_x, RHip_y, RHip_z, RKnee_x, RKnee_y, RKnee_z, RAnkle_x, RAnkle_y, RAnkle_z, RFoot_x, RFoot_y, RFoot_z\n");
                    writtenKinectDataColumnNames = true;
                }

                string Kinect_line = elapsedTime.ToString() + "," + timeLog + "," +

                    Kinect.CHipPos.x.ToString() + "," + Kinect.CHipPos.y.ToString() + "," + Kinect.CHipPos.z.ToString() + "," +
                    Kinect.SpinePos.x.ToString() + "," + Kinect.SpinePos.y.ToString() + "," + Kinect.SpinePos.z.ToString() + "," +
                    Kinect.CShoulderPos.x.ToString() + "," + Kinect.CShoulderPos.y.ToString() + "," + Kinect.CShoulderPos.z.ToString() + "," +
                    Kinect.HeadPos.x.ToString() + "," + Kinect.HeadPos.y.ToString() + "," + Kinect.HeadPos.z.ToString() + "," +

                    Kinect.LShoulderPos.x.ToString() + "," + Kinect.LShoulderPos.y.ToString() + "," + Kinect.LShoulderPos.z.ToString() + "," +
                    Kinect.LElbowPos.x.ToString() + "," + Kinect.LElbowPos.y.ToString() + "," + Kinect.LElbowPos.z.ToString() + "," +
                    Kinect.LWristPos.x.ToString() + "," + Kinect.LWristPos.y.ToString() + "," + Kinect.LWristPos.z.ToString() + "," +
                    Kinect.LHandPos.x.ToString() + "," + Kinect.LHandPos.y.ToString() + "," + Kinect.LHandPos.z.ToString() + "," +

                    Kinect.RShoulderPos.x.ToString() + "," + Kinect.RShoulderPos.y.ToString() + "," + Kinect.RShoulderPos.z.ToString() + "," +
                    Kinect.RElbowPos.x.ToString() + "," + Kinect.RElbowPos.y.ToString() + "," + Kinect.RElbowPos.z.ToString() + "," +
                    Kinect.RWristPos.x.ToString() + "," + Kinect.RWristPos.y.ToString() + "," + Kinect.RWristPos.z.ToString() + "," +
                    Kinect.RHandPos.x.ToString() + "," + Kinect.RHandPos.y.ToString() + "," + Kinect.RHandPos.z.ToString() + "," +

                    Kinect.LHipPos.x.ToString() + "," + Kinect.LHipPos.y.ToString() + "," + Kinect.LHipPos.z.ToString() + "," +
                    Kinect.LKneePos.x.ToString() + "," + Kinect.LKneePos.y.ToString() + "," + Kinect.LKneePos.z.ToString() + "," +
                    Kinect.LAnklePos.x.ToString() + "," + Kinect.LAnklePos.y.ToString() + "," + Kinect.LAnklePos.z.ToString() + "," +
                    Kinect.LFootPos.x.ToString() + "," + Kinect.LFootPos.y.ToString() + "," + Kinect.LFootPos.z.ToString() + "," +

                    Kinect.RHipPos.x.ToString() + "," + Kinect.RHipPos.y.ToString() + "," + Kinect.RHipPos.z.ToString() + "," +
                    Kinect.RKneePos.x.ToString() + "," + Kinect.RKneePos.y.ToString() + "," + Kinect.RKneePos.z.ToString() + "," +
                    Kinect.RAnklePos.x.ToString() + "," + Kinect.RAnklePos.y.ToString() + "," + Kinect.RAnklePos.z.ToString() + "," +
                    Kinect.RFootPos.x.ToString() + "," + Kinect.RFootPos.y.ToString() + "," + Kinect.RFootPos.z.ToString() + "," + "\n";

                KinectStreamWriter.Write(Kinect_line);
                KinectStreamWriter.Close();
                /* Not needed if it's writing every frame:
                times.Clear();
                cameraRot.Clear();
                falconPos.Clear();
                */

                elapsedTimeList.Add(elapsedTime);
                systemTime.Add(timeLog);

                float currShoulderAngle = (shoulderAngle(Kinect.LElbowPos.z, Kinect.LElbowPos.y, Kinect.LShoulderPos.z, Kinect.LShoulderPos.y) +
                        shoulderAngle(Kinect.RElbowPos.z, Kinect.RElbowPos.y, Kinect.RShoulderPos.z, Kinect.RShoulderPos.y)) / 2;
                float currElbowAngle = (elbowAngle(Kinect.LShoulderPos.x, Kinect.LShoulderPos.y, Kinect.LShoulderPos.z, Kinect.LElbowPos.x,
                        Kinect.LElbowPos.y, Kinect.LElbowPos.z, Kinect.LHandPos.x, Kinect.LHandPos.y, Kinect.LHandPos.z) +
                        elbowAngle(Kinect.RShoulderPos.x, Kinect.RShoulderPos.y, Kinect.RShoulderPos.z, Kinect.RElbowPos.x,
                        Kinect.RElbowPos.y, Kinect.RElbowPos.z, Kinect.RHandPos.x, Kinect.RHandPos.y, Kinect.RHandPos.z)) / 2;

                if (StateManager.cameraU || StateManager.cameraD)
                { //Only record wrist angle when camera is panning up/down to ignore when the rod rotates in the user's hands
                    float ShoulderSpeed = (currShoulderAngle - prevShoulderAngle) / Time.deltaTime;
                    float ElbowSpeed = (currElbowAngle - prevElbowAngle) / Time.deltaTime;
                    IMUAngle.Add(IMU.Euler_Roll - (currElbowAngle - prevElbowAngle) - (currShoulderAngle - prevShoulderAngle));
                    IMUSpeed.Add(IMU.Gyro_Roll - ElbowSpeed - ShoulderSpeed);
                }

                prevElbowAngle = currElbowAngle;
                prevShoulderAngle = currShoulderAngle;

                LElbowAngle.Add(elbowAngle(Kinect.LShoulderPos.x, Kinect.LShoulderPos.y, Kinect.LShoulderPos.z, Kinect.LElbowPos.x,
                    Kinect.LElbowPos.y, Kinect.LElbowPos.z, Kinect.LHandPos.x, Kinect.LHandPos.y, Kinect.LHandPos.z));
                RElbowAngle.Add(elbowAngle(Kinect.RShoulderPos.x, Kinect.RShoulderPos.y, Kinect.RShoulderPos.z, Kinect.RElbowPos.x,
                    Kinect.RElbowPos.y, Kinect.RElbowPos.z, Kinect.RHandPos.x, Kinect.RHandPos.y, Kinect.RHandPos.z));
                LArmUDAngle.Add(shoulderAngle(Kinect.LElbowPos.z, Kinect.LElbowPos.y, Kinect.LShoulderPos.z, Kinect.LShoulderPos.y));
                RArmUDAngle.Add(shoulderAngle(Kinect.RElbowPos.z, Kinect.RElbowPos.y, Kinect.RShoulderPos.z, Kinect.RShoulderPos.y));
                LArmLRAngle.Add(shoulderAngle(Kinect.LElbowPos.z, Kinect.LElbowPos.x, Kinect.LShoulderPos.z, Kinect.LShoulderPos.x));
                RArmLRAngle.Add(shoulderAngle(Kinect.RElbowPos.z, Kinect.RElbowPos.x, Kinect.RShoulderPos.z, Kinect.RShoulderPos.x));
            }
            else
            { //Raw data recorded, starting analyzation
                //var bytes = System.IO.File.ReadAllBytes(IMU_path);
                //var file = new UnityGoogleDrive.Data.File { Name = dateLog + "_IMU.csv", Content = bytes };
                //UnityGoogleDrive.Helpers.CreateOrUpdateFileAtPathAsync(file, "/User-" + userID + '/' + dateLog + "_IMU.csv");

                calculateSpeed(ref LElbowAngle, ref LElbowSpeed, ref LElbowMean);
                calculateSpeed(ref RElbowAngle, ref RElbowSpeed, ref RElbowMean);
                calculateSpeed(ref LArmLRAngle, ref LArmLRSpeed, ref LArmLRMean);
                calculateSpeed(ref RArmLRAngle, ref RArmLRSpeed, ref RArmLRMean);
                calculateSpeed(ref LArmUDAngle, ref LArmUDSpeed, ref LArmUDMean);
                calculateSpeed(ref RArmUDAngle, ref RArmUDSpeed, ref RArmUDMean);

                //bytes = System.IO.File.ReadAllBytes(Kinect_path);
                //file = new UnityGoogleDrive.Data.File { Name = dateLog + "_Kinect.csv", Content = bytes };
                //UnityGoogleDrive.Helpers.CreateOrUpdateFileAtPathAsync(file, "/User-" + userID + '/' + dateLog + "_Kinect.csv");

                string Analysis_path = userPath + dateLog + "_Analysis.csv";

                new FileStream(Analysis_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None).Close();
                StreamWriter AnalysisStreamWriter = new StreamWriter(Analysis_path, true, Encoding.ASCII);

                if (!writtenAnalysisColumnNames)
                {
                    //Only write this once:
                    AnalysisStreamWriter.Write("Elapsed_Time, System_Time, Elbow_Angle_L, Elbow_Speed_L, Elbow_Angle_R, Elbow_Speed_R,"
                        + " ArmUD_Angle_L, ArmUD_Speed_L, ArmUD_Angle_R, ArmUD_Speed_R, ArmLR_Angle_L, ArmLR_Speed_L, ArmLR_Angle_R, ArmLR_Speed_R\n");
                    writtenAnalysisColumnNames = true;
                }

                for (int i = 0; i < elapsedTimeList.Count; i++)
                {

                    string Analysis_line = elapsedTimeList[i].ToString() + "," + systemTime[i] + "," +
                        //IMUAngle[i].ToString() + "," + IMUSpeed[i].ToString() + "," + LElbowAngle[i].ToString() + "," + LElbowSpeed[i].ToString() + "," +
                        LElbowAngle[i].ToString() + "," + LElbowSpeed[i].ToString() + "," +
                        RElbowAngle[i].ToString() + "," + RElbowSpeed[i].ToString() + "," + LArmUDAngle[i].ToString() + "," + LArmUDSpeed[i].ToString() + "," +
                        RArmUDAngle[i].ToString() + "," + RArmUDSpeed[i].ToString() + "," + LArmLRAngle[i].ToString() + "," + LArmLRSpeed[i].ToString() + "," +
                        RArmLRAngle[i].ToString() + "," + RArmLRSpeed[i].ToString() + "," + "\n";

                    AnalysisStreamWriter.Write(Analysis_line);

                }

                AnalysisStreamWriter.Close();

                //bytes = System.IO.File.ReadAllBytes(Analysis_path);
                //file = new UnityGoogleDrive.Data.File { Name = dateLog + "_Analysis.csv", Content = bytes };
                //UnityGoogleDrive.Helpers.CreateOrUpdateFileAtPathAsync(file, "/User-" + userID + '/' + dateLog + "_Analysis.csv");


                string Scores_path = userPath + dateLog + "_Scores.csv";

                new FileStream(Scores_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None).Close();
                StreamWriter ScoresStreamWriter = new StreamWriter(Scores_path, true, Encoding.ASCII);

                if (!writtenScoresColumnNames)
                {
                    //Only write this once:
                    ScoresStreamWriter.Write(", Wrist, Elbow_L, Elbow_R, ArmUD_L, ArmUD_R, ArmLR_L, ArmLR_R\n");
                    writtenScoresColumnNames = true;
                }

                float IMUROM = 0f;
                float IMUMax = 0f;

                if (IMUAngle.Count > 0)
                {
                    IMUAngle.Sort();
                    float maxIMUAngle = IMUAngle[Convert.ToInt32(IMUAngle.Count * 0.9)];
                    float minIMUAngle = IMUAngle[0];
                    IMUROM = maxIMUAngle - minIMUAngle;
                    IMUMax = calculateMax(IMUSpeed);
                }


                string Scores_line = "ROM" + "," + IMUROM.ToString() + "," + ROM(LElbowAngle).ToString() + "," + ROM(RElbowAngle).ToString() + "," +
                    ROM(LArmUDAngle).ToString() + "," + ROM(RArmUDAngle).ToString() + "," + ROM(LArmLRAngle).ToString() + "," + ROM(RArmLRAngle).ToString() + "\n";

                ScoresStreamWriter.Write(Scores_line);

                float IMUStotal = 0f;

                for (int i = 0; i < IMUSpeed.Count; i++)
                {
                    IMUStotal += Mathf.Abs(IMUSpeed[i]);
                }

                float IMUSmean = IMUStotal / (elapsedTimeList.Count);

                Scores_line = "Mean Angular speed" + "," + IMUSmean.ToString() + "," + LElbowMean.ToString() + "," + RElbowMean.ToString() + "," +
                    LArmUDMean.ToString() + "," + RArmUDMean.ToString() + "," + LArmLRMean.ToString() + "," + RArmLRMean.ToString() + "\n";

                ScoresStreamWriter.Write(Scores_line);


                Scores_line = "Peak Angular speed" + "," + IMUMax.ToString() + "," + calculateMax(LElbowSpeed).ToString() + "," + calculateMax(RElbowSpeed).ToString() + "," +
                    calculateMax(LArmUDSpeed).ToString() + "," + calculateMax(RArmUDSpeed).ToString() + "," + calculateMax(LArmLRSpeed).ToString() + "," + calculateMax(RArmLRSpeed).ToString() + "\n";

                ScoresStreamWriter.Write(Scores_line);

                ScoresStreamWriter.Close();

                //Send file to GoogleDrive
                //Using https://github.com/Elringus/UnityGoogleDrive
                //bytes = System.IO.File.ReadAllBytes(Scores_path);
                //file = new UnityGoogleDrive.Data.File { Name = dateLog + "_Scores.csv", Content = bytes };
                //UnityGoogleDrive.Helpers.CreateOrUpdateFileAtPathAsync(file, "/User-" + userID + '/' + dateLog + "_Scores.csv");

                finished = true;
            }
        }
    }

    public static void Flush() // Write current data to csv file
    {
		/*
        if (!falcon)
        {
            return;
        }

        //Debug.Log("FLUSHING");
		//I'm doing this in the AddTag method so it writes to memory for each tag added:
        Transform tagTransform = GameObject.Find("TagSphere").transform; // Tag Container

        string userPath = dataPath + "User-" + userID + '/';
        Directory.CreateDirectory(userPath);

        // Log tag data
        //string imgName = tagTransform.gameObject.GetComponent<Renderer>().material.name; // Name of the image file
        //string path = userPath + imgName + ".csv";
        //new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Write).Close(); // Create the file
        //StreamWriter streamWriter = new StreamWriter(path, true, Encoding.ASCII);         // Open the file
		//streamWriter.Write("posx, posy, posz, tag, time (s), userID\n");   // Write header line to the file

		
        foreach (Transform t in tagTransform)
        {
            if (t != tagTransform)
            {
                Vector3 position = t.gameObject.GetComponent<Renderer>().bounds.center;
                string line = position.x.ToString() + "," +
                              position.y.ToString() + "," + 
                              position.z.ToString() + "," +
                              t.gameObject.name + "," + 
                              userID.ToString() + "\n";
                streamWriter.Write(line);
            }
        }
		foreach (TagInfo tg in tagInfo) {
			string line = tg.position.x.ToString () + "," +
			              tg.position.y.ToString () + "," +
			              tg.position.z.ToString () + "," +
			              tg.name + "," +
						  tg.timeStamp.ToString() + "," + 
			              tg.userID.ToString () + "\n";
			streamWriter.Write (line);
		}
        streamWriter.Close(); // Close the file after writing

		

        // Log all of falcon/camera data - should this only happen during image turnover?
        string path = userPath + "panData.csv";

        new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Write).Close();
        StreamWriter streamWriter = new StreamWriter(path, true, Encoding.ASCII);
		streamWriter.Write("time (s), rotation.x, rotation.y, falcon.x, falcon.y\n");
        for (int i = 0; i < times.Count; i++)
        {
			string line = times [i].ToString () + "," +
			                       cameraRot [i].x.ToString () + "," + cameraRot [i].y.ToString ();
			if (falcon) {
				line += "," + falconPos [i].x.ToString () + "," + falconPos [i].y.ToString () + "\n";
			} else {
				line += "\n";
			}
            streamWriter.Write(line);
        }
        streamWriter.Close();
        times.Clear();
        cameraRot.Clear();
        falconPos.Clear();
		*/

		//Only part that should remain from .Flush() method called on image turnover:
		tagInfo.Clear ();
    }

	public static void AddTag(string name, Vector3 position = new Vector3()) {
		TagInfo nTagInfo;
		nTagInfo.position = position;
		nTagInfo.name = name;
		nTagInfo.userID = userID;
		nTagInfo.timeStamp = elapsedTime;
		tagInfo.Add (nTagInfo);

		//Write to memory here instead of Flush() :

		Transform tagTransform = ClickAction.sphere.transform;
		string userPath = dataPath + "User-" + userID + '/';
		Directory.CreateDirectory(userPath);

		// Log tag data
		string imgName = tagTransform.gameObject.GetComponent<Renderer>().material.name; // Name of the image file
		string path = userPath + imgName + ".csv";
		new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write).Close(); // Create the file, set to FileMode.Create so it overwrites each time
		StreamWriter streamWriter = new StreamWriter(path, true, Encoding.ASCII);         // Open the file
		streamWriter.Write("posx, posy, posz, tag, time (s), userID\n");

		foreach (TagInfo tg in tagInfo) {
			string line = tg.position.x.ToString () + "," +
				tg.position.y.ToString () + "," +
				tg.position.z.ToString () + "," +
				tg.name + "," +
				tg.timeStamp.ToString() + "," + 
				tg.userID.ToString () + "\n";
			streamWriter.Write (line);
		}
		streamWriter.Close(); // Close the file after writing
	}

    static string getResponse(int val)
    {
        string agreement;
        if (val == 0)
            agreement = "Didn't answer";
        else if (val == 1)
            agreement = "Strongly disagree";
        else if (val == 2)
            agreement = "Disagree";
        else if (val == 3)
            agreement = "Somewhat disagree";
        else if (val == 4)
            agreement = "Neutral";
        else if (val == 5)
            agreement = "Somewhat agree";
        else if (val == 6)
            agreement = "Agree";
        else
            agreement = "Strongly agree";
        return agreement;
    }

	public static void writeFinalQuestion(int val, int val2) {


		string path = userPath + "survey.csv";
        string a1 = getResponse(val);
        string a2 = getResponse(val2);

		new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Write).Close();
		StreamWriter streamWriter = new StreamWriter(path, true, Encoding.ASCII);
        streamWriter.Write("Did you enjoy the activity?," + a1 + "\n");
        streamWriter.Write("Did you think the activity was fun?," + a2);
        streamWriter.Close ();
	}

	public static void writeFinalQuestionMORE(Slider[] sliders, string age, string gender) {
		string path = userPath + "survey.csv";
		new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Write).Close();
		StreamWriter streamWriter = new StreamWriter(path, true, Encoding.ASCII);
        streamWriter.Write("Age," + age + "\n");
        streamWriter.Write("Gender," + gender + "\n");
        foreach (Slider slide in sliders) {
	        streamWriter.Write(slide.GetComponentInChildren<Text>().text + "," + slide.value + "\n");
        }
        streamWriter.Close ();
	}

    public static float elbowAngle(float a_x, float a_y, float a_z, float b_x, float b_y, float b_z, float c_x, float c_y, float c_z)
    { //Calculate the elbow angle using vector arithmetic 
        Tuple<float, float, float> AB = new Tuple<float, float, float>((b_x - a_x), (b_y - a_y), (b_z - a_z));
        Tuple<float, float, float> BC = new Tuple<float, float, float>((c_x - b_x), (c_y - b_y), (c_z - b_z));

        float ABtimesBC = (AB.Item1 * BC.Item1 + AB.Item2 * BC.Item2 + AB.Item3 * BC.Item3);

        float ABLength = Mathf.Sqrt(Mathf.Pow(AB.Item1, 2) + Mathf.Pow(AB.Item2, 2) + Mathf.Pow(AB.Item3, 2));
        float BCLength = Mathf.Sqrt(Mathf.Pow(BC.Item1, 2) + Mathf.Pow(BC.Item2, 2) + Mathf.Pow(BC.Item3, 2));

        return (Mathf.Acos(ABtimesBC / (ABLength * BCLength)) * (180 / Mathf.PI));
    }

    public static float shoulderAngle(float a1, float a2, float b1, float b2)
    { //Calculat shoulder abduction & adduction angle and extension & flexion angle
        float rise = (a1 - b1);
        float run = (a2 - b2);
        float result = (Mathf.Atan(rise / run)) * (180 / Mathf.PI);
        if (result < 0)
        {
            result += 180;
        }
        return result;
    }

    public static void calculateSpeed(ref List<float> angles, ref List<float> speeds, ref float meanSpeed)
    { //Calculate speed at each frame by computing change in angle / elapsed time between the previous frame an the next frame
        float totalSpeed = 0f;
        speeds.Add(0f);
        for (int i = 1; i < angles.Count - 1; i++)
        {
            float new_speed = (angles[i + 1] - angles[i - 1]) / (elapsedTimeList[i + 1] - elapsedTimeList[i - 1]);
            speeds.Add(new_speed);
            totalSpeed += Mathf.Abs(new_speed);
        }
        speeds.Add(0f);

        meanSpeed = totalSpeed / (angles.Count - 2);

        return;
    }

    public static float ROM(List<float> angles)
    { //Calculate Range of Motion
        angles.Sort();
        float max = angles[Convert.ToInt32(angles.Count * 0.9)];
        float min = angles[0];
        return (max - min);
    }

    public static float calculateMax(List<float> speeds)
    { //Calculate 90th percentile as maximum speed
        speeds.Sort();
        return speeds[Convert.ToInt32(speeds.Count * 0.9)];
    }
}

public struct TagInfo {
	public Vector3 position;
	public string name;
	public int userID;
	public float timeStamp;
    //public float imageNumber;
};
