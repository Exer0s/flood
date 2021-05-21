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
	
	[ServerVar( "flood_min_players", Help = "The minimum players required to start." )]
	public static int MinPlayers { get; set; } = 2;
	[Net] public bool RespawnEnabled { get; set; } = true;
	[Net] public WaterFlood waterInstance { get; private set; }
	[Net] public float waterHeight { get; private set; }
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
			new DeathmatchHud();
		}
		
		_ = StartTickTimer();
		
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

	
	#region Spawn_Commands

	[ServerCmd("spawn_weapon")]
	public static void SpawnWeapon(string weaponName) {
		var owner = ConsoleSystem.Caller?.Pawn as FloodPlayer;

		if ( ConsoleSystem.Caller == null )
			return;
		
			BaseFloodWeapon weapon = Library.Create<BaseFloodWeapon>(weaponName);
			var inventory = owner.Inventory as Inventory;
			if ( !inventory.CanAdd( weapon ) ) return;
			Log.Info( $"{ConsoleSystem.Caller.Name} spawned {weaponName}" );
			int cost = weapon.Cost;
			if (owner.Money >= cost)
			{
				owner.Money = owner.Money - cost;
				owner.Inventory.Add(weapon, true);
			}
		
	}

	[ServerCmd("give_money")]
	public static void GiveMoney(string amount)
	{
		var owner = ConsoleSystem.Caller?.Pawn as FloodPlayer;

		if ( ConsoleSystem.Caller == null )
			return;

		owner.Money += amount.ToInt();
	}

	[ServerCmd( "spawn" )]
	public static void Spawn( string modelname )
	{
		var owner = ConsoleSystem.Caller?.Pawn;

		if ( ConsoleSystem.Caller == null )
			return;

		var tr = Trace.Ray( owner.EyePos, owner.EyePos + owner.EyeRot.Forward * 500 )
			.UseHitboxes()
			.Ignore( owner )
			.Size( 2 )
			.Run();

		var ent = new BreakableProp();
		ent.Position = tr.EndPos;
		ent.Rotation = Rotation.From( new Angles( 0, owner.EyeRot.Angles().yaw, 0 ) ) * Rotation.FromAxis( Vector3.Up, 180 );
		ent.SetModel( modelname );

		// Drop to floor
		if ( ent.PhysicsBody != null && ent.PhysicsGroup.BodyCount == 1 )
		{
			var p = ent.PhysicsBody.FindClosestPoint( tr.EndPos );

			var delta = p - tr.EndPos;
			ent.PhysicsBody.Position -= delta;
			//DebugOverlay.Line( p, tr.EndPos, 10, false );
		}

	}

	[ServerCmd( "spawn_entity" )]
	public static void SpawnEntity( string entName )
	{
		var owner = ConsoleSystem.Caller.Pawn;

		if ( owner == null )
			return;

		var attribute = Library.GetAttribute( entName );

		if ( attribute == null || !attribute.Spawnable )
			return;

		var tr = Trace.Ray( owner.EyePos, owner.EyePos + owner.EyeRot.Forward * 200 )
			.UseHitboxes()
			.Ignore( owner )
			.Size( 2 )
			.Run();

		var ent = Library.Create<Entity>( entName );
		if ( ent is BaseCarriable && owner.Inventory != null )
		{
			if ( owner.Inventory.Add( ent, true ) )
				return;
		}

		ent.Position = tr.EndPos;
		ent.Rotation = Rotation.From( new Angles( 0, owner.EyeRot.Angles().yaw, 0 ) );

		//Log.Info( $"ent: {ent}" );
	}
	#endregion


	public static void SystemMessage( string message )
	{
		Host.AssertServer();
		ChatBox.AddChatEntry( To.Everyone, "Server", message, "/ui/system.png" );
	}

	#region Server_Commands
	//Skip Round Command
	/*[ServerCmd("skipround")]
	public static void SkipRound(string args)
	{
		var player = ConsoleSystem.Caller as FloodPlayer;
		if ( player == null ) return;

		Round?.OnTimeUp();
	}*/

	#endregion

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
		if (Client.All.Count >= MinPlayers)
		{
			if (Round == null)
			{
				ChangeRound(new BuildRound());
			}
		}
	}
}
