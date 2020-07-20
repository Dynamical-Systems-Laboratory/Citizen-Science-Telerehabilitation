﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OVRTouchSample;
using System;

public class VRUser : MonoBehaviour
{
    //main stuff
    public StateManager state;

    //main controls
    public GameObject VRPerson;
    public OVRInput.Controller playerController;
    public OVRCameraRig playerHead;

    /*  TODO!!
     * prefect tracking of headset and controllers
     * recreate cursor movement based on controller positions/rotations
     * rework cameras:
     * * UI camera menu stuck to headset - controlled via highlighting UI elements (thumbstick & button/trigger selection)
     * * * selection of tags indicated by tag floating along cursor sphere OR controller replaced with scaled down tag
     * * Home Screen, Video Camera, and Profile similar to UI except float in space in front of user with current image greyed out
     * * * work with survey stuff later
     * * cursor camera moved along invis sphere with r = 95% of imageSphere.r
     * * * controlled via arm movements (tbd)
     * 
     * * * cursor reset idea
     */

    //TODO: maybe fix floating feeling with flatform at user feet (make camera lower, put platform right under, set to floor lvl instead of eye lvl)
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    { //Ctrl+K+C = comment (+K+U = uncomment)
        //player head and controllers set within Unity Scene (VRPerson's children)
        VRPerson = GameObject.Find("VRPerson");
        state = GameObject.Find("Canvas").GetComponent<StateManager>();
    }

    // Update is called once per frame
    void Update()
    {
        OVRInput.Update();
        OVRInput.FixedUpdate();
        StateManager.cursorAdd = (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Touch) + OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.Touch)) * Time.deltaTime * 0.5f;
        switch (state.getState()) //state camera control
        {
            case 0: //QUIT
                VRPerson.SetActive(false);
                break;
            case 1: //HOME
                VRPerson.transform.position = new Vector3(10f, 0f, 0f);
                break;
            case 2: //IN-GAME
                VRPerson.transform.position = new Vector3(0f, 0f, 0f);
                break;
            case 3: //PROFILE
                VRPerson.transform.position = new Vector3(20f, 0f, 0f);
                break;
            case 4: //CALIBRATE
                VRPerson.transform.position = new Vector3(0f, 10f, 0f);
                break;
            case 5: //TUTORIAL
                VRPerson.transform.position = new Vector3(0f, 0f, 0f);
                break;
            case 6: //ABOUT PROJECT
                VRPerson.transform.position = new Vector3(-10f, 0f, 0f);
                break;
            case 7: //PRACTICE LVL
                VRPerson.transform.position = new Vector3(0f, 0f, 0f);
                break;
            case 8: //SURVEY
                VRPerson.transform.position = new Vector3(-20f, 0f, 0f);
                break;
            default:
                break;
        }
        /*if (Input.GetKey(KeyCode.K))
        {
            VRPerson.transform.position += new Vector3(.1f * Time.deltaTime, 0f, 0f);
        }
        else if (Input.GetKey(KeyCode.J))
        {
            VRPerson.transform.position -= new Vector3(.1f * Time.deltaTime, 0f, 0f);
        }*/

        //pos & rotation = [0,1], triggers = [0,1], sticks = [0,10]
        //Debug.Log("VRHead: " + playerHead.GetCameraPositionOrientation() + "EyePos?: " + playerHead.centerEyeAnchor.position);
        Debug.Log("rightPos: " + OVRInput.GetLocalControllerPosition(OVRInput.Controller.RHand) + ", leftPos: " + OVRInput.GetLocalControllerPosition(OVRInput.Controller.LHand));
        Debug.Log("rightRot: " + OVRInput.GetLocalControllerRotation(OVRInput.Controller.RHand).eulerAngles + ", leftRot: " + OVRInput.GetLocalControllerRotation(OVRInput.Controller.LHand).eulerAngles);
        
        // returns a float of the Hand Trigger’s current state on the Left Oculus Touch controller.
        Debug.Log("LHandTrigger: " + OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch) + ", RHandTrigger: " + OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch));
        Debug.Log("LIndTrigger: " + OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch) + ", RIndTrigger: " + OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch));

        Debug.Log("LStick: " + OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Touch) + ", RStick: " + OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.Touch)*10f);
        Debug.Log("LStickP: " + OVRInput.Get(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.Touch) + ", RStickP: " + OVRInput.Get(OVRInput.Button.SecondaryThumbstick, OVRInput.Controller.Touch));

        //if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        //{
        //    state.setState(1); //home
        //}
        //else if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        //{
        //    state.setState(2); //game
        //}
        //else if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch))
        //{
        //    state.setState(3); //profile
        //}
        //else if (OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.LTouch))
        //{
        //    state.setState(2); //game
        //}
    }

    public static bool userContinue() //implemented for both hands
    {
        return OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch) || OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch);
    }
    public static bool userSkip()
    {
        return OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.Touch) || OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick, OVRInput.Controller.Touch);
    }
}
