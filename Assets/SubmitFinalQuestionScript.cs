using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SubmitFinalQuestionScript : MonoBehaviour {

	// Use this for initialization
	[FormerlySerializedAs("slider")] public Slider[] sliders;
    //public Slider slide2;
	public static bool startListening = false;
    public static bool isListening = false;
    private float visibleY = 83.33736f;
    private float invisibleY = 700f;
    public Text age;
    public GameObject falconContainer;

    public ToggleGroup genderToggleGroup;
    public Text otherGender;
    public Button submitButton;

    public string finalGenderChoice = "";
	//bool notAwake = true;

	void Start() {
		Vector3 pos = transform.localPosition;
		pos.y = invisibleY;
		transform.localPosition = pos;
	}
	
	// Update is called once per frame
	void Update () {
		if (startListening) {
            Vector3 pos = transform.localPosition;
            falconContainer.SetActive(false);
            pos.y = visibleY;
            transform.localPosition = pos;
			//Button btn = gameObject.GetComponentInChildren<Button> ();
			submitButton.onClick.AddListener(TaskOnClick);
            isListening = true;
			startListening = false;
		}
	}

	void TaskOnClick() {
		//DataCollector.writeFinalQuestion ((int) slide.value, (int) slide2.value);
		
		//Application.Quit ();
		
		string ageStr = age.text; //////
		if (ageStr.Equals ("")) {
			ageStr = "Prefer not to say";
		}
		IEnumerator<Toggle> genderToggleEnum = genderToggleGroup.ActiveToggles ().GetEnumerator ();
		genderToggleEnum.MoveNext ();
		if (genderToggleEnum.Current != null) {
			Toggle genderToggle = genderToggleEnum.Current;
			string genderSelection = genderToggle.transform.GetChild (1).GetComponent<Text> ().text;
			finalGenderChoice = genderSelection;
		}


		//DataCollector.ageAnswer = ageStr;
		//DataCollector.genderAnswer = finalGenderChoice;
		DataCollector.writeFinalQuestionMORE(sliders, ageStr, finalGenderChoice);
		Application.Quit();
	}
}
