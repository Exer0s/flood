using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

public partial class BaseTeam : Entity
{
	[Net] public string TeamName { get; set; }
	[Net] public FloodPlayer TeamOwner { get; set; }
	[Net] public int PlayerAmount { get { return Members.Count(); } }

	[Net] public IList<FloodPlayer> Members { get; set; } = new List<FloodPlayer>();

	public override void Spawn()
	{
		base.Spawn();
		Transmit = TransmitType.Always;
	}

	public void InitTeam(FloodPlayer owner, Client ownerclient)
	{
		TeamName = $"{ownerclient.Name}'s Team";
		TeamOwner = owner;
		Members.Add( owner );
		Log.Info( $"Initialized Team {TeamName}" );
	}

	[ServerCmd]
	public static void JoinTeam(string teamname, string name)
	{
		var joiningteam = All.OfType<BaseTeam>().Where( x => x.TeamName == teamname ).FirstOrDefault();
		var player = All.OfType<FloodPlayer>().Where(x => x.Name == name).FirstOrDefault();
		if (player.Team.Members.Count == 1)
		{
			player.Team.TeamOwner = null;
			player.Team.Members.Clear();
		} else
		{
			if (player.Team.TeamOwner == player) player.Team.TeamOwner = player.Team.Members.FirstOrDefault();
			player.Team.Members.Remove( player );
		}

		player.Team = joiningteam;
		player.Team.Members.Add( player );
		player.ShowYourTeam();
	}

	[ServerCmd]
	public static void LeaveTeam( string teamname, string name )
	{
		var leavingteam = All.OfType<BaseTeam>().Where( x => x.TeamName == teamname ).FirstOrDefault();
		var player = All.OfType<FloodPlayer>().Where( x => x.Name == name ).FirstOrDefault();
		player.Team = player.LocalTeam;
		player.Team.TeamOwner = player;
		player.Team.Members.Add(player);
		leavingteam.Members.Remove( player );
		player.ShowJoinTeams();
	}

	public void UpdateName(string name)
	{
		TeamName = name;
	}

}
