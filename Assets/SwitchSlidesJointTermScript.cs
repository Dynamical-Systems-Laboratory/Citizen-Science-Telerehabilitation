using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchSlidesJointTermScript : MonoBehaviour {
	public Texture2D otherSlide;
	bool changedSlide = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (MakeWordBank.skipTaggingTutorialStep) { //If they're skipping the tagging step, then switch to trasher slide:
			if (!changedSlide) {
				GetComponent<RawImage> ().texture = otherSlide;
				changedSlide = true;
			}
		}
	}
}
