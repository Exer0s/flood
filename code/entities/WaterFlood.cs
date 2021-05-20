namespace Sandbox
{
	[Library( "env_flood" )]
	public partial class WaterFlood : Water
	{
		public WaterFlood()
		{
			Transmit = TransmitType.Always;

			if ( IsClient )
			{
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
		
		void CreatePhysics()
		{
			var PhysGroup = SetupPhysicsFromAABB( PhysicsMotionType.Static, new Vector3( -10000, -10000, -1000 ), new Vector3( 10000, 10000, 0 ) );
			PhysGroup.SetSurface( "water" );

			ClearCollisionLayers();
			AddCollisionLayer( CollisionLayer.Water );
			AddCollisionLayer( CollisionLayer.Trigger );
			EnableSolidCollisions = false;
			EnableTouch = true;
			EnableTouchPersists = true;
		}

		public void MakeSeaMesh()
		{
			if ( IsClient )
			{
				Host.AssertClient();
			}

			//waterLevel += 10;
			const int seaSize = 65535; //Make it big even though it's size is calculated on vertex shader, this makes Vis calculation aware of it
									   //Todo: sea tesselation size should be proportional to the resolution with a divider on quality and a ceiling
			int[] screenResolution = new[] { 1920, 1080 };
			int seaLod = 10;

			MakeTesselatedPlane( screenResolution[0] / seaLod, screenResolution[1] / seaLod, seaSize, Material.Load( "materials/shadertest/test_water.vmat" ) );
		}

		public void MakeTesselatedPlane( int xRes, int yRes, int size, Material material )
		{
			var vb = new VertexBuffer();
			vb.Init( true );

			for ( int x = 0; x <= xRes; x++ )
			{
				for ( int y = 0; y <= yRes; y++ )
				{
					var uv = new Vector2( x / (float)xRes, y / (float)yRes );
					var pos = uv - new Vector2( 0.5f, 0.5f );
					vb.Add( new Vertex( new Vector3( pos.x * size, pos.y * size, 0 ), Vector3.Down, Vector3.Right, uv ) );
				}
			}

			for ( int y = 0; y < yRes; y++ )
			{
				for ( int x = 0; x < xRes; x++ )
				{
					var i = y + (x * yRes);

					vb.AddRawIndex( i + yRes + 1 );
					vb.AddRawIndex( i + 1 );
					vb.AddRawIndex( i );

					vb.AddRawIndex( i + 1 );
					vb.AddRawIndex( i + yRes + 1 );
					vb.AddRawIndex( i + yRes + 2 );
				}
			}

			var model = vb.CreateModel( $"TesselatedPlane-{NetworkIdent}.vmdl", material );
			SetModel( model );
			//Log.Info( "Updated mesh" );
		}
	}
}

