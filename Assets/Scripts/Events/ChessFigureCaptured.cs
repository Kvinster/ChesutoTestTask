using Chesuto.Chess;

namespace Chesuto.Events {
    public struct ChessFigureCaptured {
        public readonly Figure Figure;

        public ChessFigureCaptured(Figure figure) {
            Figure = figure;
        }
    }
}
