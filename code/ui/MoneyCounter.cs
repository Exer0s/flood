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
			Parent.SetClass( "spawnmenuopen", Local.Client.Input.Down( InputButton.Menu ) );
			var player = Local.Client.Pawn as FloodPlayer;
			if ( player == null ) return;
			moneyLabel.Text = "$" + player.Money.ToString();
		}
}

