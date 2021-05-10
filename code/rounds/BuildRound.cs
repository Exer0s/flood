using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flood.rounds
{
	public class BuildRound : BaseRound
	{
		public override string RoundName => "Build";
		public override int RoundDuration => 300;
		public override bool CanPlayerSuicide => true;

		public List<Player> Spectators = new ();

		private bool _isGameOver;

		public override void OnPlayerKilled( Player player )
		{
			player.Respawn();
		}

		public override void OnPlayerLeave( Player player )
		{
			base.OnPlayerLeave( player );

			Spectators.Remove( player );

		}

		public override void OnPlayerSpawn( Player player )
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
				Sandbox.Player.All.ForEach( ( player ) => SupplyLoadouts( player as Player ) );
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

			_ = LoadStatsRound( "Build Time is over!" );

			base.OnTimeUp();
		}

		private void SupplyLoadouts( Player player )
		{
			// Give everyone who is alive their starting loadouts.
			if ( player.LifeState == LifeState.Alive )
			{
				player.ClearAmmo();
				player.Inventory.DeleteContents();
				player.Inventory.Add( new PhysGun(), true );
				player.Inventory.Add( new Tool(), true );
				AddPlayer( player );
			}
		}
	}
}
