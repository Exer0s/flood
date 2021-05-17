﻿using System;
using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class FightRound : BaseRound
{
	public override string RoundName => "Fight!";
	//[ServerVar( "flood_fight_duration", Help = "The duration of the flood round" )]
	public override int RoundDuration => 180;
	public override bool CanPlayerSuicide => true;

	public List<FloodPlayer> Spectators = new ();

	private bool _isGameOver;

	public override void OnPlayerKilled( FloodPlayer player )
	{
		player.Respawn();
	}

	public override void OnPlayerLeave( FloodPlayer player )
	{
		base.OnPlayerLeave( player );

		Players.Remove( player );
		Spectators.Remove( player );
	}

	public override void OnPlayerSpawn( FloodPlayer player )
	{
		player.ClearAmmo();
		player.Inventory.DeleteContents();
		player.Inventory.Add( new Pistol(), true );
		player.Inventory.Add( new SMG(), false );
		player.Inventory.Add( new Shotgun(), false );
		if ( !Players.Contains( player ) )
		{
			AddPlayer( player );
		}



		base.OnPlayerSpawn( player );
	}

	protected override void OnStart()
	{

		if ( Host.IsServer )
		{
			Sandbox.Player.All.ForEach( ( player ) => SupplyLoadouts( player as FloodPlayer ) );
		}

		FloodGame.SystemMessage( "The flood is over, fight!" );


	}

	protected override void OnFinish()
	{
		if ( Host.IsServer )
		{
			Spectators.Clear();
		}
	}


	public override void OnTick()
	{
		
		//base.OnTick();
	}

	public override void OnSecond()
	{

		//base.OnSecond();
	}

	public override void OnTimeUp()
	{
		if ( _isGameOver ) return;

		FloodGame.Instance.ChangeRound( new PostGameRound() );

		base.OnTimeUp();
	}

	private void SupplyLoadouts( FloodPlayer player )
	{
		// Give everyone who is alive their starting loadouts.
		//if ( player.LifeState == LifeState.Alive )
		//{
		player.ClearAmmo();
		player.Inventory.DeleteContents();
		player.Inventory.Add( new Pistol(), true );
		player.Inventory.Add( new SMG(), false );
		player.Inventory.Add( new Shotgun(), false );
		if ( !Players.Contains( player ) )
		{
			AddPlayer( player );
		}

		//}
	}
}