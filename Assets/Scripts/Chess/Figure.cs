using System.Collections.Generic;

namespace Chesuto.Chess {
    public abstract class Figure {
        public readonly ChessColor Color;

        public ChessCoords Coords;
        
        public abstract FigureType Type { get; }

        protected Figure(ChessColor color) {
            Color = color;
        }

        public abstract bool CanMove(ChessCoords end, Board board);

        public abstract List<ChessCoords> GetAvailableTurns(Board board);
    }
}
