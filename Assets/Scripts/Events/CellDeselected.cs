using Chesuto.Chess;

namespace Chesuto.Events {
    public struct CellDeselected {
        public readonly ChessCoords CellCoords;

        public CellDeselected(ChessCoords cellCoords) {
            CellCoords = cellCoords;
        }
    }
}
