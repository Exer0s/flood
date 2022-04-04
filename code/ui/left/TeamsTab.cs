using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

[Library]
public class TeamsTab : Panel
{
	public Panel TeamTabs;
	public Button YourTab;
	public Button JoinTab;

	public Panel YourTeamPanel;
	public Panel JoinTeamPanel;

	public TeamsTab()
	{
		StyleSheet.Load( "ui/left/TeamsTab.scss" );
		TeamTabs = Add.Panel( "teamtabs" );
		YourTab = TeamTabs.Add.Button( "Your Team", "teambutton", ShowYourPanel );
		JoinTab = TeamTabs.Add.Button( "Join Team", "teambutton", ShowJoinPanel );
		YourTeamPanel = Add.Panel( "yourteam" );
		JoinTeamPanel = Add.Panel( "jointeam" );
	}

	private void ShowJoinPanel()
	{

		if (!JoinTeamPanel.HasClass("active"))
		{
			RefreshJoinPanel();
			JoinTeamPanel.SetClass( "active", true );
			YourTeamPanel.SetClass( "active", false );
		}
	}

	public void RefreshJoinPanel()
	{
		if ( FloodGame.Instance == null ) return;
		JoinTeamPanel.DeleteChildren();
		//if ( FloodGame.Instance == null ) return;
		foreach ( var team in Entity.All.OfType<BaseTeam>() )
		{
			if ( team.TeamOwner == Local.Pawn ) return;
			Log.Info( team.TeamName );
		}
	}

	private void ShowYourPanel()
	{
		if ( !YourTeamPanel.HasClass( "active" ) )
		{
			JoinTeamPanel.SetClass( "active", false );
			YourTeamPanel.SetClass( "active", true );
		}
	}

}
