using UnityEngine;

using System.Collections.Generic;

using Chesuto.Chess.Figures;
using Chesuto.Events;

namespace Chesuto.Chess {
    public sealed class Board {
        static readonly string[] InitLayout = {
            "rnbqkbnr",
            "pppppppp",
            "xxxxxxxx",
            "xxxxxxxx",
            "xxxxxxxx",
            "xxxxxxxx",
            "PPPPPPPP",
            "RNBQKBNR"
        };

        readonly Cell[,] _cells;

        readonly Dictionary<Figure, List<ChessCoords>> _availableTurns = new Dictionary<Figure, List<ChessCoords>>();

        readonly List<FigureInfo> _capturedFigures = new List<FigureInfo>();
        readonly List<FigureInfo> _removedFigures  = new List<FigureInfo>();
        readonly Pawn[]           _passantPawns    = new Pawn[2];

        GenericPlayer _curPlayer;

        public Board() {
            _cells = new Cell[8, 8];
            for ( var i = 0; i < 8; ++i ) {
                for ( var j = 0; j < 8; ++j ) {
                    _cells[i, j] = new Cell(new ChessCoords(i, j));
                }
            }
        }
        
        public void Reset() {
            _curPlayer = null;

            _availableTurns.Clear();
            
            _capturedFigures.Clear();
            _removedFigures .Clear();
            
            _passantPawns[0] = null;
            _passantPawns[1] = null;
            
            for ( var i = 0; i < 8; ++i ) {
                for ( var j = 0; j < 8; ++j ) {
                    var c = InitLayout[i][j];
                    _cells[j, 7 - i].SetFigure(FigureFactory.FromChar(c));
                }
            }
        }

        public Cell GetCell(ChessCoords coords) {
            if ( !coords.IsValid ) {
                Debug.LogFormat("Invalid coords: {0}", coords);
                return null;
            }
            return _cells[coords.X, coords.Y];
        }

        public Cell GetCell(int x, int y) {
            return GetCell(new ChessCoords(x, y));
        }

        public void StartTurn(GenericPlayer player) {
            _curPlayer = player;
            
            UpdateAvailableTurns();
            
            var isCheck = IsCheck(_curPlayer.Color);
            if ( _availableTurns.Count == 0 ) {
                if ( isCheck ) {
                    EventManager.Fire(new ChessCheckmate(_curPlayer.Color));
                } else {
                    EventManager.Fire(new ChessStalemate(_curPlayer.Color));
                }
            } else if ( isCheck ) {
                EventManager.Fire(new ChessCheck(_curPlayer.Color));
            }
        }

        public bool TrySummonFigure(ChessCoords coords, FigureType figureType, ChessColor color) {
            var cell = GetCell(coords);
            if ( cell == null ) {
                Debug.LogErrorFormat("Invalid coords '{0}'", coords);
                return false;
            }
            if ( !cell.IsEmpty ) {
                Debug.LogErrorFormat("Can't summon to {0} — the cell is busy", coords);
                return false;
            }
            var figure = FigureFactory.FromType(figureType, color);
            cell.SetFigure(figure);
            UpdateAvailableTurns();
            EventManager.Fire(new ChessFigureSummoned(figure));
            return true;
        }

