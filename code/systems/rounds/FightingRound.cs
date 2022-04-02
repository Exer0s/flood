using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

public class FightingRound : GameRound
{
	public override string RoundName => "Fight!";
	public override float RoundDuration => 20f;
	public override string NextRound => "Draining";

	public override void OnRoundStart()
	{
		foreach ( var player in Players )
		{
			player.GivePurchasedWeapons();
		}
		base.OnRoundStart();
	}

	public override void OnRoundEnd()
	{
		base.OnRoundEnd();
	}

}
