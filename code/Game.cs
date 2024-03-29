﻿using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

partial class FloodGame : Game
{
	public static FloodGame Instance { get; set; }

	[Net] public bool WaterDamageEnabled { get; set; } = true;
	public static float DefaultWaterLevel { get; set; } = -5f;

	[Net] public FloodHUD hudEntity { get; set; }
	
	public FloodGame()
	{
		Instance = this;
		if ( IsServer )
		{
			// Create the HUD
			hudEntity = new FloodHUD();
		}
	}

	public override void PostLevelLoaded()
	{
		base.PostLevelLoaded();
		if (IsServer)
		{
			StartRoundSystem();
			var water = All.OfType<WaterFunc>().FirstOrDefault();
			DefaultWaterLevel = water.Position.z;

			if ( All.OfType<FloodLevelManager>().FirstOrDefault() is null ) Log.Warning( "map doesn't have official support! using default settings" );

		}
		
	}


	public override void ClientJoined( Client cl )
	{
		base.ClientJoined( cl );
		var player = new FloodPlayer( cl );
		player.Respawn();
		cl.Pawn = player;
	}
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	

	public void CheckAliveTeams()
	{
		int aliveteams = 0;
		foreach ( var team in All.OfType<BaseTeam>() )
		{
			if ( team.CheckAlive() ) aliveteams++;
		}

		if (aliveteams <= 1)
		{
			//win stuff here
			if ( GameRound is FightingRound ) ProgressRound();
			if ( GameRound is RisingRound )
			{
				TimeOffset = Time.Now;
				GameRound.OnRoundEnd();
				GameRound = GameRounds["Draining"];
				GameRound.Players.Clear();
				foreach ( var player in All.OfType<FloodPlayer>() )
				{
					GameRound.Players.Add( player );
				}
				GameRound.OnRoundStart();

				SetRoundNameUI(To.Everyone, GameRound.RoundName);
				OnSecond();
			}
		}

	}
	
	public override void DoPlayerNoclip( Client player )
	{
		if ( player.Pawn is Player basePlayer )
		{
			if ( basePlayer.DevController is NoclipController )
			{
				Log.Info( "Noclip Mode Off" );
				basePlayer.DevController = null;
			}
			else
			{
				Log.Info( "Noclip Mode On" );
				basePlayer.DevController = new NoclipController();
			}
		}
	}
	
	#region Console/Utility Commands

	[ConCmd.Server( "spawn" )]
	public static void Spawn( string modelname, float health, float cost, float payout )
	{
		var owner = ConsoleSystem.Caller?.Pawn;

		if ( ConsoleSystem.Caller == null )
			return;


		if (Instance.GameRound is BuildingRound)
		{
			if ( owner is FloodPlayer player )
			{
				if ( player.Money <= cost ) return;
				else player.Money -= cost;
			}
		}

		if ( Instance.GameRound is not BuildingRound && Instance.GameRound is not WaitingRound ) return;
	


		var tr = Trace.Ray( owner.EyePosition, owner.EyePosition + owner.EyeRotation.Forward * 500 )
			.UseHitboxes()
			.Ignore( owner )
			.Run();

		var modelRotation = Rotation.From( new Angles( 0, owner.EyeRotation.Angles().yaw, 0 ) ) * Rotation.FromAxis( Vector3.Up, 180 );

		var model = Model.Load( modelname );
		if ( model == null || model.IsError )
			return;

		var ent = new FloodProp
		{
			Position = tr.EndPosition + Vector3.Down * model.PhysicsBounds.Mins.z,
			Rotation = modelRotation,
			Model = model,
			Owner = owner
		};

		// Let's make sure physics are ready to go instead of waiting
		ent.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		ent.Health = health;
		ent.maxHealth = health;
		ent.DestroyPayout = payout;
		var p = owner as FloodPlayer;
		p.SpawnedProps.Add( ent, cost );

		// If there's no physics model, create a simple OBB
		if ( !ent.PhysicsBody.IsValid() )
		{
			ent.SetupPhysicsFromOBB( PhysicsMotionType.Dynamic, ent.CollisionBounds.Mins, ent.CollisionBounds.Maxs );
		}
	}

	[ConCmd.Server("undo")]
	public static void UndoProp()
	{
		var owner = ConsoleSystem.Caller?.Pawn as FloodPlayer;
		if ( Instance.GameRound is FightingRound || Instance.GameRound is PostRound || Instance.GameRound is RisingRound ) return;
		if ( owner.SpawnedProps.Count <= 0 ) return;
		var deletingprop = owner.SpawnedProps.Last();
		if (Instance.GameRound is BuildingRound)
			owner.Money += deletingprop.Value;
		owner.SpawnedProps.Remove( deletingprop.Key );
		deletingprop.Key.Delete();
	}
	

	[ConCmd.Server( "spawn_weapon" )]
	public static void SpawnWeapon( string weaponname )
	{
		var owner = ConsoleSystem.Caller?.Pawn;
		if ( Instance.GameRound != Instance.GameRounds["Build!"] ) return;
		if ( ConsoleSystem.Caller == null )
			return;

		if (owner is FloodPlayer player)
		{
			var weapon = WeaponAsset.All.Where(x => x.Weapon == weaponname).FirstOrDefault();
			if ( player.PurchasedWeapons.Contains( weaponname ) ) //if we have, sell
			{
				var droppingweapon = player.ServerPurchasedWeapons[weaponname];
				
				player.Inventory.Drop( droppingweapon );
				player.PurchasedWeapons.Remove( weaponname );
				player.ServerPurchasedWeapons.Remove( weaponname );
				droppingweapon.Delete();
				player.Money += weapon.Cost;
			}
			else //else give em that gun
			{
				
				if ( player.Money <= weapon.Cost ) return;
				else player.Money -= weapon.Cost;
				Log.Info( $"Purchased {weapon.Title} for {weapon.Cost} balance: {player.Money}" );
				var weaponent = TypeLibrary.Create<Weapon>( weaponname );
				player.PurchasedWeapons.Add( weaponname );
				player.Inventory.Add( weaponent, true);
				player.ServerPurchasedWeapons.Add(weaponname, weaponent );
			}
			
		}
	}



	[ConCmd.Admin( "respawn_entities" )]
	public static void RespawnEntities()
	{
		Map.Reset( DefaultCleanupFilter );
	}

	[ConCmd.Admin("flood_water_damage")]
	public static void SetWaterDamage(bool state)
	{
		Instance.WaterDamageEnabled = state;
	}
	
	#endregion

}
