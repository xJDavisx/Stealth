using Jesse.CameraControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jesse.Character
{
	public class ThirdPersonPlayer : Player
	{

		protected override void Start()
		{
			base.Start();
			camera = GameObject.FindObjectOfType<ThirdPersonCameraController>();
		}

		protected override void Update()
		{
			Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
			Vector3 direction = camera.transform.TransformDirection(inputDirection);

			float inputMagnitude = direction.magnitude;
			smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);

			float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
			angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);

			velocity = transform.forward * moveSpeed * smoothInputMagnitude;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}
	}
}