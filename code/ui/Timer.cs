using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class RoundTimer : Panel
{
    public Label timer;

	public Vitals()
	{
		timer = Add.Label("5:00", "Timer");
	}

	public override void Tick()
	{
		var player = Player.Local;
		if (player == null) return;

		var seconds = 300;

		//Health.Text = $"{player.Health:n0}";
	}

}