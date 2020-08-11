// Decompile from assembly: Assembly-CSharp.dll

using System;
using UnityEngine;

public class FalconCursor : MonoBehaviour
{
	private GameObject cursor;

	private Vector3 offset;

	private bool[] buttons = new bool[4];

	public float sensitivity = 1f;

    StateManager state; // State of the application

    private void Start()
	{
		this.cursor = GameObject.Find("CursorSphere");
        state = GameObject.Find("Canvas").GetComponent<StateManager>();
    }

	private void Update()
	{
		if (!state.isKinectReady())
		{
			return;
		}
        buttons = state.getFalconButtons();
        //this.cursor.transform.localPosition = state.getCursorPosition();
	}
}
