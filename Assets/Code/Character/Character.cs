using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Jesse.Character
{
	[RequireComponent(typeof(NavMeshAgent))]
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Collider))]
	public abstract class Character : MonoBehaviour
	{
		#region Events

		//Dont forget to set all events to null in the OnDestroy method!
		public event DeathHandler Death;
		public event SpeedChangedHandler SpeedChanged;
		public event ViewDistanceChangedHandler ViewDistanceChanged;
		public event ViewAngleChangedHandler ViewAngleChanged;
		public event HealthChangedHandler HealthChanged;
		public event ConsciousStateChangedHandler ConsciousStateChanged;

		#endregion

		#region Fields

		protected NavMeshAgent navAgent;
		protected new Rigidbody rigidbody;

		private float walkSpeed = 5;
		private float runSpeed = 10;
		private float speed = 0;

		private Vector3 direction;

		private float maxViewDistance = 12;
		private float minViewDistance = 6;
		private float viewDistance = 6;

		private float maxViewAngle = 90;
		private float minViewAngle = 70;
		private float viewAngle = 80;

		private float maxHealth = 100;
		private float health = 100;
		private bool canRegenHealth = false;
		private float healthRegenWaitTime = 3;
		private float healthRegenSpeed = 5;

		private ConsciousState consciousState = ConsciousState.Awake;

		private bool disabled;

		public float smoothMoveTime = .1f;
		public float turnSpeed = 8;

		protected float angle;
		protected float smoothInputMagnitude;
		protected float smoothMoveVelocity;
		protected Vector3 velocity;

		#endregion

		#region Properties

		/// <summary>
		/// Sets the WalkSpeed of the Character Object.
		/// </summary>
		public float WalkSpeed
		{
			get
			{
				return walkSpeed;
			}
			set
			{
				float old = walkSpeed;
				walkSpeed = value;
				//SpeedChanged?.Invoke(this, new SpeedChangedEventArgs(old, walkSpeed));
			}
		}

		/// <summary>
		/// Sets the RunSpeed of the Character Object.
		/// </summary>
		public float RunSpeed
		{
			get
			{
				return runSpeed;
			}
			set
			{
				float old = runSpeed;
				runSpeed = value;
				//WalkSpeedChanged?.Invoke(this, new WalkSpeedChangedEventArgs(old, walkSpeed));
			}
		}

		/// <summary>
		/// The Current Speed of the Character Object. Setting invokes the SpeedChanged event.
		/// </summary>
		public float Speed
		{
			get
			{
				return speed;
			}
			set
			{
				float old = speed;
				speed = value;
				SpeedChanged?.Invoke(this, new SpeedChangedEventArgs(old, walkSpeed));
			}
		}

		/// <summary>
		/// The Maximum View Distance of the Character.
		/// </summary>
		public float MaxViewDistance
		{
			get
			{
				return maxViewDistance;
			}
			set
			{
				float old = maxViewDistance;
				maxViewDistance = Mathf.Clamp(value, MinViewDistance, Mathf.Infinity);
				//ViewDistanceChanged?.Invoke(this, new ViewDistanceChangedEventArgs(old, health));
			}
		}

		/// <summary>
		/// The Minimum View Distance of the Character.
		/// </summary>
		public float MinViewDistance
		{
			get
			{
				return minViewDistance;
			}
			set
			{
				float old = minViewDistance;
				minViewDistance = Mathf.Clamp(value, 0, MaxViewDistance);
				//ViewDistanceChanged?.Invoke(this, new ViewDistanceChangedEventArgs(old, health));
			}
		}

		/// <summary>
		/// Sets the ViewDistance of the Character Object and invokes the ViewDistanceChanged event.
		/// </summary>
		public float ViewDistance
		{
			get
			{
				return viewDistance;
			}
			set
			{
				float old = viewDistance;
				viewDistance = Mathf.Clamp(value, MinViewDistance, MaxViewDistance);
				ViewDistanceChanged?.Invoke(this, new ViewDistanceChangedEventArgs(old, viewDistance));
			}
		}

		/// <summary>
		/// The Minimum View Angle of the Character.
		/// </summary>
		public float MinViewAngle
		{
			get
			{
				return minViewAngle;
			}
			set
			{
				float old = minViewAngle;
				minViewAngle = Mathf.Clamp(value, 0, maxViewAngle);
				//ViewAngleChanged?.Invoke(this, new ViewAngleChangedEventArgs(old, viewAngle));
			}
		}

		/// <summary>
		/// The Maximum View Angle of the Character.
		/// </summary>
		public float MaxViewAngle
		{
			get
			{
				return maxViewAngle;
			}
			set
			{
				float old = maxViewAngle;
				maxViewAngle = Mathf.Clamp(value, minViewAngle, Mathf.Infinity);
				//ViewAngleChanged?.Invoke(this, new ViewAngleChangedEventArgs(old, viewAngle));
			}
		}

		/// <summary>
		/// Sets the ViewAngle of the Character Object and invokes the ViewAngleChanged event.
		/// </summary>
		public float ViewAngle
		{
			get
			{
				return viewAngle;
			}
			set
			{
				float old = viewAngle;
				viewAngle = Mathf.Clamp(value, minViewAngle, MaxViewAngle);
				ViewAngleChanged?.Invoke(this, new ViewAngleChangedEventArgs(old, viewAngle));
			}
		}

		/// <summary>
		/// Sets the Maximum Health of the Character Object.
		/// </summary>
		public float MaxHealth
		{
			get
			{
				return maxHealth;
			}
			set
			{
				float old = maxHealth;
				maxHealth = value;
			}
		}

		/// <summary>
		/// Sets the Health of the Character Object and always invokes the HealthChanged event. If Health is equal to 0f, the Death event will be invoked.
		/// </summary>
		public float Health
		{
			get
			{
				return health;
			}
			set
			{
				float old = health;
				health = Mathf.Clamp(value, 0f, maxHealth);
				HealthChanged?.Invoke(this, new HealthChangedEventArgs(old, health));
				if (health == 0)
				{
					Death?.Invoke(this, new DeathEventArgs());
					Destroy(this);
				}
			}
		}

		/// <summary>
		/// Can this character regenerate health over time?
		/// </summary>
		public bool CanRegenHealth
		{
			get
			{
				return canRegenHealth;
			}

			set
			{
				canRegenHealth = value;
			}
		}

		/// <summary>
		/// The time it takes to start health regeneration from the last time of damage taken.
		/// </summary>
		public float HealthRegenWaitTime
		{
			get
			{
				return healthRegenWaitTime;
			}

			set
			{
				healthRegenWaitTime = value;
			}
		}

		/// <summary>
		/// The amount of health to regenerate per second.
		/// </summary>
		public float HealthRegenSpeed
		{
			get
			{
				return healthRegenSpeed;
			}

			set
			{
				healthRegenSpeed = value;
			}
		}

		/// <summary>
		/// Sets the ConsciousState of the Character Object and only invokes the ConsciousStateChanged event if the new value is not equal to the current value.
		/// </summary>
		public ConsciousState ConsciousState
		{
			get
			{
				return consciousState;
			}

			set
			{
				if (consciousState != value)
				{
					ConsciousState old = consciousState;
					consciousState = value;
					ConsciousStateChanged?.Invoke(this, new ConsciousStateChangedEventArgs(old, consciousState));
				}
			}
		}

		/// <summary>
		/// Is Character Disabled?
		/// </summary>
		public bool Disabled
		{
			get
			{
				return disabled;
			}
			set
			{
				if (disabled != value)
				{
					disabled = value;
					///DisabledChanged?.Invoke(this, new DisabledChangedEventArgs(!value, value));
				}
			}
		}

		/// <summary>
		/// Gets and Sets the position of the Character Object. Wrapper for transform.position.
		/// </summary>
		public Vector3 Position
		{
			get
			{
				return transform.position;
			}
			set
			{
				transform.position = value;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Sets the Speed property which fires the HealthChanged event.
		/// </summary>
		/// <param name="newSpeed">The value the Health property will be set to.</param>
		protected virtual void OnSpeedChanged(float newSpeed)
		{
			WalkSpeed = newSpeed;
		}

		/// <summary>
		/// Sets the Health property which fires the HealthChanged event.
		/// </summary>
		/// <param name="newHealth">The value the Health property will be set to.</param>
		protected virtual void OnHealthChanged(float newHealth)
		{
			Health = newHealth;
		}

		/// <summary>
		/// Sets the ViewDistance property which fires the ViewDistanceChanged event.
		/// </summary>
		/// <param name="newViewDistance">The value the ViewDistance property will be set to.</param>
		protected virtual void OnViewDistanceChanged(float newViewDistance)
		{
			ViewDistance = newViewDistance;
		}

		/// <summary>
		/// Sets the ViewAngle property which fires the ViewAngleChanged event.
		/// </summary>
		/// <param name="newViewAngle">The value the ViewAngle property will be set to.</param>
		protected virtual void OnViewAngleChanged(float newViewAngle)
		{
			ViewAngle = newViewAngle;
		}

		/// <summary>
		/// Sets the ConsciousState property which fires the ConsciousStateChanged event IF the new value is different than the current value.
		/// </summary>
		/// <param name="newConsciousState">The value the ConsciousState property will be set to.</param>
		protected virtual void OnConsciousStateChanged(ConsciousState newConsciousState)
		{
			ConsciousState = newConsciousState;
		}

		/// <summary>
		/// Triggers the Enemy.Death event.
		/// </summary>
		protected virtual void OnDeath()
		{
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			rigidbody = GetComponent<Rigidbody>();
			navAgent = GetComponent<NavMeshAgent>();
		}

		protected virtual void Update()
		{
		}

		protected virtual void FixedUpdate()
		{
			rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
			rigidbody.MovePosition(rigidbody.position + velocity * Time.deltaTime);
		}

		protected virtual void OnDestroy()
		{
			Death = null;
			SpeedChanged = null;
			ViewDistanceChanged = null;
			ViewAngleChanged = null;
			HealthChanged = null;
			ConsciousStateChanged = null;
		}

		/// <summary>
		/// Deals Damage to the Character.
		/// </summary>
		/// <param name="dammager">The Game Object that is dealing the damage.</param>
		/// <param name="value">The amount of damage dealt.</param>
		public void DealDamage(GameObject dammager, float value)
		{
			Health -= value;
			Debug.Log(dammager.name + " delt " + value + " Damage to " + this.name + "! " + this.name + " health is at " + Health + "!");
		}

		#endregion
	}

	public enum ConsciousState
	{
		Awake,
		Sleeping,
		KnockedOut
	}
}
