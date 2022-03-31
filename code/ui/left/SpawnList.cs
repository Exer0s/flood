using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Tests;

[Library]
public partial class SpawnList : Panel
{
	VirtualScrollPanel Canvas;

	public static SpawnList Instance;

	public SpawnList()
	{

		Instance = this;

		AddClass( "spawnpage" );
		AddChild( out Canvas, "canvas" );

		Canvas.Layout.AutoColumns = true;
		Canvas.Layout.ItemWidth = 100;
		Canvas.Layout.ItemHeight = 100;

		Canvas.OnCreateCell = ( cell, data ) =>
		{
			var prop = (PropAsset)data;
			var panel = cell.Add.Panel( "icon" );
			panel.AddEventListener( "onclick", () => ConsoleSystem.Run( "spawn", prop.Model ) );
			panel.Style.BackgroundImage = Texture.Load( FileSystem.Mounted, prop.Icon, false );
		};
		Log.Info( PropAsset.All.Count );
		foreach ( var prop in PropAsset.All )
		{
			Log.Info( prop.Name );
			Canvas.AddItem(prop);
		}
	}

}
