using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;

public class PropHealthUI : Panel
{

	public static PropHealthUI Instance;
	public FloodProp LookingProp { get; set; }

	Panel healthBar;
	Panel whiteBar;
	Panel backBar;
	Label healthLabel;


	public PropHealthUI()
	{
		Instance = this;
		StyleSheet.Load( "ui/PropHealthUI.scss" );
		backBar = Add.Panel( "backBar" );
		healthLabel = Add.Label( "0 / 0", "healthLabel" );
		whiteBar = backBar.Add.Panel( "whiteBar" );
		healthBar = backBar.Add.Panel( "healthBar" );
	}

	public override void Tick()
	{

		if (LookingProp == null)
		{
			if ( HasClass( "active" ) )
				SetClass( "active", false );
		} else
		{
			var health = Length.Percent( LookingProp.Health / LookingProp.maxHealth * 100 );
			healthLabel.Text = $"{LookingProp.Health.CeilToInt()} / {LookingProp.maxHealth.CeilToInt()}";
			healthBar.Style.Width = health;
			//backBar.Style.Width = health;
			whiteBar.Style.Width = health;
			if (!HasClass("active"))
				SetClass( "active", true );
		}

		base.Tick();
	}

}
