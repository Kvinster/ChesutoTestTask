namespace Chesuto.Chess {
    public sealed class Cell {
        public readonly ChessCoords Coords;
        
        public Figure CurFigure { get; private set; }

        public bool IsEmpty => CurFigure == null;

        public Cell(ChessCoords coords) {
            Coords = coords;
        }

        public void SetFigure(Figure figure) {
            CurFigure = figure;
            if ( CurFigure != null ) {
                CurFigure.Coords = Coords;
            }
        }
    }
}
