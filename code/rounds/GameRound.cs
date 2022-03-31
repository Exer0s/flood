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

	public virtual string[] Weapons { get; set; } = new string[5];

	public virtual void OnRoundStart(bool startup)
	{
		if (!startup) 
		GiveWeapons();
		Log.Info( $"Starting Round {RoundName}" );
	}

	public virtual void OnRoundEnd()
	{
		Log.Info( $"Ending Round {RoundName}" );
	}

	private void GiveWeapons()
	{
		if ( Entity.All.OfType<FloodPlayer>().Count() <= 0 ) return;
		foreach ( var player in Entity.All.OfType<FloodPlayer>() )
		{
			if ( player.Inventory != null )
			{
				player.Inventory.DeleteContents();
				foreach ( var weapon in Weapons )
				{
					if ( weapon == null ) return;
					player.Inventory.Add( Library.Create<Carriable>( weapon ) );
				}
			}
		}
	}



}
