using Chesuto.Chess;

namespace Chesuto.Events {
    public struct CellSelected {
        public readonly ChessCoords CellCoords;

        public CellSelected(ChessCoords cellCoords) {
            CellCoords = cellCoords;
        }
    }
}
