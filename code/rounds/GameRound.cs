using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

public class GameRound : BaseNetworkable
{
	public virtual string RoundName { get; set; }
	public virtual float RoundDuration { get; set; }
	public virtual string NextRound { get; set; }

	public virtual void OnRoundStart()
	{
		Log.Info( $"Starting Round {RoundName}" );
	}

	public virtual void OnRoundEnd()
	{
		Log.Info( $"Ending Round {RoundName}" );
	}

}
