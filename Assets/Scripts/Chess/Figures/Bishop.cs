using System.Collections.Generic;

using UnityEngine;

namespace Chesuto.Chess.Figures {
    public sealed class Bishop : Figure {
        public override FigureType Type => FigureType.Bishop;

        public Bishop(ChessColor color) : base(color) { }
        
        public override bool CanMove(ChessCoords end, Board board) {
            if ( Mathf.Abs(Coords.X - end.X) != Mathf.Abs(Coords.Y - end.Y) ) {
                return false;
            }
            var xInc = (end.X > Coords.X) ? 1 : -1;
            var yInc = (end.Y > Coords.Y) ? 1 : -1;
            var count = Mathf.Abs(end.X - Coords.X) - 1;
            var x = Coords.X + xInc;
            var y = Coords.Y + yInc;
            for ( var i = 0; i < count; ++i, x += xInc, y += yInc ) {
                var cell = board.GetCell(x, y);
                if ( !cell.IsEmpty ) {
                    return false;
                }
            }
            var endCell = board.GetCell(end);
            return endCell.IsEmpty || (endCell.CurFigure.Color != Color);
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
            AddMoves(1, 1, res);
            AddMoves(1, -1, res);
            AddMoves(-1, 1, res);
            AddMoves(-1, -1, res);
            return res;
        }
    }
}
