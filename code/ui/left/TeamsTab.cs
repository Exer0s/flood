using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class TeamsTab : Panel
{
	public Panel TeamTabs;
	public Button YourTab;
	public Button JoinTab;

	public Panel YourTeamPanel;
	public Panel JoinTeamPanel;

	public static TeamsTab Instance;

	public TeamsTab()
	{
		Instance = this;
		StyleSheet.Load( "ui/left/TeamsTab.scss" );
		TeamTabs = Add.Panel( "teamtabs" );
		YourTab = TeamTabs.Add.Button( "Your Team", "teambutton", ShowYourPanel );
		JoinTab = TeamTabs.Add.Button( "Join Team", "teambutton", ShowJoinPanel );
		YourTeamPanel = Add.Panel( "yourteam" );
		JoinTeamPanel = Add.Panel( "jointeam" );
		JoinTeamPanel.SetClass( "active", false );
		YourTeamPanel.SetClass( "active", true );
	}

	public void ShowJoinPanel()
	{
		RefreshJoinPanel();
		if (!JoinTeamPanel.HasClass("active"))
		{
			
			JoinTeamPanel.SetClass( "active", true );
			YourTeamPanel.SetClass( "active", false );
		}
	}

	public void RefreshJoinPanel()
	{
		if ( Local.Pawn == null ) return;
		JoinTeamPanel.DeleteChildren();
		foreach ( var team in Entity.All.OfType<BaseTeam>() )
		{
			if ( !team.Members.Contains( Local.Pawn ) && !team.TeamLocked )
			{
				var ot = JoinTeamPanel.AddChild<OtherTeam>();
				ot.team = team;
				ot.InitUI();
			}
		}
	}

	public void ShowYourPanel()
	{
		RefreshTeamPanel();
		if ( !YourTeamPanel.HasClass( "active" ) )
		{
			JoinTeamPanel.SetClass( "active", false );
			YourTeamPanel.SetClass( "active", true );
		}
	}

	public Button LockButton;

	public void RefreshTeamPanel()
	{
		var player = Local.Pawn as FloodPlayer;
		YourTeamPanel.DeleteChildren();
		var header = YourTeamPanel.Add.Panel( "teamheader" );
		header.Add.Label( player.Team.TeamName, "teamname" );

		if ( player.Team.TeamOwner != player )
		{
			header.Add.Button( "Leave", "leaveteam", LeaveTeam );
		} else
		{
			if (!player.Team.TeamLocked)
			{
				LockButton = header.Add.Button( "Visible", "leaveteam" );
				LockButton.Style.BackgroundColor = Color.Green;
			} else
			{
				LockButton = header.Add.Button( "Hidden", "leaveteam" );
				LockButton.Style.BackgroundColor = Color.Red;
			}
			
			LockButton.AddEventListener( "onclick", x =>
			{
				if (!player.Team.TeamLocked)
				{
					LockButton.Text = "Hidden";
					LockButton.Style.BackgroundColor = Color.Red;
				} else
				{
					LockButton.Text = "Visible";
					LockButton.Style.BackgroundColor = Color.Green;
				}
				ConsoleSystem.Run( "util_lock_team" );
			} );
		}


		var mlist = YourTeamPanel.Add.Panel( "memberlist" );
		foreach ( var member in player.Team.Members )
		{
			var teammember = mlist.Add.Panel( "teammember" );
			teammember.Add.Image( $"avatar:{member.Client.PlayerId}", "avatar" );
			teammember.Add.Label( member.Client.Name, "name" );
			if (member != Local.Pawn)
			{
				teammember.Add.Button( "Donate", "givemoney" );
			}

			if (Local.Pawn == player.Team.TeamOwner && member != Local.Pawn)
			{
				var kick = teammember.Add.Button( "Kick", "kick");
				kick.AddEventListener( "onclick", x =>
				{
					KickPlayer( member.Client.Name, member.Team.TeamName );
				} );
			}

		}
	}

	public void LeaveTeam()
	{
		var player = Local.Pawn as FloodPlayer;
		BaseTeam.LeaveTeam( player.Team.TeamName, player.Name );
	}

	public void KickPlayer(string name, string teamname)
	{
		BaseTeam.LeaveTeam( teamname, name );
	}

}
