using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class MoneyCounter : Panel
	{
		public Label moneyLabel;

		public MoneyCounter()
		{
			moneyLabel = Add.Label( "$0", "moneyCounter" );
		}

		public override void Tick()
		{
			Parent.SetClass( "spawnmenuopen", Player.Local?.Input.Down( InputButton.Menu ) ?? false );	
			var player = Sandbox.Player.Local as FloodPlayer;
			moneyLabel.Text = "$" + player.Money.ToString();
		}
}

