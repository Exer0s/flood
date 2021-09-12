using System;
using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

	public partial class PostGameRound : BaseRound
	{
		public override string RoundName => "Post-Game";
		public override int RoundDuration { get; set; } = 30;
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
			//Spectators.Remove(player);
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
		waterHeight = FloodGame.Instance.waterHeight;
		water = FloodGame.Instance.waterInstance;
		}

		protected override void OnFinish()
		{
			if ( Host.IsServer )
			{
				//Spectators.Clear();
			}
		}
		

	public override void OnSecond()
	{
		base.OnSecond();
	}
	[Net] private float waterHeight { get; set; }
	public override void OnTick()
	{
		if ( water == null ) return;
		if ( waterHeight <= 0 ) return;
		if ( Host.IsServer )
		{
			waterHeight -= 0.2f;
		}
		water.Position = new Vector3( 0, 0, waterHeight );
		base.OnTick();
	}

	public override void OnTimeUp()
		{
			if ( _isGameOver ) return;
			
			FloodGame.Instance.ChangeRound(new BuildRound());

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
