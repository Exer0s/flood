using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

public partial class FloodProp : Prop
{

	[Net] public float maxHealth { get; set; }

	public float DestroyPayout { get; set; }

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
		if (LastAttacker is FloodPlayer attacker)
		{
			if ( LastAttacker != Owner )
			{
				if ( IsServer )
					attacker.DestroyedProp( DestroyPayout );
			}
		}
		base.OnKilled();
	}

}
