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
	public static List<GameObject> trashedTags;

    public Material tagMaterial;

    public void Awake()
    {
        state = GameObject.Find("Canvas").GetComponent<StateManager>();

        tagPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
		tagPrefab.name = "TagPrefab"; //So it can be destroyed
        tagPrefab.transform.localScale = Vector3.zero;

        sphere = GameObject.Find("TagSphere");
		canvas = GameObject.Find ("Canvas");

        cursorSphere = GameObject.Find("CursorSphere");
		trashedTags = new List<GameObject> ();
    }

	//This method is only needed when the user has clicked a tag, and the instantiated GameObject tag needs to follow the cursor:
	public void Update() {
		if (cursorTag != null) {
			try {
				//cursorTag.transform.localPosition = new Vector3(state.getCursorPosition().x, state.getCursorPosition().y, 100.25f);
			//= new Vector3 (state.getCursorPosition().x, state.getCursorPosition().y, canvas.transform.position.z - 0.5f);
			cursorTag.transform.localScale = new Vector3(-1f, 1f, 0.001f);
			Vector2 pos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Camera.current.WorldToScreenPoint(state.getCursorPosition()), Camera.current, out pos);
			cursorTag.transform.position = canvas.transform.TransformPoint(pos) + Vector3.back * -0.25f;
			cursorTag.transform.LookAt(cursorTag.transform.position + Vector3.back * cursorTag.transform.position.z);
			}
			catch (Exception e)
			{}
		}

		if (MakeWordBank.stepOfTutorial == 11 && trashedTags.Count != 0) {
			for (int i = 0; i < trashedTags.Count; i++) {
				Destroy (trashedTags [i]);
			}
			trashedTags.Clear();
		}
	}

    public void OnPointerClick(PointerEventData eventData)
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
				objectClicked.GetComponentInChildren<Text> ().color = Color.red;
				for (int i = 0; i < MakeWordBank.tags.Length; i++) {
					if (objectClicked.GetComponentInChildren<Text> ().text.Equals (MakeWordBank.tags[i].getText ())) {
						MakeWordBank.tags [i].isChangingColor = false;
					}
				}
			//}

			if (cursorTag != null) {
				Destroy (cursorTag);
			}

			//Make tag that follows cursor:
			cursorTag = Instantiate (state.getSelected().transform.parent.gameObject, canvas.transform);
			cursorTag.transform.LookAt (Vector3.zero);
			//cursorTag.transform.Rotate (new Vector3 (0f, 0f, -3f));
			cursorTag.layer = 5; //UI Layer
            if (cursorSphere != null)
            {
                cursorSphere.GetComponent<MeshRenderer>().enabled = false;
            }
			//cursorTag.name = currentTag.GetComponent<Text> ().name;
			//cursorTag.transform.localScale = new Vector3 (8.8f, 3.188f, 0.001f);
        }
		else if (objectClicked.tag == "QuitButton" && !MakeWordBank.inTutorial) // Quit button clicked by falcon
        {
            QuitGameScript.TaskOnClick();
        }
        else if (objectClicked.tag == "NextButton" && !MakeWordBank.inTutorial && !MakeWordBank.inPracticeLevel) // Next button clicked
        {
            MakeWordBank.nextImage();
        }
        else if (objectClicked.tag == "Bin") // The bin was pressed, so we move the tag to the bin
        {
            Debug.Log("Bin Clicked");
            GameObject currentTag = state.getSelected();
            if (currentTag != null)
            {
				if (MakeWordBank.sequenceIndex < MakeWordBank.wordBank.Count) {
					if (!MakeWordBank.inPracticeLevel && !MakeWordBank.inTutorial) {
						DataCollector.AddTag (currentTag.transform.parent.name);
					}
					GameObject newTrashedTag = Instantiate (state.getSelected ().transform.parent.gameObject, canvas.transform);
					newTrashedTag.transform.localScale 
					= new Vector3 (newTrashedTag.transform.localScale.x / 2.0f, newTrashedTag.transform.localScale.y / 2.5f, newTrashedTag.transform.localScale.z);
					newTrashedTag.transform.GetChild (0).GetComponent<Text> ().color = Color.black;
					newTrashedTag.transform.tag = "TrashedTag";
					newTrashedTag.transform.GetChild (0).tag = "TrashedTag";
					int verticalBump = 0;
					if (trashedTags.Count >= 14 && trashedTags.Count < 28) {
						verticalBump = 168; //To prevent overlap
					} else if (trashedTags.Count >= 28 && trashedTags.Count < 42) {
						verticalBump = 606;
					} else if (trashedTags.Count >= 42) {
						verticalBump = 774;
					}
					
					int horizontalBump = 0;
					if (trashedTags.Count >= 14 && trashedTags.Count < 28) {
						horizontalBump = 50;
					} else if (trashedTags.Count >= 28 && trashedTags.Count < 42) {
						horizontalBump = 0;
					} else if (trashedTags.Count >= 42) {
						horizontalBump = 50;
					}
					newTrashedTag.transform.position = canvas.transform.TransformPoint(new Vector2(320 + horizontalBump, -55 - 12*trashedTags.Count + verticalBump)) + Vector3.back * -0.25f;
					newTrashedTag.transform.LookAt(newTrashedTag.transform.position + Vector3.back * newTrashedTag.transform.position.z * -1);
					trashedTags.Add (newTrashedTag);
					trashedTags[trashedTags.Count - 1].layer = 5; //UI
				}
                MakeWordBank.replaceTag(currentTag, false);
				currentTag.GetComponentInChildren<Text>().color = Color.clear;
                currentTag.GetComponent<Text>().color = Color.clear; // Reset the color of the previously selected tag
            }
			if (cursorTag != null) {
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
                    GameObject newObject = Instantiate (tagPrefab, hit.point * 0.95f, Quaternion.identity); // Create the new object using the tagPrefab
					newObject.transform.LookAt (Vector3.zero); // Make it face the center of the sphere
					newObject.transform.localScale = new Vector3 (0.25f, 0.1f, 0.00001f);
					newObject.name = currentTag.transform.parent.name; // CHANGE THIS LATER
					newObject.transform.parent = sphere.transform;
                    newObject.GetComponent<Renderer>().material = new Material(Shader.Find("Diffuse"));

                    // Create the object which will hold the TextMesh
                    GameObject textContainer = new GameObject ();
					textContainer.transform.parent = newObject.transform;
                
					// Create the text mesh to be rendered over the plane
					TextMesh text = textContainer.AddComponent<TextMesh> ();
					text.text = currentTag.transform.parent.name;
					text.fontSize = 20;
					text.alignment = TextAlignment.Center;
					text.anchor = TextAnchor.MiddleCenter;
					text.name = currentTag.transform.parent.name + "_Text";
					text.transform.parent = textContainer.transform;
					text.transform.localScale = new Vector3 (-0.075f, 0.25f, 0.25f);
					text.transform.localPosition = Vector3.zero;
					text.transform.localEulerAngles = Vector3.zero;
					if (!MakeWordBank.inTutorial && !MakeWordBank.inPracticeLevel) {
						DataCollector.AddTag (currentTag.transform.parent.name, newObject.transform.position);
					}

					//int diff = MakeWordBank.sequenceIndex; //This is just a convoluted way to find out if the image turned over so the trashed tag prefabs can be deleted
					MakeWordBank.replaceTag (currentTag, true);
					currentTag.GetComponentInChildren<Text>().color = Color.clear;
					state.setSelected (null);
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
	public void OnPointerClick(GameObject objectClicked)
	{
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
			//cursorTag.name = currentTag.GetComponent<Text> ().name;
			//cursorTag.transform.localScale = new Vector3 (8.8f, 3.188f, 0.001f);
		}
		else if (objectClicked.tag == "QuitButton" && !MakeWordBank.inTutorial) // Quit button clicked by falcon
		{
			QuitGameScript.TaskOnClick();
		}
		else if (objectClicked.tag == "NextButton" && !MakeWordBank.inTutorial && !MakeWordBank.inPracticeLevel) // Next button clicked
		{
			MakeWordBank.nextImage();
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
	internal static void OnPointerClick(EventSystem current)
    {
        throw new NotImplementedException();
    }
}