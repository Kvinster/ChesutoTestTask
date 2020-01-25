using UnityEngine;

using System;
using System.Collections.Generic;

using Chesuto.Cards;
using Chesuto.Chess;
using Chesuto.Events;

namespace Chesuto.Manager {
    public sealed class GameManager {
        public enum State {
            Idle                         = 0,
            CellSelected                 = 1,
            SelectingCell_Recruitment    = 2,
            SelectingPawn_ClaymoreOfRush = 3,
            PromotingPawn                = 4,
        }
        
        public readonly Game Game;

        public event Action<State> StateChangedEvent;

        State _curState;

        State CurState {
            get => _curState;
            set {
                _curState = value;

                StateChangedEvent?.Invoke(_curState);
            }
        }

        public Cell SelectedCell { get; private set; }
        
        public Board Board => Game.Board;

        public GameManager() {
            Game = new Game();
            
            CurState = State.Idle;
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
                if ( CurState == State.CellSelected ) {
                    TryDeselectCell();
                }
            }
        }

        public void TryEndTurn(bool useAction = false) {
            if ( Game.TryEndTurn(useAction) ) {
                TryDeselectCell();
                CurState = State.Idle;
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

        public void ResetState() {
            switch ( CurState ) {
                case State.Idle: {
                    break;
                }
                case State.CellSelected:
                case State.SelectingCell_Recruitment:
                case State.SelectingPawn_ClaymoreOfRush: {
                    TryDeselectCell();
                    CurState = State.Idle;
                    break;
                }
                default: {
                    Debug.LogErrorFormat("Unsupported state '{0}'", CurState.ToString());
                    break;
                }
            }
        }

        public void TryActivateCard(CardType cardType) {
            if ( (CurState != State.Idle) && (CurState != State.CellSelected) ) {
                TryDeselectCell();
                CurState = State.Idle;
            }
            switch ( cardType ) {
                case CardType.Recruitment: {
                    TryDeselectCell();
                    if ( Game.CanActivateCard(cardType) ) {
                        CurState = State.SelectingCell_Recruitment;
                    }
                    break;
                }
                case CardType.StrengthenedRise: {
                    if ( Game.TryActivateCard(cardType) ) {
                        TryDeselectCell();
                        CurState = State.Idle;
                        Game.TryEndTurn();
                    }
                    break;
                }
                case CardType.ClaymoreOfRush: {
                    if ( Game.CanActivateCard(cardType) ) {
                        TryDeselectCell();
                        if ( Game.Board.HasFigure(FigureType.Pawn, Game.CurPlayer.Color.Opposite()) ) {
                            CurState = State.SelectingPawn_ClaymoreOfRush;
                        } else if ( Game.TryActivateCard(cardType) ) {
                            CurState = State.Idle;
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
            switch ( CurState ) {
                case State.Idle: {
                    TrySelectCell(cell);
                    break;
                }
                case State.CellSelected: {
                    if ( (SelectedCell == cell) || Game.TryMove(SelectedCell.Coords, coords) ) {
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
                         Game.TryActivateCard(CardType.Recruitment) &&
                         Game.Board.TrySummonFigure(coords, FigureType.Pawn, Game.CurPlayer.Color) ) {
                        CurState = State.Idle;
                        Game.TryEndTurn();
                    }
                    break;
                }
                case State.SelectingPawn_ClaymoreOfRush: {
                    var color = Game.CurPlayer.Color;
                    if ( !cell.IsEmpty && (cell.CurFigure.Type == FigureType.Pawn) &&
                         (cell.CurFigure.Color == color.Opposite()) && Game.TryActivateCard(CardType.ClaymoreOfRush) ) {
                        Game.Board.RemoveFigure(coords);
                        Game.TryEndTurn();
                        CurState = State.Idle;
                    }
                    break;
                }
                default: {
                    Debug.LogErrorFormat("Unsupported state '{0}'", CurState.ToString());
                    break;
                }
            }
        }

        void TrySelectCell(Cell cell) {
            if ( SelectedCell != null ) {
                Debug.LogErrorFormat("Can't select cell {0}: cell {1} is already selected", SelectedCell.Coords,
                    SelectedCell.Coords);
                return;
            }
            if ( !cell.IsEmpty && (cell.CurFigure.Color == Game.CurPlayer.Color) ) {
                SelectedCell = cell;
                CurState     = State.CellSelected;
                EventManager.Fire(new CellSelected(cell.Coords));
            }
        }

        void TryDeselectCell() {
            if ( SelectedCell != null ) {
                var deselectedCellCoords = SelectedCell.Coords;
                SelectedCell = null;
                CurState     = State.Idle;
                EventManager.Fire(new CellDeselected(deselectedCellCoords));
            }
        }

        void OnGameEnded(GameEnded ev) {
            TryDeselectCell();
        }
    }
}
