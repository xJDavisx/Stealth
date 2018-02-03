using Jesse.CameraControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jesse.Character
{
	public abstract class Player : Character
	{
		protected new CameraController camera;

		/// <summary>
		/// The direction the player should move based on input axis.
		/// </summary>
		protected Vector3 inputDirection;

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

		protected override void OnSpeedChanged(float newSpeed)
		{
			base.OnSpeedChanged(newSpeed);
		}

		protected override void OnHealthChanged(float newHealth)
		{
			base.OnHealthChanged(newHealth);
		}

		protected override void OnViewDistanceChanged(float newViewDistance)
		{
			base.OnViewDistanceChanged(newViewDistance);
		}

		protected override void OnViewAngleChanged(float newViewAngle)
		{
			base.OnViewAngleChanged(newViewAngle);
		}

		protected override void OnConsciousStateChanged(ConsciousState newConsciousState)
		{
			base.OnConsciousStateChanged(newConsciousState);
		}

		protected override void OnDeath()
		{
			base.OnDeath();
		}

		protected override void Awake()
		{
			base.Awake();
		}

		protected override void Update()
		{
			base.Update();

			inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
			if (Input.GetButton("Run"))
			{
				Speed = RunSpeed;
			}
			else
			{
				Speed = WalkSpeed;
			}
		}
	}
}