        public bool TryMove(ChessCoords start, ChessCoords end, ChessColor curPlayerColor) {
            if ( !CanMove(start, end, curPlayerColor) ) {
                return false;
            }
            var startCell = GetCell(start);
            var endCell   = GetCell(end);
            var figure    = startCell.CurFigure;
            
            switch ( figure.Type ) {
                case FigureType.Rook when (figure is Rook rook): {
                    rook.Moved = true;
                    break;
                }
                case FigureType.King when (figure is King king): {
                    if ( !king.Moved && (end.Y == king.Coords.Y) ) {
                        Cell rookCell    = null;
                        Cell endRookCell = null;
                        switch ( end.X ) {
                            case 2: {
                                rookCell    = GetCell(0, end.Y);
                                endRookCell = GetCell(3, end.Y);
                                break;
                            }
                            case 6: {
                                rookCell    = GetCell(7, end.Y);
                                endRookCell = GetCell(5, end.Y);
                                break;
                            }
                        }
                        if ( (rookCell != null) && !rookCell.IsEmpty && (rookCell.CurFigure.Type == FigureType.Rook) &&
                             (rookCell.CurFigure is Rook rook) && (endRookCell != null) && endRookCell.IsEmpty ) {
                            rook.Moved = true;
                            rookCell.SetFigure(null);
                            endRookCell.SetFigure(rook);
                            EventManager.Fire(new ChessFigureMoved(rookCell.Coords, endRookCell.Coords));
                        }
                    }
                    
                    king.Moved = true;
                    break;
                }
            }
            
            startCell.SetFigure(null);
            if ( !endCell.IsEmpty ) {
                CaptureFigure(endCell);
            }
            endCell.SetFigure(figure);

            var passantIndex = (curPlayerColor == ChessColor.White) ? 0 : 1;
            var passantPawn  = _passantPawns[passantIndex];
            if ( passantPawn != null ) {
                passantPawn.JustMovedTwoSquares = false;
                _passantPawns[passantIndex] = null;
            }
            Pawn promotePawn = null;
            if ( (figure.Type == FigureType.Pawn) && (figure is Pawn pawn) ) {
                if ( Mathf.Abs(end.Y - start.Y) == 2 ) {
                    pawn.JustMovedTwoSquares = true;
                    _passantPawns[passantIndex] = pawn;
                } else if ( Mathf.Abs(end.X - start.X) == 1 ) {
                    var yInc = (curPlayerColor == ChessColor.White) ? -1 : 1; // opposite direction
                    var passantCell = GetCell(end.X, end.Y + yInc);
                    if ( (passantCell != null) && !passantCell.IsEmpty &&
                         (passantCell.CurFigure.Type == FigureType.Pawn) &&
                         (passantCell.CurFigure is Pawn victimPawn) && victimPawn.JustMovedTwoSquares ) {
                        CaptureFigure(passantCell);
                        passantIndex = 1 - passantIndex;
                        Debug.Assert(_passantPawns[passantIndex] == victimPawn);
                        _passantPawns[passantIndex] = null;
                    }
                }
                var endY = (pawn.Color == ChessColor.White) ? 7 : 0;
                if ( end.Y == endY ) {
                    promotePawn = pawn;
                }
            }
            EventManager.Fire(new ChessFigureMoved(start, end));
            if ( promotePawn != null ) {
                EventManager.Fire(new PawnReadyToPromote(promotePawn));
            }
            return true;
        }

