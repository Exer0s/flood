using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;

public class PostGameUI : Panel
{
	public PostGameUI()
	{
		StyleSheet.Load( "ui/PostGameUI.scss" );
		var bg = Add.Panel( "background" );
	}
}
