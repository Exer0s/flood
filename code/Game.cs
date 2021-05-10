using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// This is the heart of the gamemode. It's responsible
/// for creating the player and stuff.
/// </summary>
[Library( "flood", Title = "Flood" )]
partial class FloodGame : Game
{
	
	[ServerVar( "flood_min_players", Help = "The minimum players required to start." )]
	public static int MinPlayers { get; set; } = 2;

	

	[Net] public BaseRound Round { get; private set; }
	private BaseRound _lastRound;

	#region Singleton
	public static FloodGame Instance
	{
		get => Current as FloodGame;
	}
	#endregion
	
	public FloodGame()
	{
		//
		// Create the HUD entity. This is always broadcast to all clients
		// and will create the UI panels clientside. It's accessible 
		// globally via Hud.Current, so we don't need to store it.
		//
		if ( IsServer )
		{
			new FloodHud();
		}
		
		_ = StartTickTimer();
		
	}

	/// <summary>
	/// Called when a player joins and wants a player entity. We create
	/// our own class so we can control what happens.
	/// </summary>
	public override Player CreatePlayer() => new FloodPlayer();


	public override void PostLevelLoaded()
	{
		base.PostLevelLoaded();
		_ = StartSecondTimer();
		ItemRespawn.Init();
	}
	
	public async Task StartSecondTimer()
	{
		while (true)
		{
			await Task.DelaySeconds( 1 );
			OnSecond();
		}
	}

	public async Task StartTickTimer()
	{
		while (true)
		{
			await Task.NextPhysicsFrame();
			OnTick();
		}
	}
	
	// Changes the round, to the one you pass in
	public void ChangeRound( BaseRound round )
	{
		Assert.NotNull( round );

		Round?.Finish();
		Round = round;
		Round?.Start();
	}

	/// <summary>
	/// Called when a player has died, or been killed
	/// </summary>
	public override void PlayerKilled( Player player )
	{
		Log.Info( $"{player.Name} was killed" );

		if ( player.LastAttacker != null )
		{
			if ( player.LastAttacker is Player attackPlayer )
			{
				KillFeed.AddEntry( attackPlayer.SteamId, attackPlayer.Name, player.SteamId, player.Name, player.LastAttackerWeapon?.ClassInfo?.Name );
			}
			else
			{
				KillFeed.AddEntry( (ulong)player.LastAttacker.NetworkIdent, player.LastAttacker.ToString(), player.SteamId, player.Name, "killed" );
			}
		}
		else
		{
			KillFeed.AddEntry( (ulong)0, "", player.SteamId, player.Name, "died" );
		}
	}

	private void OnSecond()
	{
		CheckMinimumPlayers();
		Round?.OnSecond();
	}

	private void OnTick()
	{
		Round?.OnTick();

		if ( IsClient )
		{
			// We have to hack around this for now until we can detect changes in net variables.
			if ( _lastRound != Round )
			{
				_lastRound?.Finish();
				_lastRound = Round;
				_lastRound.Start();
			}
		}
	}

	
	private void CheckMinimumPlayers()
	{
		if (Sandbox.Player.All.Count >= MinPlayers)
		{
			if (Round == null)
			{
				ChangeRound(new BuildRound());
			}
		}
	}
}
