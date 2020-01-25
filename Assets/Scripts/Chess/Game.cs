using UnityEngine;

using System.Collections.Generic;

using Chesuto.Cards;
using Chesuto.Events;

namespace Chesuto.Chess {
    public sealed class Game {
        public readonly Board Board;

        GenericPlayer _whitePlayer;
        GenericPlayer _blackPlayer;

        int _actionsLeft;

        readonly List<object> _pausedBy = new List<object>();
        
        public GenericPlayer CurPlayer { get; private set; }

        public bool IsActive    { get; private set; }

        public int  ActionsLeft {
            get => _actionsLeft;
            private set {
                var newVal = Mathf.Max(value, 0);
                if ( _actionsLeft != newVal ) {
                    _actionsLeft = newVal;
                    EventManager.Fire(new GameActionsLeftChanged(_actionsLeft));
                }
            }
        }
        
        public bool HasActions => (ActionsLeft > 0);
        
        bool IsPaused => (_pausedBy.Count > 0);

        public Game() {
            Board = new Board();
            Board.Reset();
        }

        public void Start(DeckPreset deckPreset) {
            Board.Reset();
            
            _whitePlayer = new HumanPlayer(ChessColor.White, Deck.FromPreset(deckPreset), this);
            _blackPlayer = new RandomAIPlayer(ChessColor.Black, Deck.FromPreset(deckPreset), this);

            IsActive  = true;
            NextTurn();
            EventManager.Fire(new GameStarted());
            
            EventManager.Subscribe<ChessKingRemoved>(OnKingRemoved);
        }

        public void Deinit() {
            EventManager.Unsubscribe<ChessKingRemoved>(OnKingRemoved);
        }

        public bool SkipTurn() {
            if ( !IsActive ) {
                return false;
            }
            CurPlayer.DrawCard();
            NextTurn();
            return true;
        }

        public bool TryMove(ChessCoords start, ChessCoords end) {
            if ( !IsActive || !HasActions ) {
                return false;
            }
            if ( Board.TryMove(start, end, CurPlayer.Color) ) {
                if ( IsActive ) {
                    var turnEnded = TryEndTurn(true);
                    if ( !turnEnded ) {
                        Board.UpdateAvailableTurns();
                    }
                }
                return true;
            }
            return false;
        }

        public bool CanActivateCard(CardType cardType) {
            return HasActions && CurPlayer.CanActivateCard(cardType);
        }

        public bool TryActivateCard(CardType cardType) {
            --ActionsLeft;
            return CurPlayer.TryActivateCard(cardType);
        }

        public bool CanEndTurn() {
            return !HasActions && !IsPaused && CurPlayer.CanEndTurn();
        }

        public bool TryEndTurn(bool useAction = false) {
            if ( useAction ) {
                Debug.Assert(HasActions);
                --ActionsLeft;
            }
            if ( CanEndTurn() ) {
                NextTurn();
                return true;
            }
            return false;
        }

        public GenericPlayer GetPlayer(ChessColor color) {
            if ( !IsActive ) {
                return null;
            }
            switch ( color ) {
                case ChessColor.White: return _whitePlayer;
                case ChessColor.Black: return _blackPlayer;
                default: {
                    Debug.LogErrorFormat("Unsupported color '{0}'", color.ToString());
                    return null;
                }
            }
        }

        public void Surrender(GenericPlayer player) {
            End(player.Color.Opposite());
        }

        public void Pause(object initiator) {
            _pausedBy.Add(initiator);
        }

        public void Unpause(object obj) {
            _pausedBy.Remove(obj);
        }

        public void AddActions(int addActions) {
            ActionsLeft += addActions;
        }

        public bool TrySpendActions(int subActions) {
            if ( ActionsLeft >= subActions ) {
                ActionsLeft -= subActions;
                TryEndTurn();
                return true;
            }
            return false;
        }

        void NextTurn() {
            if ( CurPlayer == null ) {
                CurPlayer = _whitePlayer;
            } else {
                var prevPlayerColor = CurPlayer.Color;
                CurPlayer.EndTurn();
                CurPlayer = (CurPlayer == _whitePlayer) ? _blackPlayer : _whitePlayer;
                EventManager.Fire(new TurnEnded(prevPlayerColor));
            }
            ActionsLeft = 1;
            Board.StartTurn(CurPlayer);
            CurPlayer.StartTurn();
            EventManager.Fire(new TurnStarted(CurPlayer.Color));
        }

        void End(ChessColor winner) {
            IsActive = false;
            EventManager.Fire(new GameEnded(winner));
        }

        void OnKingRemoved(ChessKingRemoved ev) {
            // anything that can prevent the end of the game?
            End(ev.KingColor.Opposite());
        }
    }
}