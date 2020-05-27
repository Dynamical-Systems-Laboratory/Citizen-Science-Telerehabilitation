using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitGameScript : MonoBehaviour {

    // Use this for initialization
    public static GameObject finalQuestionObj;
    public static GameObject finalQuestionObj2;
    public GameObject dragInFinalQuestion;
    public GameObject dragInFinalQuestion2;
    public GameObject dragInSubmitBtn;
    public static GameObject finalSubmitBtn;

    void Start ()
    {
        finalQuestionObj = dragInFinalQuestion;
        finalQuestionObj2 = dragInFinalQuestion2;
        finalSubmitBtn = dragInSubmitBtn;
        Button button = GetComponent<Button>();
        button.onClick.AddListener(TaskOnClick);
	}
	
	// Update is called once per frame
	public static void TaskOnClick () {
		//Do final survey question "I enjoyed the activity:", then quit application:
		quitGame();
	}
	public static void quitGame()
    {
        DataCollector.startAnalyzing = true;
        finalQuestionObj.SetActive(true);
        finalQuestionObj2.SetActive(true);
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("TrashedTag")) {
			g.transform.localPosition = new Vector3 (10001, 10001, 0);
		}
		finalSubmitBtn.SetActive (true);
		SubmitFinalQuestionScript.startListening = true;
	}   
}
