using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

public partial class RisingRound : GameRound
{
	public override string RoundName => "Flood";
	public override float RoundDuration => 15f;
	public override string NextRound => "Fight!";

	public override void OnRoundStart()
	{
		base.OnRoundStart( );
	}

	public override void OnRoundEnd()
	{
		base.OnRoundEnd();
	}

	public override void RoundTick()
	{
		base.RoundTick();

		foreach ( var water in Entity.All.OfType<WaterFunc>() )
		{
			water.Position += Vector3.Up * 0.2f;
		}

		foreach ( var prop in Entity.All.OfType<Prop>() )
		{
			if (prop.Root == prop)
			prop.Position += Vector3.Up * 0.2f;
		}


	}


}
