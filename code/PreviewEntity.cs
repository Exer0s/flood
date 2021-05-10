
namespace Sandbox.Tools
{
	public class PreviewEntity : ModelEntity
	{
		public bool RelativeToNormal { get; set; } = true;
		public bool OffsetBounds { get; set; } = false;
		public Rotation RotationOffset { get; set; } = Rotation.Identity;
		public Vector3 PositionOffset { get; set; } = Vector3.Zero;

		internal bool UpdateFromTrace( TraceResult tr )
		{
			if ( !IsTraceValid( tr ) )
			{
				return false;
			}

			if ( RelativeToNormal )
			{
				WorldRot = Rotation.LookAt( tr.Normal, tr.Direction ) * RotationOffset;
				WorldPos = tr.EndPos + WorldRot * PositionOffset;
			}
			else
			{
				WorldRot = Rotation.Identity * RotationOffset;
				WorldPos = tr.EndPos + PositionOffset;
			}

			if ( OffsetBounds )
			{
				WorldPos += tr.Normal * CollisionBounds.Size * 0.5f;
			}

			return true;
		}

		protected virtual bool IsTraceValid( TraceResult tr ) => tr.Hit;
	}
}
