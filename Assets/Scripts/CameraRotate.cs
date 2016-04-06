using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class CameraRotate : MonoBehaviour {
    public bool selectionMode = false;
    public Transform target;
	public float distance = 25.0f;
	public float xSpeed = 12.0f;
	public float ySpeed = 12.0f;
	
	public float yMinLimit = 20f;
	public float yMaxLimit = 40f;
	public float xMinLimit = 20f;
	public float xMaxLimit = 40f;
	
	public float distanceMin = 10f;
	public float distanceMax = 30f;

	
	float x = 0.0f;
	float y = 0.0f;
       
	
	// Use this for initialization
	void Start () {
        
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;

	}


    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
            SwitchCursor();
    }

	void LateUpdate () {
		if (target &! selectionMode) {
			if(distance > 50){
				xSpeed = 2;
			}else if(distance > 15){
				xSpeed = 3;
			}else{
				xSpeed = 4;
			}
			distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel")*5, distanceMin, distanceMax);
			Quaternion rotation = Quaternion.Euler(y, x, 0);
			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			Vector3 position = rotation * negDistance + target.position;
            x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
            negDistance = new Vector3(0.0f, 0.0f, -distance);
            position = rotation * negDistance + target.position;
            x = ClampAngle(x, xMinLimit, xMaxLimit);
            y = ClampAngle(y, yMinLimit, yMaxLimit);
            if (Input.GetAxis ("Mouse ScrollWheel") != 0){
				negDistance = new Vector3(0.0f, 0.0f, -distance);
				position = rotation * negDistance + target.position;
			}

			transform.rotation = rotation;
			transform.position = position;

		}
	}




    void SwitchCursor()
    {
        selectionMode = !selectionMode;
        if (!selectionMode)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }


    public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}
	
	
}