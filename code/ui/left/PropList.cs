﻿using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using Sandbox.UI.Tests;

public partial class PropList : Panel
{
	VirtualScrollPanel Canvas;

	public static PropList Instance;

	public PropList()
	{
		StyleSheet.Load( "ui/left/PropList.scss" );
		Instance = this;

		AddChild( out Canvas, "canvas" );

		Canvas.Layout.AutoColumns = true;
		Canvas.Layout.ItemWidth = 100;
		Canvas.Layout.ItemHeight = 100;

		Canvas.OnCreateCell = ( cell, data ) =>
		{
			var prop = (PropAsset)data;
			var btn = cell.Add.Button( $"${prop.Cost}" );
			btn.AddClass( "icon" );
			btn.AddEventListener( "onclick", () => ConsoleSystem.Run( "spawn", prop.Model, prop.Health, prop.Cost, prop.DestroyPayout ) );
			btn.Style.BackgroundImage = Texture.Load( FileSystem.Mounted, prop.Icon, false );
		};

		foreach ( var prop in PropAsset.All )
		{
			Canvas.AddItem(prop);
		}
	}

}
