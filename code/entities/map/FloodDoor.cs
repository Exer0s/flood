using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.Component;

[Library("flood_door")]
public partial class FloodDoor: ModelEntity, IUse
{

	public Glow glow;

	[Net] public BaseTeam OwningTeam { get; set; }

	public bool IsUsable( Entity user )
	{
		if ( user is FloodPlayer player )
		{
			if ( OwningTeam == null )
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
		OwningTeam = player.Team;
		Tags.Add( player.Team.TeamTag );
		glow.Color = Color.Red;
		return false;
	}

	public override void Spawn()
	{
		SetupPhysicsFromModel( PhysicsMotionType.Static );
		glow = Components.GetOrCreate<Glow>();
		glow.Active = true;
		glow.Enabled = true;
		glow.Color = Color.Green;
		SetInteractsExclude( CollisionLayer.Player );
		EnableTraceAndQueries = true;
		base.Spawn();
	}
}
