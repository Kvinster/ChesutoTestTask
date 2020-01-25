using Chesuto.Chess;

namespace Chesuto.Events {
    public struct TurnEnded {
        public readonly ChessColor PlayerColor;

        public TurnEnded(ChessColor playerColor) {
            PlayerColor = playerColor;
        }
    }
}
