using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

    public class RoundTimer : Panel
    {
        public Panel Container;
        public Label RoundName;
        public Label TimeLeft;
        public Panel Icon;

        public RoundTimer()
        {
            StyleSheet.Load( "/ui/RoundTimer.scss" );

            Container = Add.Panel( "roundContainer" );
            RoundName = Container.Add.Label( "Round", "roundName" );
            TimeLeft = Container.Add.Label( "00:00", "timeLeft" );
        }

        public override void Tick()
        {
            var player = Sandbox.Player.Local;
            if ( player == null ) return;

            var game = Game.Instance;
            if ( game == null ) return;

            var round = game.Round;
            if ( round == null ) return;

            RoundName.Text = round.RoundName;
            TimeLeft.Text = round.TimeLeftFormatted;
            }
    }