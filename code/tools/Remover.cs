namespace Sandbox.Tools
{
	public interface IRemovable
	{
		void Remove();
	}

	[Library( "tool_remover", Title = "Remover", Description = "Remove entities", Group = "construction" )]
	public partial class RemoverTool : BaseTool
	{
		private Prop prop;
		private IRemovable removable;

		public override void OnPlayerControlTick()
		{
			if ( !Host.IsServer )
				return;

			using ( Prediction.Off() )
			{
				if ( prop.IsValid() )
				{
					prop.Delete();
					prop = null;

					return;
				}

				if ( this.removable != null )
				{
					this.removable.Remove();
					this.removable = null;

					return;
				}

				var input = Owner.Input;

				if ( !input.Pressed( InputButton.Attack1 ) )
					return;

				var startPos = Owner.EyePos;
				var dir = Owner.EyeRot.Forward;

				var tr = Trace.Ray( startPos, startPos + dir * MaxTraceDistance )
					.Ignore( Owner )
					.HitLayer( CollisionLayer.Debris )
					.Run();

				if ( !tr.Hit || !tr.Entity.IsValid() )
					return;

				CreateHitEffects( tr.EndPos );

				if ( tr.Entity.IsWorld )
					return;

				if ( tr.Entity is IRemovable removable )
				{
					this.removable = removable;
				}
				else if ( tr.Entity is Prop prop )
				{
					prop.PhysicsGroup?.Wake();
					this.prop = prop;
				}
				else
				{
					return;
				}

				var particle = Particles.Create( "particles/physgun_freeze.vpcf" );
				particle.SetPos( 0, tr.Entity.WorldPos );
			}
		}
	}
}
