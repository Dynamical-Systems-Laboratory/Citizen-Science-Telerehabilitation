using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubmitSurveyAction : MonoBehaviour
{
	public Text age;

	public ToggleGroup genderToggleGroup;
	public Text otherGender;

	public ToggleGroup englishFirstLanguageToggleGroup;
	public string finalGenderChoice = "";

	public Slider[] questionSliders;
	void Start()
	{
		Button btn = gameObject.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick()
	{
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
			if (finalGenderChoice.Equals ("Other:")) {
				finalGenderChoice = otherGender.text; ///////
			}
		}

		IEnumerator<Toggle> englishFirstLangEnum = englishFirstLanguageToggleGroup.ActiveToggles ().GetEnumerator ();
		englishFirstLangEnum.MoveNext ();
		string englishFirstLanguageAnswer = englishFirstLangEnum.Current.transform.GetChild (1).GetComponent<Text> ().text;

		/*
		Debug.Log (ageStr);
		Debug.Log (finalGenderChoice);
		Debug.Log (englishFirstLanguageAnswer);
		*/

		DataCollector.ageAnswer = ageStr;
		DataCollector.englishSpeakingAnswer = englishFirstLanguageAnswer;
		DataCollector.genderAnswer = finalGenderChoice;

		DataCollector.sliderAnswers = new int[questionSliders.Length];
		for (int i = 0; i < DataCollector.sliderAnswers.Length; i++) {
			DataCollector.sliderAnswers [i] = (int) questionSliders [i].value;
		}
		DataCollector.sliderQuestions = new string[questionSliders.Length];
		for (int i = 0; i < DataCollector.sliderQuestions.Length; i++) {
			DataCollector.sliderQuestions [i] = questionSliders [i].transform.GetChild (3).gameObject.GetComponent<Text> ().text;
		}


		gameObject.transform.parent.parent.gameObject.SetActive (false);
		MakeWordBank.inTutorial = true;
        if (GameObject.Find("Falcon"))
        {
            foreach(Camera obj in Resources.FindObjectsOfTypeAll<Camera>())
            {
                if (obj.name == "CursorCamera")
                {
                    obj.transform.parent.gameObject.SetActive(true);
                }
            }
        }
        GameObject.Find("Canvas").GetComponent<StateManager>().enabled = true;
	}
}
