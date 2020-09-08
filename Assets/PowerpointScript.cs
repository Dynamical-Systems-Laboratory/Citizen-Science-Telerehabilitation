using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerpointScript : MonoBehaviour {
	//public static bool inSlides = true;
	public static bool hasBeenToTutorial = false;

	public static RawImage[] slides;
	public static int slideIndex = 1; //Start shifting to first text element of first slide

	static float delay;
	static Color colorBegin;
	static Color colorEnd;
	static float transitionTime = 1.0f;
	public StateManager state;

	//extra
	private GameObject background;

	private static GameObject mainObj; //gameObject refrence

	void Awake()
	{
		colorBegin = new Color (1f, 1f, 1f, 0f);
		colorEnd = new Color(colorBegin.r, colorBegin.g, colorBegin.b, 1f);
	}

	void Start () {
		slides = new RawImage[transform.childCount];

		int index = 0;
		foreach (Transform img in transform) {
			slides [index] = img.GetComponent<RawImage>();
			index++;
		}
		gameObject.SetActive(true);

		state = GameObject.Find("Canvas").GetComponent<StateManager>();
		mainObj = GameObject.Find("Powerpoint");
		background = MakeWordBank.welcomeScreen;
	}

	void Update () {
        if (state.getState() == 6) {
			//TODO: Change Slide Text so that it mirrors proper skip controls
			//gameObject.SetActive(true);
			if (background != null)
			{
				background.SetActive(true);
			}
			Debug.Log("Slide #: " + slideIndex);
			if (MakeWordBank.skip())
            {
				slideIndex = 1;
				if (!hasBeenToTutorial || !state.user.getPracticeLevelState()[0])
                {
					//SimpleTutorial.inSimpleTutorial = true;
					state.setState(4);
					hasBeenToTutorial = true;
				}
                else
                {
					state.setState(1);
					background.SetActive(false);
				}
                //gameObject.SetActive(false);
                //gameObject.transform.position = new Vector3(0f,0f, -500f);
			}

            delay += Time.deltaTime;
			slides [slideIndex].color = Color.Lerp (colorBegin, colorEnd, delay / transitionTime);
			if (slides [slideIndex].color == Color.white) { //Change slides
				if (slideIndex < slides.Length - 1) { //Still in powerpoint:
					//If we're still in a slide or going to the next slide:
					if (slides [slideIndex + 1].name.Substring (0, 7).Equals (slides [slideIndex].name.Substring (0, 7)) && delay > 2f)
                    {
						slideIndex++;
						delay = 0f;
						transitionTime = 1f;
					}
                    else
                    { //Wait for keystroke
						if (MakeWordBank.moveOn() && !Input.GetKeyDown(KeyCode.BackQuote) && !MakeWordBank.skip())
                        {
							slideIndex++;
							delay = 0f;
							transitionTime = 0.75f;
						}
					}
				} else { //Powerpoint over:
					if (MakeWordBank.moveOn()) {
						slideIndex = 1;
						if (!hasBeenToTutorial || !state.user.getPracticeLevelState()[0])
						{
							//SimpleTutorial.inSimpleTutorial = true;
							state.setState(4); //calibrating
							hasBeenToTutorial = true;
						}
						else
						{
							state.setState(1);
							background.SetActive(false);
						}
					}
				}
			}
        }
	}
}
