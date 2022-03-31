using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

public class BuildingRound : GameRound
{
	public override string RoundName => "Build!";
	public override float RoundDuration => 5f;
	public override string NextRound => "Prepare";

	public override bool Tools => true;

	public override void OnRoundStart()
	{
		base.OnRoundStart();
	}
	
	public override void OnRoundEnd()
	{
		base.OnRoundEnd();
	}



}
