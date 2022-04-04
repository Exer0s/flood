using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Timer : Panel
{

	public Label RoundName;
	public Label GameTime;

	public static Timer Instance;

	public Timer()
	{
		Instance = this;
		StyleSheet.Load( "ui/Timer.scss" );
		if ( FloodGame.Instance != null && FloodGame.Instance.GameRound != null ) RoundName = Add.Label( FloodGame.Instance.GameRound.RoundName, "roundname" );
		else
		RoundName = Add.Label( "Waiting", "roundname" );
		
		GameTime = Add.Label( "00:00", "gametime" );
	}

	[Event.Tick.Client]
	public void ClienTick()
	{
		GameTime.Text = FloodGame.Instance.GameTime;
	}

	public void CheckRound()
	{
		RoundName.Text = FloodGame.Instance.GameRound.RoundName;
	}

}
