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

	public FloodLevelManager levelmanager;

	public override void OnRoundStart()
	{
		levelmanager = Entity.All.OfType<FloodLevelManager>().FirstOrDefault();
		var roundtime = levelmanager.FloodTime;
		RoundEndTime = Time.Now + roundtime;
		Log.Info( $"Starting Round {RoundName}" );
		Log.Info( roundtime );

		foreach ( var prop in Entity.All.OfType<FloodProp>() )
		{
			var rootEnt = prop.Root;
			if ( !rootEnt.IsValid() ) return;

			var physicsGroup = rootEnt.PhysicsGroup;
			if ( physicsGroup == null ) return;

			for ( int i = 0; i < physicsGroup.BodyCount; ++i )
			{
				var body = physicsGroup.GetBody( i );
				if ( !body.IsValid() ) continue;

				if ( body.BodyType == PhysicsBodyType.Static )
				{
					body.BodyType = PhysicsBodyType.Dynamic;
				}
			}
		}

		if ( FloodLevelManager.Instance != null ) FloodLevelManager.Instance.OnFloodStart.Fire( FloodGame.Instance );

		//base.OnRoundStart();
	}

	public override void OnRoundEnd()
	{

		if ( FloodLevelManager.Instance != null ) FloodLevelManager.Instance.OnFloodEnd.Fire( FloodGame.Instance );

		base.OnRoundEnd();
	}

	public override void RoundTick()
	{
		base.RoundTick();

		foreach ( var water in Entity.All.OfType<WaterFunc>() )
		{
			if ( water.Position.z >= levelmanager.WaterHeight ) return;
			water.Position += Vector3.Up * levelmanager.RiseSpeed;
		}

		foreach ( var prop in Entity.All.OfType<Prop>() )
		{
			if (prop.Root == prop)
			prop.Position += Vector3.Up * levelmanager.RiseSpeed;
		}


	}


}
