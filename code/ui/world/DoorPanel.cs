using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;

public class DoorPanel : WorldPanel
{
	public Image TeamAvatar;
	public Label TopLabel;
	public Label BottomLabel;

	public static List<DoorPanel> DoorPanels = new List<DoorPanel>();

	public DoorPanel()
	{
		DoorPanels.Add( this );
		WorldScale = 4f;
		StyleSheet.Load( "ui/world/DoorPanel.scss" );
		TopLabel = Add.Label( "This room is unclaimed", "toplabel" );
		BottomLabel = Add.Label( $"Press {Input.GetKeyWithBinding("iv_use")} to claim", "bottomlabel" );
	}



	public void SetTeamInfo(BaseTeam team)
	{
		TeamAvatar = Add.Image( $"avatar:{team.TeamOwner.Client.PlayerId}", "avatar" );
		TopLabel.Text = team.TeamName + "'s room";
		BottomLabel.Delete();
	}

	public void ResetTeamInfo()
	{
		TeamAvatar.Delete();
		TopLabel.Delete();

		TopLabel = Add.Label( "This room is unclaimed", "toplabel" );
		BottomLabel = Add.Label( $"Press {Input.GetKeyWithBinding( "iv_use" )} to claim", "bottomlabel" );
	}


}
