using Sandbox;
using System;
using System.Linq;

partial class FloodInventory : BaseInventory
{


	public FloodInventory( Player player ) : base ( player )
	{

	}

	public override bool Add( Entity ent, bool makeActive = false )
	{
		if (!ent is BaseFloodWeapon || !ent is Carriable) return false;
		var player = Owner as FloodPlayer;

		var notices = !player.SupressPickupNotices;
		
		// We don't want to pick up the same weapon twice
		if ( ent != null && IsCarryingType( ent.GetType() ) && ent is BaseFloodWeapon weapon)
		{
			var ammo = weapon.AmmoClip;
			var ammoType = weapon.AmmoType;
			if ( ammo > 0 )
			{
				player.GiveAmmo( ammoType, ammo );

				if ( notices )
				{
					Sound.FromWorld( "dm.pickup_ammo", ent.WorldPos );
					PickupFeed.OnPickup( player, $"+{ammo} {ammoType}" );
				}
			}
			//There wont be items on the ground, so I'm commenting this out for now
			//ItemRespawn.Taken( ent );

			// Despawn it
			ent.Delete();
			return false;
		}

		if ( ent != null && ent is BaseFloodWeapon && notices )
		{
			Sound.FromWorld( "dm.pickup_weapon", ent.WorldPos );
			//PickupFeed.OnPickup( player, $"{ent.ClassInfo.Title}" ); 
		}

		
			//ItemRespawn.Taken( ent );
		return base.Add( ent, makeActive );
	}

	public bool IsCarryingType( Type t )
	{
		return List.Any( x => x.GetType() == t );
	}
}
