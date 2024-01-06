using Comfort.Common;
using CommonAssets.Scripts.Game;
using EFT;
using EFT.UI;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CustomExtracts
{
	public class ExtractTestComponent : MonoBehaviour, IPhysicsTrigger
	{
		public string Description => "ExtractComponent";
		public float Duration;

		private float _timer;
		private Coroutine _coroutine;
		private MethodInfo _stopSession;

		private ExtractTestComponent()
		{
			_stopSession = typeof(EndByExitTrigerScenario).GetNestedTypes().Single(x => x.IsInterface).GetMethod("StopSession");
		}

		public void OnTriggerEnter(Collider collider)
		{
			Console.WriteLine("ENTERED");

			Player player = Singleton<GameWorld>.Instance.GetPlayerByCollider(collider);
			if (player == null || !player.IsYourPlayer)
				return;

			_timer = Duration;
			Singleton<GameUI>.Instance.BattleUiPanelExitTrigger.Show(_timer);
			_coroutine = StartCoroutine(Timer(player.ProfileId));
		}

		public void OnTriggerExit(Collider collider)
		{
			Console.WriteLine("EXITED");

			Player player = Singleton<GameWorld>.Instance.GetPlayerByCollider(collider);
			if (player == null || !player.IsYourPlayer)
				return;

			_timer = Duration;
			Singleton<GameUI>.Instance.BattleUiPanelExitTrigger.Close();

			if (_coroutine == null)
				return;

			StopCoroutine(_coroutine);
		}

		private void OnDestroy()
		{
			if (Singleton<GameUI>.Instantiated)
				Singleton<GameUI>.Instance.BattleUiPanelExitTrigger.Close();

			if (_coroutine == null)
				return;

			StopCoroutine(_coroutine);
		}

		private IEnumerator Timer(string profileId)
		{
			while (_timer > 0)
			{
				_timer -= Time.deltaTime;
				yield return null;
			}

			_stopSession.Invoke(Singleton<AbstractGame>.Instance, new object[]
			{
				profileId,
				ExitStatus.Survived,
				"new extract"
			});
		}
	}
}
