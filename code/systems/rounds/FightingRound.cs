﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

public class FightingRound : GameRound
{
	public override string RoundName => "Fight!";
	public override float RoundDuration => 1;

	[ConVar.Server( "flood_fight_duration", Help = "The duration of the fight round" )]
	public static float NewRoundDuration { get; set; } = 50;
	public override string NextRound => "Draining";

	public override void OnRoundStart()
	{
		RoundEndTime = Time.Now + NewRoundDuration;
		Log.Info( $"Starting Round {RoundName}" );

		foreach ( var player in Players )
		{
			if (!player.Spectating)
			{
				if ( player.PurchasedWeapons.Count > 0 )
				{
					player.GivePurchasedWeapons();
				}
				else
				{
					player.Inventory.Add( new Pistol(), true );
				}
			}
		}

		if ( FloodLevelManager.Instance != null ) FloodLevelManager.Instance.OnFightStart.Fire( FloodGame.Instance );

		//base.OnRoundStart();
	}

	public override void OnRoundEnd()
	{

		if ( FloodLevelManager.Instance != null ) FloodLevelManager.Instance.OnFightEnd.Fire( FloodGame.Instance );

		base.OnRoundEnd();
	}

}
