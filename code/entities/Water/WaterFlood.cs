namespace Sandbox
{
	/// <summary>
	/// Simple water effect
	/// </summary>
	[Library( "env_flood" )]
	[Hammer.EditorModel( "models/hammer/env_sea.vmdl" )]
	public partial class WaterFlood : Water
	{
		public override void Spawn()
		{
			base.Spawn();

			Transmit = TransmitType.Always;
			CreatePhysics();
		}

		public override void ClientSpawn()
		{
			Host.AssertClient();

			base.ClientSpawn();

			MakeSeaMesh();
			CreatePhysics();
			//SceneLayer = "water";
		}

		void CreatePhysics()
		{
			SetInteractsExclude( CollisionLayer.STATIC_LEVEL );

			var physicsGroup = SetupPhysicsFromAABB( PhysicsMotionType.Static, new Vector3( -10000, -10000, -1000 ), new Vector3( 10000, 10000, 0 ) );
			physicsGroup.SetSurface( "water" );

			ClearCollisionLayers();
			AddCollisionLayer( CollisionLayer.Water );
			AddCollisionLayer( CollisionLayer.Trigger );

			EnableSolidCollisions = false;
			EnableTouch = true;
			EnableTouchPersists = true;
		}

		public void MakeSeaMesh()
		{
			Host.AssertClient();

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

			var mesh = new Mesh( material );
			mesh.CreateBuffers( vb );
			mesh.SetBounds( new Vector3( -10000, -10000, -1000 ), new Vector3( 10000, 10000, 0 ) );

			var model = Model.Builder
				.AddMesh( mesh )
				.Create();

			SetModel( model );
		}
	}
}
