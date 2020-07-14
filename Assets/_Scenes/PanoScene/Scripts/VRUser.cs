using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OVRTouchSample;
using System;

public class VRUser : MonoBehaviour
{
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
     */

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    { //Ctrl+K+C = comment (+K+U = uncomment)
        //player head and controllers set within Unity Scene (VRPerson's children)
        VRPerson = GameObject.Find("VRPerson");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Vr Info: " + OVRInput.GetControllerPositionTracked(playerController));
        Debug.Log("rightPos: " + OVRInput.GetLocalControllerPosition(OVRInput.Controller.RHand) + ", leftPos: " + OVRInput.GetLocalControllerPosition(OVRInput.Controller.LHand));
        Debug.Log("rightRot: " + OVRInput.GetLocalControllerRotation(OVRInput.Controller.RHand) + ", leftRot: " + OVRInput.GetLocalControllerRotation(OVRInput.Controller.LHand));
        if (OVRInput.GetDown(OVRInput.Button.Any))
        {
            Debug.Log("VR Buttons: " + OVRInput.Button.Any);
            Vector3 controllerPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RHand);
        }
    }
}
