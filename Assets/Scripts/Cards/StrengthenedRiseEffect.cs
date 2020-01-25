using Chesuto.Chess;
using Chesuto.Chess.Figures;
using Chesuto.Events;

namespace Chesuto.Cards {
    public sealed class StrengthenedRiseEffect : GenericPlayerEffect {
        public override PlayerEffectType Type => PlayerEffectType.StrengthenedRise;

        protected override void Init() {
            EventManager.Subscribe<ChessFigureSummoned>(OnChessFigureSummoned);
            EventManager.Subscribe<GameEnded>(OnGameEnded);
        }

        void OnChessFigureSummoned(ChessFigureSummoned ev) {
            var figure = ev.Figure;
            if ( (figure.Type == FigureType.Pawn) && (figure.Color == Player.Color) && (figure is Pawn pawn) ) {
                var pawnCell = Game.Board.GetCell(pawn.Coords);
                if ( pawnCell.CurFigure != figure ) {
                    // Pawn has already been removed, probably by another Strengthened Rise
                    return;
                } 
                Game.Board.PromotePawn(pawn, FigureType.Rook);

                Player.EndEffect(this);

                EventManager.Unsubscribe<ChessFigureSummoned>(OnChessFigureSummoned);
                EventManager.Unsubscribe<GameEnded>(OnGameEnded);
            }
        }

        void OnGameEnded(GameEnded ev) {
            Player.EndEffect(this);
            
            EventManager.Unsubscribe<ChessFigureSummoned>(OnChessFigureSummoned);
            EventManager.Unsubscribe<GameEnded>(OnGameEnded);
        }
    }
}
