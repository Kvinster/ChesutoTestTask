using Chesuto.Chess;

namespace Chesuto.Events {
    public struct ChessKingRemoved {
        public readonly ChessColor KingColor;

        public ChessKingRemoved(ChessColor kingColor) {
            KingColor = kingColor;
        }
    }
}
