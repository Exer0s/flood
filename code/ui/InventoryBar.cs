﻿using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;

public class InventoryBar : Panel
{
	readonly List<InventoryIcon> slots = new();

	public InventoryBar()
	{
		StyleSheet.Load( "ui/InventoryBar.scss" );
		for ( int i = 0; i < 5; i++ )
		{
			var icon = new InventoryIcon( i + 1, this );
			slots.Add( icon );
		}
	}

	public override void Tick()
	{
		base.Tick();

		var player = Local.Pawn as Player;
		if ( player == null ) return;
		if ( player.Inventory == null ) return;

		//Shrink the hotbar on round start
		if ( FloodGame.Instance.GameRound is FightingRound || FloodGame.Instance.GameRound is RisingRound )
		{
			if ( slots.Count == 7 )
			{
				slots[5].Delete();
				slots[6].Delete();
				slots.RemoveAt(6);
				slots.RemoveAt(5);
			}
		}
		
		//Expand hotbar if building round
		if ( FloodGame.Instance.GameRound is WaitingRound || FloodGame.Instance.GameRound is BuildingRound )
		{
			if ( slots.Count < 7 )
			{
				slots.Add(new InventoryIcon(6, this));
				slots.Add(new InventoryIcon(7, this));
			}
		}
		
		for ( int i = 0; i < slots.Count; i++ )
		{
			UpdateIcon( player.Inventory.GetSlot( i ), slots[i], i );
		}
	}
	
	private static void UpdateIcon( Entity ent, InventoryIcon inventoryIcon, int i )
	{
		var player = Local.Pawn as Player;

		if ( ent == null )
		{
			inventoryIcon.Clear();
			return;
		}

		if (ent is Carriable tool && ent is not Weapon)
		{
			inventoryIcon.Icon.Style.BackgroundImage = Texture.Load(FileSystem.Mounted, tool.Icon);
			inventoryIcon.Icon.SetClass( "disabled", false );
		}

		if ( ent is Weapon weapon )
		{
			inventoryIcon.Icon.Style.BackgroundImage = Texture.Load( FileSystem.Mounted, weapon.Icon );
			inventoryIcon.Icon.SetClass( "disabled", false );
		}


		inventoryIcon.TargetEnt = ent;
		inventoryIcon.Label.Text = ent.ClassName;
		inventoryIcon.SetClass( "active", player.ActiveChild == ent );
	}

	[Event( "buildinput" )]
	public void ProcessClientInput( InputBuilder input )
	{
		var player = Local.Pawn as Player;
		if ( player == null )
			return;

		var inventory = player.Inventory;
		if ( inventory == null )
			return;

		if ( player.ActiveChild is PhysGun physgun && physgun.BeamActive )
		{
			return;
		}

		if ( input.Pressed( InputButton.Slot1 ) ) SetActiveSlot( input, inventory, 0 );
		if ( input.Pressed( InputButton.Slot2 ) ) SetActiveSlot( input, inventory, 1 );
		if ( input.Pressed( InputButton.Slot3 ) ) SetActiveSlot( input, inventory, 2 );
		if ( input.Pressed( InputButton.Slot4 ) ) SetActiveSlot( input, inventory, 3 );
		if ( input.Pressed( InputButton.Slot5 ) ) SetActiveSlot( input, inventory, 4 );
		if ( input.Pressed( InputButton.Slot6 ) ) SetActiveSlot( input, inventory, 5 );
		if ( input.Pressed( InputButton.Slot7 ) ) SetActiveSlot( input, inventory, 6 );
		if ( input.Pressed( InputButton.Slot8 ) ) SetActiveSlot( input, inventory, 7 );
		if ( input.Pressed( InputButton.Slot9 ) ) SetActiveSlot( input, inventory, 8 );

		if ( input.MouseWheel != 0 ) SwitchActiveSlot( input, inventory, -input.MouseWheel );
	}

	private static void SetActiveSlot( InputBuilder input, IBaseInventory inventory, int i )
	{
		var player = Local.Pawn as Player;

		if ( player == null )
			return;

		var ent = inventory.GetSlot( i );
		if ( player.ActiveChild == ent )
			return;

		if ( ent == null )
			return;

		input.ActiveChild = ent;
	}

	private static void SwitchActiveSlot( InputBuilder input, IBaseInventory inventory, int idelta )
	{
		var count = inventory.Count();
		if ( count == 0 ) return;

		var slot = inventory.GetActiveSlot();
		var nextSlot = slot + idelta;

		while ( nextSlot < 0 ) nextSlot += count;
		while ( nextSlot >= count ) nextSlot -= count;

		SetActiveSlot( input, inventory, nextSlot );
	}
}
