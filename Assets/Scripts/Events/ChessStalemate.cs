using Chesuto.Chess;

namespace Chesuto.Events {
    public struct ChessStalemate {
        public readonly ChessColor PlayerColor;

        public ChessStalemate(ChessColor playerColor) {
            PlayerColor = playerColor;
        }
    }
}
