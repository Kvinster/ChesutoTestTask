using UnityEngine;

using System.Collections.Generic;

namespace Chesuto.Chess.Figures {
    public sealed class Pawn : Figure {
        // TODO set and unset this
        public bool JustMovedTwoSquares = false; // for en passant
        
        public override FigureType Type => FigureType.Pawn;

        public Pawn(ChessColor color) : base(color) { }
        
        public override bool CanMove(ChessCoords end, Board board) {
            var endCell = board.GetCell(end);
            if ( (endCell == null) || (!endCell.IsEmpty && (endCell.CurFigure.Color == Color)) ) {
                return false;
            }
            var yInc = (Color == ChessColor.White) ? 1 : -1;
            if ( end.X == Coords.X ) {
                if ( end.Y == Coords.Y + yInc ) {
                    return endCell.IsEmpty;
                }
                if ( end.Y == Coords.Y + 2 * yInc ) {
                    var betweenCell = board.GetCell(Coords.X, Coords.Y + yInc);
                    if ( (betweenCell == null) || !betweenCell.IsEmpty ) {
                        return false;
                    }
                    var startY = (Color == ChessColor.White) ? 1 : 6;
                    return (Coords.Y == startY) && endCell.IsEmpty;
                }
            }
            var xDist = Mathf.Abs(end.X - Coords.X);
            if ( (xDist == 1) && (end.Y == Coords.Y + yInc) ) {
                if ( endCell.IsEmpty ) {
                    var passantCell = board.GetCell(end.X, Coords.Y);
                    if ( (passantCell == null) || passantCell.IsEmpty ) {
                        return false;
                    }
                    var passantFigure = passantCell.CurFigure;
                    if ( (passantFigure.Color == Color) || (passantFigure.Type != FigureType.Pawn) ) {
                        return false;
                    }
                    return (passantFigure is Pawn passantPawn) && passantPawn.JustMovedTwoSquares;
                } else {
                    return (endCell.CurFigure.Color != Color);
                }
            }
            return false;
        }

        public override List<ChessCoords> GetAvailableTurns(Board board) {
            void AddMove(int xOffset, int yOffset, List<ChessCoords> list) {
                var coords = new ChessCoords(Coords.X + xOffset, Coords.Y + yOffset);
                if ( coords.IsValid && CanMove(coords, board) ) {
                    list.Add(coords);
                }
            }
            
            var res    = new List<ChessCoords>();
            var yInc   = (Color == ChessColor.White) ? 1 : -1;
            AddMove(0, yInc, res);
            AddMove(0, 2 * yInc, res);
            AddMove(1, yInc, res);
            AddMove(-1, yInc, res);
            AddMove(-1, yInc, res);
            AddMove(-1, yInc, res);
            res.TrimExcess();
            return res;
        }
    }
}
