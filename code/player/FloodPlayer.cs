﻿using Sandbox;
using System.Collections.Generic;


public partial class FloodPlayer : Player
{
	[Net]
	public float Money { get; set; } = 5000f;
	[Net]
	public float maxHealth { get; set; } = 100f;

	private TimeSince timeSinceDropped;
	private TimeSince timeSinceJumpReleased;

	private DamageInfo lastDamage;

	[Net] public IList<string> PurchasedWeapons { get; set; } = new List<string>();

	public IDictionary<string, Weapon> ServerPurchasedWeapons { get; set; } = new Dictionary<string, Weapon>();

	[Net] public BaseTeam Team { get; set; }
	[Net] public BaseTeam LocalTeam { get; set; }

	[Net] public Dictionary<FloodProp, float> SpawnedProps { get; set; } = new Dictionary<FloodProp, float>();

	/// <summary>
	/// The clothing container is what dresses the citizen
	/// </summary>
	public ClothingContainer Clothing = new();

	/// <summary>
	/// Default init
	/// </summary>
	public FloodPlayer()
	{
		Inventory = new Inventory( this );
	}

	/// <summary>
	/// Initialize using this client
	/// </summary>
	public FloodPlayer( Client cl ) : this()
	{
		// Load clothing from client data
		Clothing.LoadFromClient( cl );
	}

	[Net] public bool Spectating { get; set; }


	public override void Respawn()
	{
		if ( !Tags.Has( "player" ) ) Tags.Add( "player" );
		if ( FloodGame.Instance.GameRound is not WaitingRound && FloodGame.Instance.GameRound is not BuildingRound )
		{
			Controller = new FlyingController();
			CameraMode = new FirstPersonCamera();
			EnableAllCollisions = false;
			EnableDrawing = false;
			EnableShadowInFirstPerson = false;
			Spectating = true;
			base.Respawn();
			return;
		}

		Spectating = false;
		SetModel( "models/citizen/citizen.vmdl" );

		Controller = new FloodWalkController();
		Animator = new StandardPlayerAnimator();

		if ( DevController is NoclipController )
		{
			DevController = null;
		}

		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		Clothing.DressEntity( this );

		CameraMode = new FirstPersonCamera();
		Inventory.Add( new PhysGun(), true );
		Inventory.Add( new Tool(), false );
		base.Respawn();
	}

	public override void OnKilled()
	{
		base.OnKilled();

		if ( lastDamage.Flags.HasFlag( DamageFlags.Vehicle ) )
		{
			Particles.Create( "particles/impact.flesh.bloodpuff-big.vpcf", lastDamage.Position );
			Particles.Create( "particles/impact.flesh-big.vpcf", lastDamage.Position );
			PlaySound( "kersplat" );
		}

		if (IsServer)
		{
			FloodGame.Instance.CheckAliveTeams();
		}


		if (!Spectating)
		{
			BecomeRagdollOnClient( Velocity, lastDamage.Flags, lastDamage.Position, lastDamage.Force, lastDamage.BoneIndex );

			Controller = null;

			EnableAllCollisions = false;
			EnableDrawing = false;

			CameraMode = new SpectateRagdollCamera();

			foreach ( var child in Children )
			{
				child.EnableDrawing = false;
			}

			Inventory.DropActive();
			Inventory.DeleteContents();
		}
		
	}

	public override void TakeDamage( DamageInfo info )
	{
		if ( info.Hitbox.HasTag("head") )
		{
			info.Damage *= 10.0f;
		}

		lastDamage = info;

		TookDamage( lastDamage.Flags, lastDamage.Position, lastDamage.Force );

		base.TakeDamage( info );
	}


	public void GetShot(DamageInfo info)
	{
		var attacker = info.Attacker as FloodPlayer;

		if ( IsServer )
		{
			attacker.ShowPlayerHitmarker(To.Single(attacker));
		}
		else
		{
			attacker.ShowPlayerHitmarker(To.Single(attacker));
		}
		

	}


	[ClientRpc]
	public void TookDamage( DamageFlags damageFlags, Vector3 forcePos, Vector3 force )
	{
	}

	public override PawnController GetActiveController()
	{
		if ( DevController != null ) return DevController;

		return base.GetActiveController();
	}

	[ClientRpc]
	public void RefreshTeamPanel()
	{
		TeamsTab.Instance.RefreshJoinPanel();
	}

	[ClientRpc]
	public void ShowJoinTeams()
	{
		TeamsTab.Instance.ShowJoinPanel();
	}

