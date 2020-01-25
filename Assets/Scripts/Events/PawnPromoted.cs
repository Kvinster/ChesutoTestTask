using Chesuto.Chess;

namespace Chesuto.Events {
    public struct PawnPromoted {
        public readonly ChessCoords Coords;
        public readonly Figure      NewFigure;

        public PawnPromoted(ChessCoords coords, Figure newFigure) {
            Coords    = coords;
            NewFigure = newFigure;
        }
    }
}
