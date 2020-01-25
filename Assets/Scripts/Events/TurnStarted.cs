using Chesuto.Chess;

namespace Chesuto.Events {
    public struct TurnStarted {
        public readonly ChessColor PlayerColor;

        public TurnStarted(ChessColor playerColor) {
            PlayerColor = playerColor;
        }
    }
}
