using UnityEngine;

using System.Collections.Generic;

using Chesuto.Chess;
using Chesuto.Chess.Figures;
using Chesuto.Events;

namespace Chesuto.Cards {
    public sealed class ClaymoreOfRushEffect : GenericPlayerEffect {
        enum State {
            WaitingPlayerTurn,
            WaitingPawnFirstMove,
            WaitingPawnSecondMove,
        }

        readonly ClaymoreOfRush _parentCard;
        
        State _curState;

        Pawn _pawn;
        
        public override PlayerEffectType Type => PlayerEffectType.ClaymoreOfRush;

        public ClaymoreOfRushEffect(ClaymoreOfRush parentCard) {
            _parentCard = parentCard;
        }

        protected override void Init() {
            _curState = State.WaitingPlayerTurn;

            EventManager.Subscribe<TurnStarted>(OnTurnStarted);
            EventManager.Subscribe<CardActivated>(OnCardActivated);
        }

        public override void FilterAvailableTurns(Dictionary<Figure, List<ChessCoords>> availableTurns) {
            if ( _curState == State.WaitingPawnSecondMove ) {
                var toRemove = new HashSet<Figure>();
                var hasPawn  = false;
                foreach ( var pair in availableTurns ) {
                    if ( (pair.Key.Type != FigureType.Pawn) || !(pair.Key is Pawn pawn) || (_pawn != pawn) ) {
                        toRemove.Add(pair.Key);
                    } else {
                        hasPawn = true;
                    }
                }
                if ( hasPawn ) {
                    foreach ( var key in toRemove ) {
                        availableTurns.Remove(key);
                    }
                } else {
                    if ( !Game.TrySpendActions(1) ) {
                        Debug.LogError("Can't take away additional action");
                    }
                    EndEffect();
                }
            }
        }

        public override bool CanActivateCard(CardType cardType) {
            return (_curState != State.WaitingPawnSecondMove);
        }

        protected override void EndEffect() {
            base.EndEffect();

            EventManager.Unsubscribe<TurnStarted>(OnTurnStarted);
            EventManager.Unsubscribe<TurnEnded>(OnTurnEnded);
            EventManager.Unsubscribe<ChessFigureMoved>(OnFigureMoved);
            EventManager.Unsubscribe<CardActivated>(OnCardActivated);
        }

        void OnTurnStarted(TurnStarted ev) {
            if ( ev.PlayerColor != Player.Color ) {
                return;
            }
            _curState = State.WaitingPawnFirstMove;
            
            EventManager.Unsubscribe<TurnStarted>(OnTurnStarted);
            EventManager.Subscribe<TurnEnded>(OnTurnEnded);
            EventManager.Subscribe<ChessFigureMoved>(OnFigureMoved);
        }

        void OnTurnEnded(TurnEnded ev) {
            if ( ev.PlayerColor == Player.Color ) {
                EndEffect();
            }
        }

        void OnCardActivated(CardActivated ev) {
            if ( (Game.CurPlayer == Player) && (ev.Card != _parentCard) ) {
                if ( _curState == State.WaitingPawnSecondMove ) {
                    if ( !Game.TrySpendActions(1) ) {
                        Debug.LogError("Can't take away additional action");
                    }
                }
                EndEffect();
            }
        }
        
        void OnFigureMoved(ChessFigureMoved ev) {
            switch ( _curState ) {
                case State.WaitingPawnFirstMove: {
                    var cell = Game.Board.GetCell(ev.End);
                    if ( (cell == null) || cell.IsEmpty || (cell.CurFigure.Type != FigureType.Pawn) ||
                         (cell.CurFigure.Color != Player.Color) || !(cell.CurFigure is Pawn pawn) ) {
                        EndEffect();
                        break;
                    }
                    Game.AddActions(1);
                    _pawn     = pawn;
                    _curState = State.WaitingPawnSecondMove;
                    EventManager.Fire(new ClaymoreOfRush_SecondMoveStarted());
                    break;
                }
                case State.WaitingPawnSecondMove: {
                    var cell = Game.Board.GetCell(ev.End);
                    if ( (cell == null) || cell.IsEmpty || (cell.CurFigure.Type != FigureType.Pawn) ||
                         (cell.CurFigure.Color != Player.Color) || !(cell.CurFigure is Pawn pawn) || (_pawn != pawn) ) {
                        Debug.LogError("Unexpected figure was moved");
                    }
                    EndEffect();
                    break;
                }
            }
        }
    }
}
