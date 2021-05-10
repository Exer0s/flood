
using Sandbox;
using Sandbox.UI;

public partial class KillFeed : Panel
{
	public static KillFeed Current;

	public KillFeed()
	{
		Current = this;

		StyleSheet.Load( "/ui/KillFeed.scss" );
	}

	[ClientRpc]
	public static void AddEntry( ulong lsteamid, string left, ulong rsteamid, string right, string method )
	{
		if ( Current == null )
			return;

		Log.Info( $"{left} killed {right} using {method}" );

		var e = Current.AddChild<KillFeedEntry>();

		e.AddClass( method );

		e.Left.Text = left;
		e.Left.SetClass( "me", lsteamid == (Player.Local?.SteamId) );

		e.Right.Text = right;
		e.Right.SetClass( "me", rsteamid == (Player.Local?.SteamId) );
	}
}
