﻿using System;
using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

	public class FloodRound : BaseRound
	{
		public override string RoundName => "Fight!";
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
			Spectators.Remove(player);
		}

		public override void OnPlayerSpawn( FloodPlayer player )
		{
			player.ClearAmmo();
			player.Inventory.DeleteContents();
			player.Inventory.Add( new Pistol(), true );
			player.Inventory.Add( new SMG(), false );
			player.Inventory.Add( new Shotgun(), false );
			Log.Info("Received weapons");
			if (!Players.Contains(player))
			{
				AddPlayer(player);
			}
			
				
				
			base.OnPlayerSpawn( player );
		}

	public WaterSea water;
		protected override void OnStart()
		{
			Log.Info( "Started Fight Round" );

			if ( Host.IsServer )
			{
				Sandbox.Player.All.ForEach( ( player ) => SupplyLoadouts( player as FloodPlayer ) );
			}

		water = new WaterSea();
		


		}

		protected override void OnFinish()
		{
			Log.Info( "Finished Fight Round" );

			if ( Host.IsServer )
			{
				Spectators.Clear();
			}
		}
		private float heightChange = 0.05f;
		private float oldHeight;
		public override void OnTick()
		{
		if ( water == null ) return;
		water.waterHeight += 0.02f;
		oldHeight += 0.02f;
		if (oldHeight > heightChange)
		{
			water.MakeSeaMesh();
			water.CreatePhysics();
			oldHeight = 0;
		}
		//base.OnTick();
	}

	public override void OnSecond()
	{
		if ( water == null ) return;
		
		//base.OnSecond();
	}

	protected override void OnTimeUp()
		{
			if ( _isGameOver ) return;

			Log.Info( "Fight Time Up!" );


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
			Log.Info("Received weapons");
			if (!Players.Contains(player))
			{
				AddPlayer(player);
			}

				Log.Info("Supplied loadout");
				//}
		}
	}