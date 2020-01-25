using Chesuto.Chess;

namespace Chesuto.Events {
    public struct ChessFigureSummoned {
        public readonly Figure Figure;

        public ChessFigureSummoned(Figure figure) {
            Figure = figure;
        }
    }
}
