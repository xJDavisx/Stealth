using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Jesse.AI
{
	public class Waypoint : MonoBehaviour
	{
		List<Waypoint> AllWaypoints = new List<Waypoint>();

		bool occupied;

		public bool IsOccupied
		{
			get
			{
				return occupied;
			}

			protected set
			{
				occupied = value;
			}
		}

		private void Awake()
		{
			AllWaypoints = FindObjectsOfType<Waypoint>().ToList().Where(x => x != this).ToList();
		}

		public Waypoint NextWaypoint()
		{
			float distance = Mathf.Infinity;
			Waypoint closest = null;
			foreach (Waypoint w in AllWaypoints)
			{
				if (w != this && !w.IsOccupied)
				{
					float check;
					if ((check = Vector3.Distance(this.transform.position, w.transform.position)) < distance)
					{
						distance = check;
						closest = w;
					}
				}
			}
			this.IsOccupied = false;
			closest.IsOccupied = true;
			return closest;
		}

		public Waypoint RandomWaypointInRange(Vector3 position, float range, Waypoint currentWaypoint = null)
		{
			if (AllWaypoints != null)
			{
				List<Waypoint> inRange = new List<Waypoint>();
				foreach (Waypoint w in AllWaypoints)
				{
					if (!w.IsOccupied && Vector3.Distance(position, w.transform.position) < range)
					{
						inRange.Add(w);
					}
				}
				if (inRange.Count == 0)
				{
					Debug.Log("No waypoints found within " + range + " units.");
					return null;
				}
				Waypoint chosen = inRange[Random.Range(0, inRange.Count)];
				if (currentWaypoint != null)
					currentWaypoint.IsOccupied = false;
				chosen.IsOccupied = true;
				return chosen;
			}
			Debug.LogError("AllWaypoints is null.");
			return null;
		}

		private void OnDestroy()
		{
			if (AllWaypoints != null)
			{
				if (AllWaypoints.Contains(this))
				{
					AllWaypoints.Remove(this);
				}
			}
		}

	}
}