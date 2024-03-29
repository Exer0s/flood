﻿using Sandbox;
using Sandbox.Tools;
using Sandbox.UI;
using Sandbox.UI.Construct;


[Library]
public partial class SpawnMenu : Panel
{
	public static SpawnMenu Instance;
	Panel toollist;


	private Panel leftPanel;
	private Panel rightPanel;
	
	public SpawnMenu()
	{
		Instance = this;

		StyleSheet.Load( "/ui/SpawnMenu.scss" );

		leftPanel = Add.Panel( "left" );
		{
			var tabs = leftPanel.AddChild<ButtonGroup>();
			tabs.AddClass( "tabs" );

			var body = leftPanel.Add.Panel( "body" );

			{
				var props = body.AddChild<PropList>();
				tabs.SelectedButton = tabs.AddButtonActive( "Props", ( b ) => props.SetClass( "active", b ) );

				var weapons = body.AddChild<WeaponList>();
				tabs.AddButtonActive( "Weapons", ( b ) => weapons.SetClass( "active", b ) );

				var teams = body.AddChild<TeamsTab>();
				tabs.AddButtonActive( "Teams", ( b ) =>
				{
					teams.SetClass( "active", b );
					teams.RefreshTeamPanel();
				} );
			}
		}

		rightPanel = Add.Panel( "right" );
		{
			var tabs = rightPanel.Add.Panel( "tabs" );
			{
				tabs.Add.Button( "Tools" ).AddClass( "active" );
			}
			var body = rightPanel.Add.Panel( "body" );
			{
				toollist = body.Add.Panel( "toollist" );
				{
					RebuildToolList();
				}
				body.Add.Panel( "inspector" );
			}
		}

	}

	
	void RebuildToolList()
	{
		toollist.DeleteChildren( true );

		foreach ( var entry in TypeLibrary.GetDescriptions<BaseTool>() )
		{
			if ( entry.Title == "Base Tool" )
				continue;

			var button = toollist.Add.Button( entry.Title );
			button.SetClass( "active", entry.Name == ConsoleSystem.GetValue( "tool_current" ) );

			button.AddEventListener( "onclick", () =>
			{
				ConsoleSystem.Run( "tool_current", entry.Name );
				ConsoleSystem.Run( "inventory_current", "weapon_tool" );

				foreach ( var child in toollist.Children )
					child.SetClass( "active", child == button );
			} );
		}
	}

	public override void Tick()
	{
		base.Tick();
		Parent.SetClass( "spawnmenuopen", Input.Down( InputButton.Menu ) );

		UpdateActiveTool();
	}

	void UpdateActiveTool()
	{
		var toolCurrent = ConsoleSystem.GetValue( "tool_current" );
		var tool = string.IsNullOrWhiteSpace( toolCurrent ) ? null : TypeLibrary.GetDescription<BaseTool>( toolCurrent );

		foreach ( var child in toollist.Children )
		{
			if ( child is Button button )
			{
				child.SetClass( "active", tool != null && button.Text == tool.Title );
			}
		}
	}

	public override void OnHotloaded()
	{
		base.OnHotloaded();

		RebuildToolList();
	}
}
