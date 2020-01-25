using Chesuto.Chess;

namespace Chesuto.Events {
    public struct GameEnded {
        public readonly ChessColor Winner;

        public GameEnded(ChessColor winner) {
            Winner = winner;
        }
    }
}
