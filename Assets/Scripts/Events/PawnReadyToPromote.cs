using Chesuto.Chess.Figures;

namespace Chesuto.Events {
    public struct PawnReadyToPromote {
        public readonly Pawn Pawn;

        public PawnReadyToPromote(Pawn pawn) {
            Pawn = pawn;
        }
    }
}
