﻿using Sandbox;
using System;
using System.Linq;

partial class DeathmatchPlayer
{
	[Net]
	public NetList<int> Ammo { get; set; } = new ();

	public void ClearAmmo()
	{
		Ammo.Clear();
	}

	public int AmmoCount( AmmoType type )
	{
		
		if ( Ammo == null ) return 0;

		return Ammo.Get( type );
	}

	public bool GiveAmmo( AmmoType type, int amount )
	{
		if ( !Host.IsServer ) return false;
		if ( Ammo == null ) return false;

		var currentAmmo = AmmoCount( type );
		return Ammo.Set( type, currentAmmo + amount );
	}

	public int TakeAmmo( AmmoType type, int amount )
	{
		//if ( Ammo == null ) return 0;

		var available = Ammo.Get( type );
		amount = Math.Min( Ammo.Get( type ), amount );

		Ammo.Set( type, available - amount );
		NetworkDirty( "Ammo", NetVarGroup.Net );

		return amount;
	}
}

public enum AmmoType
{
	Pistol,
	Buckshot,
	Crossbow
}