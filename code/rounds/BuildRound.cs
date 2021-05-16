using System;
using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

	public class BuildRound : BaseRound
	{
		public override string RoundName => "Build!";
		[ServerVar( "flood_build_duration", Help = "The duration of the build round" )]
		public override int RoundDuration => 300;
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
			player.Inventory.Add( new PhysGun(), true );
			player.Inventory.Add( new Tool(), false );
			Log.Info("Received building tools");
			if (!Players.Contains(player))
			{
				AddPlayer(player);
			}
			
				
				
			base.OnPlayerSpawn( player );
		}
		
		protected override void OnStart()
		{
			Log.Info( "Started Build Round" );

			if ( Host.IsServer )
			{
				Sandbox.Player.All.ForEach( ( player ) => SupplyLoadouts( player as FloodPlayer ) );
			}
			
		


		}

		protected override void OnFinish()
		{
			Log.Info( "Finished Build Round" );

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

	protected override void OnTimeUp()
		{
			if ( _isGameOver ) return;

			Log.Info( "Build Time Up!" );
			FloodGame.Instance.ChangeRound(new FloodRound());

			base.OnTimeUp();
		}

		private void SupplyLoadouts( FloodPlayer player )
		{
			// Give everyone who is alive their starting loadouts.
			//if ( player.LifeState == LifeState.Alive )
			//{
				player.ClearAmmo();
				player.Inventory.DeleteContents();
				player.Inventory.Add( new PhysGun(), true );
				player.Inventory.Add( new Tool(), false );
				if (!Players.Contains(player))
				{
					AddPlayer(player);
				}
				Log.Info("Supplied loadout");
				//}
		}
	}
