using Chesuto.Chess;

namespace Chesuto.Events {
    public struct ChessFigureMoved {
        public readonly ChessCoords Start;
        public readonly ChessCoords End;

        public ChessFigureMoved(ChessCoords start, ChessCoords end) {
            Start = start;
            End   = end;
        }
    }
}
