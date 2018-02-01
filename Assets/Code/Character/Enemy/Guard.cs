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
		public Light spotlight;



		protected override void Start()
		{
			base.Start();
			ViewAngle = spotlight.spotAngle;
			Normal();
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
			base.OnDestroy();
		}

		protected override IEnumerator NormalRoutine()
		{
			while (true)
			{
				if (waypoint == null)
				{
					waypoint = FindObjectOfType<Waypoint>();
					waypoint = waypoint.NextWaypoint();
				}
				navAgent.SetDestination(waypoint.transform.position);
				while (Vector3.Distance(transform.position, waypoint.transform.position) > waypointReachedDistance)
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
			yield return null;
		}
	}

}