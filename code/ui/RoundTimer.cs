using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class RoundTimer : Panel
{
	
	public Panel Container;
    public Label Timer;
    public Label roundLabel;

	public RoundTimer()
	{
		Container = Add.Panel( "roundContainer" );
		roundLabel = Container.Add.Label( "Round", "roundName" );
		Timer = Add.Label("00:00", "timeLeft");
	}

	public override void Tick()
	{
		var player = Player.Local;
		if (player == null) return;
		
		var player = Sandbox.Player.Local;
		if ( player == null ) return;

		var game = Game.Instance;
		if ( game == null ) return;

		var round = game.Round;
		if ( round == null ) return;

		roundLabel.Text = round.RoundName;

		if ( round.RoundDuration > 0 && !string.IsNullOrEmpty( round.TimeLeftFormatted ) )
		{
			Timer.Text = round.TimeLeftFormatted;
			Container.SetClass( "roundNameOnly", false );
		}
		else
		{
			Container.SetClass( "roundNameOnly", true );
		}
		
	}

}
