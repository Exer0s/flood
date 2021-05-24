using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using Sandbox.UI.Tests;

public class TeamPage : Panel
{
	VirtualScrollPanel Canvas;

	public TeamPage()
	{
		AddClass( "teampage" );
		AddChild( out Canvas, "canvas" );

		Canvas.Layout.AutoColumns = true;
		Canvas.Layout.ItemSize = new Vector2( 50, 50 );
		Canvas.OnCreateCell = ( cell, data ) =>
		{
			var client = (Client)data;

			var btn = cell.Add.Button("", "playername");
			var pName = cell.Add.Label($"{client.Name}", "playername");
			btn.AddClass("background");

		};

		foreach (var client in Client.All)
		{
			if (client.Pawn is FloodPlayer player)
			{
				if ( player == null ) return;
				
				Log.Info( "Added player to team page" );
			}
			Canvas.AddItem( client );
		}

	}

}

