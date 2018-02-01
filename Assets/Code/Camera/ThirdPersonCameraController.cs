using Jesse.Managers;
using Jesse.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jesse.CameraControl
{
	public class ThirdPersonCameraController : CameraController
	{
		[SerializeField]
		float distance = 2.0f;
		[SerializeField]
		float xSpeed = 20.0f;
		[SerializeField]
		float ySpeed = 20.0f;
		[SerializeField]
		float yMinLimit = -90f;
		[SerializeField]
		float yMaxLimit = 90f;
		[SerializeField]
		float distanceMin = 10f;
		[SerializeField]
		float distanceMax = 10f;
		[SerializeField]
		float smoothTime = 15f;

		float rotationYAxis = 0.0f;
		float rotationXAxis = 0.0f;
		float velocityX = 0.0f;
		float velocityY = 0.0f;
		Vector3 camPosition;

		// Use this for initialization
		protected override void Start()
		{
			base.Start();
			Vector3 angles = transform.eulerAngles;
			rotationYAxis = angles.y;
			rotationXAxis = angles.x;
			// Make the rigid body not change rotation
			if (GetComponent<Rigidbody>())
			{
				GetComponent<Rigidbody>().freezeRotation = true;
			}
			camPosition = transform.position;
		}

		protected override void LateUpdate()
		{
			if (target && !GameUI.IsPaused && !GameUI.IsGameOver)
			{
				velocityX += xSpeed * Input.GetAxis("Mouse X") * distance * 0.02f;
				velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
				rotationYAxis += velocityX;
				rotationXAxis -= velocityY;
				rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
				Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
				Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
				Quaternion rotation = toRotation;

				distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
				Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
				Vector3 position = rotation * negDistance + target.position;
				RaycastHit hit;
				if (Physics.Linecast(target.position, position, out hit))
				{
					//distance -= hit.distance;
					position = new Vector3(hit.point.x + hit.normal.x * 0.5f, camPosition.y, hit.point.z + hit.normal.z * 0.5f);
				}

				transform.rotation = rotation;
				//transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * smoothTime);
				transform.position = position;

				velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
				velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);
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
}

