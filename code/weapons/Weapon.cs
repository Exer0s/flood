using Sandbox;

public partial class Weapon : BaseWeapon, IUse
{
	public virtual float ReloadTime => 3.0f;
	public virtual string Icon { get; set; }

	public PickupTrigger PickupTrigger { get; protected set; }

	[Net, Predicted]
	public TimeSince TimeSinceReload { get; set; }

	[Net, Predicted]
	public bool IsReloading { get; set; }

	[Net, Predicted]
	public TimeSince TimeSinceDeployed { get; set; }

	[Net]
	public int CurrentClip { get; set; }

	public virtual int ClipSize { get; set; }
	public virtual float BloomAmount { get; set; }
	public virtual float SecondaryBloomAmount { get; set; }
	public virtual float SecondaryAmmoTaken { get; set; }


	public override void Spawn()
	{
		base.Spawn();

		PickupTrigger = new PickupTrigger
		{
			Parent = this,
			Position = Position,
			EnableTouch = true,
			EnableSelfCollisions = false
		};

		PickupTrigger.PhysicsBody.AutoSleep = false;

		CurrentClip = ClipSize;

	}

	public override bool CanPrimaryAttack()
	{
		if ( CurrentClip <= 0 ) return false;
		return base.CanPrimaryAttack();
	}
	
	[ClientRpc]
	public void CrosshairBloom(float bloomamount)
	{
		Log.Info( "bloom" + bloomamount );
		if ( FloodCrossPanel.Instance.CircleSpread >= 3 ) FloodCrossPanel.Instance.CircleSpread += bloomamount / 3;
		else FloodCrossPanel.Instance.CircleSpread += bloomamount;
	}

	public override void AttackPrimary()
	{
		if ( IsServer )
		{
			CrosshairBloom(To.Single( Owner ), BloomAmount);
			CurrentClip--;
		}
		
		base.AttackPrimary();
	}

	public override void AttackSecondary()
	{
		if ( IsServer )
		{
			CrosshairBloom(To.Single( Owner ), SecondaryBloomAmount);
			CurrentClip -= SecondaryAmmoTaken.CeilToInt();
		}
		
		base.AttackSecondary();
	}

	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart( ent );

		TimeSinceDeployed = 0;
	}

	public override void Reload()
	{
		if ( IsReloading )
			return;

		if (CurrentClip >= ClipSize ) return;

		TimeSinceReload = 0;
		IsReloading = true;

		(Owner as AnimatedEntity)?.SetAnimParameter( "b_reload", true );

		StartReloadEffects();
	}

	public override void Simulate( Client owner )
	{
		if ( TimeSinceDeployed < 0.6f )
			return;

		if ( !IsReloading )
		{
			base.Simulate( owner );
		}

		if ( IsReloading && TimeSinceReload > ReloadTime )
		{
			OnReloadFinish();
		}
	}

	public virtual void OnReloadFinish()
	{
		IsReloading = false;
		CurrentClip = ClipSize;
	}

	[ClientRpc]
	public virtual void StartReloadEffects()
	{
		ViewModelEntity?.SetAnimParameter( "reload", true );

		// TODO - player third person model reload
	}

	public override void CreateViewModel()
	{
		Host.AssertClient();

		if ( string.IsNullOrEmpty( ViewModelPath ) )
			return;

		ViewModelEntity = new ViewModel
		{
			Position = Position,
			Owner = Owner,
			EnableViewmodelRendering = true
		};

		ViewModelEntity.SetModel( ViewModelPath );
	}

	public bool OnUse( Entity user )
	{
		if ( Owner != null )
			return false;

		if ( !user.IsValid() )
			return false;

		user.StartTouch( this );

		return false;
	}

	public virtual bool IsUsable( Entity user )
	{
		var player = user as Player;
		if ( Owner != null ) return false;

		if ( player.Inventory is Inventory inventory )
		{
			return inventory.CanAdd( this );
		}

		return true;
	}

	public void Remove()
	{
		Delete();
	}

	[ClientRpc]
	protected virtual void ShootEffects()
	{
		if ( CurrentClip <= 0 ) return;
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );

		if ( IsLocalPawn )
		{
			//_ = new Sandbox.ScreenShake.Perlin();
		}

		ViewModelEntity?.SetAnimParameter( "fire", true );
		//CrosshairPanel?.CreateEvent( "fire" );
	}

	/// <summary>
	/// Shoot a single bullet
	/// </summary>
	public virtual void ShootBullet( Vector3 pos, Vector3 dir, float spread, float force, float damage, float bulletSize )
	{
		if ( CurrentClip <= 0 ) return;
		var forward = dir;
		forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
		forward = forward.Normal;

		//
		// ShootBullet is coded in a way where we can have bullets pass through shit
		// or bounce off shit, in which case it'll return multiple results
		//
		foreach ( var tr in TraceBullet( pos, pos + forward * 5000, bulletSize ) )
		{
			tr.Surface.DoBulletImpact( tr );

			if ( !IsServer ) continue;
			if ( !tr.Entity.IsValid() ) continue;

			//
			// We turn predictiuon off for this, so any exploding effects don't get culled etc
			//
			using ( Prediction.Off() )
			{
				var damageInfo = DamageInfo.FromBullet( tr.EndPosition, forward * 100 * force, damage )
					.UsingTraceResult( tr )
					.WithAttacker( Owner )
					.WithWeapon( this );
				if (tr.Entity is FloodPlayer player)
				{
					player.GetShot(damageInfo);
					
				} else 
				tr.Entity.TakeDamage( damageInfo );
			}
		}
	}



	/// <summary>
	/// Shoot a single bullet from owners view point
	/// </summary>
	public virtual void ShootBullet( float spread, float force, float damage, float bulletSize )
	{
		if ( CurrentClip <= 0 ) return;
		ShootBullet( Owner.EyePosition, Owner.EyeRotation.Forward, spread, force, damage, bulletSize );
	}

	/// <summary>
	/// Shoot a multiple bullets from owners view point
	/// </summary>
	public virtual void ShootBullets( int numBullets, float spread, float force, float damage, float bulletSize )
	{
		if ( CurrentClip <= 0 ) return;
		var pos = Owner.EyePosition;
		var dir = Owner.EyeRotation.Forward;

		for ( int i = 0; i < numBullets; i++ )
		{
			ShootBullet( pos, dir, spread, force / numBullets, damage, bulletSize );
		}
	}
}
