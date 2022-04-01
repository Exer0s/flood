using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

[Library("flood_door")]
public class FloodDoor : ModelEntity
{
	public override void Spawn()
	{
		SetupPhysicsFromModel( PhysicsMotionType.Static );
		base.Spawn();
	}




}
