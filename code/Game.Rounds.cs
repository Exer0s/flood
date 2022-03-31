using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

partial class FloodGame
{

	[Net] public GameRound GameRound { get; set; }
	[Net] public string GameTime { get; set; }
	[Net] public float TimeOffset { get; set; } = 0f;

	public Dictionary<string, GameRound> GameRounds = new Dictionary<string, GameRound>();

	public void StartRoundSystem()
	{
		_ = StartSecondTimer();
		GameRounds.Add( "Waiting...", new WaitingRound());
		GameRounds.Add( "Building", new BuildingRound());
		GameRound = GameRounds["Waiting..."];
		GameRound.OnRoundStart(false);
	}


	public async Task StartSecondTimer()
	{
		while ( true )
		{
			await Task.DelaySeconds( 1 );
			OnSecond();
		}
	}

	public virtual void OnSecond()
	{
		if ( Host.IsServer )
		{
			GameTime = TimeSpan.FromSeconds( Time.Now - TimeOffset ).ToString( @"mm\:ss" );

			if (GameRound is WaitingRound)
			{
				if (Client.All.Count >= 2)
				{
					ProgressRound();
				}
			}

		}

	}

	public void ProgressRound()
	{
		TimeOffset = Time.Now;
		GameRound.OnRoundEnd();
		GameRound = GameRounds[GameRound.NextRound];
		GameRound.OnRoundStart(false);
		SetRoundNameUI();
	}


	[ClientRpc]
	public void SetRoundNameUI()
	{
		Timer.Instance.RoundName.Text = GameRound.RoundName;
	}

}
