using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class RoundTimer : Panel
{
    public Label Timer;

	public RoundTimer()
	{
		Timer = Add.Label("100", "timer");
	}

	public override void Tick()
	{
		var player = Player.Local;
		if (player == null) return;

		var seconds = 300;
		
		Timer.Text = seconds.ToString();
	}

}
