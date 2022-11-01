using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

public class PostRound : GameRound
{
	public override string RoundName => "Draining";
	public override float RoundDuration => 15f;
	public override string NextRound => "Build!";

	public FloodLevelManager levelmanager;

	public override void OnRoundStart()
	{
		levelmanager = Entity.All.OfType<FloodLevelManager>().FirstOrDefault();
		var roundtime = levelmanager.FloodTime;
		RoundEndTime = Time.Now + roundtime;
		Log.Info( $"Starting Round {RoundName}" );
		Log.Info( roundtime );

		if ( FloodLevelManager.Instance != null ) FloodLevelManager.Instance.OnDrainStart.Fire( FloodGame.Instance );

		//base.OnRoundStart();
	}

	public override void OnRoundEnd()
	{
		

		
		foreach ( var player in Entity.All.OfType<FloodPlayer>() )
		{
			
			if (!player.Spectating) player.Pay(2500);
			
			player.SpawnedProps.Clear();
		}
		Map.Reset( FloodGame.DefaultCleanupFilter );
		foreach ( var player in Players )
		{
			if ( player.Spectating && FloodGame.Instance.DieRemoveWeapons )
			{
				player.PurchasedWeapons.Clear();
				player.ServerPurchasedWeapons.Clear();
				FloodGame.Instance.hudEntity.RefreshSpawnPanel(To.Single(player));
			}
			player.OnKilled();
			
		}

		if ( FloodLevelManager.Instance != null ) FloodLevelManager.Instance.OnDrainEnd.Fire( FloodGame.Instance );

		base.OnRoundEnd();
	}

	public override void RoundTick()
	{
		base.RoundTick();

		foreach ( var water in Entity.All.OfType<FloodWater>() )
		{
			if ( water.Position.z <= FloodGame.DefaultWaterLevel ) return;
			water.Position -= Vector3.Up * levelmanager.RiseSpeed;
		}

	}

}
