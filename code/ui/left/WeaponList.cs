using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using Sandbox.UI.Tests;
using System.Linq;

public partial class WeaponList : Panel
{
	VirtualScrollPanel Canvas;

	public WeaponList()
	{
		StyleSheet.Load( "ui/left/WeaponList.scss" );
		AddClass( "spawnpage" );
		AddChild( out Canvas, "canvas" );

		Canvas.Layout.AutoColumns = true;
		Canvas.Layout.ItemWidth = 100;
		Canvas.Layout.ItemHeight = 100;
		Canvas.OnCreateCell = ( cell, data ) =>
		{
			var weapon = (WeaponAsset)data;
			var btn = cell.Add.Button( weapon.Title );
			btn.Add.Label( $"${weapon.Cost}", "cost" );
			btn.AddClass( "icon" );
			btn.AddEventListener( "onclick", () => ConsoleSystem.Run( "spawn_weapon", weapon.Weapon ) );
			btn.Style.BackgroundImage = Texture.Load( FileSystem.Mounted, weapon.Icon, false );
		};

		var weapons = WeaponAsset.All.OrderBy( x => x.Title ).ToArray();

		foreach ( var weapon in weapons )
		{
			Canvas.AddItem( weapon );
		}
	}
}