	[ClientRpc]
	public void ShowYourTeam()
	{
		TeamsTab.Instance.ShowYourPanel();
	}

	public override void BuildInput( InputBuilder input )
	{
		if ( Input.Pressed( InputButton.Walk ) )
		{
			ConsoleSystem.Run( "undo" );
		}

		if ( Input.Pressed( InputButton.Chat ) )
		{
			FloodChat.Current.Open();
		}
		
		base.BuildInput( input );
	}

	public bool InDoor = false;
	public float DoorHeight;

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		if ( LocalTeam == null && Client != null && IsServer )
		{
			var team = new BaseTeam();
			team.InitTeam( this, Client );
			Team = team;
			LocalTeam = team;
		}

		if ( Input.ActiveChild != null )
		{
			ActiveChild = Input.ActiveChild;
		}

		if ( LifeState != LifeState.Alive )
			return;

		var controller = GetActiveController();
		if ( controller != null )
			EnableSolidCollisions = !controller.HasTag( "noclip" );

		TickPlayerUse();
		SimulateActiveChild( cl, ActiveChild );

		if ( IsClient ) CheckLookingProp();

		if ( Input.Pressed( InputButton.View ) )
		{
			if ( Spectating ) return;
			if ( CameraMode is ThirdPersonCamera )
			{
				CameraMode = new FirstPersonCamera();
			}
			else
			{
				CameraMode = new ThirdPersonCamera();
			}
		}

		if ( Input.Pressed( InputButton.Drop ) )
		{
			var dropped = Inventory.DropActive();
			if ( dropped != null )
			{
				dropped.PhysicsGroup.ApplyImpulse( Velocity + EyeRotation.Forward * 500.0f + Vector3.Up * 100.0f, true );
				dropped.PhysicsGroup.ApplyAngularImpulse( Vector3.Random * 100.0f, true );

				timeSinceDropped = 0;
			}
		}

		if ( Input.Released( InputButton.Jump ) )
		{
			if ( timeSinceJumpReleased < 0.3f )
			{
				Game.Current?.DoPlayerNoclip( cl );
			}

			timeSinceJumpReleased = 0;
		}

		if ( Input.Left != 0 || Input.Forward != 0 )
		{
			timeSinceJumpReleased = 1;
		}

		if (Team != null && Team.ClaimedDoor != null)
		{
			var controller2 = Controller as FloodWalkController;
			if ( WorldSpaceBounds.Overlaps(Team.ClaimedDoor.WorldSpaceBounds) )
			{
				controller2.Gravity = 0;
			} else
			{
				controller2.Gravity = 800f;
			}
		}
		

	}

	public override void StartTouch( Entity other )
	{
		if ( timeSinceDropped < 1 ) return;

		base.StartTouch( other );
	}

	[ConCmd.Server( "inventory_current" )]
	public static void SetInventoryCurrent( string entName )
	{
		var target = ConsoleSystem.Caller.Pawn as Player;
		if ( target == null ) return;

		var inventory = target.Inventory;
		if ( inventory == null )
			return;

		for ( int i = 0; i < inventory.Count(); ++i )
		{
			var slot = inventory.GetSlot( i );
			if ( !slot.IsValid() )
				continue;

			if (slot.ClassName != entName )
				continue;

			inventory.SetActiveSlot( i, false );

			break;
		}
	}
	[ConCmd.Server]
	public void GivePurchasedWeapons()
	{
		foreach ( var weapon in PurchasedWeapons )
		{
			Inventory.Add(TypeLibrary.Create<Weapon>(weapon), true);
		}
	}
	[ConCmd.Server]
	public void RemoveWeapons()
	{
		Inventory.DeleteContents();
	}
	
	[ClientRpc]
	public void ShowHitmarker(float dmg)
	{
		FloodCrossPanel.Instance.OnHit( dmg );
	}

	[ClientRpc]
	public void ShowPlayerHitmarker()
	{
		FloodCrossPanel.Instance.OnHitPlayer();
	}

	public void DidDamage( DamageInfo info )
	{
		if ( IsServer )
		{
			ShowHitmarker( To.Single(this), info.Damage);
		}
		else
		{
			ShowHitmarker(info.Damage);
		}
		
	}

	public void DestroyedProp(float payout)
	{
		//notification/something with money UI
		Money += payout;
	}

	//generic pay func that calls noti UI and stuff
	public void Pay(float amount)
	{
		Money += amount;
		//UI shit 
	}


}
