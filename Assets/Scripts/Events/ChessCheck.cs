using Chesuto.Chess;

namespace Chesuto.Events {
    public struct ChessCheck {
        public readonly ChessColor PlayerColor;

        public ChessCheck(ChessColor playerColor) {
            PlayerColor = playerColor;
        }
    }
}
