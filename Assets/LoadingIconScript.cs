using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingIconScript : MonoBehaviour {

	// Use this for initialization
	public static bool active;
	bool previouslyInactive = true;
	float delay;
	int frameIndex;
	public Texture2D[] frames;

	void Start () {
		//Set inactive at first..
		frameIndex = 0;
		active = false;
		GetComponent<RawImage> ().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (active) {
			if (previouslyInactive) {
				previouslyInactive = false;
				GetComponent<RawImage> ().enabled = true;
			}
			delay += Time.deltaTime;
			if (delay >= 0.02f) {
				delay = 0;
				if (frameIndex < frames.Length) {
					GetComponent<RawImage> ().texture = frames [frameIndex];
					frameIndex++;
				} else {
					frameIndex = 0;
					GetComponent<RawImage> ().texture = frames [frameIndex];
				}
			}
		} else {
			if (!previouslyInactive) { //Was just turned off
				previouslyInactive = true;
				gameObject.SetActive (false);
			}
		}
	}
}
