using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

public partial class BaseTeam : Entity
{
	[Net] public string TeamName { get; set; }
	[Net] public bool TeamLocked { get; set; } = false;
	[Net] public FloodPlayer TeamOwner { get; set; }
	[Net] public int PlayerAmount { get { return Members.Count(); } }

	[Net] public IList<FloodPlayer> Members { get; set; } = new List<FloodPlayer>();

	[Net] public string TeamTag { get; set; }
	[Net] public FloodDoor ClaimedDoor { get; set; }
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
		TeamTag = $"{ownerclient.Name}{All.OfType<BaseTeam>().Count()}";
		Log.Info( $"Initialized Team {TeamName} with tag {TeamTag}" );
	}

	[ConCmd.Server]
	public static void JoinTeam(string teamname, string name)
	{
		var joiningteam = All.OfType<BaseTeam>().Where( x => x.TeamName == teamname ).FirstOrDefault();
		if ( joiningteam.TeamLocked ) return;
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
		player.ShowYourTeam(To.Single(player));
	}

	[ConCmd.Server]
	public static void LeaveTeam( string teamname, string name )
	{
		var leavingteam = All.OfType<BaseTeam>().Where( x => x.TeamName == teamname ).FirstOrDefault();
		var player = All.OfType<FloodPlayer>().Where( x => x.Name == name ).FirstOrDefault();
		player.Team = player.LocalTeam;
		player.Team.TeamOwner = player;
		player.Team.Members.Add( player );
		leavingteam.Members.Remove( player );
		player.ShowJoinTeams( To.Single( player ) );
	}

	public static int AliveTeamCount()
	{
		int aliveteams = 0;
		foreach ( var team in All.OfType<BaseTeam>() )
		{
			if ( team.CheckAlive() ) aliveteams++;
		}
		return aliveteams;
	}


	[ConCmd.Server("util_lock_team")]
	public static void LockTeam()
	{
		var player = ConsoleSystem.Caller.Pawn as FloodPlayer;
		var team = player.Team;
		if (player == team.TeamOwner) team.TeamLocked = !team.TeamLocked;
		player.RefreshTeamPanel( To.Everyone );
	}


	public bool CheckAlive()
	{
		foreach ( var member in Members )
		{
			if ( !member.Spectating && member.LifeState != LifeState.Dead ) return true;
		}
		return false;
	}


	public void OnElimination()
	{
		foreach ( var player in Members )
		{
			player.Pay( 250 );
		}
	}

	public void OnSquadWipe()
	{
		foreach ( var player in Members )
		{
			player.Pay( 500 );
		}
	}

	public void UpdateName(string name)
	{
		TeamName = name;
	}

}
