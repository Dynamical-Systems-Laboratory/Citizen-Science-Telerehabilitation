using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerScript : MonoBehaviour {
	public static bool hudOff = false;
    public static bool turnedOff = false;
	NetworkManagerHUD hd;
	// Use this for initialization
	void Start () {
		hd = FindObjectOfType<NetworkManagerHUD>();
	}
	
	// Update is called once per frame
	void Update () {
		if (hudOff && hd.enabled) {
			if (!turnedOff)
            {
                hd.enabled = false;
                turnedOff = true;
            }
		}
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            hd.enabled = !hd.enabled;
            hudOff = !hd.enabled;
        }
	}
}
