using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

public class PostRound : GameRound
{
	public override string RoundName => "Post Game";
	public override float RoundDuration => 15f;
	public override string NextRound => "Build!";

	public override void OnRoundStart()
	{
		base.OnRoundStart();
	}

	public override void OnRoundEnd()
	{
		Map.Reset( FloodGame.DefaultCleanupFilter );
		foreach ( var player in Players )
		{
			player.OnKilled();
		}
		base.OnRoundEnd();
	}

	public override void RoundTick()
	{
		base.RoundTick();

		foreach ( var water in Entity.All.OfType<WaterFunc>() )
		{
			water.Position -= Vector3.Up * 0.2f;
		}

	}

}
