using System.Collections.Generic;

using UnityEngine;

namespace Chesuto.Chess.Figures {
    public sealed class Queen : Figure {
        public override FigureType Type => FigureType.Queen;

        public Queen(ChessColor color) : base(color) { }
        
        public override bool CanMove(ChessCoords end, Board board) {
            var endCell = board.GetCell(end);
            if ( (endCell == null) || (!endCell.IsEmpty && (endCell.CurFigure.Color == Color)) ) {
                return false;
            }
            var xDist = Mathf.Abs(end.X - Coords.X);
            var yDist = Mathf.Abs(end.Y - Coords.Y);
            if ( (xDist == yDist) || (xDist == 0) || (yDist == 0) ) {
                if ( (xDist == 0) && (yDist == 0) ) {
                    return false;
                }
                var xInc = (xDist == 0)
                    ? 0
                    : (end.X > Coords.X)
                        ? 1
                        : -1;
                var yInc = (yDist == 0)
                    ? 0
                    : (end.Y > Coords.Y)
                        ? 1
                        : -1;
                var count = (xDist > 0) ? (xDist - 1) : (yDist - 1);
                var x     = Coords.X + xInc;
                var y     = Coords.Y + yInc;
                for ( var i = 0; i < count; ++i, x += xInc, y += yInc ) {
                    var cell = board.GetCell(x, y);
                    if ( (cell == null) || !cell.IsEmpty ) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public override List<ChessCoords> GetAvailableTurns(Board board) {
            void AddMoves(int xInc, int yInc, List<ChessCoords> list) {
                ChessCoords coords;
                var i = 1;
                while ( (coords = new ChessCoords(Coords.X + xInc * i, Coords.Y + yInc * i)).IsValid &&
                        CanMove(coords, board) ) {
                    list.Add(coords);
                    ++i;
                }
            }

            var res = new List<ChessCoords>();
            AddMoves(1, 0, res);
            AddMoves(1, 1, res);
            AddMoves(1, -1, res);
            AddMoves(-1, 0, res);
            AddMoves(-1, 1, res);
            AddMoves(-1, -1, res);
            AddMoves(0, 1, res);
            AddMoves(0, -1, res);
            return res;
        }
    }
}
