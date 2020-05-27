using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerScript : NetworkBehaviour {
	// Use this for initialization

	private static bool finishedWaiting = false; //Once this is true, client and server should be playing together and constantly sending signals back and forth
	//private static bool holdingATag = false;
	public static string holdingTag = ""; //The tag's unique name in the wordbank, like Tag0, Tag9
	public static string trashedTagText = "";
	private static string nameLastTag = "";
	private static bool terminated = false; //To prevent the final question from coming up over and over upon termination

	///**************
	/// jointTermination is the only variable needed to change to make it either both users end at once or one keeps playing
	/// 
	bool jointTermination = true;
	///
	///**************


	static int frame = 0;

	static bool taggerPanelIsSet = false;
	static bool trasherPanelIsSet = false;

	void Start () {
		if (!localPlayerAuthority) {
			return;
		}

		if (isServer) {  //Server is tagger, client is trasher
			
		} else {
			
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!localPlayerAuthority) {
			return;
		}
		frame++;
			
		if (frame > 3) { //This is done to not slow network traffic by calling commands every frame
			if (isServer) {
                if (!NetworkManagerScript.turnedOff)
                {
				    NetworkManagerScript.hudOff = true;
                }
				if (MakeWordBank.waitingForOtherPlayer) {
					RpcAskClientIfFinishedTutorial ();
				}
				if (!taggerPanelIsSet) {
					MakeWordBank.taggerPanel.transform.Translate (new Vector3 (0, -5000, 0));
					MakeWordBank.skipTrashingTutorialStep = true;
					taggerPanelIsSet = true;
				}

				if (finishedWaiting) { //Playing the game:
					//Tell client (trasher) when the tagger is holding a tag:
					if (ClickAction.state.getSelected ()) {
						nameLastTag = ClickAction.state.getSelected ().GetComponent<Text> ().text;
						holdingTag = ClickAction.state.getSelected ().GetComponent<Text> ().name;
						RpcTellClientTagIsHeld (nameLastTag);
					} else {
						if (!holdingTag.Equals ("")) { //Means Server just dropped a tag, put it in client's game:
							Transform tagTransform;
							if (ClickAction.sphere.transform.childCount != 0) { //If the image hasn't already turned over
								tagTransform = ClickAction.sphere.transform.GetChild (ClickAction.sphere.transform.childCount - 1);
								Vector3 position = tagTransform.localPosition;
								Vector3 rotation = tagTransform.localRotation.eulerAngles;
								Vector3 scale = tagTransform.localScale;
								string name = tagTransform.GetChild (0).GetComponent<TextMesh> ().text;

								RpcAddTagToSphere (position, name, holdingTag); //holdingTag is for the MakeWordBank.replaceTag function (Tag0, Tag1 etc since they're unique)
							} else {
								Vector3 position = MakeWordBank.positionLastTag;
								string name = nameLastTag;

								RpcAddTagToSphere (position, name, holdingTag); //holdingTag is for the MakeWordBank.replaceTag function (Tag0, Tag1 etc since they're unique)

							}
							holdingTag = "";
						}
					}
					if (SubmitFinalQuestionScript.isListening) { //Means the server is quitting currently:
						if (!terminated) {
							if (jointTermination) {
								MakeWordBank.taggerPanel.transform.Translate (new Vector3 (0, 5000, 0));
								RpcQuitGame ();
							} else {
								RpcContinuePlaying ();
							}
							terminated = true;
						}
					}
				}
			} else {
                if (!NetworkManagerScript.turnedOff)
                {
                    NetworkManagerScript.hudOff = true;
                }
                if (MakeWordBank.otherPlayerHasFinished) {
					CmdTellServerClientIsFinished ();
				}

				if (!trasherPanelIsSet) {
					MakeWordBank.trasherPanel.transform.Translate (new Vector3 (0, -5000, 0));
					MakeWordBank.skipTaggingTutorialStep = true;
					trasherPanelIsSet = true;
				}

				if (finishedWaiting) {
					//**********
					//Potential glitch: trasher trashes a tag and before the signal can reach the tagger, the tagger picks it up.
					//In this case, it'll be the one time the trasher overrules the tagger (makes sense this way and is probably gonna be easier to implement)

					if (ClickAction.state.getSelected ()) {
						holdingTag = ClickAction.state.getSelected ().GetComponent<Text> ().name;
						trashedTagText = ClickAction.state.getSelected ().GetComponent<Text> ().text;
                        CmdTellServerTagIsHeld(trashedTagText);
					} else {
						if (!holdingTag.Equals ("")) { //Means trasher has just dropped a tag into the trash:
							CmdTellServerThrowAwayTag(trashedTagText, holdingTag);

							trashedTagText = "";
							holdingTag = "";
						}
					}
					if (SubmitFinalQuestionScript.isListening) { //Client has quit
						if (!terminated) {
							if (jointTermination) {
								MakeWordBank.trasherPanel.transform.Translate (new Vector3 (0, 5000, 0));
								CmdQuitGame ();
							} else {
								CmdContinuePlaying ();
							}
							terminated = true;
						}
					}
				}
			}
			frame = 0;
		}
	}

	[ClientRpc]
	void RpcAskClientIfFinishedTutorial() {
		if (!isServer) { //Server is a client too
			if (MakeWordBank.waitingForOtherPlayer) { //If client is already waiting
				MakeWordBank.otherPlayerHasFinished = true;
				finishedWaiting = true;
			}
		}
	}

	[Command]
	void CmdTellServerClientIsFinished() {
		MakeWordBank.otherPlayerHasFinished = true;
		finishedWaiting = true;
	}


	[ClientRpc]
	void RpcTellClientTagIsHeld(string name) {
		if (!isServer) {
			for (int i = 0; i < MakeWordBank.tags.Length; i++) {
				if (MakeWordBank.tags [i].getText ().Equals (name)) {
					MakeWordBank.tags [i].text.color = Color.red;
				} else if (!MakeWordBank.tags[i].getText().Equals(trashedTagText)) {
					MakeWordBank.tags [i].text.color = Color.black;
				}
			}
			if (ClickAction.state.getSelected ()) { //Check if the trasher is holding the same tag as the server: if not, do nothing:
				string trasherTagHeld = ClickAction.state.getSelected ().GetComponent<Text> ().text;
                ClickAction.state.getSelected().GetComponent<Text>().color = Color.red;

                if (trasherTagHeld.Equals (name)) {
					holdingTag = "";
					ClickAction.state.setSelected (null);
					Destroy (ClickAction.cursorTag);
					ClickAction.cursorTag = null;
				}
			}
		}
	}

    [Command]
    void CmdTellServerTagIsHeld(string name)
    {
        for (int i = 0; i < MakeWordBank.tags.Length; i++)
        {
            if (MakeWordBank.tags[i].getText().Equals(name))
            {
                MakeWordBank.tags[i].text.color = Color.red;
            }
            else if (!MakeWordBank.tags[i].getText().Equals(nameLastTag))
            {
                MakeWordBank.tags[i].text.color = Color.black;
            }
        }
        if (ClickAction.state.getSelected())
        {
            string taggerTagHeld = ClickAction.state.getSelected().GetComponent<Text>().text;
            ClickAction.state.getSelected().GetComponent<Text>().color = Color.red;
            if (taggerTagHeld.Equals(name))
            {
                RpcTellClientTagIsHeld(name);
            }
        }
    }

	[ClientRpc]
	void RpcAddTagToSphere (Vector3 position, string name, string tagUniqueName) {
		if (!isServer) {
			GameObject newTag = Instantiate (ClickAction.tagPrefab);
			newTag.name = name;
			newTag.GetComponent<Renderer>().material = new Material(Shader.Find("Diffuse"));

			newTag.transform.localScale = new Vector3 (0.25f, 0.1f, 0.00001f);
			newTag.transform.parent = ClickAction.sphere.transform;

			newTag.transform.localPosition = position;
			newTag.transform.LookAt (Vector3.zero); // Make it face the center of the sphere

			GameObject textContainer = new GameObject ();
			textContainer.transform.parent = newTag.transform;

			TextMesh text = textContainer.AddComponent<TextMesh> ();
			text.text = name;
			text.fontSize = 20;
			text.alignment = TextAlignment.Center;
			text.anchor = TextAnchor.MiddleCenter;
			text.name = name + "_Text";
			text.transform.parent = textContainer.transform;
			text.transform.localScale = new Vector3 (-0.075f, 0.25f, 0.25f);
			text.transform.localPosition = Vector3.zero;
			text.transform.localEulerAngles = Vector3.zero;


			GameObject obj = new GameObject ();
			obj.name = tagUniqueName; //Doing this because I'm trying not to really change any scripts outside of this one significantly

			MakeWordBank.replaceTag (obj, true);
		}
	}

	[Command]
	void CmdTellServerThrowAwayTag(string tagText, string uniqueTagName) {
		//Nevermind:
        /*
		for (int i = 0; i < MakeWordBank.tags.Length; i++) {
			if (MakeWordBank.tags [i].getText ().Equals (tagText)) {
				if (MakeWordBank.tags [i].text.color == Color.red) { //If they just then clicked on it, deselect it for tagger:
					MakeWordBank.tags[i].text.color = Color.black;
					if (ClickAction.cursorTag != null) {
						Destroy(ClickAction.cursorTag);
						holdingTag = "";
						ClickAction.cursorTag = null;
						if (ClickAction.cursorSphere != null)
						{
							ClickAction.cursorSphere.GetComponent<MeshRenderer>().enabled = true;
						}
					}
				}
			}
		}
        */

		GameObject objToInstantiate = null; //Object to put under trash can:
		for (int i = 0; i < MakeWordBank.tags.Length; i++) {
			if (MakeWordBank.tags [i].getText ().Equals (tagText)) {
				objToInstantiate = MakeWordBank.tags [i].tag;
			}
		}


		if (MakeWordBank.sequenceIndex < MakeWordBank.wordBank.Count) {
			GameObject newTrashedTag = Instantiate (objToInstantiate, ClickAction.canvas.transform);
			newTrashedTag.transform.localScale 
			= new Vector3 (newTrashedTag.transform.localScale.x / 2.0f, newTrashedTag.transform.localScale.y / 2.5f, newTrashedTag.transform.localScale.z);
			newTrashedTag.transform.GetChild (0).GetComponent<Text> ().color = Color.black;
			newTrashedTag.transform.tag = "TrashedTag";
			newTrashedTag.transform.GetChild (0).tag = "TrashedTag";
			int verticalBump = 0;
			if (ClickAction.trashedTags.Count >= 14 && ClickAction.trashedTags.Count < 28) {
				verticalBump = 168; //To prevent overlap
			} else if (ClickAction.trashedTags.Count >= 28 && ClickAction.trashedTags.Count < 42) {
				verticalBump = 606;
			} else if (ClickAction.trashedTags.Count >= 42) {
				verticalBump = 774;
			}

			int horizontalBump = 0;
			if (ClickAction.trashedTags.Count >= 14 && ClickAction.trashedTags.Count < 28) {
				horizontalBump = 50;
			} else if (ClickAction.trashedTags.Count >= 28 && ClickAction.trashedTags.Count < 42) {
				horizontalBump = 0;
			} else if (ClickAction.trashedTags.Count >= 42) {
				horizontalBump = 50;
			}
			newTrashedTag.transform.position = ClickAction.canvas.transform.TransformPoint(new Vector2(320 + horizontalBump, -55 - 12*ClickAction.trashedTags.Count + verticalBump)) + Vector3.back * -0.25f;
			newTrashedTag.transform.LookAt(newTrashedTag.transform.position + Vector3.back * newTrashedTag.transform.position.z * -1);
			ClickAction.trashedTags.Add (newTrashedTag);
			ClickAction.trashedTags[ClickAction.trashedTags.Count - 1].layer = 5; //UI
		}

		GameObject obj = new GameObject ();
		obj.name = uniqueTagName;

		MakeWordBank.replaceTag(obj, false);
		//currentTag.GetComponentInChildren<Text>().color = Color.clear;
		//currentTag.GetComponent<Text>().color = Color.clear;
	}

	[ClientRpc]
	void RpcQuitGame() {
		if (!isServer) {
			if (!SubmitFinalQuestionScript.isListening) { //So this isn't done twice
				QuitGameScript.TaskOnClick ();
			}
		}
	}

	[Command]
	void CmdQuitGame() {
		if (!SubmitFinalQuestionScript.isListening) {
			QuitGameScript.TaskOnClick ();
		}
	}

	[Command]
	void CmdContinuePlaying() { //Independent termination:
		MakeWordBank.practiceLevelText.SetActive(true); //For convenience, the text object practiceLevelText will be the text that says the other person quit
		MakeWordBank.practiceLevelText.transform.localPosition = new Vector3(248.9f, -54.5f, 0f);
		MakeWordBank.practiceLevelText.GetComponent<RectTransform> ().sizeDelta
		= new Vector2 (270.36f, 30f);
		MakeWordBank.practiceLevelText.GetComponent<Text> ().fontSize = 10;
		MakeWordBank.practiceLevelText.GetComponent<Text> ().text = "The other party has left the session";
	}

	[ClientRpc]
	void RpcContinuePlaying() {
		if (!isServer) {
			MakeWordBank.practiceLevelText.SetActive(true); //For convenience, the text object practiceLevelText will be the text that says the other person quit
			MakeWordBank.practiceLevelText.transform.localPosition = new Vector3(248.9f, -54.5f, 0f);
			MakeWordBank.practiceLevelText.GetComponent<RectTransform> ().sizeDelta
			= new Vector2 (270.36f, 30f);
			MakeWordBank.practiceLevelText.GetComponent<Text> ().fontSize = 10;
			MakeWordBank.practiceLevelText.GetComponent<Text> ().text = "The other party has left the session";
			MakeWordBank.continueAfterOtherQuit = true;
		}
	}
}
