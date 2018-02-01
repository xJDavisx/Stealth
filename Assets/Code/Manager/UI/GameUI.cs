using Jesse.Character;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Jesse.Managers
{
	[RequireComponent(typeof(Animator))]
	public class GameUI : Manager
	{

		[SerializeField]
		private GameObject gameLoseUI;
		[SerializeField]
		private GameObject gameWinUI;
		[SerializeField]
		private GameObject quitUI;
		[SerializeField]
		private GameObject pauseUI;
		[SerializeField]
		private GameObject alertUI;

		[SerializeField]
		private Text alertTitleText;
		[SerializeField]
		private Text alertTimeText;
		[SerializeField]
		private Text pauseTitleText;

		private bool gameIsOver, isPaused;

		private float pausePreviousTimeScale;

		private Animator animator;

		public void SetAlertLevel(int level)
		{
			animator.SetInteger("AlertLevel", level);
			if (level == 1)
			{
				alertTitleText.text = "CAUTION";
			}
			else if (level == 2)
			{
				alertTitleText.text = "ALERT";
			}
		}

		private Player player;

		protected override void Awake()
		{
			base.Awake();
			animator = GetComponent<Animator>();
			player = FindObjectOfType<Player>();
			player.Death += Player_Death;
			Enemy.SpottedTarget += Enemy_SpottedPlayer;

			Time.timeScale = 1;
			pausePreviousTimeScale = 1;
			SetIsPaused(false);
		}

		protected override void Start()
		{
			base.Start();
			((DaanRuiter.CMDPlus.CMDWindow)CMD.Controller.View).WindowToggleEvent += CMDToggleWindowEvent;
		}

		private void Enemy_SpottedPlayer(object sender, SpottedTargetEventArgs d)
		{

		}

		private void Player_Death(object sender, DeathEventArgs d)
		{
			ShowGameLoseUI();
		}


		void Update()
		{
			if (gameIsOver)
			{
				if (Input.GetKeyDown(KeyCode.Space))
				{
					SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
				}
			}
			if (Input.GetButtonDown("Pause"))
			{
				if (!IsPaused)
				{
					IsPaused = true;
				}
			}
		}

		public void SetIsPaused(bool value)
		{
			isPaused = value;
			if (isPaused)
			{
				pausePreviousTimeScale = Time.timeScale != 0 ? Time.timeScale : pausePreviousTimeScale;
				Time.timeScale = 0;
			}
			else
			{
				Time.timeScale = pausePreviousTimeScale;
			}
			SetMouseCursorLocked(!value);
			pauseUI.SetActive(IsPaused);
		}

		internal void SetAlertTime(float value)
		{
			alertTimeText.text = SplitFloatToTime(value);
		}

		public static bool IsPaused
		{
			get
			{
				return Manager.GetManager<GameUI>().isPaused;
			}
			set
			{
				Manager.GetManager<GameUI>().SetIsPaused(value);
			}
		}

		public static bool IsGameOver
		{
			get
			{
				return Manager.GetManager<GameUI>().gameIsOver;
			}
			set
			{
				Manager.GetManager<GameUI>().SetGameIsOver(value);
			}
		}

		private void SetGameIsOver(bool value)
		{
			gameIsOver = value;
		}

		public void Quit()
		{
			Application.Quit();
		}

		public void CancelQuit()
		{
			Time.timeScale = 1;
			quitUI.SetActive(false);
		}

		public void SetPauseTitleText(string text)
		{
			pauseTitleText.text = text;
		}

		public void ShowQuitConfirm()
		{
			pauseUI.SetActive(false);
			quitUI.SetActive(true);
		}

		public void HideQuitConfirm()
		{
			quitUI.SetActive(false);
			pauseUI.SetActive(true);
		}

		void ShowGameWinUI()
		{
			OnGameOver(gameWinUI);
		}

		void ShowGameLoseUI()
		{
			OnGameOver(gameLoseUI);
		}

		void OnGameOver(GameObject gameOverUI)
		{
			gameOverUI.SetActive(true);
			gameIsOver = true;
			Enemy.SpottedTarget -= Enemy_SpottedPlayer;
			Time.timeScale = 0;
		}

		string SplitFloatToTime(float f)
		{
			if (f < 0f)
			{
				f = 0f;
			}
			string timeString = f.ToString();
			if (timeString.Contains("."))
			{
				string[] s = f.ToString().Split(new char[] { '.' });
				string whole = s[0], dec = s[1].Length >= 2 ? s[1].Substring(0, 2) : s[1];
				if (f >= 10f)
				{
					return whole + ":" + dec;
				}
				else
				{
					return "0" + whole + ":" + dec;
				}
			}
			else
			{
				if (f >= 10f)
				{
					return timeString + ":00";
				}
				else
				{
					return "0" + timeString + ":00";
				}
			}
		}

		void SetMouseCursorLocked(bool value)
		{
			if (value)
			{
				Cursor.lockState = CursorLockMode.Locked;
			}
			else
			{
				Cursor.lockState = CursorLockMode.None;
			}
			Cursor.visible = !value;
		}

		private void CMDToggleWindowEvent(bool newState)
		{
			SetMouseCursorLocked(!newState);
			SetIsPaused(newState);
			pauseUI.SetActive(false);
		}

		protected override void OnDestroy()
		{
			((DaanRuiter.CMDPlus.CMDWindow)CMD.Controller.View).WindowToggleEvent -= CMDToggleWindowEvent;
			base.OnDestroy();
		}
	}
}