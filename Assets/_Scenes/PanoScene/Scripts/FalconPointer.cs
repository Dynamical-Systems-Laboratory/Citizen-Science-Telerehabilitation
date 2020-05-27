using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FalconPointer : PointerInputModule
{
    private ClickAction eventListener;

    private bool[] buttons;
    private bool prevClick; // To know if the button had been pressed in the last frame

    StateManager state; // State of the application

    public override void Process()
	{
		PointerEventData falconEventData = this.GetFalconEventData();
	}

    public void Awake ()
    {
        state = GameObject.Find("Canvas").GetComponent<StateManager>();
        this.buttons = new bool[] { false, false, false, false }; // Buttons on the Falcon
        eventListener = GameObject.Find("Canvas").GetComponent<ClickAction>();
    }

    public void Update()
    {
        if (state.isKinectReady())
        {
            this.buttons = state.getFalconButtons(); // Which buttons are currently pressed?
            if (!prevClick && (this.buttons[3] || buttons[1]))
            {
                //Process();
                try {
                    eventListener.OnPointerClick(GetFalconEventData());
                }
                catch (Exception e) {
                    Debug.Log("Cannot process eventListener.OnPointerClick(GetFalconEventData())");
                }
            }
            prevClick = (this.buttons[3] || buttons[1]);
        }
    }

    public virtual PointerEventData GetFalconEventData() //was protected
    {
        PointerEventData pointerEventData;
        bool pointerData = base.GetPointerData(-1, out pointerEventData, true);
        pointerEventData.Reset();
        if (pointerData)
        {
            pointerEventData.position = GameObject.Find("CursorCamera").GetComponent<Camera>().WorldToScreenPoint(base.gameObject.transform.position);
        }
        Vector2 vector = GameObject.Find("CursorCamera").GetComponent<Camera>().WorldToScreenPoint(base.gameObject.transform.position);
        pointerEventData.delta = vector - pointerEventData.position;
        pointerEventData.position = vector;
        RaycastHit raycastHit;
        try
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(vector), out raycastHit))
            {
                pointerEventData.pointerCurrentRaycast = new RaycastResult
                {
                    gameObject = raycastHit.transform.gameObject
                };
            }
        }
        catch (Exception e)
        {
        }
        ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(pointerEventData.pointerCurrentRaycast.gameObject, pointerEventData, ExecuteEvents.pointerDownHandler);
        base.eventSystem.RaycastAll(pointerEventData, this.m_RaycastResultCache);
        RaycastResult pointerCurrentRaycast = BaseInputModule.FindFirstRaycast(this.m_RaycastResultCache);
        pointerEventData.pointerCurrentRaycast = pointerCurrentRaycast;
        pointerEventData.button = PointerEventData.InputButton.Left;
        this.m_RaycastResultCache.Clear();
		return pointerEventData;
	}
}
