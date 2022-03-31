using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

public class BuildingRound : GameRound
{
	public override string RoundName => "Building";
	public override float RoundDuration => 300f;
	public override string NextRound => "Fight!";

	public override string[] Weapons => new string[]
	{
		"physgun",
		"weapon_tool",
	};

	public override void OnRoundStart(bool startup)
	{
		base.OnRoundStart(startup);
	}
	
	public override void OnRoundEnd()
	{
		base.OnRoundEnd();
	}



}
