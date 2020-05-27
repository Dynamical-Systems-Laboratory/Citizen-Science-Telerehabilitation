using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using UnityEngine.UI;
using System.Linq;

public class DataReader : MonoBehaviour
{
    static bool writtenAnalysisColumnNames = false;
    static bool writtenScoresColumnNames = false;

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
    public static float LElbowMean;
    public static float RElbowMean;
    public static float LArmUDMean;
    public static float RArmUDMean;
    public static float LArmLRMean;
    public static float RArmLRMean;

    public static float prevShoulderAngle = 0f;
    public static float prevElbowAngle = 0f;

    public int stopCount;

    // Start is called before the first frame update
    void Start()
    { //Analyze raw data, essentially the same as DataCollector
        string IMUPath = "Builds/Data/User-5/20190716_IMU_Data.csv";
        string KinectPath = "Builds/Data/User-5/20190716_Kinect_Data.csv";
        string Analysis_path = "Builds/Data/User-5/20190716_Analysis.csv";
        string Scores_path = "Builds/Data/User-5/20190716_Scores.csv";

        string IMUData = System.IO.File.ReadAllText(IMUPath);
        List<string> IMULines = IMUData.Split('\n').ToList();

        string KinectData = System.IO.File.ReadAllText(KinectPath);
        List<string> KinectLines = KinectData.Split('\n').ToList();

        for (int i = 1; i < IMULines.Count; i++)
        {
            if (IMULines[i] != "" && KinectLines[i] != "")
            {
                List<string> IMULineStr = IMULines[i].Split(',').ToList();
                List<float> IMULine = new List<float>();
                IMULine.Add(0f);
                IMULine.Add(0f);
                for (int j = 2; j < IMULineStr.Count - 1; j++)
                {
                    if (IMULineStr[j] != "")
                    {
                        IMULine.Add(Convert.ToSingle(IMULineStr[j]));

                    }
                }

                List<string> KinectLineStr = KinectLines[i].Split(',').ToList();
                List<float> KinectLine = new List<float>();
                KinectLine.Add(0f);
                KinectLine.Add(0f);
                for (int j = 2; j < KinectLineStr.Count - 1; j++)
                {
                    if (KinectLineStr[j] != "")
                    {
                        KinectLine.Add(Convert.ToSingle(KinectLineStr[j]));
                    }
                }

                elapsedTimeList.Add(Convert.ToSingle(IMULineStr[0]));
                systemTime.Add(IMULineStr[1]);

                float currShoulderAngle = (shoulderAngle(KinectLine[19], KinectLine[18], KinectLine[16], KinectLine[15]) +
                            shoulderAngle(KinectLine[31], KinectLine[30], KinectLine[28], KinectLine[27])) / 2;
                float currElbowAngle = (elbowAngle(KinectLine[14], KinectLine[15], KinectLine[16], KinectLine[17],
                        KinectLine[18], KinectLine[19], KinectLine[23], KinectLine[24], KinectLine[25]) +
                        elbowAngle(KinectLine[26], KinectLine[27], KinectLine[28], KinectLine[29],
                    KinectLine[30], KinectLine[31], KinectLine[35], KinectLine[36], KinectLine[37])) / 2;

                if ((IMULine[5] < -12 && IMULine[2] > 150) || (IMULine[5] > 12 && IMULine[2] < 50))
                {
                    float ShoulderSpeed = (currShoulderAngle - prevShoulderAngle) / Time.deltaTime;
                    float ElbowSpeed = (currElbowAngle - prevElbowAngle) / Time.deltaTime;
                    IMUAngle.Add(IMULine[2] - (currElbowAngle - prevElbowAngle) - (currShoulderAngle - prevShoulderAngle));
                    IMUSpeed.Add(IMULine[5] - ElbowSpeed - ShoulderSpeed);
                }

                prevElbowAngle = currElbowAngle;
                prevShoulderAngle = currShoulderAngle;

                LElbowAngle.Add(elbowAngle(KinectLine[14], KinectLine[15], KinectLine[16], KinectLine[17],
                        KinectLine[18], KinectLine[19], KinectLine[23], KinectLine[24], KinectLine[25]));
                RElbowAngle.Add(elbowAngle(KinectLine[26], KinectLine[27], KinectLine[28], KinectLine[29],
                    KinectLine[30], KinectLine[31], KinectLine[35], KinectLine[36], KinectLine[37]));
                LArmUDAngle.Add(shoulderAngle(KinectLine[19], KinectLine[18], KinectLine[16], KinectLine[15]));
                RArmUDAngle.Add(shoulderAngle(KinectLine[31], KinectLine[30], KinectLine[28], KinectLine[27]));
                LArmLRAngle.Add(shoulderAngle(KinectLine[19], KinectLine[17], KinectLine[16], KinectLine[14]));
                RArmLRAngle.Add(shoulderAngle(KinectLine[31], KinectLine[29], KinectLine[28], KinectLine[26]));
            }
            
        }


        calculateSpeed(ref LElbowAngle, ref LElbowSpeed, ref LElbowMean);
        calculateSpeed(ref RElbowAngle, ref RElbowSpeed, ref RElbowMean);
        calculateSpeed(ref LArmLRAngle, ref LArmLRSpeed, ref LArmLRMean);
        calculateSpeed(ref RArmLRAngle, ref RArmLRSpeed, ref RArmLRMean);
        calculateSpeed(ref LArmUDAngle, ref LArmUDSpeed, ref LArmUDMean);
        calculateSpeed(ref RArmUDAngle, ref RArmUDSpeed, ref RArmUDMean);


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
                LElbowAngle[i].ToString() + "," + LElbowSpeed[i].ToString() + "," +
                RElbowAngle[i].ToString() + "," + RElbowSpeed[i].ToString() + "," + LArmUDAngle[i].ToString() + "," + LArmUDSpeed[i].ToString() + "," +
                RArmUDAngle[i].ToString() + "," + RArmUDSpeed[i].ToString() + "," + LArmLRAngle[i].ToString() + "," + LArmLRSpeed[i].ToString() + "," +
                RArmLRAngle[i].ToString() + "," + RArmLRSpeed[i].ToString() + "," + "\n";

            AnalysisStreamWriter.Write(Analysis_line);

        }
        AnalysisStreamWriter.Close();


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
            for (int i = 0; i < IMUAngle.Count; i++)
            {
                if (System.Single.IsNaN(IMUAngle[i]))
                {
                    IMUAngle.Remove(i);
                }
            }
            IMUAngle.Sort();
            float maxIMUAngle = IMUAngle[Convert.ToInt32(IMUAngle.Count - 1)];
            float minIMUAngle = IMUAngle[0];
            Debug.Log(maxIMUAngle);
            Debug.Log(minIMUAngle);
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
    }

    // Update is called once per frame
    void Update()
    {
        
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
            if (!System.Single.IsNaN(angles[i - 1]) && !System.Single.IsNaN(angles[i + 1]))
            {
                float new_speed = (angles[i + 1] - angles[i - 1]) / (elapsedTimeList[i + 1] - elapsedTimeList[i - 1]);
                speeds.Add(new_speed);
                totalSpeed += Mathf.Abs(new_speed);
            }
            else
            {
                speeds.Add(0f);
            }
        }
        speeds.Add(0f);

        meanSpeed = totalSpeed / (angles.Count - 2);

        return;
    }

    public static float ROM(List<float> angles)
    { //Calculate Range of Motion
        for (int i = 0; i < angles.Count; i++)
        {
            if (System.Single.IsNaN(angles[i]))
            {
                angles.Remove(i);
            }
        }
        angles.Sort();
        float max = angles[Convert.ToInt32(angles.Count - 1)];
        float min = angles[0];
        return (max - min);
    }

    public static float calculateMax(List<float> speeds)
    { //Calculate 90th percentile as maximum speed
        speeds.Sort();
        return speeds[Convert.ToInt32(speeds.Count * 0.9)];
    }
}
