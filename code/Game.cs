using Sandbox;
using System.Linq;
using System.Threading.Tasks;

partial class FloodGame : Game
{

	public static FloodGame Instance;

	public FloodGame()
	{
		Instance = this;
		if ( IsServer )
		{
			// Create the HUD
			_ = new FloodHUD();
			StartRoundSystem();
		}
	}


	public WaterFunc WaterObj;

	public override void PostLevelLoaded()
	{
		base.PostLevelLoaded();
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

	[ServerCmd( "spawn" )]
	public static void Spawn( string modelname )
	{
		var owner = ConsoleSystem.Caller?.Pawn;

		if ( ConsoleSystem.Caller == null )
			return;

		var tr = Trace.Ray( owner.EyePosition, owner.EyePosition + owner.EyeRotation.Forward * 500 )
			.UseHitboxes()
			.Ignore( owner )
			.Run();

		var modelRotation = Rotation.From( new Angles( 0, owner.EyeRotation.Angles().yaw, 0 ) ) * Rotation.FromAxis( Vector3.Up, 180 );

		var model = Model.Load( modelname );
		if ( model == null || model.IsError )
			return;

		var ent = new Prop
		{
			Position = tr.EndPosition + Vector3.Down * model.PhysicsBounds.Mins.z,
			Rotation = modelRotation,
			Model = model
		};

		// Let's make sure physics are ready to go instead of waiting
		ent.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

		// If there's no physics model, create a simple OBB
		if ( !ent.PhysicsBody.IsValid() )
		{
			ent.SetupPhysicsFromOBB( PhysicsMotionType.Dynamic, ent.CollisionBounds.Mins, ent.CollisionBounds.Maxs );
		}
	}

	[ServerCmd( "spawn_weapon" )]
	public static void SpawnWeapon( string weaponname )
	{
		var owner = ConsoleSystem.Caller?.Pawn;
		if ( Instance.GameRound != Instance.GameRounds["Build!"] ) return;
		if ( ConsoleSystem.Caller == null )
			return;

		if (owner is FloodPlayer player)
		{
			var weapon = WeaponAsset.All.Where(x => x.Weapon == weaponname).FirstOrDefault();
			if ( player.Money <= weapon.Cost ) return;
			else player.Money -= weapon.Cost;

			Log.Info( $"Purchased {weapon.Title} for {weapon.Cost} balance: {player.Money}" );
			player.PurchasedWeapons.Add( weaponname );
			player.Inventory.Add( Library.Create<Weapon>(weaponname), true);
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

	[AdminCmd( "respawn_entities" )]
	public static void RespawnEntities()
	{
		Map.Reset( DefaultCleanupFilter );
	}
}
