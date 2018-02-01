using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jesse.Character
{
	/// <summary>
	/// The delegate for the Death event
	/// </summary>
	/// <param name="sender">The Character that died</param>
	/// <param name="d">The Death Event Arguments for this event.</param>
	public delegate void DeathHandler(Character sender, DeathEventArgs e);
	public class DeathEventArgs : EventArgs
	{
		public DeathEventArgs()
		{

		}
	}

	public delegate void HealthChangedHandler(Character sender, HealthChangedEventArgs e);
	public class HealthChangedEventArgs : EventArgs
	{
		float _oldHealth, _newHealth;

		public HealthChangedEventArgs(float oldHealth, float newHealth)
		{
			_oldHealth = oldHealth;
			_newHealth = newHealth;
		}

		public float OldHealth
		{
			get
			{
				return _oldHealth;
			}
		}

		public float NewHealth
		{
			get
			{
				return _newHealth;
			}
		}
	}

	public delegate void ConsciousStateChangedHandler(Character sender, ConsciousStateChangedEventArgs e);
	public class ConsciousStateChangedEventArgs : EventArgs
	{
		ConsciousState _oldConsciousState, _newConsciousState;

		public ConsciousStateChangedEventArgs(ConsciousState oldConsciousState,
			ConsciousState newConsciousState)
		{
			_oldConsciousState = oldConsciousState;
			_newConsciousState = newConsciousState;
		}

		public ConsciousState OldConsciousState
		{
			get
			{
				return _oldConsciousState;
			}
		}

		public ConsciousState NewConsciousState
		{
			get
			{
				return _newConsciousState;
			}
		}
	}

	public delegate void ViewAngleChangedHandler(Character sender, ViewAngleChangedEventArgs e);
	public class ViewAngleChangedEventArgs : EventArgs
	{
		float _oldViewAngle, _newViewAngle;

		public ViewAngleChangedEventArgs(float oldViewAngle, float newViewAngle)
		{
			_oldViewAngle = oldViewAngle;
			_newViewAngle = newViewAngle;
		}

		public float OldViewAngle
		{
			get
			{
				return _oldViewAngle;
			}
		}

		public float NewViewAngle
		{
			get
			{
				return _newViewAngle;
			}
		}
	}

	public delegate void ViewDistanceChangedHandler(Character sender, ViewDistanceChangedEventArgs e);
	public class ViewDistanceChangedEventArgs : EventArgs
	{
		float _oldViewDistance, _newViewDistance;

		public ViewDistanceChangedEventArgs(float oldViewDistance, float newViewDistance)
		{
			_oldViewDistance = oldViewDistance;
			_newViewDistance = newViewDistance;
		}

		public float OldViewDistance
		{
			get
			{
				return _oldViewDistance;
			}
		}

		public float NewViewDistance
		{
			get
			{
				return _newViewDistance;
			}
		}
	}

	public delegate void SpeedChangedHandler(Character sender, SpeedChangedEventArgs e);
	public class SpeedChangedEventArgs : EventArgs
	{
		float _oldSpeed, _newSpeed;

		public SpeedChangedEventArgs(float oldSpeed, float newSpeed)
		{
			_oldSpeed = oldSpeed;
			_newSpeed = newSpeed;
		}

		public float OldSpeed
		{
			get
			{
				return _oldSpeed;
			}
		}

		public float NewSpeed
		{
			get
			{
				return _newSpeed;
			}
		}
	}
}
