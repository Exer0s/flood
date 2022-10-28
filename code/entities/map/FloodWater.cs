using Sandbox;
using SandboxEditor;

[Library( "flood_water" )]
[HammerEntity, Solid]
[HideProperty( "enable_shadows" )]
[HideProperty( "SetColor" )]
[Title( "Flood Water Volume" ), Category( "Gameplay" ), Icon( "water" )]
public partial class FloodWater : WaterFunc
{
	
	public WaterHurtbox hurtBox;

	public override void Spawn()
	{
		base.Spawn();
		hurtBox = new WaterHurtbox();
		var hurtmaxs = WorldSpaceBounds.Maxs;
		hurtmaxs.z -= 23.5f;
		hurtBox.SetupPhysicsFromOBB( PhysicsMotionType.Keyframed, WorldSpaceBounds.Mins, hurtmaxs );
		hurtBox.SetParent(this);
	}

}

public class WaterHurtbox : TriggerMultiple
{
	public override void Touch( Entity other )
	{
		base.Touch( other );
		
		if (other is FloodPlayer player)
		{
			if (FloodGame.Instance.GameRound is FightingRound || FloodGame.Instance.GameRound is RisingRound)
			{
				if ( FloodGame.Instance.WaterDamageEnabled == true)
					player.TakeDamage( DamageInfo.Generic( 0.1f ) );
			}
		}
		
	}
}
