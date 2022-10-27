using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.Component;
using SandboxEditor;

[Spawnable]
[HammerEntity]
public partial class FloodDoor: ModelEntity, IUse
{

	public Glow glow;
	public DoorPanel panel;
	[Net] public BaseTeam OwningTeam { get; set; }

	[Property( "position_offset", "UI Position Offset", "Offset the UI Position" )]
	[Net] public Vector3 PositionOffset { get; set; }
	[Property( "rotation_offset", "UI Rotation Offset", "Offset the UI rotation (pitch, yaw, roll)" )]
	[Net] public Vector3 RotationOffset { get; set; }
	[Property( "ui_scale", "UI Scale", "Scale of the UI" )]
	[Net] public float UIScale { get; set; } = 4f;

	public bool IsUsable( Entity user )
	{
		if ( user is FloodPlayer player )
		{
			if ( OwningTeam == null && player.Team.ClaimedDoor == null )
			{
				return true;
			}
			else return false;
		}
		else return false;
	}

	public bool OnUse( Entity user )
	{
		var player = user as FloodPlayer;
		player.Team.ClaimedDoor = this;
		OwningTeam = player.Team;
		Tags.Add( player.Team.TeamTag );
		glow.Color = Color.Red;
		SetPanelInfo();
		return false;
	}

	public override void Spawn()
	{
		SetupPhysicsFromModel( PhysicsMotionType.Static );
		glow = Components.GetOrCreate<Glow>();
		glow.Enabled = true;
		glow.Color = Color.Green;
		
		EnableTraceAndQueries = true;
		base.Spawn();
	}

	public override void ClientSpawn()
	{
		if ( !IsClient ) return;
		panel = new DoorPanel();
		panel.Position = Position + PositionOffset;
		panel.WorldScale = UIScale;
		panel.Rotation = Rotation;
		if ( RotationOffset != Vector3.Zero)
		panel.Rotation += Rotation.From( RotationOffset.x, RotationOffset.y, RotationOffset.z );
		//panel.Position *= Vector3.Up * 25f;
		base.ClientSpawn();
	}

	protected override void OnDestroy()
	{
		DestroyUI();
		base.OnDestroy();
	}

	[ClientRpc]
	public void DestroyUI()
	{
		panel.Delete();
	}



	[ClientRpc]
	public void SetPanelInfo()
	{
		panel.SetTeamInfo(OwningTeam);
	}

	[ClientRpc]
	public void ResetPanelInfo()
	{
		panel.ResetTeamInfo();
	}

	public void ResetDoor()
	{
		Tags.Remove( OwningTeam.TeamTag );
		OwningTeam.ClaimedDoor = null;
		OwningTeam = null;
		glow.Color = Color.Green;
		ResetPanelInfo();
	}

}
