using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Health : Panel
{
	public Label Label;

	public Health()
	{
		StyleSheet.Load( "ui/Health.scss" );
		Label = Add.Label( "100", "value" );
	}

	public override void Tick()
	{
		var player = Local.Pawn;
		if ( player == null ) return;

		Label.Text = $"{player.Health.CeilToInt()}";
	}
}
