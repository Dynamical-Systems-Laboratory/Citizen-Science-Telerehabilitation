using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableTextFieldScript : MonoBehaviour {
	public InputField inputField;
	public static bool interactable = true;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (interactable == gameObject.GetComponent<Toggle> ().isOn) {
			interactable = !interactable;
			inputField.interactable = interactable;
		}
	}
}
