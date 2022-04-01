using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

public class WaitingRound : GameRound
{
	public override string RoundName => "Waiting...";
	public override float RoundDuration => 0f;
	public override string NextRound => "Build!";

	public override void OnRoundStart()
	{
		base.OnRoundStart();
	}

	public override void OnRoundEnd()
	{
		Map.Reset( FloodGame.DefaultCleanupFilter );
		base.OnRoundEnd();
	}



}
