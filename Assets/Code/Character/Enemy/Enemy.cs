﻿using Jesse.AI;
using Jesse.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Jesse.Character
{
	public abstract class Enemy : Character
	{
		public static event SpottedTargetHandler SpottedTarget;

		public static Vector3 GlobalLastKnownPosition;

		public AudioManager audioManager;
		public EnemyStateManager enemyStateManager;

		public Character target;
		public LayerMask viewMask;

		[Header("Enemy Values")]
		[SerializeField]
		public float waitTime = .3f;
		[SerializeField]
		public float patrolturnSpeed = 90;
		[SerializeField]
		public float alertturnSpeed = 120;
		[SerializeField]
		public float timeToSpotPlayer = 2f;

		public float playerVisibleTimer;
		public Vector3 localLastKnownPosition;
		public Waypoint waypoint;

		public float waypointReachedDistance = 1f;
		private float searchRange = 20;

		protected override void Awake()
		{
			base.Awake();
			SpeedChanged += Enemy_SpeedChanged;
		}

		private void Enemy_SpeedChanged(Character sender, SpeedChangedEventArgs e)
		{
			navAgent.speed = e.NewSpeed;
			navAgent.isStopped = Speed > 0 ? false : true;
		}

		protected override void Start()
		{
			base.Start();
			rigidbody.isKinematic = true;
			target = GameObject.FindObjectOfType<Player>();
			audioManager = Manager.GetManager<AudioManager>();
			enemyStateManager = Manager.GetManager<EnemyStateManager>();
			enemyStateManager.EnemyStateChanged += EnemyStateChanged;
		}

		protected override void FixedUpdate()
		{

		}

		[CMDCommandButton("Alert")]
		[CMDCommand("Set Enemy State to Alert Level.")]
		public static void Alert()
		{
			Manager.GetManager<EnemyStateManager>().State = EnemyState.Chasing;
		}

		private void EnemyStateChanged()
		{
			switch (enemyStateManager.State)
			{
				case EnemyState.Normal:
					Normal();
					break;
				case EnemyState.Searching:
					Search();
					break;
				case EnemyState.Chasing:
					Chase();
					break;
			}
		}

		public virtual void Chase()
		{
			StopAllCoroutines();
			navAgent.isStopped = true;
			Speed = RunSpeed - 2;
			StartCoroutine(ChaseRoutine());
		}

		protected abstract IEnumerator NormalRoutine();

		protected abstract IEnumerator SearchRoutine();

		protected abstract IEnumerator ChaseRoutine();

		protected abstract IEnumerator AttackTarget();


		public virtual void Search()
		{
			StopAllCoroutines();
			Speed = WalkSpeed * .75f;
			StartCoroutine(SearchRoutine());
		}

		public virtual void Normal()
		{
			StopAllCoroutines();
			Speed = WalkSpeed;
			StartCoroutine(NormalRoutine());
		}

		public bool CanSeeTarget()
		{
			if (target == null)
			{
				return false;
			}
			if (Vector3.Distance(transform.position, target.Position) < ViewDistance)
			{
				Vector3 dirToTarget = (target.Position - transform.position).normalized;
				float targetAngle = Vector3.Angle(transform.forward, dirToTarget);
				if (targetAngle < ViewAngle / 2f)
				{
					if (!Physics.Linecast(transform.position, target.Position, viewMask))
					{
						return true;
					}
				}
			}
			return false;
		}

		public virtual void OnSpottedTarget()
		{
			audioManager.PlaySound("Audio/Sounds/mgsExclamation");
			SpottedTarget?.Invoke(this, new SpottedTargetEventArgs());
		}

		public float SearchRange
		{
			get
			{
				return searchRange;
			}
			set
			{
				searchRange = value;
			}
		}

		protected override void OnDestroy()
		{
			SpottedTarget = null;
			GlobalLastKnownPosition = Vector3.zero;
			SpeedChanged -= Enemy_SpeedChanged;
			base.OnDestroy();
		}
	}
}
