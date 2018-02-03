using Jesse.AI;
using Jesse.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Jesse.Character
{
	public class Guard : Enemy
	{

		[Header("Guard Values")]
		[SerializeField]
		Light spotlight;

		static List<Waypoint> GuardWaypoints;

		protected override void Start()
		{
			base.Start();
			if (GuardWaypoints == null)
				FindGuardWaypoints();
			ViewAngleChanged += Guard_ViewAngleChanged;
			ViewDistanceChanged += Guard_ViewDistanceChanged;
			ViewAngle = MinViewAngle;
			ViewDistance = MinViewDistance;
			Normal();
		}

		private void FindGuardWaypoints()
		{
			GuardWaypoints = new List<Waypoint>();
			foreach (GameObject go in GameObject.FindGameObjectsWithTag("GuardWaypoint"))
			{
				GuardWaypoints.Add(new Waypoint(go.transform.position));
			}
		}

		private void Guard_ViewDistanceChanged(Character sender, ViewDistanceChangedEventArgs e)
		{
			spotlight.range = e.NewViewDistance;
		}

		private void Guard_ViewAngleChanged(Character sender, ViewAngleChangedEventArgs e)
		{
			spotlight.spotAngle = e.NewViewAngle;
		}

		protected override void Update()
		{
			base.Update();
			if (enemyStateManager.State == EnemyState.Normal)
			{
				if (CanSeeTarget())
				{
					playerVisibleTimer += Time.deltaTime;
				}
				else
				{
					playerVisibleTimer -= Time.deltaTime;
				}
				playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);

				if (playerVisibleTimer >= timeToSpotPlayer)
				{
					//TODO: add some sort of slow motion effect when an enemy spots the player.
					OnSpottedTarget();
					localLastKnownPosition = target.Position;
					Chase();
					enemyStateManager.State = EnemyState.Chasing;
				}
			}
			else if (enemyStateManager.State == EnemyState.Chasing)
			{
				if (CanSeeTarget())
				{
					Enemy.GlobalLastKnownPosition = target.Position;
					enemyStateManager.ResetAlertTime();
				}
			}
			else if (enemyStateManager.State == EnemyState.Searching)
			{
				if (CanSeeTarget())
				{
					OnSpottedTarget();
					Enemy.GlobalLastKnownPosition = target.Position;
					enemyStateManager.State = EnemyState.Chasing;
				}
			}
		}

		protected override void OnDestroy()
		{
			ViewDistanceChanged -= Guard_ViewDistanceChanged;
			ViewAngleChanged -= Guard_ViewAngleChanged;
			base.OnDestroy();
		}

		protected override IEnumerator NormalRoutine()
		{
			ViewAngle = MinViewAngle;
			ViewDistance = MinViewDistance;
			while (true)
			{
				if (waypoint == null)
				{
					waypoint = GuardWaypoints[Random.Range(0, GuardWaypoints.Count)];
					waypoint = waypoint.NextWaypoint();
				}
				navAgent.SetDestination(waypoint.Position);
				while (Vector3.Distance(transform.position, waypoint.Position) > waypointReachedDistance)
				{
					yield return new WaitForSeconds(.5f);
				}
				waypoint = waypoint.NextWaypoint();
				yield return new WaitForSeconds(Random.Range(1f, 10f));
			}
		}

		protected override IEnumerator AttackTarget()
		{
			float damage = Random.Range(5f, 15f);
			target.DealDamage(gameObject, damage);
			yield return new WaitForSeconds(Random.Range(.5f, 2f));
		}

		protected override IEnumerator ChaseRoutine()
		{
			ViewAngle = MaxViewAngle;
			ViewDistance = MaxViewDistance;
			yield return new WaitForSeconds(UnityEngine.Random.Range(.5f, 1f));
			navAgent.isStopped = false;
			while (true)
			{
				navAgent.SetDestination(target.Position);
				if (Vector3.Distance(transform.position, target.Position) < 3)
				{
					StartCoroutine(AttackTarget());
				}
				else
				{
					StopCoroutine(AttackTarget());
				}
				yield return new WaitForSeconds(.1f);
			}
		}

		protected override IEnumerator SearchRoutine()
		{
			ViewAngle = MinViewAngle + ((MaxViewAngle - MinViewAngle) * .5f);
			ViewDistance = MinViewDistance + ((MaxViewDistance - MinViewDistance) * .5f);
			navAgent.isStopped = true;
			yield return new WaitForSeconds(UnityEngine.Random.Range(.5f, 1f));
			navAgent.isStopped = false;
			while (true)
			{
				Waypoint w = Waypoint.CreateInRange(GlobalLastKnownPosition, SearchRange);
				NavMeshPath p = new NavMeshPath();
				if (!navAgent.CalculatePath(w.Position, p))
					continue;
				navAgent.path = p;
				while (Vector3.Distance(transform.position, w.Position) > waypointReachedDistance)
				{
					yield return new WaitForSeconds(.5f);
				}
				yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 2f));
			}
		}
	}

}