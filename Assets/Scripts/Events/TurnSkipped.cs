using Chesuto.Chess;

namespace Chesuto.Events {
    public struct TurnSkipped {
        public readonly ChessColor NewPlayerColor;

        public TurnSkipped(ChessColor newPlayerColor) {
            NewPlayerColor = newPlayerColor;
        }
    }
}
