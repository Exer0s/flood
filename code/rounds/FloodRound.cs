using System;
using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

	public class FloodRound : BaseRound
	{
		public override string RoundName => "Flood!";
		//[ServerVar( "flood_fight_duration", Help = "The duration of the flood round" )]
		public override int RoundDuration => 10;
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
			if (!Players.Contains(player))
			{
				AddPlayer(player);
			}
			
				
				
			base.OnPlayerSpawn( player );
		}

		public WaterFlood water;
		protected override void OnStart()
		{

			if ( Host.IsServer )
			{
				Sandbox.Player.All.ForEach( ( player ) => SupplyLoadouts( player as FloodPlayer ) );
			}

		water = new WaterFlood();
		Log.Info("Created water mesh");
		FloodGame.SystemMessage( "The build phase is over, Fight!" );


		}

		protected override void OnFinish()
		{
			water.waterLevel = 1f;
			oldHeight += 0f;
			water.MakeSeaMesh();
			if ( Host.IsServer )
			{
				Spectators.Clear();
			}
		}
		
		
		private float heightChange = 0.5f;
		private float heightGain = 0.1f;
		private float oldHeight;
		public override void OnTick()
		{
		if ( water == null ) return;
		water.waterLevel += heightGain;
		oldHeight += heightGain;
		if (oldHeight > heightChange)
		{
			water.MakeSeaMesh();
			//water.CreatePhysics();
			oldHeight = 0;
		}
		base.OnTick();
	}

	public override void OnSecond()
	{
		if ( water == null ) return;
		//water.MakeSeaMesh();
		Log.Info( "waterHeight updated : " + water.waterLevel.ToString() + " | " + oldHeight.ToString() );
		base.OnSecond();
	}

	public override void OnTimeUp()
		{
			if ( _isGameOver ) return;
			Log.Info( "Flood round time up" );
			FloodGame.Instance.ChangeRound(new FightRound());

			base.OnTimeUp();
		}

		private void SupplyLoadouts( FloodPlayer player )
		{
			// Give everyone who is alive their starting loadouts.
			//if ( player.LifeState == LifeState.Alive )
			//{
			player.ClearAmmo();
			player.Inventory.DeleteContents();
			if (!Players.Contains(player))
			{
				AddPlayer(player);
			}
			
				//}
		}
	}
