using System;
using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

	public class BuildRound : BaseRound
	{
		public override string RoundName => "Build";
		public override int RoundDuration => 300;
		public override bool CanPlayerSuicide => true;

		public List<Player> Spectators = new ();

		private bool _isGameOver;

		public override void OnPlayerKilled( FloodPlayer player )
		{
			player.Respawn();
		}

		public override void OnPlayerLeave( FloodPlayer player )
		{
			base.OnPlayerLeave( player );

			Spectators.Remove( player );

		}

		public override void OnPlayerSpawn( FloodPlayer player )
		{
			player.MakeSpectator();

			Spectators.Add( player );
			Players.Remove( player );

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

		protected override void OnTimeUp()
		{
			if ( _isGameOver ) return;

			Log.Info( "Build Time Up!" );


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
				player.Inventory.Add( new Tool(), true );
				AddPlayer( player );
			//}
		}
	}
