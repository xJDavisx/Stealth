using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jesse.CameraControl
{
	public abstract class CameraController : MonoBehaviour
	{
		static CameraController controller;

		[SerializeField]
		protected Transform target;

		protected virtual void Awake()
		{
			if (controller == null)
			{
				controller = this;
			}
			else if (controller != this)
			{
				Destroy(this);
				Debug.LogError("Instance was destroyed: " + this.name + ".\r\n" +
					"Instance ID: " + this.GetInstanceID() +
					"Make sure you only have 1 " + this.GetType().Name + " in the scene.");
			}
		}

		protected virtual void Start()
		{
			Debug.Log(this.GetType().Name + " used: " + this.name + ".\r\n" +
					"Instance ID: " + this.GetInstanceID());
		}

		protected abstract void LateUpdate();

		protected virtual void OnDestroy()
		{
			if (controller == this)
			{
				controller = null;
			}
		}
	}
}