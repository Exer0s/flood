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

		
		Log.Info("Created water mesh");
		FloodGame.SystemMessage( "The build phase is over, the water rises..." );


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
		
		base.OnTick();
	}

	public override void OnSecond()
	{
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
