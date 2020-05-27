using UnityEngine;

public class RotateCamera : MonoBehaviour
{
	private Vector3 mousepos;
	private Vector3 nextpos;

	public float sensitivity = 1f;
	private float zoom;

	private static bool[] buttons;
	private bool panning;

	private GameObject cam;
    StateManager state; // State of the application

    private Rect cameraBounds;

	private void Start() {
		this.mousepos = Input.mousePosition; // Tracking the mouse position in case the controller isn't connected
		this.zoom = 0f;
        buttons = new bool[] { false, false, false, false };

        this.cam = GameObject.Find("Main Camera");
        // Get camera bounds
        cameraBounds = cam.GetComponent<Camera>().pixelRect;

        state = GameObject.Find("Canvas").GetComponent<StateManager>();

        this.panning = false;
	}

	private void Update()
	{
        
        if (!state.isKinectReady()) // Not using the contoller, so mouse is fine
		{
			if (Input.mouseScrollDelta.y != 0f)
			{
				this.zoom += Mathf.Sign(Input.mouseScrollDelta.y) * 0.1f;
			}
            // Enforce a boundary on the zoom
            this.zoom = Mathf.Max(Mathf.Min(0.8f, this.zoom), 0f);

            // The new camera rotation will depend on the movement of the mouse
			this.nextpos = new Vector3(base.transform.localEulerAngles.x - Input.mousePosition.y + this.mousepos.y, base.transform.localEulerAngles.y + Input.mousePosition.x - this.mousepos.x, 0f);
			this.mousepos = Input.mousePosition;

            // Enforce a boundary on rotating up/down
            if (this.nextpos.x > 270f && this.nextpos.x < 280f)
            {
                this.nextpos.x = 280f;
            }
            else if (this.nextpos.x > 35f && this.nextpos.x < 90f)
            {
                this.nextpos.x = 35f;
            }
        }
		else // Using the controller
		{
			buttons = StateManager.falconButtons; // Which buttons are currently pressed?

            //Set the new position based on the movement of the controller
            this.nextpos = state.getCameraPosition();
        }

        // Update the zoom
        this.cam.transform.localPosition = new Vector3(this.cam.transform.localPosition.x, this.cam.transform.localPosition.y, this.zoom);

        // If the pan button is pressed (right click on mouse, or left button on controller), update the camera rotations
		if ((buttons[0]) || Input.GetMouseButton(1))
		{
			base.transform.localEulerAngles = this.nextpos;

            this.panning = true;
		}
		else
		{
			this.panning = false;
		}

        //Debug.Log(this.nextpos);

    }

	public bool isPanning() {
		return this.panning;
	}
}
