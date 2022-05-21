using Sandbox;
using Sandbox.UI;

public partial class FloodHUD : HudEntity<RootPanel>
{
	public FloodHUD()
	{
		if ( !IsClient )
			return;

		RootPanel.StyleSheet.Load( "/ui/FloodHUD.scss" );
		AddPanels();
	}

	[Event.Hotload]
	public void HotLoadUI()
	{
		if ( !IsClient ) return;
		DeletePanels();
		AddPanels();
	}

	public void AddPanels()
	{
		RootPanel.AddChild<FloodCrossPanel>();
		RootPanel.AddChild<FloodChat>();
		RootPanel.AddChild<VoiceList>();
		RootPanel.AddChild<Health>();
		RootPanel.AddChild<InventoryBar>();
		RootPanel.AddChild<CurrentTool>();
		RootPanel.AddChild<SpawnMenu>();
		RootPanel.AddChild<Timer>();
		RootPanel.AddChild<AmmoCounter>();
		RootPanel.AddChild<PropHealthUI>();
	}

	public void DeletePanels()
	{
		RootPanel.DeleteChildren();
	}


}
