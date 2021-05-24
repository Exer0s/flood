using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Sandbox.UI;

partial class FloodGame
    {
        
	    #region Spawn_Commands

	[ServerCmd("sell_weapon")]
	public static void SellWeapon(string weaponName)
	{
		var owner = ConsoleSystem.Caller?.Pawn as FloodPlayer;

		if ( ConsoleSystem.Caller == null ) return;

		var inventory = owner.Inventory as Inventory;

		BaseFloodWeapon weapon = Library.Create<BaseFloodWeapon>( weaponName );

		if (!inventory.CanAdd(weapon))
		{
			inventory.Drop( owner.playerWeapons[weaponName] );
			owner.Money += weapon.Cost;
		}
	}

	[ServerCmd("spawn_weapon")]
	public static void SpawnWeapon(string weaponName) {
		var owner = ConsoleSystem.Caller?.Pawn as FloodPlayer;

		if ( ConsoleSystem.Caller == null )
			return;
			//Creates weapon entity reference
			BaseFloodWeapon weapon = Library.Create<BaseFloodWeapon>(weaponName);
			//get player inv
			var inventory = owner.Inventory as Inventory;
			//see if we already have it
			if ( !inventory.CanAdd( weapon ) ) return;
			Log.Info( $"{ConsoleSystem.Caller.Name} spawned {weaponName}" );
			int cost = weapon.Cost;
			//Spends money for weapon
			if (owner.Money >= cost)
			{
				owner.Money = owner.Money - cost;
				//Adds weapon to inv
				owner.Inventory.Add(weapon, true);
				owner.playerWeapons.Add( weaponName, weapon );
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
		//Finds point where the player is looking at
		var tr = Trace.Ray( owner.EyePos, owner.EyePos + owner.EyeRot.Forward * 500 )
			.UseHitboxes()
			.Ignore( owner )
			.Size( 2 )
			.Run();
		//Spawns new prop
		var ent = new BreakableProp();
		//sets prop position and rotation to where player is looking
		ent.Position = tr.EndPos;
		ent.Rotation = Rotation.From( new Angles( 0, owner.EyeRot.Angles().yaw, 0 ) ) * Rotation.FromAxis( Vector3.Up, 180 );
		//set model of the prop
		ent.SetModel( modelname );

		// Drop to floor
		if ( ent.PhysicsBody != null && ent.PhysicsGroup.BodyCount == 1 )
		{
			var p = ent.PhysicsBody.FindClosestPoint( tr.EndPos );
			
			var delta = p - tr.EndPos;
			ent.PhysicsBody.Position -= delta;
			//DebugOverlay.Line( p, tr.EndPos, 10, false );
		}

		var tr2 = Trace.Ray( owner.EyePos, owner.EyePos + owner.EyeRot.Forward * 500 )
			.UseHitboxes()
			.Ignore( owner )
			.Size( 2 )
			.Run();
		Log.Info($"spawned prop with surface {tr2.Surface.Name}");
	}

	[ServerCmd( "spawn_entity" )]
	public static void SpawnEntity( string entName )
	{
		var owner = ConsoleSystem.Caller.Pawn;

		if ( owner == null )
			return;
		//gets entity reference
		var attribute = Library.GetAttribute( entName );

		if ( attribute == null || !attribute.Spawnable )
			return;
		//finds where player is looking
		var tr = Trace.Ray( owner.EyePos, owner.EyePos + owner.EyeRot.Forward * 200 )
			.UseHitboxes()
			.Ignore( owner )
			.Size( 2 )
			.Run();
		//spawns entity
		var ent = Library.Create<Entity>( entName );
		if ( ent is BaseCarriable && owner.Inventory != null )
		{
			if ( owner.Inventory.Add( ent, true ) )
				return;
		}
		//
		ent.Position = tr.EndPos;
		ent.Rotation = Rotation.From( new Angles( 0, owner.EyeRot.Angles().yaw, 0 ) );
		
	}
	#endregion
	
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
}
