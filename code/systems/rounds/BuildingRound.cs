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
				player.Inventory.Add( new PhysGun(), true );
				player.Inventory.Add( new Tool(), false );
			}
		}
		//base.OnRoundStart();
	}
	
	public override void OnRoundEnd()
	{
		Log.Info( Players.Count );
		foreach ( var player in Players ) player.RemoveWeapons();
		base.OnRoundEnd();
	}



}
