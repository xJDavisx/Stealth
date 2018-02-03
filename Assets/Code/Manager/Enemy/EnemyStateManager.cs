using Jesse.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jesse.Managers
{
	public class EnemyStateManager : Manager
	{

		public event System.Action EnemyStateChanged;
		private EnemyState enemyState = EnemyState.Normal;

		public float maxAlertTime = 99.99f, maxCautionTime = 99.99f, speedMultipier = 1f;
		float alertTime = 0, cautionTime = 0;
		GameUI gameUI;
		BGMManager bgmMaanager;

		protected override void Start()
		{
			base.Start();
			gameUI = Manager.GetManager<GameUI>();
			bgmMaanager = Manager.GetManager<BGMManager>();
		}

		public EnemyState State
		{
			get
			{
				return enemyState;
			}
			set
			{
				if (enemyState != value)
				{
					enemyState = value;
					EnemyStateChanged?.Invoke();
					switch (State)
					{
						case EnemyState.Normal:
							gameUI.SetAlertLevel(0);
							alertTime = cautionTime = 0;
							bgmMaanager.CurrentBGM = BGMManager.BGMNames.Cavern;
							break;
						case EnemyState.Searching:
							gameUI.SetAlertLevel(1);
							cautionTime = maxCautionTime;
							break;
						case EnemyState.Chasing:
							gameUI.SetAlertLevel(2);
							alertTime = maxAlertTime;
							bgmMaanager.CurrentBGM = BGMManager.BGMNames.Encounter;
							break;
						default:
							break;
					}
				}
			}
		}

		float AlertTime
		{
			get
			{
				return alertTime;
			}
			set
			{
				alertTime = value;
				if (alertTime <= 0f)
				{
					alertTime = 0f;
					AlertOver();
				}
			}
		}

		float CautionTime
		{
			get
			{
				return cautionTime;
			}
			set
			{
				cautionTime = value;
				if (cautionTime <= 0f)
				{
					cautionTime = 0f;
					CautionOver();
				}
			}
		}

		private void CautionOver()
		{
			State = EnemyState.Normal;
		}

		private void AlertOver()
		{
			State = EnemyState.Searching;
		}

		// Update is called once per frame
		void Update()
		{
			switch (State)
			{
				case EnemyState.Normal:
					break;
				case EnemyState.Searching:
					CautionTime -= Time.deltaTime * (speedMultipier * .5f);
					gameUI.SetAlertTime(CautionTime);
					break;
				case EnemyState.Chasing:
					AlertTime -= Time.deltaTime * speedMultipier;
					gameUI.SetAlertTime(AlertTime);
					break;
			}
		}

		internal void ResetAlertTime()
		{
			AlertTime = maxAlertTime;
		}

		protected override void OnDestroy()
		{
			if (Manager.GetManager<EnemyStateManager>() == this)
			{
				EnemyStateChanged = null;
			}
			base.OnDestroy();
		}
	}

	public enum EnemyState
	{
		Normal,
		Searching,
		Chasing
	}
}