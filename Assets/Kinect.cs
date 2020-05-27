using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinect : MonoBehaviour
{ //Using https://assetstore.unity.com/packages/tools/kinect-with-ms-sdk-7747
    public static Vector3 CHipPos;
    public static Vector3 SpinePos;
    public static Vector3 CShoulderPos;
    public static Vector3 HeadPos;

    public static Vector3 LShoulderPos;
    public static Vector3 LElbowPos;
    public static Vector3 LWristPos;
    public static Vector3 LHandPos;

    public static Vector3 RShoulderPos;
    public static Vector3 RElbowPos;
    public static Vector3 RWristPos;
    public static Vector3 RHandPos;

    public static Vector3 LHipPos;
    public static Vector3 LKneePos;
    public static Vector3 LAnklePos;
    public static Vector3 LFootPos;

    public static Vector3 RHipPos;
    public static Vector3 RKneePos;
    public static Vector3 RAnklePos;
    public static Vector3 RFootPos;

    private uint playerID;

    public bool MirroredMovement = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    { //Getting all joint positions
        KinectManager manager = KinectManager.Instance;

        // get 1st player
        playerID = manager != null ? manager.GetPlayer1ID() : 0;

        /*CHipPos = manager.GetJointPosition(playerID, 0); //should just be pos 0's
        SpinePos = manager.GetJointPosition(playerID, 1);
        CShoulderPos = manager.GetJointPosition(playerID, 2);
        HeadPos = manager.GetJointPosition(playerID, 3);

        LShoulderPos = manager.GetJointPosition(playerID, 4);
        LElbowPos = manager.GetJointPosition(playerID, 5);
        LWristPos = manager.GetJointPosition(playerID, 6);
        LHandPos = manager.GetJointPosition(playerID, 7);

        RShoulderPos = manager.GetJointPosition(playerID, 8);
        RElbowPos = manager.GetJointPosition(playerID, 9);
        RWristPos = manager.GetJointPosition(playerID, 10);
        RHandPos = manager.GetJointPosition(playerID, 11);

        LHipPos = manager.GetJointPosition(playerID, 12);
        LKneePos = manager.GetJointPosition(playerID, 13);
        LAnklePos = manager.GetJointPosition(playerID, 14);
        LFootPos = manager.GetJointPosition(playerID, 15);

        RHipPos = manager.GetJointPosition(playerID, 16);
        RKneePos = manager.GetJointPosition(playerID, 17);
        RAnklePos = manager.GetJointPosition(playerID, 18);
        RFootPos = manager.GetJointPosition(playerID, 19);*/

        //Debug.Log(LHandPos);
        
    }
}
