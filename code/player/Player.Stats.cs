using Sandbox;

partial class FloodPlayer
{
	[Net] public int PositionPlaced { get; set; }
	[Net] public float DamageDealt { get; set; }
	[Net] public int PropsBroken { get; set; }
}
