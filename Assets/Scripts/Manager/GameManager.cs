using UnityEngine;

using System.Collections.Generic;

using Chesuto.Cards;
using Chesuto.Chess;
using Chesuto.Chess.Figures;
using Chesuto.Events;

namespace Chesuto.Manager {
    public sealed class GameManager {
        enum State {
            Idle                         = 0,
            CellSelected                 = 1,
            SelectingCell_Recruitment    = 2,
            SelectingPawn_ClaymoreOfRush = 3,
        }
        
        public readonly Game Game;
        
        State _curState;
        Cell  _selectedCell;

        public Board Board => Game.Board;

        public GameManager() {
            Game = new Game();
            
            _curState = State.Idle;
            EventManager.Subscribe<GameEnded>(OnGameEnded);
        }

        public void StartGame(DeckPreset deckPreset) {
            Game.Start(deckPreset);
        }

        public void Deinit() {
            EventManager.Unsubscribe<GameEnded>(OnGameEnded);

            Game.Deinit();
        }

        public void SkipTurn() {
            if ( Game.SkipTurn() ) {
                if ( _curState == State.CellSelected ) {
                    TryDeselectCell();
                }
            }
        }

        public void TryEndTurn(bool useAction = false) {
            if ( Game.TryEndTurn(useAction) ) {
                TryDeselectCell();
                _curState = State.Idle;
            }
        }

        public Cell GetCell(ChessCoords coords) {
            return Game.Board.GetCell(coords);
        }

        public List<ChessCoords> GetAvailableTurns(Figure figure) {
            return Game.IsActive ? Game.Board.GetAvailableTurns(figure) : null;
        }

        public bool CanActivateCard(CardType cardType) {
            return Game.CanActivateCard(cardType);
        }

        public void TryActivateCard(CardType cardType) {
            switch ( cardType ) {
                case CardType.Recruitment: {
                    TryDeselectCell();
                    if ( Game.TryActivateCard(cardType) ) {
                        _curState = State.SelectingCell_Recruitment;
                    }
                    break;
                }
                case CardType.StrengthenedRise: {
                    if ( Game.TryActivateCard(cardType) ) {
                        TryDeselectCell();
                        _curState = State.Idle;
                        Game.TryEndTurn();
                    }
                    break;
                }
                case CardType.ClaymoreOfRush: {
                    if ( Game.TryActivateCard(cardType) ) {
                        TryDeselectCell();
                        if ( Game.Board.HasFigure(FigureType.Pawn, Game.CurPlayer.Color.Opposite()) ) {
                            _curState = State.SelectingPawn_ClaymoreOfRush;
                        } else {
                            _curState = State.Idle;
                            Game.TryEndTurn();
                        }
                    }
                    break;
                }
                default: {
                    Debug.LogErrorFormat("Unsupported card type '{0}'", cardType.ToString());
                    return;
                }
            }
        }

        public void OnCellClick(ChessCoords coords) {
            var cell = Game.Board.GetCell(coords);
            if ( cell == null ) {
                Debug.LogErrorFormat("Can't get cell {0}", coords);
                return;
            }
            switch ( _curState ) {
                case State.Idle: {
                    TrySelectCell(cell);
                    break;
                }
                case State.CellSelected: {
                    if ( (_selectedCell == cell) || Game.TryMove(_selectedCell.Coords, coords) ) {
                        TryDeselectCell();
                    } else {
                        TryDeselectCell();
                        TrySelectCell(cell);
                    }
                    break;
                }
                case State.SelectingCell_Recruitment: {
                    var isWhite = (Game.CurPlayer.Color == ChessColor.White);
                    var yMin    = isWhite ? 0 : 4;
                    var yMax    = isWhite ? 3 : 7;
                    if ( cell.IsEmpty && (cell.Coords.Y >= yMin) && (cell.Coords.Y <= yMax) &&
                         Game.Board.TrySummonFigure(coords, FigureType.Pawn, Game.CurPlayer.Color) ) {
                        _curState = State.Idle;
                        Game.TryEndTurn();
                    }
                    break;
                }
                case State.SelectingPawn_ClaymoreOfRush: {
                    var color = Game.CurPlayer.Color;
                    if ( !cell.IsEmpty && (cell.CurFigure.Type == FigureType.Pawn) &&
                         (cell.CurFigure.Color == color.Opposite()) ) {
                        Game.Board.RemoveFigure(coords);
                        Game.TryEndTurn();
                        _curState = State.Idle;
                    }
                    break;
                }
                default: {
                    Debug.LogErrorFormat("Unsupported state '{0}'", _curState.ToString());
                    break;
                }
            }
        }

        void TrySelectCell(Cell cell) {
            if ( _selectedCell != null ) {
                Debug.LogErrorFormat("Can't select cell {0}: cell {1} is already selected", _selectedCell.Coords,
                    _selectedCell.Coords);
                return;
            }
            if ( !cell.IsEmpty && (cell.CurFigure.Color == Game.CurPlayer.Color) ) {
                _selectedCell = cell;
                _curState     = State.CellSelected;
                EventManager.Fire(new CellSelected(cell.Coords));
            }
        }

        void TryDeselectCell() {
            if ( _selectedCell != null ) {
                var deselectedCellCoords = _selectedCell.Coords;
                _selectedCell = null;
                _curState     = State.Idle;
                EventManager.Fire(new CellDeselected(deselectedCellCoords));
            }
        }

        void OnGameEnded(GameEnded ev) {
            TryDeselectCell();
        }
    }
}
