using Chesuto.Chess;

namespace Chesuto.Events {
    public struct ChessCheckmate {
        public readonly ChessColor PlayerColor;

        public ChessCheckmate(ChessColor playerColor) {
            PlayerColor = playerColor;
        }
    }
}
