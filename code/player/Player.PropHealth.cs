using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

public partial class FloodPlayer
{

	public bool LookingAtProp { get; set; }

	private TraceResult PropTrace()
	{
		if ( EyePosition == Vector3.Zero ) return new TraceResult() { Hit = false }; //NRE was here for some reason
		var tr = Trace.Ray( EyePosition + EyeRotation.Forward * 64f, EyePosition + EyeRotation.Forward * 1500f )
			.Ignore( this )
			.WorldAndEntities()
			.HitLayer( CollisionLayer.Player, false ) // Why the fuck doesn't this work?
			.HitLayer( CollisionLayer.Debris, true )
			.Run();
		if ( tr.Hit && tr.Entity is not Player )
			return tr;
		return new TraceResult() { Hit = false };
	}

	private PropHealthUI HealthUI;

	public void CheckLookingProp()
	{

		if ( HealthUI == null ) HealthUI = PropHealthUI.Instance;
		if ( FloodGame.Instance.GameRound is FightingRound )
		{
			var tr = PropTrace();
			if ( tr.Hit && tr.Entity.IsValid() )
			{
				if ( tr.Entity is not FloodProp )
				{
					HealthUI.LookingProp = null;
					LookingAtProp = false;
					return;
				}

				if ( !LookingAtProp )
				{
					LookingAtProp = true;
					HealthUI.LookingProp = tr.Entity as FloodProp;
				}
				else
				{
					if ( tr.Entity as FloodProp != HealthUI.LookingProp )
					{
						HealthUI.LookingProp = tr.Entity as FloodProp;
					}
				}



			}
			else
			{
				HealthUI.LookingProp = null;
				LookingAtProp = false;
			}

		} else
		{
			HealthUI.LookingProp = null;
			LookingAtProp = false;
		}
	}
}
