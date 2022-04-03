using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

public class BaseTeam : BaseNetworkable
{
	public string TeamName { get; set; }
	public FloodPlayer TeamOwner { get; set; }
	public int PlayerAmount { get { return Members.Count(); } }

	[Net] public static List<BaseTeam> AllTeams { get; set; } = new List<BaseTeam>();

	public List<FloodPlayer> Members = new List<FloodPlayer>();

	public void InitTeam(FloodPlayer owner, Client ownerclient)
	{
		TeamName = $"{ownerclient.Name}'s Team";
		TeamOwner = owner;
		Members.Add( owner );
		Log.Info( $"Initialized Team {TeamName}" );
		AllTeams.Add( this );
	}

	public void JoinTeam(FloodPlayer player)
	{
		player.Team.TeamOwner = null;
		player.Team.Members.Clear();
		player.Team = this;
		Members.Add( player );
	}

	public void LeaveTeam( FloodPlayer player )
	{
		player.Team = player.LocalTeam;
		player.Team.TeamOwner = player;
		player.Team.Members.Add(player);
		Members.Remove( player );
	}

	public void UpdateName(string name)
	{
		TeamName = name;
	}

}
