using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickAction : MonoBehaviour, IPointerClickHandler
{
    public static StateManager state; // State of the application

	public static GameObject tagPrefab;
	public static GameObject sphere;
	public static GameObject canvas;

	public static GameObject cursorTag; //Tag that follows cursor
	public static GameObject cursorSphere; // Falcon cursor
	public static List<GameObject> trashedTags; //representation of tags
	public Material tagMaterial;

	public static GameObject tagCopy;
	public static Vector3 initTagPos;
	public static GameObject background; //image ref

    public static bool tagIsFollowing = false;

	public static GameObject trashy; //trash ref
    public static float maxBinDist = 8f; //distance of cursor from bin after cursorPosMod factor

	public static GameObject nextButton;
	public static GameObject quitButton;
	public void Awake()
    {
        state = GameObject.Find("Canvas").GetComponent<StateManager>();

		tagPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
		tagPrefab.name = "TagPrefab"; //So it can be destroyed
        tagPrefab.transform.localScale = Vector3.zero;

        sphere = GameObject.Find("gameSphere"); //TagSphere
		canvas = GameObject.Find ("Canvas");

        cursorSphere = GameObject.Find("CursorSphere");
		trashedTags = new List<GameObject> ();
		background = GameObject.Find("Image"); // backgreound for the location where to place an image
        trashy = GameObject.Find("Bin");
		nextButton = MakeWordBank.nextButton;
		quitButton = MakeWordBank.quitButton;
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
		if (MakeWordBank.stepOfTutorial == 11 && trashedTags.Count != 0) {
			for (int i = 0; i < trashedTags.Count; i++) {
				Destroy (trashedTags [i]);
			}
			trashedTags.Clear();
		}

		//bool isNull = state.getSelected() == null;
		//Debug.Log("Selected Elem: " + state.getSelected() + ", isNull? " + isNull.ToString());

		if (tagIsFollowing)
		{
			state.getSelected().transform.localPosition = state.getCursorPosition();
		}
        Debug.Log("Bin Distance: " + (trashy.transform.position - state.getCursorPosition()).magnitude + ", isBy? " + binClose(state.getCursorPosition()) + ", Count: " + trashedTags.Count);
        Debug.Log("Next Button Distance: " + (nextButton.transform.position - state.getCursorPosition()).magnitude);
        Debug.Log("Quit Button Distance: " + (quitButton.transform.position - state.getCursorPosition()).magnitude);
    }
	public void OnPointerClick(GameObject objectClicked = null) //same method but takes in game obj
	{
        //if (objectClicked != null) // check later
        //{
        //    objectClicked = background;
        //}

        if (objectClicked == null && binClose(state.getCursorPosition()) && state.getSelected() != null)
		{//if the cursor is over the trash, the obj we are looking at is the trash
			objectClicked = trashy;
        }

        if (objectClicked == null && state.getSelected() != null && state.getCursorPosition().x < -101f) // tag was placed  *******
        {
            state.getSelected().GetComponentInChildren<Text>().color = Color.blue; //instead of just GetComponent
			state.getSelected().transform.localScale -= new Vector3(0.5f, 0.5f, 0f); //scale it down to 40% size (not thickness tho)
            tagIsFollowing = false;
			state.tagsPlaced.Add(state.getSelected()); //adds to movement list
            state.setSelected(null);
        }

		else if (objectClicked.tag == "Bin" && state.getSelected() != null) // The bin was pressed, so we move the tag to the bin
		{
			//if (!MakeWordBank.inPracticeLevel && !MakeWordBank.inTutorial)
			//{
			//	DataCollector.AddTag(state.getSelected().transform.parent.name);
			//}
			state.getSelected().GetComponentInChildren<Text>().color = Color.black; //transform tag
			state.getSelected().transform.localScale -= new Vector3(0.85f, 0.85f, 0f);
			state.getSelected().transform.tag = "TrashedTag";
			state.getSelected().transform.GetChild(0).tag = "TrashedTag";

			//newTrashedTag.transform.position = canvas.transform.TransformPoint(new Vector2(320 + horizontalBump, -55 - 12 * trashedTags.Count + verticalBump)) + Vector3.back * -0.25f;
			state.getSelected().transform.position = new Vector3(trashy.transform.position.x, trashy.transform.position.y - 12.7f - (8.5f*trashedTags.Count), trashy.transform.position.z);
			state.getSelected().transform.LookAt(state.getSelected().transform.position + Vector3.back * state.getSelected().transform.position.z * -1);
            trashedTags.Add(state.getSelected());
			//trashedTags[trashedTags.Count - 1].layer = 5; //UI

			//MakeWordBank.replaceTag(state.getSelected(), false); //check over
			state.setSelected(null);
            tagIsFollowing = false;
		}

		else if (objectClicked.tag == "Tag" && state.getSelected() == null) // A tag was pressed  *******
		{//state.getCursorPosition().x < MakeWordBank.tagsRemainingText.transform.position.x
		 //Debug.Log(objectClicked.name); // Name of the object

			initTagPos = objectClicked.transform.position; //save position of tag
			tagCopy = Instantiate(objectClicked, canvas.transform); //create copy of tag to click/drag
			state.setSelected(tagCopy); //clicked tag = copy of tag

			tagCopy.GetComponentInChildren<Text>().color = Color.red; //changes clicked tag's text color to red
			tagCopy.layer = 4; //UI Layer
			//Image newImage = Instantiate(tagCopy.GetComponent<Image>()); //deep copy image
            //Destroy(tagCopy.GetComponent<Image>());
			//tagCopy.AddComponent<Image>();

			tagIsFollowing = true;

			for (int i = 0; i < MakeWordBank.tags.Length; i++)
			{
				if (tagCopy.GetComponentInChildren<Text>().text.Equals(MakeWordBank.tags[i].getText()))
				{//if a tag's text equals the selected tag's text, make sure its not changing color
					MakeWordBank.tags[i].isChangingColor = false;
				}
			}
        }

		else if (objectClicked.tag == "QuitButton") // Quit button clicked
		{
			//destroyTags();
		}
		else if (objectClicked.tag == "NextButton") // Next button clicked
		{
			++MakeWordBank.imageIndex;
			MakeWordBank.nextImage(MakeWordBank.imageIndex);
			//tagSphere.GetComponent<Renderer>().material = imageMaterials[imageIndex]
		}

        ////else if ((objectClicked == null || objectClicked.tag == "Image") && state.getSelected() != null) // The image area was pressed, so here we cast a tag onto the sphere
        ////{
        //Vector3 cursorPosition = Camera.current.WorldToScreenPoint(state.getCursorPosition()); // Use the cursor position to cast a ray onto the sphere
        //Ray ray = Camera.main.ScreenPointToRay(cursorPosition);  // The ray that will be casted onto the sphere

        //// In the following two lines, since the sphere collider is outside the sphere
        //// We move the point of the ray well outside of the sphere, then invert the direction
        //// This way, we cast ray to the same point of the sphere, but from the outside rather than the inside
        //ray.origin = ray.GetPoint(100);
        //ray.direction = -ray.direction;

        //RaycastHit hit; // The raycast

        //Debug.DrawRay(ray.origin, ray.direction, Color.red, 5);
        //if (Physics.Raycast(ray, out hit))
        //{

        //}

        ////game object instiantiated
        //GameObject newObject = Instantiate(tagPrefab, canvas.transform.position, Quaternion.identity); // Create the new object using the tagPrefab
        //newObject.transform.LookAt(Vector3.zero); // Make it face the center of the sphere*
        //newObject.transform.localScale = new Vector3(0.25f, 0.1f, 0.00001f);
        //newObject.name = state.getSelected().transform.parent.name; // CHANGE THIS LATER
        //newObject.transform.parent = sphere.transform;
        ////newObject.GetComponent<Renderer>().material = new Material(Shader.Find("Diffuse"));

        ////object to hold container
        //GameObject textContainer = new GameObject();
        //textContainer.transform.parent = newObject.transform;

        ////creates text thing that displays
        //TextMesh text = textContainer.AddComponent<TextMesh>();
        //text.text = state.getSelected().transform.parent.name;
        //text.fontSize = 20;
        //text.alignment = TextAlignment.Center;
        //text.anchor = TextAnchor.MiddleCenter;
        //text.name = state.getSelected().transform.parent.name + "_Text";

        ///*state.getSelected().transform.localScale /= 239.36f; // state.cursorPosMod
        //      state.getSelected().transform.position = state.getCursorPosition(); // state.cursorPosMod
        //state.getSelected().transform.LookAt(Vector3.zero);
        //state.getSelected().GetComponentInChildren<Text>().color = Color.red;*

        ////get rid of the existing copy
        //Destroy(state.getSelected());*/
        //state.setSelected(null);

        ////stop following cursor
        //tagIsFollowing = false;
        ////}
        else
        {
			Debug.Log("OnPointerClicked is not doing the things...");
			Debug.Log("Obj: " + objectClicked.name + ", tag: " + objectClicked.tag);
        }
	}
	internal static void OnPointerClick(EventSystem current)
    {
        throw new NotImplementedException();
    }
    public static void dropObject()
    {
		Destroy(state.getSelected());
		state.setSelected(null);
		tagIsFollowing = false;
		initTagPos = new Vector3(0f,0f,0f);
		//tagCopy = null;
	}

	public static bool tagClose(Vector3 pos) //tags
	{ //nextButton.transform.position
		Vector3 diff = pos - state.getCursorPosition();
		// x:[99,-78], y:[32,-25]
		if (diff.x > 96 || diff.x < -75)// old --> x:[-100,78] y:[-33,37]
		{
			return false;
		}
		if (diff.y > 24 || diff.y < -25)
		{
			return false;
		}
		return true;
	}
	public static bool binClose(Vector3 pos) //tags
	{
		Vector3 diff = pos - state.getCursorPosition();
		if (diff.x > -65 || diff.x < -172)
		{
			return false;
		}
		if (diff.y > 282 || diff.y < -197)
		{
			return false;
		}
		return true;
	}
	public static bool uiButtonClose(Vector3 pos) //tags
	{
		Vector3 diff = pos - state.getCursorPosition() + VRUser.uiButtonOffset;
		if (diff.x > 82 || diff.x < -58)
		{
			return false;
		}
		if (diff.y > -68 || diff.y < -114)
		{
			return false;
		}
		return true;
	}
	public static bool uiButtonClose2(Vector3 pos) //tags
	{
		Vector3 diff = pos - state.getCursorPosition() + VRUser.uiButtonOffset;
		if (diff.x > 82 || diff.x < -58)
		{
			return false;
		}
		if (diff.y > 248 || diff.y < 211)
		{
			return false;
		}
		return true;
	}
	public static bool buttonClose2(Vector3 pos) //for home screen ui
	{ //nextButton.transform.position
		Vector3 diff = (pos - HomeScreen.stateModifier) - (state.getCursorPosition() * StateManager.cursorPosMod * HomeScreen.scale); //2:3 scaling modifier																								 //}
		if (diff.y > 11 || diff.y < -19.8)
		{
			return false;
		}
		if (diff.x > 41 || diff.x < -42)
		{
			return false;
		}
		return true;
	}
	public static void destroyTags()
    {
		foreach (GameObject tag in state.tagsPlaced)
		{
			Destroy(tag);
		}
		state.tagsPlaced.Clear();
	}

	//old...
	public void OnPointerClick(PointerEventData eventData) //not in use atm...
	{
		// OnClick code goes here ...
		GameObject objectClicked = eventData.pointerCurrentRaycast.gameObject; // get the object that was pressed

		Debug.Log("Clicked: " + objectClicked.tag);

		if (objectClicked.tag == "Tag") // A tag was pressed
		{
			/*
			if (MakeWordBank.inTutorial && MakeWordBank.stepOfTutorial != 4 && MakeWordBank.stepOfTutorial != 8)
            {
                return;
            }
			if (MakeWordBank.stepOfTutorial == 8 && MakeWordBank.timeSpentOnStep8 <= 0.25f) {
				return; //prevents glitch
			}
            
			if (objectClicked.GetComponentInChildren<Text> ().color == Color.red) {
				return; //Prevents trasher from clicking on a tag the tagger has selected in multiplayer
			}
            if (objectClicked.GetComponent<Text>() != null && objectClicked.GetComponent<Text>().color == Color.red)
            {
                return;
            }
            */

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
			if (currentTag != null && !currentTag.transform.parent.name.Equals("")) // TODO: Check if a tag is currently selected and that the tag isn't blank
			{
				Vector3 cursorPosition = Camera.current.WorldToScreenPoint(state.getCursorPosition()); // Use the cursor position to cast a ray onto the sphere
				Ray ray = Camera.main.ScreenPointToRay(cursorPosition);  // The ray that will be casted onto the sphere

				// In the following two lines, since the sphere collider is outside the sphere
				// We move the point of the ray well outside of the sphere, then invert the direction
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
}