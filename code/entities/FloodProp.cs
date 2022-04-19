using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

public partial class FloodProp : Prop
{
	public override void TakeDamage( DamageInfo info )
	{
		if (info.Attacker is FloodPlayer player)
		{
			player.DidDamage(info);
		}

		base.TakeDamage( info );
	}

	public override void OnKilled()
	{
		if (Owner is FloodPlayer player) player.SpawnedProps.Remove( this );
		base.OnKilled();
	}

}
