using Steamworks.Data;
using System;

namespace Sandbox
{
	public class WaterController
	{
		public Entity WaterEntity { get; set; }

		public float WaterThickness = 80.0f;

		public TimeSince TimeSinceLastEffect = 0;

		public void StartTouch( Entity other )
		{
			other.WaterLevel.WaterEntity = WaterEntity;
		}

		public void EndTouch( Entity other )
		{
			other.WaterLevel.Fraction = 0.0f;

			if ( other.WaterLevel.WaterEntity != WaterEntity )
				return;

			other.WaterLevel.WaterEntity = null;
		}

		public void Touch( Entity other )
		{
			if ( other.WaterLevel.WaterEntity != WaterEntity )
				return;


			if ( other is ModelEntity me )
			{
				var bodyCount = other.PhysicsGroup.BodyCount;
				for ( int i = 0; i < bodyCount; i++ )
				{
					var body = me.PhysicsGroup.GetBody( i );
					UpdateBody( other, body );

					if ( bodyCount == 1 )
					{
						me.WaterLevel.Fraction = body.WaterLevel;
					}
				}
			}
		}
		public float levelModifier = 1f;
		public void UpdateBody( Entity ent, PhysicsBody body )
		{
			var waterDensity = 1000;

			var oldLevel = body.WaterLevel;
			var density = body.Density;
			var waterSurface = WaterEntity.WorldPos;
			var bounds = body.GetBounds();
			var velocity = body.Velocity;
			var pos = bounds.Center;
			pos.z = waterSurface.z;

			var densityDiff = density - waterDensity;
			var volume = bounds.Volume;
			var level = waterSurface.z.LerpInverse( bounds.Mins.z, bounds.Maxs.z, true );
			
			
			body.WaterLevel = level;

			if ( ent.IsClientOnly == Host.IsClient )
			{
				var bouyancy = densityDiff.LerpInverse( 0.0f, -300f );
				bouyancy = MathF.Pow( bouyancy, 0.1f );

			//	DebugOverlay.Text( pos, $"{bouyancy}", Host.Color, 0.1f, 10000 );

				if ( bouyancy <= 0 )
				{
					body.GravityScale = 1.0f - body.WaterLevel * 0.8f;
				}
				else
				{
					var point = bounds.Center;
					if ( level < 1.0f ) point.z =  bounds.Mins.z - 100;
					var closestpoint = body.FindClosestPoint( point );

					float depth = (waterSurface.z - bounds.Maxs.z) / 100.0f;
					depth = depth.Clamp( 1.0f, 10.0f );
					//DebugOverlay.Text( point, $"{depth}", Host.Color, 0.1f, 10000 );
					//DebugOverlay.Line( point, closestpoint, 1.0f );

					//body.ApplyImpulseAt( closestpoint, (Vector3.Up * volume * level * bouyancy * 0.0001f) );
					body.ApplyForceAt( closestpoint, (Vector3.Up * volume * level * bouyancy * 0.05f * depth) );

					//body.ApplyImpulseAt( )
					body.GravityScale = 1.0f - MathF.Pow( body.WaterLevel.Clamp( 0, 0.5f ) * 2.0f, 0.5f );
				}

				body.LinearDrag = body.WaterLevel * WaterThickness;
				body.AngularDrag = body.WaterLevel * WaterThickness * 0.5f;
			}

			if ( Host.IsClient )
			{
				if ( oldLevel == 0 )
					return;

				var change = MathF.Abs( oldLevel - level );

				//Log.Info( $"{change}" );

				if ( change > 0.001f && body.LastWaterEffect > 0.2f )
				{
					if ( oldLevel < 0.3f && level >= 0.35f )
					{
						var particle = Particles.Create( "particles/water_splash.vpcf", pos );
						particle.SetForward( 0, Vector3.Up );
						body.LastWaterEffect = 0;

						Sound.FromWorld( "water_splash_medium", pos ); 
					}

					if ( velocity.Length > 2f && velocity.z > -10 && velocity.z < 10 )
					{
						var particle = Particles.Create( "particles/water_bob.vpcf", pos );
						particle.SetForward( 0, Vector3.Up );
						body.LastWaterEffect = 0;
					}

				}
			}
		}
	}
}
