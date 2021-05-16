using System.Linq;
using System.Collections.Generic;

namespace Sandbox
{
	[Library( "env_sea" )]
	public partial class WaterSea : Water
	{
		public WaterSea()
		{
			Transmit = TransmitType.Always;

			if ( IsClient )
			{
				Host.AssertClient();
				MakeSeaMesh();
				CreatePhysics();
				SceneLayer = "water";
			}
		}

		public override void Spawn()
		{
			base.Spawn();

			CreatePhysics();
		}
		public float waterHeight;
		public void CreatePhysics()
		{
			var PhysGroup = SetupPhysicsFromAABB( PhysicsMotionType.Static, new Vector3( -10000, -10000, 1 * waterHeight ), new Vector3( 10000, 10000, 0) );
			PhysGroup.SetSurface( "water" );
			ClearCollisionLayers();
			AddCollisionLayer( CollisionLayer.Water );
			AddCollisionLayer( CollisionLayer.Trigger );
			EnableSolidCollisions = false;
			EnableTouch = true;
			EnableTouchPersists = true;
			//Log.Info( "Updated physics info" );
		}



		public void MakeSeaMesh()
		{
			

			const int seaSize = 65535; //Make it big even though it's size is calculated on vertex shader, this makes Vis calculation aware of it
										//Todo: sea tesselation size should be proportional to the resolution with a divider on quality and a ceiling
			int[] screenResolution = new[] { 1920, 1080 };
			int seaLod = 10;

			MakeTesselatedPlane( screenResolution[0] / seaLod, screenResolution[1] / seaLod, seaSize, Material.Load( "materials/shadertest/test_water.vmat" ) );
		}


		
		public void MakeTesselatedPlane( int xRes, int yRes, int size, Material material )
		{
			var vertices = new List<Vertex>();
			var indices = new List<int>();

			for ( int x = 0; x <= xRes; x++ )
			{
				for ( int y = 0; y <= yRes; y++ )
				{
					var uv = new Vector2( x / (float)xRes, y / (float)yRes );
					var pos = uv - new Vector2( 0.5f, 0.5f );
					vertices.Add( new Vertex( new Vector3( pos.x * size, pos.y * size, 1 * waterHeight ), Vector3.Down, Vector3.Right, uv ) );
				}
			}

			for ( int y = 0; y < yRes; y++ )
			{
				for ( int x = 0; x < xRes; x++ )
				{
					var i = y + (x * yRes);

					indices.Add( i + yRes + 1 );
					indices.Add( i + 1 );
					indices.Add( i );

					indices.Add( i + 1 );
					indices.Add( i + yRes + 1 );
					indices.Add( i + yRes + 2 );
				}
			}

			var model = Model.Create( $"TesselatedPlane-{NetworkIdent}-{waterHeight}.vmdl", material, vertices.ToArray(), indices.ToArray() );
			SetModel( model );
			//Log.Info( "Updated sea mesh + " + waterHeight.ToString() );
		}

	}
}
