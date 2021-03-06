﻿using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbitImproved : MonoBehaviour {

	public Transform target;
	public Transform altTarget;
	private Transform currentTarget;

	public float distance = 5.0f;
	public float xSpeed = 120.0f;
	public float ySpeed = 120.0f;

	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;

	public float distanceMin = .5f;
	public float distanceMax = 15f;

	//private Rigidbody rigidbody;

	float x = 0.0f;
	float y = 0.0f;

	// Use this for initialization
	void Start () 
	{
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;

		//rigidbody = GetComponent<Rigidbody>();

		// Make the rigid body not change rotation
		/*if (rigidbody != null)
		{
			rigidbody.freezeRotation = true;
		}*/
	}

	void LateUpdate () 
	{
		if (Input.GetMouseButtonDown(0))
		{
			Cursor.lockState = CursorLockMode.Locked;
			//Cursor.visible = false;
		}
		if (Input.GetMouseButtonUp(0))
		{
			Cursor.lockState = CursorLockMode.None;
			//Cursor.visible = true;
		}

		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			if (currentTarget == target) {
				currentTarget = null;
			} else {
				currentTarget = altTarget;
			}
		}
		if( Input.GetKeyDown (KeyCode.LeftArrow)){
			if (currentTarget == altTarget) {
				currentTarget = null;
			} else {
				currentTarget = target;
			}
		}

		if (target && altTarget || currentTarget) 
		{
			float distSpeedModifier = 1.0f / Mathf.Sqrt (distance);
			if (Input.GetMouseButton (0)) {
				x += Input.GetAxis ("Mouse X") * xSpeed * distance * 0.02f * distSpeedModifier;
				y -= Input.GetAxis ("Mouse Y") * ySpeed * 0.02f * distSpeedModifier;
			}
			y = ClampAngle(y, yMinLimit, yMaxLimit);

			Quaternion rotation = Quaternion.Euler(y, x, 0);

			distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel")*5, distanceMin, distanceMax);

			/*RaycastHit hit;
			if (Physics.Linecast (target.position, transform.position, out hit)) 
			{
				distance -=  hit.distance;
			}*/
			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			Vector3 position;
			if (currentTarget) {
				position = rotation * negDistance + currentTarget.position;
			} else {
				position = rotation * negDistance + (target.position + altTarget.position)/2.0f;
			}
			transform.rotation = rotation;
			transform.position = position;
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