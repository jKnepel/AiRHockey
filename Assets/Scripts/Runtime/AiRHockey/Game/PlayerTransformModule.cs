using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CENTIS.UnityModuledNet.Modules;

namespace HTW.AiRHockey.Game
{
	public class PlayerTransformModule : UnreliableModule
	{
		public override string ModuleID => "PlayerTransformModule";

		public override void OnReceiveData(byte sender, byte[] data)
		{
			throw new System.NotImplementedException();
		}
	}
}