        public bool CanMove(ChessCoords start, ChessCoords end, ChessColor curPlayerColor) {
            if ( start == end ) {
                return false;
            }
            var startCell = GetCell(start);
            if ( (startCell == null) || startCell.IsEmpty || (startCell.CurFigure.Color != curPlayerColor) ) {
                return false;
            }
            if ( _availableTurns.ContainsKey(startCell.CurFigure) ) {
                var figureTurns = _availableTurns[startCell.CurFigure];
                foreach ( var turn in figureTurns ) {
                    if ( turn == end ) {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool HasFigure(FigureType figureType, ChessColor figureColor) {
            foreach ( var cell in _cells ) {
                if ( !cell.IsEmpty && (cell.CurFigure.Type == figureType) && (cell.CurFigure.Color == figureColor) ) {
                    return true;
                }
            }
            return false;
        }

        public bool IsCheck(ChessCoords start, ChessCoords end) {
            var startCell = GetCell(start);
            if ( (startCell == null) || startCell.IsEmpty ) {
                Debug.LogErrorFormat("Invalid start cell '{0}'", start);
                return false;
            }
            var endCell = GetCell(end);
            if ( (endCell == null) || (!endCell.IsEmpty && (endCell.CurFigure.Color == startCell.CurFigure.Color)) ) {
                Debug.LogErrorFormat("Invalid end cell '{0}'", end);
                return false;
            }
            var endFigure   = endCell.CurFigure;
            var startFigure = startCell.CurFigure;
            endCell.SetFigure(startFigure);
            startCell.SetFigure(null);
            
            // check for check
            var res = IsCheck(startFigure.Color);

            endCell.SetFigure(endFigure);
            startCell.SetFigure(startFigure);
            
            return res;
        }

        public bool IsCheck(ChessColor playerColor) {
            Cell kingCell = null;
            for ( var i = 0; (i < 8) && (kingCell == null); ++i ) {
                for ( var j = 0; j < 8; ++j ) {
                    var cell = _cells[i, j];
                    if ( !cell.IsEmpty && (cell.CurFigure.Type == FigureType.King) && (cell.CurFigure.Color == playerColor) ) {
                        kingCell = cell;
                        break;
                    }
                }
            }
            if ( kingCell == null ) {
                return false;
            }
            for ( var i = 0; i < 8; ++i ) {
                for ( var j = 0; j < 8; ++j ) {
                    var cell = _cells[i, j];
                    if ( !cell.IsEmpty && (cell.CurFigure.Color != playerColor) ) {
                        if ( cell.CurFigure.CanMove(kingCell.Coords, this) ) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool PromotePawn(Pawn pawn, FigureType newRank) {
            var pawnCell = GetCell(pawn.Coords);
            if ( (pawnCell == null) || pawnCell.IsEmpty || (pawnCell.CurFigure.Type != FigureType.Pawn) ||
                 !(pawnCell.CurFigure is Pawn cellPawn) || (cellPawn != pawn) ) {
                return false;
            }
            var newFigure = FigureFactory.FromType(newRank, pawn.Color);
            pawnCell.SetFigure(newFigure);
            EventManager.Fire(new PawnPromoted(pawnCell.Coords, newFigure));
            return true;
        }

        public void RemoveFigure(ChessCoords coords) {
            var cell = GetCell(coords);
            if ( (cell == null) || cell.IsEmpty ) {
                Debug.LogErrorFormat("Invalid coords '{0}'", coords);
                return;
            }
            _removedFigures.Add(new FigureInfo(cell.CurFigure.Type, cell.CurFigure.Color));
            var figure = cell.CurFigure;
            cell.SetFigure(null);
            EventManager.Fire(new ChessFigureRemoved(figure));
            if ( figure.Type == FigureType.King ) {
                EventManager.Fire(new ChessKingRemoved(figure.Color));
            }
        }

        public List<ChessCoords> GetAvailableTurns(Figure figure) {
            return _availableTurns.TryGetValue(figure, out var turns) ? new List<ChessCoords>(turns) : null;
        }

        public Dictionary<Figure, List<ChessCoords>> GetAvailableTurns() {
            return _availableTurns;
        }

        public bool HasAvailableTurns() {
            return (_availableTurns.Count > 0);
        }

        public void UpdateAvailableTurns() {
            _availableTurns.Clear();
            foreach ( var cell in _cells ) {
                if ( !cell.IsEmpty && (cell.CurFigure.Color == _curPlayer.Color) ) {
                    var turns = GetAvailableTurns(cell);
                    if ( (turns != null) && (turns.Count > 0) ) {
                        _availableTurns.Add(cell.CurFigure, turns);
                    }
                }
            }
            _curPlayer.FilterAvailableTurns(_availableTurns);
        }

        List<ChessCoords> GetAvailableTurns(Cell figureCell) {
            if ( (figureCell == null) || figureCell.IsEmpty ) {
                Debug.LogErrorFormat("Invalid cell '{0}'", figureCell?.Coords);
                return null;
            }
            var figure = figureCell.CurFigure;
            var figureTurns = figure.GetAvailableTurns(this);
            for ( var i = figureTurns.Count - 1; i >= 0; --i ) {
                if ( IsCheck(figure.Coords, figureTurns[i]) ) {
                    figureTurns.RemoveAt(i);
                }
            }
            return figureTurns;
        }

        void CaptureFigure(Cell cell) {
            if ( (cell == null) || cell.IsEmpty ) {
                Debug.LogErrorFormat("Invalid cell '{0}'", cell?.Coords);
                return;
            }
            _capturedFigures.Add(new FigureInfo(cell.CurFigure.Type, cell.CurFigure.Color));
            var figure = cell.CurFigure;
            cell.SetFigure(null);
            EventManager.Fire(new ChessFigureCaptured(figure));
            if ( figure.Type == FigureType.King ) {
                EventManager.Fire(new ChessKingRemoved(figure.Color));
            }
        }
    }
}
