﻿using System;
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
		GameRounds.Add( "Waiting...", new WaitingRound());
		GameRounds.Add( "Building", new BuildingRound());
		GameRounds.Add( "Prepare", new RisingRound());
		GameRounds.Add( "Fight!", new FightingRound());
		GameRounds.Add( "Post Game", new PostRound());
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
		GameRound.OnRoundStart();
		GiveRoundWeapons();
		SetRoundNameUI();
		OnSecond();
		Log.Info( "Progressed Round to " + GameRound.RoundName );
	}

	public void GiveRoundWeapons()
	{
		if (IsServer)
		{
			foreach ( var player in All.OfType<FloodPlayer>().ToArray() )
			{
				player.Inventory.DeleteContents();
				if (GameRound.Weapons.Count() > 0)
				{
					foreach ( var weapon in GameRound.Weapons )
					{
						player.Inventory.Add( Library.Create<Carriable>(weapon), true );
					}
				}
			}

		}
	}


	[ClientRpc]
	public void SetRoundNameUI()
	{
		Timer.Instance.RoundName.Text = GameRound.RoundName;
		Timer.Instance.GameTime.Text = GameTime;
	}

}