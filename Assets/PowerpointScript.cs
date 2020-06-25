using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerpointScript : MonoBehaviour {
	public static bool inSlides = true;
	public static bool hasBeenToTutorial = false;

	public static RawImage[] slides;
	public static int slideIndex = 1; //Start shifting to first text element of first slide

	static float delay;
	static Color colorBegin;
	static Color colorEnd;
	static float transitionTime = 1.0f;

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
	}
	
	void Update () {
        if (inSlides) {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameObject.SetActive(false); //??
				slideIndex = 1;
				inSlides = false;
                if (!hasBeenToTutorial)
                {
					SimpleTutorial.inSimpleTutorial = true;
					hasBeenToTutorial = true;
				}
                else
                {
					HomeScreen.homeCamera.SetActive(true);
                }
			}

            delay += Time.deltaTime;
			slides [slideIndex].color = Color.Lerp (colorBegin, colorEnd, delay / transitionTime);
			if (slides [slideIndex].color == Color.white) { //Change slides
				if (slideIndex < slides.Length - 1) { //Still in powerpoint:
					//If we're still in a slide or going to the next slide:
					if (slides [slideIndex + 1].name.Substring (0, 7).Equals (slides [slideIndex].name.Substring (0, 7)) && delay > 2f) {
						slideIndex++;
						delay = 0f;
						transitionTime = 1f;
					} else { //Wait for keystroke
						if (MakeWordBank.moveOn() && !Input.GetKeyDown(KeyCode.BackQuote) && !Input.GetKeyDown(KeyCode.Escape)) {
							slideIndex++;
							delay = 0f;
							transitionTime = 0.75f;
						}
					}
				} else { //Powerpoint over:
					if (MakeWordBank.moveOn()) {
						gameObject.SetActive(false); //??
						slideIndex = 1;
						inSlides = false;
						if (!hasBeenToTutorial)
						{
							SimpleTutorial.inSimpleTutorial = true;
							hasBeenToTutorial = true;
						}
						else
						{
							HomeScreen.homeCamera.SetActive(true);
						}
					}
				}
			}
        }
	}
}
