using System;
using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
				foreach ( var client in Client.All )
				{
					if ( client.Pawn is FloodPlayer player )
						SupplyLoadouts( player );
				}
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


		private float waterHeight;
		public override void OnTick()
		{
		if ( water == null ) return;
		waterHeight += 0.25f;
		water.Position = new Vector3(0, 0, waterHeight);
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
