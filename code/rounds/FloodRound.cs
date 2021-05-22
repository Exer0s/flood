using System;
using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

	public partial class FloodRound : BaseRound
	{
		public override string RoundName => "Flood!";
		[ServerVar( "flood_flood_duration", Help = "The duration of the flood round" )]
		public override int RoundDuration { get; set; } = 10;
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

		private WaterFlood water;
		protected override void OnStart()
		{

			if ( Host.IsServer )
			{
				foreach ( var client in Client.All )
				{
					if ( client.Pawn is FloodPlayer player )
						SupplyLoadouts( player );
				}
			FloodGame.Instance.canUseWeapons = false;
		}

		water = FloodGame.Instance.waterInstance;
		FloodGame.SystemMessage( "The build phase is over, the water rises..." );


		}

		protected override void OnFinish()
		{
			
			if ( Host.IsServer )
			{
				Spectators.Clear();
			}
		}


		[Net] private float waterHeight { get; set; }
		public override void OnTick()
		{
		if ( water == null ) return;
		if (Host.IsServer)
		{
			waterHeight += 0.2f;
		}
		water.Position = new Vector3(0, 0, waterHeight);
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
			FloodGame.Instance.waterHeight = waterHeight;
			FloodGame.Instance.ChangeRound(new FightRound());

			base.OnTimeUp();
		}

		private void SupplyLoadouts( FloodPlayer player )
		{
			// Give everyone who is alive their starting loadouts.
			if ( player.LifeState == LifeState.Alive )
			{
			//Log.Info( "Dropping build tools, holstering weapon" );

			player.Inventory.Drop( player.pgun );
			player.Inventory.Drop( player.tgun );
			player.ActiveChild = null;
			if (!Players.Contains(player))
			{
				AddPlayer(player);
			}
			
			}
		}
	}
