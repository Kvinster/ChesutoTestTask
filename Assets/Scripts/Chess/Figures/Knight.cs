using UnityEngine;

using System.Collections.Generic;

namespace Chesuto.Chess.Figures {
    public sealed class Knight : Figure {
        public override FigureType Type => FigureType.Knight;

        public Knight(ChessColor color) : base(color) { }
        
        public override bool CanMove(ChessCoords end, Board board) {
            var endCell = board.GetCell(end);
            if ( (endCell == null) || (!endCell.IsEmpty && (endCell.CurFigure.Color == Color)) ) {
                return false;
            }
            var xDist = Mathf.Abs(end.X - Coords.X);
            var yDist = Mathf.Abs(end.Y - Coords.Y);
            return ((xDist == 2) && (yDist == 1) || (xDist == 1) && (yDist == 2));
        }

        public override List<ChessCoords> GetAvailableTurns(Board board) {
            void AddMoveIfCan(int xOffset, int yOffset, List<ChessCoords> list) {
                var coords = new ChessCoords(Coords.X + xOffset, Coords.Y + yOffset); 
                if ( coords.IsValid && CanMove(coords, board) ) {
                    list.Add(coords);
                }
            }
            
            var res = new List<ChessCoords>();
            AddMoveIfCan(1, 2, res);
            AddMoveIfCan(1, -2, res);
            AddMoveIfCan(-1, 2, res);
            AddMoveIfCan(-1, -2, res);
            AddMoveIfCan(2, 1, res);
            AddMoveIfCan(2, -1, res);
            AddMoveIfCan(-2, 1, res);
            AddMoveIfCan(-2, -1, res);
            return res;
        }
    }
}
