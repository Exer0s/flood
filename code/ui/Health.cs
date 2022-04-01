using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Health : Panel
{

	Panel healthBar;
	Panel whiteBar;
	Panel backBar;
	Label healthLabel;

	public Health()
	{
		StyleSheet.Load( "ui/Health.scss" );
		backBar = Add.Panel( "backBar" );
		healthLabel = Add.Label( "0 / 0", "healthLabel" );
		whiteBar = backBar.Add.Panel( "whiteBar" );
		healthBar = backBar.Add.Panel( "healthBar" );
	}

	public override void Tick()
	{
		var p = Local.Pawn as FloodPlayer;
		if ( p == null ) return;
		var health = Length.Percent( (p.Health / p.maxHealth) * 100 );
		healthLabel.Text = $"{p.Health} / {p.maxHealth}";
		healthBar.Style.Width = health;
		whiteBar.Style.Width = health;
		//Log.Info( p.Health / 100 );
		base.Tick();
	}

}
