using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using SandboxEditor;

[HammerEntity]
[Library("flood_level_manager")]
public partial class FloodLevelManager : Entity
{

	public static FloodLevelManager Instance;

	public override void Spawn()
	{
		Transmit = TransmitType.Always;
		Instance = this;
		base.Spawn();
	}

	[Property( "water_height", "Water Height", "How much the water should rise when flooding" )]
	public float WaterHeight { get; set; }
	[Property( "flood_speed", "Flood Speed", "How fast should the water rise" )]
	public float RiseSpeed { get; set; }
	[Property( "flood_time", "Flooding Time", "How long should the flooding/draining rounds be" )]
	public float FloodTime { get; set; }

	public Output OnBuildStart { get; set; }
	public Output OnBuildEnd { get; set; }
	public Output OnFloodStart { get; set; }
	public Output OnFloodEnd { get; set; }
	public Output OnFightStart { get; set; }
	public Output OnFightEnd { get; set; }
	public Output OnDrainStart { get; set; }
	public Output OnDrainEnd { get; set; }
}
