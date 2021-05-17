using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using Sandbox.UI.Tests;
using System.Linq;

[Library]
public partial class WeaponList : Panel
{
	VirtualScrollPanel Canvas;

	public WeaponList()
	{
		AddClass( "spawnpage" );
		AddChild( out Canvas, "canvas" );

		Canvas.Layout.AutoColumns = true;
		Canvas.Layout.ItemSize = new Vector2( 100, 100 );
		Canvas.OnCreateCell = ( cell, data ) =>
		{
			var entry = (LibraryAttribute)data;
			if ( entry.Title == "BaseFloodWeapon" ) return;
			var btn = cell.Add.Button( $"{entry.Title} - {FloodGame.Instance.weaponCosts[entry.Title]}" );
			btn.AddClass( "icon" );
			btn.AddEvent( "onclick", () => ConsoleSystem.Run( "spawn_weapon", entry.Name ) );
			btn.Style.Background = new PanelBackground
			{
				Texture = Texture.Load( $"/ui/weapons/{entry.Name}.png", false )
				
			};
		};

		var ents = Library.GetAllAttributes<BaseFloodWeapon>();

		foreach ( var entry in ents )
		{

			Canvas.AddItem( entry );
			Log.Info( $"Weapon loaded :{entry.Name} " );
		}
	}
}

