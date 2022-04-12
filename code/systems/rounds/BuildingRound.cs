using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

public class BuildingRound : GameRound
{
	public override string RoundName => "Build!";

	public override float RoundDuration => 1;

	[ServerVar( "flood_build_duration", Help = "The duration of the build round" )]
	public static float NewRoundDuration { get; set; } = 50;
	public override string NextRound => "Flood";

	public override bool Tools => true;

	public override void OnRoundStart()
	{
		RoundEndTime = Time.Now + NewRoundDuration;
		Log.Info( $"Starting Round {RoundName}" );
		FloodGame.ClearSkipList();
		foreach ( var player in Players )
		{
			if ( Tools )
			{
				if ( player.Spectating ) return;
				player.Inventory.Add( new PhysGun(), true );
				player.Inventory.Add( new Tool(), false );
			}
		}

		if ( FloodLevelManager.Instance != null ) FloodLevelManager.Instance.OnBuildStart.Fire(FloodGame.Instance);

		//base.OnRoundStart();
	}
	
	public override void OnRoundEnd()
	{
		Log.Info( Players.Count );
		foreach ( var player in Players ) player.RemoveWeapons();
		/*foreach ( var door in Entity.All.OfType<FloodDoor>() )
		{
			door.ResetDoor();
		}*/

		if ( FloodLevelManager.Instance != null ) FloodLevelManager.Instance.OnBuildEnd.Fire( FloodGame.Instance );

		base.OnRoundEnd();
	}



}
