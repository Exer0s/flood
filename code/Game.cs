using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Sandbox.UI;

/// <summary>
/// This is the heart of the gamemode. It's responsible
/// for creating the player and stuff.
/// </summary>
[Library( "flood", Title = "Flood" )]
partial class FloodGame : Game
{
	
	#region Networked Variables
	[ServerVar( "flood_min_players", Help = "The minimum players required to start." )]
	public static int MinPlayers { get; set; } = 2;
	[Net] public bool RespawnEnabled { get; set; } = true;
	[Net] public WaterFlood waterInstance { get; private set; }
	[Net] public float waterHeight { get; private set; }
	[Net] public BaseRound Round { get; private set; }
	private BaseRound _lastRound;
	[Net] public bool canUseWeapons { get; set; } = true;
	#endregion
	
	
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
			new DeathmatchHud();
		}
		
		_ = StartTickTimer();

		Global.PhysicsSubSteps = 5;

	}

	/// <summary>
	/// Called when a player joins and wants a player entity. We create
	/// our own class so we can control what happens.
	/// </summary>
	public override void ClientJoined( Client cl )
	{
		base.ClientJoined(cl);
		
		var player = new FloodPlayer();
		player.Respawn();

		cl.Pawn = player;
	}

	
	

	//Sends message to all clients
	public static void SystemMessage( string message )
	{
		//Host.AssertServer();
		ChatBox.AddChatEntry( To.Everyone, "Server", message, "/ui/system.png" );
	}

	
	//Called after level loads
	public override void PostLevelLoaded()
	{
		base.PostLevelLoaded();
		//Starts timer for the round system
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

	public override void OnKilled(Entity ent)
	{
		if (ent is FloodPlayer player)
		{
			Round?.OnPlayerKilled( player );
		}
		base.OnKilled( ent );
	}
	
	private void CheckMinimumPlayers()
	{
		if (Client.All.Count >= MinPlayers)
		{
			if (Round == null)
			{
				ChangeRound(new BuildRound());
			}
		}
	}
}
