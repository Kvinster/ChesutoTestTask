using Chesuto.Cards;

namespace Chesuto.Chess {
    public sealed class HumanPlayer : GenericPlayer {
        public HumanPlayer(ChessColor color, Deck deck, Game game) : base(color, deck, game) { }

        public override void StartTurn() {
            base.StartTurn();

            if ( !Game.Board.HasAvailableTurns() ) {
                var canPlayCard = false;
                if ( TurnCount > 3 ) {
                    foreach ( GenericCard card in Hand ) {
                        if ( card.CanActivate(Game, this) ) {
                            canPlayCard = true;
                            break;
                        }
                    }
                }
                if ( !canPlayCard ) {
                    Game.Surrender(this);
                }
            }
        }
    }
}
