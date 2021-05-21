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
			var localPlayer = Local.Pawn as FloodPlayer;
			var localClient = Local.Client;
			BaseFloodWeapon weapon = Library.Create<BaseFloodWeapon>(entry.Name);
			var btn = cell.Add.Button( $"{entry.Title} - {weapon.Cost}" );
			btn.AddClass( "icon" );
			btn.AddEvent( "onclick", () => {

				ConsoleSystem.Run( "spawn_weapon", entry.Name );
				
			});
			btn.Style.Background = new PanelBackground
			{
				Texture = Texture.Load( $"/ui/weapons/{entry.Name}.png", false )
				
			};
		};

		var ents = Library.GetAllAttributes<BaseFloodWeapon>();

		foreach ( var entry in ents )
		{
			//Log.Info( $"Weapon loaded :{entry.Name} | {entry.Title} " );
			//if ( entry.Title.Contains("BaseFloodWeapon") ) return;
			var weaponIcon = Texture.Load( $"/ui/weapons/{entry.Name}.png", false );

			if ( weaponIcon != null )
			{
				Canvas.AddItem( entry );
			}
			
			
		}
	}
}

