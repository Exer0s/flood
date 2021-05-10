using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class RoundTimer : Panel
{
	
    public Label Timer;
    public Label roundLabel;

	public RoundTimer()
	{
		Timer = Add.Label("N/A", "timeLeft");
		roundLabel = Add.Label( "Waiting for Players...", "roundName" );
		Log.Info( "Round Timer Created" );
	}

	public override void Tick()
	{
		var player = Player.Local;
		if ( player == null ) return;

		var game = FloodGame.Instance;
		if ( game == null ) return;

		var round = game.Round;
		if ( round == null ) return;

		Timer.Text = round.TimeLeftFormatted;
		roundLabel.Text = round.RoundName;
		
	}

}
