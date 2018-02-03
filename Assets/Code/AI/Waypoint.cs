using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Jesse.AI
{
	public class Waypoint
	{
		static List<Waypoint> AllWaypoints = new List<Waypoint>();

		bool occupied;
		Vector3 position;

		public Waypoint(Vector3 position)
		{
			this.position = position;
			AllWaypoints.Add(this);
		}

		public Waypoint(float x, float y, float z)
		{
			this.position = new Vector3(x, y, z);
			AllWaypoints.Add(this);
		}

		~Waypoint()
		{
			AllWaypoints.Remove(this);
		}

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

		public Vector3 Position
		{
			get
			{
				return position;
			}

			set
			{
				position = value;
			}
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
					if ((check = Vector3.Distance(position, w.position)) < distance)
					{
						distance = check;
						closest = w;
					}
				}
			}
			IsOccupied = false;
			closest.IsOccupied = true;
			return closest;
		}

		/// <summary>
		/// Returns a random waypoint from the list of all waypoints in the scene that is within range of a position.
		/// </summary>
		/// <param name="position">The Vector3 that acts as the center of the search area.</param>
		/// <param name="range">The range or distance to be searched for waypoints from the center position.</param>
		/// <param name="currentWaypoint">Pass in a waypoint to set it's IsOccupied property to false. </param>
		/// <returns>The chosen random waypoint in range.</returns>
		public static Waypoint RandomWaypointInRange(Vector3 position, float range, Waypoint currentWaypoint = null)
		{
			if (AllWaypoints != null)
			{
				List<Waypoint> inRange = new List<Waypoint>();
				foreach (Waypoint w in AllWaypoints)
				{
					if (!w.IsOccupied && Vector3.Distance(position, w.position) < range)
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

		public static Waypoint CreateInRange(Vector3 center, float range, bool snapYTo0 = true)
		{
			Vector3 v = Random.insideUnitSphere * range;
			if (snapYTo0)
				v.y = 0;
			return new Waypoint(v.x, v.y, v.z);
		}
	}
}