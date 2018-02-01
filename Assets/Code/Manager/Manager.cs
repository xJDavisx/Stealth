using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Jesse.Managers
{
	public abstract class Manager : MonoBehaviour
	{
		private static List<Manager> managers;

		protected virtual void Awake()
		{
			if (managers == null)
				managers = new List<Manager>();

			if (!ContainsType(this.GetType()))
			{
				managers.Add(this);
			}
			else if (managers.Find(x => x.GetType() == this.GetType()) != this)
			{
				Destroy(this);
				Debug.LogError("Instance was destroyed: " + this.name + ".\r\n" +
					"Instance ID: " + this.GetInstanceID() +
					"Make sure you only have 1 " + this.GetType().Name + " in the scene.");
				return;
			}
		}

		/// <summary>
		/// Returns the instance of a Manager of type T.
		/// </summary>
		/// <typeparam name="T">The Type of Manager to return.</typeparam>
		/// <returns>The Manager of type T.</returns>
		public static T GetManager<T>() where T : Manager
		{
			T m = null;
			try
			{
				m = (T)managers.Single(x => x.GetType() == typeof(T));
			}
			catch (InvalidOperationException ex)
			{
				Debug.LogError("Manager of type " + typeof(T).Name + " not found.\r\n" + ex);
			}
			return m;
		}

		private bool ContainsType(Type t)
		{
			foreach (Manager m in managers)
			{
				if (m.GetType() == t)
				{
					return true;
				}
			}
			return false;
		}

		protected virtual void Start()
		{
			Debug.Log(this.GetType().Name + " used: " + this.name + ".\r\n" +
					"Instance ID: " + this.GetInstanceID());
		}

		/// <summary>
		/// Base Class OnDestroy Method. Be sure to call this in any overridden methods AT THE END of the method!
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (ContainsType(this.GetType()))
			{
				managers.Remove(this);
			}
			if (managers.Count == 0)
			{
				managers = null;
			}
		}
	}
}