using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalQuestionStartScript : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        GameObject.Find("SubmitFinalQuestion").SetActive(false);
        GameObject.Find("SliderFinalQuestion (1)").SetActive(false);
        
        gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
