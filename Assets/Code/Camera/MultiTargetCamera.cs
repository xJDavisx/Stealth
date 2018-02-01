using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jesse.CameraControl
{
	[RequireComponent(typeof(UnityEngine.Camera))]
	public class MultiTargetCamera : MonoBehaviour
	{
		//Inspector Vars
		public List<Transform> targets = new List<Transform>();
		public Vector3 offset;
		[Header("Movement")]
		public bool CanMove = true;
		public float smoothTime = .5f;

		[Header("Zooming")]
		public bool CanZoom = true;
		public float minZoom = 40f,
			maxZoom = 10f,
			zoomLimiter = 50f;

		//public static vars
		public static MultiTargetCamera Main;

		//private vars
		private Vector3 velocity;
		private UnityEngine.Camera cam;
		Bounds currentBounds = new Bounds();
		bool needBoundsRecalc = true;

		// Use this for initialization
		void Start()
		{
			cam = GetComponent<UnityEngine.Camera>();
			Main = this;
		}

		void Update()
		{
			needBoundsRecalc = true;
		}

		void LateUpdate()
		{
			if (CanMove)
				Move();
			if (CanZoom)
				Zoom();
		}

		private void Zoom()
		{
			float newZoom = Mathf.Lerp(maxZoom, minZoom, GreatestDistance / zoomLimiter);
			cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
		}

		private void UpdateBounds()
		{
			if (targets.Count == 0)
			{
				currentBounds = new Bounds(Vector3.zero, Vector3.zero);
				return;
			}
			currentBounds = new Bounds(targets[0].position, Vector3.zero);
			for (int i = 0; i < targets.Count; i++)
			{
				currentBounds.Encapsulate(targets[i].position);
			}
			needBoundsRecalc = false;
		}

		private void Move()
		{
			Vector3 newPos = CenterPoint + offset;
			transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
		}

		private Vector3 CenterPoint
		{
			get
			{
				if (needBoundsRecalc)
					UpdateBounds();
				return currentBounds.center;
			}
		}

		private float GreatestDistance
		{
			get
			{
				if (needBoundsRecalc)
					UpdateBounds();
				return currentBounds.size.x;
			}
		}
	}
}
