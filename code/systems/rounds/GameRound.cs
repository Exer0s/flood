using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

public partial class GameRound : BaseNetworkable
{
	public virtual string RoundName { get; set; }
	public virtual float RoundDuration { get; set; }
	public virtual string NextRound { get; set; }
	public virtual float RoundEndTime { get; set; }

	public virtual string[] Weapons { get; set; } = new string[5];

	public virtual void OnRoundStart()
	{
		RoundEndTime = Time.Now + RoundDuration;
		Log.Info( $"Starting Round {RoundName}" );
	}

	public virtual void OnRoundEnd()
	{
		Log.Info( $"Ending Round {RoundName}" );
	}

	public virtual void RoundTick()
	{

	}



}
