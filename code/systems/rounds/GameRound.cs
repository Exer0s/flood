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

	public virtual bool Tools { get; set; } = false;

	public virtual void OnRoundStart()
	{

		RoundEndTime = Time.Now + RoundDuration;
		Log.Info( $"Starting Round {RoundName}" );

		foreach ( var player in Entity.All.OfType<FloodPlayer>().ToArray() )
		{
			if ( Tools )
			{
				player.Inventory.Add( new PhysGun(), true );
				player.Inventory.Add( new Tool(), false );
			}
		}
	}

	public virtual void OnRoundEnd()
	{
		if (Tools)
		{
			foreach ( var player in Entity.All.OfType<FloodPlayer>().ToArray() )
			{
				player.Inventory.DeleteContents();
			}
		}
		
		Log.Info( $"Ending Round {RoundName}" );
	}

	public virtual void RoundTick()
	{

	}



}
