using Chesuto.Chess;

namespace Chesuto.Cards {
    public sealed class ClaymoreOfRush : GenericCard {
        public override CardType Type => CardType.ClaymoreOfRush;
        
        public override bool TryActivate(Game game, GenericPlayer player, out GenericPlayerEffect playerEffect) {
            if ( game.CurPlayer != player ) {
                playerEffect = null;
                return false;
            }
            playerEffect = new ClaymoreOfRushEffect(this);
            return true;
        }

        public override bool CanActivate(Game game, GenericPlayer player) {
            for ( var x = 0; x < 8; ++x ) {
                for ( var y = 0; y < 8; ++y ) {
                    var cell = game.Board.GetCell(x, y);
                    if ( (cell != null) && !cell.IsEmpty && (cell.CurFigure.Type == FigureType.Pawn) ) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
