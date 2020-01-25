using System;

namespace Chesuto.Chess {
    [Serializable]
    public struct ChessCoords {
        public readonly int X;
        public readonly int Y;

        public bool IsValid => (X >= 0) && (X < 8) && (Y >= 0) && (Y < 8);

        public ChessCoords(int x, int y) {
            X = x;
            Y = y;
        }

        public override string ToString() {
            return $"({X},{Y})";
        }
        
        public bool Equals(ChessCoords other) {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj) {
            return obj is ChessCoords other && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                return (X * 397) ^ Y;
            }
        }

        public static bool operator==(ChessCoords? a, ChessCoords? b) {
            if ( !a.HasValue || !b.HasValue ) {
                return ReferenceEquals(a, b);
            }
            return (a.Value.X == b.Value.X) && (a.Value.Y == b.Value.Y);
        }

        public static bool operator!=(ChessCoords? a, ChessCoords? b) {
            return !(a == b);
        }
    }
}
