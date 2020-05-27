using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewConeScript : MonoBehaviour {

	// Use this for initialization
	static GameObject thisObj;
	void Start () {
		thisObj = gameObject;
	}
	
	// Update is called once per frame
	void Update () {
	}

	public static void modifyRotation(float angle) {
		thisObj.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
	}
	//DarkenerScript.cs should also call this method:
	public static void modifyScale(float scale) {
		//This cone image should be scaled to a minimum 0.5 times its original size (falcon is all the way back),
		//or a maximum of 1.5 times (falcon is all the way forward). 1.0 times would be if the falcon is right in the middle.
		thisObj.transform.localScale = new Vector3 (scale, scale, 1.0f);
	}
}
