using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickAction : MonoBehaviour //, IPointerClickHandler
{
    public static StateManager state; // State of the application

	public static GameObject tagPrefab;
	public static GameObject sphere;
	public static GameObject canvas;
	public static GameObject tagCanvas;

	public static GameObject cursorTag; //Tag that follows cursor
	public static GameObject cursorSphere; // Falcon cursor
	public static List<GameObject> trashedTags; //representation of tags
	public Material tagMaterial;

	public static GameObject tagCopy;
	public static int lastTag = -1;
	public static GameObject background; //image ref

    public static bool tagIsFollowing = false;

	public static GameObject trashy; //trash ref
    public static float maxBinDist = 8f; //distance of cursor from bin after cursorPosMod factor

	public static GameObject nextButton;
	public static GameObject quitButton;

	private static GameObject playerHead;
	private static float showNum = 0; //random num that is shown in debug
	//private static float tagScalor = 0.885f; //val that downscales tags
	public static Vector3 tagDownScale = new Vector3(0.003393029f, 0.004489522f, 0.003547033f);
	private static Vector3 trashTagDownScale = new Vector3(0.02400159f, 0.03175797f, 0.02509099f);

	public static GameObject gameSphere;

	public void Awake()
    {
		gameSphere = GameObject.Find("gameSphere");

		state = GameObject.Find("Canvas").GetComponent<StateManager>();
		playerHead = GameObject.Find("CenterEyeAnchor");

		tagPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
		tagPrefab.name = "TagPrefab"; //So it can be destroyed
        tagPrefab.transform.localScale = Vector3.zero;

        sphere = GameObject.Find("gameSphere"); //TagSphere
		canvas = GameObject.Find ("Canvas");
		tagCanvas = GameObject.Find("tagCanvas");

        cursorSphere = GameObject.Find("CursorSphere");
		trashedTags = new List<GameObject> ();
		background = GameObject.Find("Image"); // backgreound for the location where to place an image
        trashy = GameObject.Find("Bin");
		nextButton = MakeWordBank.nextButton;
		quitButton = MakeWordBank.quitButton;

		tagDownScale = GameObject.Find("tagRef").transform.localScale;
		//trashTagDownScale = GameObject.Find("trashTagRef").transform.localScale;
	}

    /* Clicking Logic:
     * * if the click action is called AND a tag is within a certain distance/location parameters relative to the cursor AND there is not another tag already selected it is clicked
     * * * on click, the tag's text turns red and the tag is moved to the present location of the cursor
     * if there is an object held AND...
     * * if the click action is called again AND the cursor is on the canvus, the text goes back to black and the tag is anchored to the image, and a new instance of the tag is put back in the original locations
     * * if the click action is called AND the cursor is within a certain distance of a different tag, the tag is put back in its original location and the next tag is selected
     * * if the click action is called again AND the cursor is over the trash can the tag goes into the trash (smaller tags are displayed under the trash)
     *
     * if words are in trash add a new word to the list of tags
     * later -> implement button functionality for moving onto another image and quitting the game
     * */


	//This method is only needed when the user has clicked a tag, and the instantiated GameObject tag needs to follow the cursor:
	public void Update() {
		if (state.isGaming())
        {
			//Debug.Log("Tag%Num: " + showNum);

			//Debug.Log("Selected Elem: " + state.getSelected() + ", isNull? " + isNull.ToString());
			if (tagIsFollowing)
			{
				state.getSelected().transform.position = state.getCursorPosition(false);
			}
			Debug.Log("(" + trashedTags.Count + ")InTrash: " + String.Join(",", trashedTags));
			/*Debug.Log("Trash Rollcall (" + trashedTags.Count + ") -->");
			foreach(GameObject trash in trashedTags)
            {
				Debug.Log("--Trashed " + trash.name + ": " + trash.transform.localPosition + ", " + trash.transform.position);
            }*/
		}
    }
	public void OnPointerClick(GameObject objectClicked = null) //method that handles backend stuff for clicking
	{
		//if (objectClicked != null) { objectClicked = background; } // check later

		if (objectClicked == null && binClose())
		{//if the cursor is over the trash, the obj we are looking at is the trash
			objectClicked = trashy;
			Debug.Log("Trash in click method...");
        }

        if (objectClicked == null && state.getSelected() != null && state.getCursorPosition().x < 21f)
		{ // tag was placed  *******
			state.getSelected().GetComponentInChildren<Text>().color = Color.blue; //instead of just GetComponent
			state.getSelected().transform.localScale -= new Vector3(0.35f, 0.35f, 0f); //scale it down to 65% size (not thickness tho)
            tagIsFollowing = false;

			state.getSelected().transform.localScale = tagDownScale;

			//for each obj already a child of the tag canvas, put somewhere else, transform canvas back up to original size,
			//put new obj in, transform back to small scale, add old objs back

			state.tagsPlaced.Add(state.getSelected()); //adds to movement list
			state.getSelected().layer = 16; //VisibleTags layer
			state.getSelected().transform.SetParent(tagCanvas.transform);//make child of other canvas to save pos
			state.getSelected().tag = "Untagged"; //just in case
			offsetTagsCloser();
			// raycasting attempt graveyard
			/*Ray cursorRay = new Ray(state.getCursorPosition(), (state.getCursorPosition() - playerHead.transform.position).normalized);
			RaycastHit[] hits = Physics.RaycastAll(cursorRay, (state.getCursorPosition() - playerHead.transform.position).magnitude);
			foreach (RaycastHit hit in hits)
            {
				//RaycastHit hit = new RaycastHit();
				if (Physics.Raycast(state.getCursorPosition(), (state.getCursorPosition() - playerHead.transform.position).normalized))
				{
					//Instantiate(state.getSelected(), hit.point, Quaternion.identity);
					Debug.Log(hit.collider.gameObject.name + ": " + hit.collider.gameObject.transform.position);
				}
			}*/
			//option 2: movetoward till dist <= sphere radius
			/* Alternatives to raycast
			* Vector3.MoveToward()
			* Quaternion.Lerp
			* Quaternion.
			*/
			/*int i = 0;
			while ((state.getSelected().transform.position - playerHead.transform.position).magnitude > 0.5f && i < 1000)
			{
				state.getSelected().transform.position = Vector3.MoveTowards(state.getCursorPosition(), playerHead.transform.position, 0.00000001f * Time.deltaTime); //move in increments of 1?
				i++;
			}*/
			state.user.logTag(state.getSelected(), MakeWordBank.imageIndex);
			state.setSelected(null);
        }

		else if (objectClicked != null && objectClicked.tag == "Bin" && state.getSelected() != null)
		{// The bin was pressed, so we move the tag to the bin
			state.getSelected().GetComponentInChildren<Text>().color = Color.black; //transform tag
			state.getSelected().transform.tag = "TrashedTag"; //retag
			state.getSelected().transform.GetChild(0).tag = "TrashedTag";
			state.getSelected().transform.SetParent(GameObject.Find("trashObjects").transform);
			state.getSelected().transform.localScale = trashTagDownScale;// GameObject.Find("trashTagRef").transform.localScale;
			state.getSelected().transform.position = GameObject.Find("trashTagRef").transform.position;
			state.getSelected().transform.localPosition -= new Vector3(0f, 1.05f*trashedTags.Count, 0f);
			trashedTags.Add(state.getSelected());
			if (trashedTags.Count >= 5 && trashedTags.Count < 11)
            {
				GameObject.Find("trashObjects").transform.localPosition += new Vector3(0f, 1.05f, 0f); //raise all tags to fit more
				/*foreach (Transform tagThing in GameObject.Find("trashObjects").transform) //GameObject.Find("Bin").transform
				{
					tagThing.localPosition += new Vector3(0f, 1.05f, 0f); //raise all tags to fit more
				}*/
			}
			//trashedTags[trashedTags.Count - 1].layer = 5; //UI

			//MakeWordBank.replaceTag(state.getSelected(), false); //check over
			state.setSelected(null);
			//Debug.Log("Getting to this step in trash: " + trashedTags[trashedTags.Count-1].name);
			tagIsFollowing = false;
		}

		else if (objectClicked != null && objectClicked.tag == "interactableTag" && state.getSelected() == null)
		{// A tag was pressed  *******
		 //state.getCursorPosition().x < MakeWordBank.tagsRemainingText.transform.position.x
		 //initTagPos = objectClicked.transform.localPosition; //save position of tag
		 //lastTag = VRUser.interactables[objConv];
			if (state.getSelected() == null)
            { //double checking
				tagCopy = Instantiate(objectClicked, objectClicked.transform); //create copy of tag to click/drag
				state.setSelected(tagCopy); //clicked tag = copy of tag
			}
			else
            {
				Debug.LogError("Tag selection error: selecting while holding?");
            }
			state.getSelected().transform.localScale -= new Vector3(0.97f, 0.97f, 0.97f);
			state.getSelected().GetComponent<Image>().color = VRUser.tagColor;
			tagCopy.GetComponentInChildren<Text>().color = Color.red; //changes clicked tag's text color to red
			//tagCopy.layer = 4; //UI Layer
			//Image newImage = Instantiate(tagCopy.GetComponent<Image>()); //deep copy image
            //Destroy(tagCopy.GetComponent<Image>());
			//tagCopy.AddComponent<Image>();

			tagIsFollowing = true;

			/*for (int i = 0; i < MakeWordBank.tags.Length; i++)
			{
				if (tagCopy.GetComponentInChildren<Text>().text.Equals(MakeWordBank.tags[i].getText()))
				{//if a tag's text equals the selected tag's text, make sure its not changing color
					MakeWordBank.tags[i].isChangingColor = false;
				}
			}*/
			state.getSelected().transform.SetParent(GameObject.Find("SelectedTag").transform);
        }

		else if (objectClicked != null && objectClicked.tag == "QuitButton") // Quit button clicked
		{
			//destroyTags();
		}
		else if (objectClicked != null && objectClicked.tag == "NextButton") // Next button clicked
		{
			//++MakeWordBank.imageIndex;
			//MakeWordBank.nextImage(MakeWordBank.imageIndex);
			//tagSphere.GetComponent<Renderer>().material = imageMaterials[imageIndex]
		}
        else
        {
			Debug.Log("OnPointerClicked is not doing the things...");
			//Debug.Log("Obj: " + objectClicked.name + ", tag: " + objectClicked.tag);
        }
	}
    
	//Button/Tag Tracking Things
	public static bool tag1Close()
	{
		Vector3 diff = state.getCursorPosition() + VRUser.uiButtonOffset;
		if (diff.x > 25 && diff.x < 55.8 && diff.y > 4.2 && diff.y < 16.8)
		{
			return true;
		}
		return false;
	}
	public static bool tag2Close()
	{
		Vector3 diff = state.getCursorPosition() + VRUser.uiButtonOffset;
		if (diff.x > 25 && diff.x < 55.8 && diff.y > -12.7 && diff.y < 0.7)
		{
			return true;
		}
		return false;
	}
	public static bool tag3Close()
	{
		Vector3 diff = state.getCursorPosition() + VRUser.uiButtonOffset;
		if (diff.x > 25 && diff.x < 55.8 && diff.y > -28.7 && diff.y < -15.6)
		{
			return true;
		}
		return false;
	}
	public static bool tag4Close()
	{
		Vector3 diff = state.getCursorPosition() + VRUser.uiButtonOffset;
		if (diff.x > 25 && diff.x < 55.8 && diff.y > -45.5 && diff.y < -32.2)
		{
			return true;
		}
		return false;
	}

	public static bool binClose() //bin
	{
		Vector3 diff = state.getCursorPosition() + VRUser.uiButtonOffset;
		if (diff.x > 58.5 && diff.x < 76.3 && diff.y > -20 && diff.y < 33.8)
		{
			return true;
        }
		return false;
	}
	public static bool uiButtonClose() //next
	{
		Vector3 diff = state.getCursorPosition() + VRUser.uiButtonOffset;
		if (diff.x > 25.8 && diff.x < 55 && diff.y > 21.6 && diff.y < 33.8)
        {
			return true;
        }
		return false;
	}
	public static bool uiButtonClose2() //home
	{
		Vector3 diff = state.getCursorPosition() + VRUser.uiButtonOffset;
		if (diff.x > 25.8 && diff.x < 55 && diff.y > -61.8 && diff.y < -49.7)
		{
			return true;
		}
		return false;
	}

	public static int homeButtonClose() //buttons in home sreen
    {
		if (state.getCursorPosition().x > -13 && state.getCursorPosition().x < 19.2)
        {
			if (state.getCursorPosition().y > 13.7 && state.getCursorPosition().y < 27)
			{
				return 1; //start
			}
			else if (state.getCursorPosition().y > -4.8 && state.getCursorPosition().y < 8.4)
			{
				return 2; //user
			}
			else if (state.getCursorPosition().y > -22.6 && state.getCursorPosition().y < -9.8)
			{
				return 3; //calibrate
			}
			else if (state.getCursorPosition().y > -42.2 && state.getCursorPosition().y < -28.6)
			{
				return 4; //tutorial
			}
		}
        else
        {
			if (state.getCursorPosition().y > -27.5 && state.getCursorPosition().y < -14 && state.getCursorPosition().x > -55.4 && state.getCursorPosition().x < -22.5)
			{
				return 6; //quit
			}
			else if (state.getCursorPosition().y > -27.5 && state.getCursorPosition().y < -14 && state.getCursorPosition().x > 29.4 && state.getCursorPosition().x < 62)
            {
				return 5; //about
            }
		}
		return 0;
    }
	public static int profileButtonClose() //ui in profile screen
	{
		if (state.getCursorPosition().y > 18.3 && state.getCursorPosition().y < 26.2 && state.getCursorPosition().x > -31.4 && state.getCursorPosition().x < 0.5)
		{
			return 1; //user name field
		}
		else if (state.getCursorPosition().y > -28 && state.getCursorPosition().y < -15 && state.getCursorPosition().x > -48 && state.getCursorPosition().x < -18)
		{
			return 2; //home
		}
		else if (state.getCursorPosition().y > -27.3 && state.getCursorPosition().y < -20 && state.getCursorPosition().x > -12.5 && state.getCursorPosition().x < 33)
		{
			return 3; //difficulty
		}
		return 0;
	}

	public static void offsetTagsCloser()
	{ // translates tag obj based on cursor/user position
	  // x[-90, 88], y[-90, 66]
	  // a/A = b/B, (a*B)/A = b
		float a = (GameObject.Find("headsetForward").transform.position - playerHead.transform.position).magnitude;
		float A = (GameObject.Find("cursorCenter").transform.position - playerHead.transform.position).magnitude;
		float B = (state.getSelected().transform.position - playerHead.transform.position).magnitude; //GameObject.Find("exampleCursor")
		float b = a * B / A;
		float offset = Math.Abs(B - A);
		offset = 1 - offset;
		offset *= b / 20f;
		showNum = offset;
		Debug.Log("Tag Offset On Place: " + offset);

		/*Vector3 moveTo = new Vector3();
		Debug.Log("Cursor center-x offset: " + (state.getCursorPosition().x - GameObject.Find("cursorCenter").transform.position.x));
		if ((state.getCursorPosition().x - GameObject.Find("cursorCenter").transform.position.x) < 1) //left side of the screen
		{
			moveTo = (playerHead.transform.position + GameObject.Find("headsetLeft").transform.position * offset) / 2; //move left/right*offset based on tag
		}
        else
        {
			moveTo = (playerHead.transform.position + GameObject.Find("headsetRight").transform.position * offset) / 2;
		}
		Debug.Log("Testing Head Offset: " + moveTo + ", vs. " + playerHead.transform.position);*/

		GameObject tempTag = state.getSelected();
		tempTag.transform.SetParent(playerHead.transform);
		Vector3 moveTo = tempTag.transform.localPosition; //local vs world space  - position
		Vector3 current = playerHead.transform.position; ///2?
		float moveDist = gameSphere.GetComponent<SphereCollider>().radius * (gameSphere.transform.localScale.x + gameSphere.transform.localScale.y+ gameSphere.transform.localScale.z)/3f * 0.8f;
		Debug.Log("MoveDist should be 1.3: " + moveDist.ToString()); //image radius(.5) * image scale(3.47) * .75 = 1.3
		state.getSelected().transform.position = Vector3.MoveTowards(current, moveTo, moveDist);
		tempTag.transform.SetParent(tagCanvas.transform);
		//state.getSelected().transform.LookAt(playerHead.transform);
	}

	//cleaning crew
	public static void dropObject()
	{
		Destroy(state.getSelected());
		state.setSelected(null);
		tagIsFollowing = false;
		lastTag = -1;// new Vector3(0f,0f,0f);
					 //tagCopy = null;
	}
	public static void destroyTags()
    {
		Debug.Log("Tag clearing");
		foreach (GameObject tag in state.tagsPlaced)
		{
			Debug.Log("Tag Destroyed-" + tag.name);
			if (tag != null)
			{
				Destroy(tag); //preventing memory leakage?
			}
		}
		state.tagsPlaced.Clear();
		clearTrash();
	}
	public static void clearTrash()
    {
		Debug.Log("Trash clearing");
		foreach (GameObject tag in trashedTags)
		{
			if (tag != null)
			{
				Destroy(tag); //preventing memory leakage?
			}
		}
		trashedTags.Clear();
	}

	//old...
	internal static void OnPointerClick(EventSystem current) //not sure what this is for
	{
		throw new NotImplementedException();
	}
	/*
	public void OnPointerClick(PointerEventData eventData) //not in use atm...
	{
		// OnClick code goes here ...
		GameObject objectClicked = eventData.pointerCurrentRaycast.gameObject; // get the object that was pressed

		Debug.Log("Clicked: " + objectClicked.tag);

		if (objectClicked.tag == "Tag") // A tag was pressed
		{
			//if (MakeWordBank.inTutorial && MakeWordBank.stepOfTutorial != 4 && MakeWordBank.stepOfTutorial != 8)
   //         {
   //             return;
   //         }
			//if (MakeWordBank.stepOfTutorial == 8 && MakeWordBank.timeSpentOnStep8 <= 0.25f) {
			//	return; //prevents glitch
			//}
            
			//if (objectClicked.GetComponentInChildren<Text> ().color == Color.red) {
			//	return; //Prevents trasher from clicking on a tag the tagger has selected in multiplayer
			//}
   //         if (objectClicked.GetComponent<Text>() != null && objectClicked.GetComponent<Text>().color == Color.red)
   //         {
   //             return;
   //         }

			Debug.Log(objectClicked.name); // Name of the object
			GameObject currentTag = state.getSelected();

			if (currentTag != null && currentTag.GetComponent<Text>() != null)
			{
				currentTag.GetComponent<Text>().color = Color.black; // Reset the color of the previously selected tag
			}
			state.setSelected(objectClicked);

			//if (MakeWordBank.trasherPanel.transform.localPosition.y >= 3000) { //If the player doesn't have a panel blocking putting tags on the image:
			objectClicked.GetComponentInChildren<Text>().color = Color.red;
			for (int i = 0; i < MakeWordBank.tags.Length; i++)
			{
				if (objectClicked.GetComponentInChildren<Text>().text.Equals(MakeWordBank.tags[i].getText()))
				{
					MakeWordBank.tags[i].isChangingColor = false;
				}
			}
			//}

			if (cursorTag != null)
			{
				Destroy(cursorTag);
			}

			//Make tag that follows cursor:
			cursorTag = Instantiate(state.getSelected().transform.parent.gameObject, canvas.transform);
			cursorTag.transform.LookAt(Vector3.zero);
			//cursorTag.transform.Rotate (new Vector3 (0f, 0f, -3f));
			cursorTag.layer = 5; //UI Layer
			if (cursorSphere != null)
			{
				cursorSphere.GetComponent<MeshRenderer>().enabled = false;
			}
			//cursorTag.name = currentTag.GetComponent<Text>().name;
			//cursorTag.transform.localScale = new Vector3(8.8f, 3.188f, 0.001f);
		}
		else if (objectClicked.tag == "QuitButton" && !MakeWordBank.inTutorial) // Quit button clicked by falcon
		{
			QuitGameScript.TaskOnClick();
		}
		else if (objectClicked.tag == "NextButton" && !MakeWordBank.inTutorial && !MakeWordBank.inPracticeLevel) // Next button clicked
		{
			++MakeWordBank.imageIndex;
			MakeWordBank.nextImage(MakeWordBank.imageIndex);
		}
		else if (objectClicked.tag == "Bin") // The bin was pressed, so we move the tag to the bin
		{
			Debug.Log("Bin Clicked");
			GameObject currentTag = state.getSelected();
			if (currentTag != null)
			{
				if (MakeWordBank.sequenceIndex < MakeWordBank.wordBank.Count)
				{
					if (!MakeWordBank.inPracticeLevel && !MakeWordBank.inTutorial)
					{
						DataCollector.AddTag(currentTag.transform.parent.name);
					}
					GameObject newTrashedTag = Instantiate(state.getSelected().transform.parent.gameObject, canvas.transform);
					newTrashedTag.transform.localScale
					= new Vector3(newTrashedTag.transform.localScale.x / 2.0f, newTrashedTag.transform.localScale.y / 2.5f, newTrashedTag.transform.localScale.z);
					newTrashedTag.transform.GetChild(0).GetComponent<Text>().color = Color.black;
					newTrashedTag.transform.tag = "TrashedTag";
					newTrashedTag.transform.GetChild(0).tag = "TrashedTag";
					int verticalBump = 0;
					if (trashedTags.Count >= 14 && trashedTags.Count < 28)
					{
						verticalBump = 168; //To prevent overlap
					}
					else if (trashedTags.Count >= 28 && trashedTags.Count < 42)
					{
						verticalBump = 606;
					}
					else if (trashedTags.Count >= 42)
					{
						verticalBump = 774;
					}

					int horizontalBump = 0;
					if (trashedTags.Count >= 14 && trashedTags.Count < 28)
					{
						horizontalBump = 50;
					}
					else if (trashedTags.Count >= 28 && trashedTags.Count < 42)
					{
						horizontalBump = 0;
					}
					else if (trashedTags.Count >= 42)
					{
						horizontalBump = 50;
					}
					newTrashedTag.transform.position = canvas.transform.TransformPoint(new Vector2(320 + horizontalBump, -55 - 12 * trashedTags.Count + verticalBump)) + Vector3.back * -0.25f;
					newTrashedTag.transform.LookAt(newTrashedTag.transform.position + Vector3.back * newTrashedTag.transform.position.z * -1);
					trashedTags.Add(newTrashedTag);
					trashedTags[trashedTags.Count - 1].layer = 5; //UI
				}
				MakeWordBank.replaceTag(currentTag, false);
				currentTag.GetComponentInChildren<Text>().color = Color.clear;
				currentTag.GetComponent<Text>().color = Color.clear; // Reset the color of the previously selected tag
			}
			if (cursorTag != null)
			{
				Destroy(cursorTag);
				cursorTag = null;
				if (cursorSphere != null)
				{
					cursorSphere.GetComponent<MeshRenderer>().enabled = true;
				}
			}
			state.setSelected(null);
		}
		else if (objectClicked.tag == "Image") // The image area was pressed, so here we cast a tag onto the sphere
		{
			Debug.Log("Image Clicked");
			GameObject currentTag = state.getSelected();
			if (currentTag != null && !currentTag.transform.parent.name.Equals(""))
			{
				Vector3 cursorPosition = Camera.current.WorldToScreenPoint(state.getCursorPosition()); // Use the cursor position to cast a ray onto the sphere
				Ray ray = Camera.main.ScreenPointToRay(cursorPosition);  // The ray that will be casted onto the sphere

				// In the following two lines, since the sphere collider is outside the sphere
				// We move the point of the ray wellsp outside of the here, then invert the direction
				// This way, we cast ray to the same point of the sphere, but from the outside rather than the inside
				ray.origin = ray.GetPoint(100);
				ray.direction = -ray.direction;

				RaycastHit hit; // The raycast

				Debug.DrawRay(ray.origin, ray.direction, Color.red, 5);
				if (Physics.Raycast(ray, out hit))
				{
					Destroy(cursorTag);
					cursorTag = null;
					if (cursorSphere != null)
					{
						cursorSphere.GetComponent<MeshRenderer>().enabled = true;
					}
					GameObject newObject = Instantiate(tagPrefab, hit.point * 0.95f, Quaternion.identity); // Create the new object using the tagPrefab
					newObject.transform.LookAt(Vector3.zero); // Make it face the center of the sphere
					newObject.transform.localScale = new Vector3(0.25f, 0.1f, 0.00001f);
					newObject.name = currentTag.transform.parent.name; // CHANGE THIS LATER
					newObject.transform.parent = sphere.transform;
					newObject.GetComponent<Renderer>().material = new Material(Shader.Find("Diffuse"));

					// Create the object which will hold the TextMesh
					GameObject textContainer = new GameObject();
					textContainer.transform.parent = newObject.transform;

					// Create the text mesh to be rendered over the plane
					TextMesh text = textContainer.AddComponent<TextMesh>();
					text.text = currentTag.transform.parent.name;
					text.fontSize = 20;
					text.alignment = TextAlignment.Center;
					text.anchor = TextAnchor.MiddleCenter;
					text.name = currentTag.transform.parent.name + "_Text";
					text.transform.parent = textContainer.transform;
					text.transform.localScale = new Vector3(-0.075f, 0.25f, 0.25f);
					text.transform.localPosition = Vector3.zero;
					text.transform.localEulerAngles = Vector3.zero;
					if (!MakeWordBank.inTutorial && !MakeWordBank.inPracticeLevel)
					{
						DataCollector.AddTag(currentTag.transform.parent.name, newObject.transform.position);
					}

					//int diff = MakeWordBank.sequenceIndex; //This is just a convoluted way to find out if the image turned over so the trashed tag prefabs can be deleted
					MakeWordBank.replaceTag(currentTag, true);
					currentTag.GetComponentInChildren<Text>().color = Color.clear;
					state.setSelected(null);
					//diff -= MakeWordBank.sequenceIndex;
					//if (diff > 0) { //Means image turned over:
					//	for (int i = 0; i < trashedTags.Count; i++) {
					//		Destroy (trashedTags [i]);
					//	}
					//	trashedTags.Clear();
					//}


					// ---- Below is old code used to create the tag whereever the click happened. It isn't being used now but may be useful later
					// --------------------------------------------------------------------------------------------------------------------------
					//GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.tag, hit.point * 0.95f, Quaternion.identity);
					//gameObject.transform.LookAt(Vector3.zero);
					//gameObject.name = "Tag " + this.tags.Count;
					//gameObject.transform.localScale = new Vector3(20f, 5f, 1f);
					//this.tag.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
					//gameObject.AddComponent(typeof(Tag));
					//gameObject.transform.parent = this.tagContainer.transform;
					//this.tags.Add(gameObject);
					//this.keycam.transform.position = Vector3.zero;
					//this.keycam.transform.LookAt(this.ray.point);
					//this.keycam.transform.position = Vector3.MoveTowards(this.keycam.transform.position, this.ray.point, Vector3.Distance(this.keycam.transform.position, this.ray.point) * 0.8f);
					//this.startTag(gameObject);
				}
			}
		}
	}
	*/
}