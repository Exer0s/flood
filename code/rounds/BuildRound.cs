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
		public override int RoundDuration { get; set; } = 10;
		public override bool CanPlayerSuicide => true;

		private bool _isGameOver;

		public override void OnPlayerKilled( FloodPlayer player )
		{
			player.Respawn();
	}

		public override void OnPlayerLeave( FloodPlayer player )
		{
			base.OnPlayerLeave( player );
		Players.Remove( player );
		}

		public override void OnPlayerSpawn( FloodPlayer player )
		{
			player.ClearAmmo();
			player.Inventory.DeleteContents();
			player.pgun = Library.Create<PhysGun>( "physgun" );
			player.tgun = Library.Create<Tool>( "weapon_tool" );
			player.Inventory.Add( player.pgun, true );
			player.Inventory.Add( player.tgun, false );
			//ConsoleSystem.Run( "god" );
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
					{
						SupplyLoadouts( player );
						player.Respawn();
					}
				}

			if ( FloodGame.Instance.waterInstance == null )
			{
				water = new WaterFlood();
				FloodGame.Instance.waterInstance = water;
			}
			FloodGame.Instance.RespawnEnabled = true;
		}
		
		
		FloodGame.SystemMessage("The build phase has started");
		}

		protected override void OnFinish()
		{
			base.OnFinish();
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
				if (player.pgun == null)
				{
				player.pgun = Library.Create<PhysGun>( "physgun" );
				player.tgun = Library.Create<Tool>( "weapon_tool" );
				}
				
				player.Inventory.Add( player.pgun, true );
				player.Inventory.Add( player.tgun, false );
				if (!Players.Contains(player))
				{
					AddPlayer(player);
				}
				//}
		}
	}
