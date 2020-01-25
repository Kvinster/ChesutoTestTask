using Chesuto.Chess;

namespace Chesuto.Cards {
    public sealed class Recruitment : GenericCard {
        public override CardType Type => CardType.Recruitment;
        
        public override bool TryActivate(Game game, GenericPlayer player, out GenericPlayerEffect playerEffect) {
            playerEffect = null;
            return true;
        }

        public override bool CanActivate(Game game, GenericPlayer player) {
            if ( game.CurPlayer != player ) {
                return false;
            }
            var isWhite = (player.Color == ChessColor.White);
            var yMin    = isWhite ? 0 : 4;
            var yMax    = isWhite ? 3 : 7;
            for ( var y = yMin; y <= yMax; ++y ) {
                for ( var x = 0; x < 8; ++x ) {
                    var cell = game.Board.GetCell(x, y);
                    if ( (cell != null) && cell.IsEmpty ) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
