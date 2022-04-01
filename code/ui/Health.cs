using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Health : Panel
{

	Panel healthBar;
	Panel whiteBar;
	Panel backBar;
	Label healthLabel;
	Label Money;
	Label Heart;
	Image Avatar;

	public Health()
	{
		StyleSheet.Load( "ui/Health.scss" );
		backBar = Add.Panel( "backBar" );
		Heart = backBar.Add.Label( "favorite", "heart" );
		healthLabel = Add.Label( "0 / 0", "healthLabel" );
		whiteBar = backBar.Add.Panel( "whiteBar" );
		healthBar = backBar.Add.Panel( "healthBar" );
	}

	public override void Tick()
	{
		var p = Local.Pawn as FloodPlayer;
		if ( p == null ) return;
		if (Avatar == null ) Avatar = Add.Image( $"avatar:{Local.Client.PlayerId}", "avatar" );
		if (Money == null ) Money = Add.Label( $"${p.Money}", "money" );
		var health = Length.Percent( p.Health / p.maxHealth * 100 );
		healthLabel.Text = $"{p.Health.CeilToInt()} / {p.maxHealth.CeilToInt()}";
		if ( Money != null ) Money.Text = $"${p.Money}";
		healthBar.Style.Width = health;
		//backBar.Style.Width = health;
		whiteBar.Style.Width = health;
		//Log.Info( p.Health / 100 );
		base.Tick();
	}

}
