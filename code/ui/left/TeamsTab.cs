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
	}

	private void ShowJoinPanel()
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
		Log.Info( "refreshing join panel" );
		Log.Info( Entity.All.OfType<BaseTeam>().Count() );
		foreach ( var team in Entity.All.OfType<BaseTeam>() )
		{
			if ( !team.Members.Contains( Local.Pawn ) )
			{
				var ot = JoinTeamPanel.AddChild<OtherTeam>();
				ot.team = team;
				ot.InitUI();
			}
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
