using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

public class FightingRound : GameRound
{
	public override string RoundName => "Fight!";
	public override float RoundDuration => 5f;
	public override string NextRound => "Post Game";

	public override void OnRoundStart()
	{
		base.OnRoundStart();
	}

	public override void OnRoundEnd()
	{
		base.OnRoundEnd();
	}

}
