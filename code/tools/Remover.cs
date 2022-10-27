namespace Sandbox.Tools
{
	public partial class RemoverTool : BaseTool
	{
		public override void Simulate()
		{
			if ( !Host.IsServer )
				return;

			using ( Prediction.Off() )
			{
				if ( !Input.Pressed( InputButton.PrimaryAttack ) )
					return;

				var startPos = Owner.EyePosition;
				var dir = Owner.EyeRotation.Forward;

				var tr = Trace.Ray( startPos, startPos + dir * MaxTraceDistance )
					.Ignore( Owner )
					.WithTag( "prop" )
					.Run();

				if ( !tr.Hit || !tr.Entity.IsValid() )
					return;

				if ( !CanManipulate( tr.Entity, Owner as FloodPlayer ) ) return;

				if ( tr.Entity is Player )
					return;

				CreateHitEffects( tr.EndPosition );

				if ( tr.Entity.IsWorld )
					return;

				tr.Entity.Delete();

				var particle = Particles.Create( "particles/physgun_freeze.vpcf" );
				particle.SetPosition( 0, tr.Entity.Position );
			}
		}
	}
}
