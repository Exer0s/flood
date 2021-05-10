﻿using Sandbox;
using System;
using System.Linq;

partial class FloodViewModel : BaseViewModel
{
	float walkBob = 0;

	public override void UpdateCamera( Camera camera )
	{
		base.UpdateCamera( camera );

		camera.ViewModelFieldOfView = camera.FieldOfView + (FieldOfView - 80);

		AddCameraEffects( camera );
	}

	private void AddCameraEffects( Camera camera )
	{

		WorldRot = Player.Local.EyeRot;

		//
		// Bob up and down based on our walk movement
		//
		var speed = Owner.Velocity.Length.LerpInverse( 0, 320 );
		var left = camera.Rot.Left;
		var up = camera.Rot.Up;

		if ( Owner.GroundEntity != null )
		{
			walkBob += Time.Delta * 25.0f * speed;
		}

		WorldPos += up * MathF.Sin( walkBob ) * speed * -1;
		WorldPos += left * MathF.Sin( walkBob * 0.6f ) * speed * -0.5f;

	}
}
