using System;
using UnityEngine;

namespace CustomExtracts
{
	public class CustomExtract : MonoBehaviour
	{
		public GameObject extract;



		private void Awake()
		{
			Console.WriteLine("Awake called for " + this.gameObject.name);
		}



		private void OnDestroy()
		{
			Console.WriteLine("OnDestroy called for " + this.gameObject.name);
		}
	}
}
