using Sandbox;
/// <summary>
/// This is the heart of the gamemode. It's responsible
/// for creating the player and stuff.
/// </summary>
[Library( "flood", Title = "Flood" )]
partial class DeathmatchGame : Game
{

	[Net] public BaseRound Round { get; private set; }
	private BaseRound _lastRound;

	public DeathmatchGame()
	{
		//
		// Create the HUD entity. This is always broadcast to all clients
		// and will create the UI panels clientside. It's accessible 
		// globally via Hud.Current, so we don't need to store it.
		//
		if ( IsServer )
		{
			new DeathmatchHud();
		}
	}

	/// <summary>
	/// Called when a player joins and wants a player entity. We create
	/// our own class so we can control what happens.
	/// </summary>
	public override Player CreatePlayer() => new DeathmatchPlayer();


	public override void PostLevelLoaded()
	{
		base.PostLevelLoaded();

		ItemRespawn.Init();
	}

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
}
