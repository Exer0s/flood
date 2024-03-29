﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

partial class FloodGame
{

	[Net] public GameRound GameRound { get; set; } = null;
	[Net] public string GameTime { get; set; }
	[Net] public float TimeOffset { get; set; } = 0f;

	[ConVar.Server("flood_die_remove_weapons")] public bool DieRemoveWeapons { get; set; }

	[Net]
	public Dictionary<string, GameRound> GameRounds { get; set; } = new Dictionary<string, GameRound>();

	public void StartRoundSystem()
	{
		GameRounds.Add( "Waiting...", new WaitingRound());
		GameRounds.Add( "Build!", new BuildingRound());
		GameRounds.Add( "Flood", new RisingRound());
		GameRounds.Add( "Fight!", new FightingRound());
		GameRounds.Add( "Draining", new PostRound());
		GameRound = GameRounds["Waiting..."];
		GameRound.OnRoundStart();
		_ = StartSecondTimer();
	}


	public async Task StartSecondTimer()
	{
		while ( true )
		{
			await Task.DelaySeconds( 1 );
			OnSecond();
		}
	}

	[Event.Tick.Server]
	public void DoRoundTick()
	{
		if ( GameRound != null ) GameRound.RoundTick();
	}

	public virtual void OnSecond()
	{
		if ( IsServer )
		{
			if ( GameRound == null ) return;
			if ( GameRound.RoundDuration != 0f)
			{
				GameTime = TimeSpan.FromSeconds( GameRound.RoundEndTime - Time.Now ).ToString(@"mm\:ss"); //Countdown
			} else
			{
				GameTime = TimeSpan.FromSeconds( Time.Now - TimeOffset ).ToString( @"mm\:ss" ); //Count up
			}
			if (GameRound is WaitingRound)
			{
				if (Client.All.Count >= 2)
				{
					ProgressRound();
				}
			} else
			{
				if (GameRound.RoundEndTime - Time.Now <= 1) ProgressRound();
			}

		}

	}

	public void ProgressRound()
	{
		if ( !IsServer ) return;
		TimeOffset = Time.Now;
		GameRound.OnRoundEnd();
		GameRound = GameRounds[GameRound.NextRound];
		GameRound.Players.Clear();
		foreach ( var player in All.OfType<FloodPlayer>() )
		{
			GameRound.Players.Add( player );
		}
		GameRound.OnRoundStart();

		SetRoundNameUI(GameRound.RoundName);
		OnSecond();
		Log.Info( "Progressed Round to " + GameRound.RoundName );
	}


	[ClientRpc]
	public void SetRoundNameUI(string name)
	{
		Timer.Instance.RoundName.Text = name;
		Timer.Instance.GameTime.Text = GameTime;
	}


	public List<FloodPlayer> PlayersSkipping { get; set; } = new List<FloodPlayer>();

	[ConCmd.Server("skip_round")]
	public static void SkipRound()
	{
		var player = ConsoleSystem.Caller.Pawn as FloodPlayer;
		if ( Instance.PlayersSkipping.Contains( player ) ) return; // u already voted bro?!?!
		if ( Instance.GameRound is not BuildingRound ) return; // u cannot skip the lobby bruh

		Instance.PlayersSkipping.Add(player);

		var skipped = (float)Instance.PlayersSkipping.Count();
		var all = (float)All.OfType<FloodPlayer>().Count();

		var divided = skipped / all;
		SystemMessage( $" {player.Client.Name} voted to skip the round! {Instance.PlayersSkipping.Count()} / {All.OfType<FloodPlayer>().Count()}" );


		if ( divided < 0.9f ) return;
		Instance.ProgressRound();
		SystemMessage( $"Skipped {Instance.GameRound.RoundName} round" );
	}

	[ConCmd.Server]
	public static void ClearSkipList()
	{
		Instance.PlayersSkipping.Clear();
	}



	[ConCmd.Admin( "force_skip" )]
	public static void ForceSkip()
	{
		Instance.ProgressRound();
	}

	public static void SystemMessage( string message )
	{
		//Host.AssertServer();
		FloodChat.AddChatEntry( To.Everyone, "Server", message, "/ui/system.png" );
	}


}
