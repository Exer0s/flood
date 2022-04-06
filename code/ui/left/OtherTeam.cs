using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class OtherTeam : Button
{

	public BaseTeam team;

	public OtherTeam()
	{
		StyleSheet.Load( "ui/left/OtherTeam.scss" );
	}

	public void InitUI()
	{
		if ( team == null || team.TeamOwner == null ) 
		{
			Delete();
			return;
		}
		if ( team.TeamLocked ) Style.BackgroundColor = Color.Red;
		Add.Image( $"avatar:{team.TeamOwner.Client.PlayerId}", "avatar" );
		Add.Label( team.TeamName, "otherteamname" );
	}

	protected override void OnClick( MousePanelEvent e )
	{
		BaseTeam.JoinTeam( team.TeamName, Local.Pawn.Name );
		//TeamsTab.Instance.RefreshJoinPanel();
		base.OnClick( e );
	}

}
