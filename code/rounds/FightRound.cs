using System;
using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class FightRound : BaseRound
{
	public override string RoundName => "Fight!";
	public override int RoundDuration { get; set; } = 30;
	public override bool CanPlayerSuicide => true;

	public List<FloodPlayer> Spectators = new ();

	private bool _isGameOver;

	public override void OnPlayerKilled( FloodPlayer player )
	{
		Players.Remove( player );
		Spectators.Add( player );

		player.MakeSpectator();
	}

	public override void OnPlayerLeave( FloodPlayer player )
	{
		base.OnPlayerLeave( player );

		Players.Remove( player );
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

		if ( Host.IsServer )
		{
			foreach ( var client in Client.All )
			{
				if (client.Pawn is FloodPlayer player)
				{
					SupplyLoadouts( player );
				}
			}
			FloodGame.Instance.RespawnEnabled = false;
			FloodGame.Instance.canUseWeapons = true;
		}
		
		FloodGame.SystemMessage( "The flood is over, fight!" );


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
		Log.Info( "Fight time over" );
		FloodGame.Instance.ChangeRound( new PostGameRound() {
			Spectators = Spectators,
			Players = Players
		} );

		base.OnTimeUp();
	}

	private void SupplyLoadouts( FloodPlayer player )
	{
		// Give everyone who is alive their starting loadouts.
		if ( player.LifeState == LifeState.Alive )
		{
			var inventory = player.Inventory as Inventory;
			//If the player bought no weapons, we give them a pistol at the start of the round
			if ( player.playerWeapons.Count == 0 && inventory.CanAdd(new Pistol()))
			{
				player.Inventory.Add(new Pistol(), true);
			}


			if ( !Players.Contains( player ) )
			{
				AddPlayer( player );
			}

		}
	}


}
