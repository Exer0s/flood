using Sandbox;

public class RagdollCamera : BaseCamera
{
	private Vector3 focusPoint;

	public override void Activated()
	{
		base.Activated();

		focusPoint = GetSpectatePoint();
		Pos = focusPoint + GetViewOffset();
		FieldOfView = LastFieldOfView;
	}

	public override void Update()
	{
		if ( Player.Local is not BasePlayer player ) return;

		focusPoint = GetSpectatePoint();
		Pos = focusPoint + GetViewOffset();

		var tr = Trace.Ray( focusPoint, Pos )
			.Ignore( player )
			.WorldOnly()
			.Radius( 4 )
			.Run();

		//
		// Doing a second trace at the half way point is a little trick to allow a larger camera collision radius
		// without getting initially stuck
		//
		tr = Trace.Ray( focusPoint + tr.Direction * (tr.Distance * 0.5f), tr.EndPos )
			.Ignore( player )
			.WorldOnly()
			.Radius( 8 )
			.Run();

		Pos = tr.EndPos;
		Rot = player.EyeRot;

		Viewer = null;

		ShowCorpse();
	}

	private void ShowCorpse()
	{
		if ( Player.Local is not BasePlayer player )
			return;

		if ( !player.Corpse.IsValid() || player.Corpse is not ModelEntity corpse )
			return;

		corpse.EnableDrawing = true;

		foreach ( var child in corpse.Children )
		{
			if ( child is ModelEntity e )
			{
				e.EnableDrawing = true;
			}
		}
	}

	public virtual Vector3 GetSpectatePoint()
	{
		if ( Player.Local is not BasePlayer player )
			return LastPos;

		if ( !player.Corpse.IsValid() || player.Corpse is not ModelEntity corpse )
			return player.GetBoneTransform( player.GetBoneIndex( "spine2" ) ).Pos;

		return corpse.GetBoneTransform( corpse.GetBoneIndex( "spine2" ) ).Pos;
	}

	public virtual Vector3 GetViewOffset()
	{
		if ( Player.Local is not BasePlayer player ) return Vector3.Zero;

		return player.EyeRot.Forward * -100 + Vector3.Up * 20;
	}
}
