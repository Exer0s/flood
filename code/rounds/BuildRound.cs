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
			Log.Info( "Started Hunt Round" );

			if ( Host.IsServer )
			{
				Sandbox.Player.All.ForEach( ( player ) => SupplyLoadouts( player as Player ) );
			}
		}

		protected override void OnFinish()
		{
			Log.Info( "Finished Hunt Round" );

			if ( Host.IsServer )
			{
				Spectators.Clear();
			}
		}

		protected override void OnTimeUp()
		{
			if ( _isGameOver ) return;

			Log.Info( "Hunt Time Up!" );

			_ = LoadStatsRound( "I.R.I.S. Survived Long Enough" );

			base.OnTimeUp();
		}

		private void SupplyLoadouts( Player player )
		{
			// Give everyone who is alive their starting loadouts.
			if ( player.Team != null && player.LifeState == LifeState.Alive )
			{
				player.ClearAmmo();
				player.Inventory.DeleteContents();
				player.Inventory.Add( new Pistol(), true );
				AddPlayer( player );
			}
		}

		private async Task LoadStatsRound( string winner, int delay = 3 )
		{
			_isGameOver = true;

			await Task.Delay( delay * 1000 );

			if ( Game.Instance.Round != this )
				return;

			var hidden = Game.Instance.GetTeamPlayers<HiddenTeam>().FirstOrDefault();

			Game.Instance.ChangeRound( new StatsRound
			{
				HiddenName = hidden != null ? hidden.Name : "",
				HiddenKills = _hiddenKills,
				FirstDeath = _firstDeath,
				HiddenHunter = _hiddenHunter,
				Winner = winner
			} );
		}
	}
}
