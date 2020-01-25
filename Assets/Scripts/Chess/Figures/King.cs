using System.Collections.Generic;

using UnityEngine;

namespace Chesuto.Chess.Figures {
    public sealed class King : Figure {
        public override FigureType Type => FigureType.King;

        public bool Moved;

        public King(ChessColor color) : base(color) { }
        
        public override bool CanMove(ChessCoords end, Board board) {
            if ( Mathf.Abs(end.X - Coords.X) > 1 ) {
                if ( Moved || (Coords.Y != end.Y) || board.IsCheck(Color) ) {
                    return false;
                }
                switch ( end.X ) {
                    case 2: { // left castling
                        var rookCell = board.GetCell(0, Coords.Y);
                        if ( (rookCell == null) || rookCell.IsEmpty || (rookCell.CurFigure.Color != Color) ||
                             (rookCell.CurFigure.Type != FigureType.Rook) || !(rookCell.CurFigure is Rook rook) ||
                             rook.Moved ) {
                            return false;
                        }
                        for ( var x = Coords.X - 1; x > 0; --x ) {
                            var cell = board.GetCell(x, Coords.Y);
                            if ( (cell == null) || !cell.IsEmpty ) {
                                return false;
                            }
                            if ( (x > 1) &&  board.IsCheck(Coords, cell.Coords) ) {
                                return false;
                            }
                        }
                        return true;
                    }
                    case 6: { // right castling
                        var rookCell = board.GetCell(7, Coords.Y);
                        if ( (rookCell == null) || rookCell.IsEmpty || (rookCell.CurFigure.Color != Color) ||
                             (rookCell.CurFigure.Type != FigureType.Rook) || !(rookCell.CurFigure is Rook rook) ||
                             rook.Moved ) {
                            return false;
                        }
                        for ( var x = Coords.X + 1; x < 7; ++x ) {
                            var cell = board.GetCell(x, Coords.Y);
                            if ( (cell == null) || !cell.IsEmpty || board.IsCheck(Coords, cell.Coords) ) {
                                return false;
                            }
                        }
                        return true;
                    }
                    default: {
                        return false;
                    }
                }
            }
            if ( Mathf.Abs(end.Y - Coords.Y) > 1 ) {
                return false;
            }
            var endCell = board.GetCell(end);
            if ( !endCell.IsEmpty && (endCell.CurFigure.Color == Color) ) {
                return false;
            }
            for ( var i = end.X - 1; i <= end.X + 1; ++i ) {
                for ( var j = end.Y - 1; j <= end.Y + 1; ++j ) {
                    var coords = new ChessCoords(i, j);
                    if ( !coords.IsValid || (coords == end) ) {
                        continue;
                    }
                    var cell = board.GetCell(coords);
                    if ( !cell.IsEmpty && (cell.CurFigure.Type == FigureType.King) &&
                         (cell.CurFigure.Color != Color) ) {
                        return false;
                    }
                }
            }
            return true;
        }

        public override List<ChessCoords> GetAvailableTurns(Board board) {
            var res = new List<ChessCoords>();
            ChessCoords coords;
            for ( var i = Coords.X - 1; i <= Coords.X + 1; ++i ) {
                for ( var j = Coords.Y - 1; j <= Coords.Y + 1; ++j ) {
                    coords = new ChessCoords(i, j);
                    if ( coords.IsValid && CanMove(coords, board) ) {
                        res.Add(coords);
                    }
                }
            }

            var y = (Color == ChessColor.White) ? 0 : 7;
            if ( CanMove((coords = new ChessCoords(2, y)), board) ) {
                res.Add(coords);
            }
            if ( CanMove((coords = new ChessCoords(6, y)), board) ) {
                res.Add(coords);
            }
            return res;
        }
    }
}
