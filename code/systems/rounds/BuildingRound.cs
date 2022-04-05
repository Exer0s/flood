using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

public class BuildingRound : GameRound
{
	public override string RoundName => "Build!";
	public override float RoundDuration => 120f;
	public override string NextRound => "Flood";

	public override bool Tools => true;

	public override void OnRoundStart()
	{
		base.OnRoundStart();
	}
	
	public override void OnRoundEnd()
	{
		Log.Info( Players.Count );
		foreach ( var player in Players ) player.RemoveWeapons();
		base.OnRoundEnd();
	}



}
