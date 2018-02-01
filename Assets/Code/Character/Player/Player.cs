using Jesse.CameraControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jesse.Character
{
	public abstract class Player : Character
	{
		protected new CameraController camera;
		protected override void Start()
		{
			base.Start();
		}

		protected override void FixedUpdate()
		{
			rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
			rigidbody.MovePosition(rigidbody.position + velocity * Time.deltaTime);
		}
		protected override void OnDestroy()
		{
			base.OnDestroy();
		}
	}
}
