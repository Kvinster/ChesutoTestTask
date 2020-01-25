using Chesuto.Chess;

namespace Chesuto.Events {
    public struct ChessFigureRemoved {
        public readonly Figure Figure;

        public ChessFigureRemoved(Figure figure) {
            Figure = figure;
        }
    }
}
