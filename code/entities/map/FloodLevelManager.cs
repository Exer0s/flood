using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

[Library("flood_level_manager")]
public class FloodLevelManager : Entity
{

	public FloodLevelManager()
	{
		Transmit = TransmitType.Always;
	}

	[Property( "water_height", "Water Height", "How much the water should rise when flooding" )]
	public float WaterHeight { get; set; }
	[Property( "flood_speed", "Flood Speed", "How fast should the water rise" )]
	public float RiseSpeed { get; set; }
	[Property( "flood_time", "Flooding Time", "How long should the flooding/draining rounds be" )]
	public float FloodTime { get; set; }

}
