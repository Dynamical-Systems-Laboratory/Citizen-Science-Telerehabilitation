using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MakeWordBank : MonoBehaviour {

	//I'm putting the CSV files in the directory Assets/Tags, 
	//I'm only using image1.csv for now..
	//Hello

	static string image1Path = "Assets/Tags/image1.csv";

	//For the next two arrays, I drag and dropped the objects from the Editor into the script window in the inspector - 
	//This script takes that data and automatically makes however many "Tag" objects (class below) are needed:
	public GameObject[] tagGameObjects;
	public Text[] textObjects;

	//Numbers 0-300 (elements of .csv file) in a random order:
	public static int[] SEQUENCE = {
		180, 225, 165, 123, 74, 17, 188, 55, 274, 191, 58, 269, 52, 128, 203, 95, 169, 175, 146, 75, 3, 194, 70, 
		259, 140, 28, 177, 22, 156, 261, 262, 122, 99, 149, 133, 210, 291, 72, 270, 38, 211, 118, 207, 54, 125, 
		144, 113, 77, 152, 250, 42, 101, 71, 108, 59, 40, 87, 217, 235, 206, 221, 167, 27, 295, 215, 246, 41, 296, 
		219, 26, 272, 208, 183, 104, 232, 172, 164, 171, 39, 256, 284, 158, 170, 220, 15, 255, 111, 264, 145, 237, 
		51, 276, 24, 81, 142, 181, 293, 14, 1, 263, 91, 8, 43, 205, 67, 78, 127, 76, 209, 60, 292, 85, 298, 0, 115,
		35, 130, 25, 243, 124, 139, 7, 173, 184, 29, 273, 117, 47, 282, 260, 196, 253, 30, 254, 36, 248, 252, 65, 
		162, 213, 16, 285, 223, 106, 23, 271, 288, 277, 257, 199, 249, 280, 187, 242, 63, 90, 195, 69, 214, 297, 
		112, 116, 103, 148, 88, 231, 9, 150, 244, 161, 129, 226, 89, 230, 114, 168, 100, 134, 33, 119, 46, 283, 251, 
		239, 155, 57, 120, 204, 198, 121, 6, 19, 31, 174, 176, 102, 50, 92, 110, 236, 37, 11, 159, 83, 48, 197, 
		265, 178, 247, 202, 97, 79, 190, 228, 10, 135, 294, 61, 32, 153, 224, 166, 34, 56, 132, 98, 138, 163, 
		96, 189, 20, 186, 131, 234, 68, 93, 45, 286, 267, 157, 49, 4, 80, 227, 241, 182, 185, 109, 201, 21, 289,
		216, 94, 107, 245, 229, 278, 258, 240, 281, 66, 84, 2, 44, 222, 86, 73, 233, 147, 136, 279, 160, 154, 
		300, 299, 200, 62, 18, 266, 64, 53, 218, 238, 287, 151, 290, 143, 137, 275, 179, 82, 192, 13, 193, 105,
		268, 141, 5, 212, 12, 126
	};
	static int sequenceIndex = 0;

	public static List<string> wordBank = new List<string>();

	//Array of the container class I made below for a "Tag" object - since it's static, 
	//you can have an eventlistener on another class and call methods like MakeWordBank.replaceTag(GameObject obj)
	//which replaces the Tag with the next Tag name in line, uploaded from the .csv file.
	//This script should work fine, the important thing is that the Text objects whose parents are the
	//tag GameObjects should have unique names (doesn't matter what the names are), the parent
	//GameObjects' names can be changed though with no problem
	public static Tag[] tags;

	void Start () {
		tags = new Tag[tagGameObjects.Length];
		for (int i = 0; i < tags.Length; i++) {
			tags [i] = new Tag (tagGameObjects [i], textObjects [i]);
		}
		//Read CSV File:
		using (StreamReader sr = new StreamReader(image1Path))
		{
			string line;

			while ((line = sr.ReadLine()) != null)
			{
				string[] parts = line.Split(',');

				string elem = parts[parts.Length - 1]; //Last column of .csv must be the tag names
				if (!string.Equals (elem, "")) {
					wordBank.Add (elem);
				}
			}
		}
		wordBank.RemoveAt (0); //<-- Column name

		for (int i = 0; i < tags.Length; i++) {
			//I cleaned up the CSV file for image1 (removing duplicates, underscores, etc)
			while (SEQUENCE [sequenceIndex] >= wordBank.Count) {
				sequenceIndex++; //Prevents index from being out of bounds
			}
			tags[i].setText(wordBank [ SEQUENCE[sequenceIndex] ]);
			sequenceIndex++;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	//This method can be called from the EventListener script using the GameObject that was clicked on as input:
	public static void replaceTag(GameObject obj) {
		//Find tag with this object:
		for (int i = 0; i < tags.Length; i++) {
			if(obj.name == tags[i].text.name) {
				if (sequenceIndex < SEQUENCE.Length) {
					while (SEQUENCE [sequenceIndex] >= wordBank.Count) {
						sequenceIndex++; //Prevents index from being out of bounds
					}
					tags[i].setText(wordBank [ SEQUENCE[sequenceIndex] ]);
					sequenceIndex++;
				} else {
					tags [i].setText ("");
				}
			}
		}
	}
}
/*
 * Container class for each tag which contains the 
 * Tag GameObject and the Text object: 
 */
public class Tag {
	public GameObject tag;
	public Text text;
	public Tag(GameObject tag, Text text) {
		this.tag = tag;
		this.text = text;
	}
	public string getText() {
		return text.text;
	}
	public void setText(string next_text) {
		tag.name = next_text;
		//text.name = next_text; //The Text Object name acts as the identifier when you click on it and should be unique
		text.text = next_text;
	}
}