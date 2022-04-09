using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

public class FloodUnstuck
{
	public BasePlayerController Controller;

	public bool IsActive; // replicate

	internal int StuckTries = 0;

	public FloodUnstuck( BasePlayerController controller )
	{
		Controller = controller;
	}

	public virtual bool TestAndFix()
	{
		var result = Controller.TraceBBox( Controller.Position, Controller.Position );

		// Not stuck, we cool
		if ( !result.StartedSolid )
		{
			StuckTries = 0;
			return false;
		}

		//If we are hitting our claimed door, dont freak the fuck out
		var player = Controller.Pawn as FloodPlayer;
		if (player.Team != null)
		{
			if ( result.Entity.Tags.Has( player.Team.TeamTag ) ) return false;
		}
		

		if ( result.StartedSolid )
		{
			if ( BasePlayerController.Debug )
			{
				DebugOverlay.Text( Controller.Position, $"[stuck in {result.Entity}]", Color.Red );
				Box( result.Entity, Color.Red );
			}
		}

		//
		// Client can't jiggle its way out, needs to wait for
		// server correction to come
		//
		if ( Host.IsClient )
			return true;

		int AttemptsPerTick = 20;

		for ( int i = 0; i < AttemptsPerTick; i++ )
		{
			var pos = Controller.Position + Vector3.Random.Normal * (((float)StuckTries) / 2.0f);

			// First try the up direction for moving platforms
			if ( i == 0 )
			{
				pos = Controller.Position + Vector3.Up * 5;
			}

			result = Controller.TraceBBox( pos, pos );

			if ( !result.StartedSolid )
			{
				if ( BasePlayerController.Debug )
				{
					DebugOverlay.Text( Controller.Position, $"unstuck after {StuckTries} tries ({StuckTries * AttemptsPerTick} tests)", Color.Green, 5.0f );
					DebugOverlay.Line( pos, Controller.Position, Color.Green, 5.0f, false );
				}

				Controller.Position = pos;
				return false;
			}
			else
			{
				if ( BasePlayerController.Debug )
				{
					DebugOverlay.Line( pos, Controller.Position, Color.Yellow, 0.5f, false );
				}
			}
		}

		StuckTries++;

		return true;
	}

	public void Box( Entity ent, Color color, float duration = 0.0f )
	{
		if ( ent is ModelEntity modelEnt )
		{
			var bbox = modelEnt.CollisionBounds;
			DebugOverlay.Box( duration, modelEnt.Position, modelEnt.Rotation, bbox.Mins, bbox.Maxs, color );
		}
		else
		{
			DebugOverlay.Box( duration, ent.Position, ent.Rotation, -1, 1, color );
		}
	}
}
