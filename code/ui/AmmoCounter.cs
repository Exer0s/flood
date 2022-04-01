using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class AmmoCounter : Label
{
	public AmmoCounter()
	{
		StyleSheet.Load( "ui/AmmoCounter.scss" );
		Text = "N / A";
	}

	public override void Tick()
	{
		if (Local.Pawn is FloodPlayer player && Local.Pawn != null)
		{
			
			if (player.ActiveChild is Weapon weapon)
			{
				SetClass( "disable", false );
				Text = $"{weapon.CurrentClip} / {weapon.ClipSize}";
			} else
			{
				SetClass( "disable", true );
			}
		}
		base.Tick();
	}

} 
