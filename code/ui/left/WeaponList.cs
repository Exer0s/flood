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
		AddClass( "weaponpage" );
		AddChild( out Canvas, "canvas" );

		Canvas.Layout.AutoColumns = true;
		Canvas.Layout.ItemSize = new Vector2( 100, 100 );
		Canvas.OnCreateCell = ( cell, data ) =>
		{
			var entry = (LibraryAttribute)data;
			var localPlayer = Local.Pawn as FloodPlayer;
			var localInventory = localPlayer.Inventory as Inventory;
			var localClient = Local.Client;
			BaseFloodWeapon weapon = Library.Create<BaseFloodWeapon>(entry.Name);
			var btn = cell.Add.Button( $"{entry.Title}" );
			var cost = cell.Add.Label($"${weapon.Cost}", "weaponcost");
			
			btn.AddClass( "icon" );
			btn.AddEvent( "onclick", () => {

				if ( localPlayer == null ) return;
				//Buying weapons
				if ( localInventory.CanAdd(weapon) && !localPlayer.playerWeapons.ContainsKey(entry.Name))
				{
					if (localPlayer.Money >= weapon.Cost)
					{
						ConsoleSystem.Run( "spawn_weapon", entry.Name );
						SetClass("purchased", true);
						cost.Text = "Purchased";
					}
				} else
				{
					Log.Info($"Purchase info : {localInventory.CanAdd( weapon ).ToString()} || {localPlayer.playerWeapons.ContainsKey( entry.Name ).ToString()}" );
				}
				//Selling weapons 
				//!! This doesnt work till they make Dictionary's networkable
				if ( !localInventory.CanAdd( weapon ) && localPlayer.playerWeapons.ContainsKey( entry.Name ) )
				{
					
					ConsoleSystem.Run( "sell_weapon", entry.Name );
					SetClass( "purchased", false );
					cost.Text = $"${weapon.Cost}";
				}
				
				
				
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

