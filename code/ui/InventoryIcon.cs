
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class InventoryIcon : Panel
{
	public Entity TargetEnt;
	public Label Label;
	public Image Icon;
	public Label Number;

	public InventoryIcon( int i, Panel parent )
	{
		StyleSheet.Load( "ui/InventoryIcon.scss" );
		Parent = parent;
		Label = Add.Label( "empty", "item-name" );
		Icon = Add.Image( "", "item-icon" );
		Icon.SetClass( "disabled", true );
		Number = Add.Label( $"{i}", "slot-number" );
	}

	public void Clear()
	{
		Icon.SetClass( "disabled", true );
		Icon.Style.BackgroundImage = null;
		Label.Text = "";
		SetClass( "active", false );
	}
}
