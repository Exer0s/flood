﻿namespace Sandbox.Tools
{
	[Library( "tool_boxgun", Title = "Box Shooter", Description = "Shoot boxes", Group = "fun" )]
	public class BoxShooter : BaseTool
	{
		TimeSince timeSinceShoot;

		string modelToShoot = "models/citizen_props/crate01.vmdl";

		public override void Simulate()
		{
			if ( Host.IsServer )
			{
				if ( Input.Pressed( InputButton.Reload ) )
				{
					var tr = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * 4000 ).Ignore( Owner ).Run();

					if ( tr.Entity is ModelEntity ent && !string.IsNullOrEmpty( ent.GetModelName() ) )
					{
						modelToShoot = ent.GetModelName();
						Log.Trace( $"Shooting model: {modelToShoot}" );
					}
				}

				if ( Input.Pressed( InputButton.Attack1 ) )
				{
					ShootBox();
				}

				if ( Input.Down( InputButton.Attack2 ) && timeSinceShoot > 0.05f )
				{
					timeSinceShoot = 0;
					ShootBox();
				}
			}
		}

		void ShootBox()
		{
			var ent = new Prop
			{
				Position = Owner.EyePosition + Owner.EyeRotation.Forward * 50,
				Rotation = Owner.EyeRotation
			};

			ent.SetModel( modelToShoot );
			ent.Velocity = Owner.EyeRotation.Forward * 1000;
		}
	}
}
