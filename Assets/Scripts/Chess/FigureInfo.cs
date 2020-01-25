namespace Chesuto.Chess {
    public struct FigureInfo {
        public readonly FigureType Type;
        public readonly ChessColor Color;

        public FigureInfo(FigureType type, ChessColor color) {
            Type  = type;
            Color = color;
        }
    }
}
