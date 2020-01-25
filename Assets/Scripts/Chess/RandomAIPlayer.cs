using UnityEngine;

using Chesuto.Cards;
using Chesuto.Events;
using Chesuto.Utils;

namespace Chesuto.Chess {
    // ReSharper disable once InconsistentNaming
    public sealed class RandomAIPlayer : GenericPlayer {
        public RandomAIPlayer(ChessColor color, Deck deck, Game game) : base(color, deck, game) {
            EventManager.Subscribe<PawnReadyToPromote>(OnPawnReadyToPromote);
        }

        ~RandomAIPlayer() {
            EventManager.Unsubscribe<PawnReadyToPromote>(OnPawnReadyToPromote);
        }
        
        public override void StartTurn() {
            base.StartTurn();

            if ( Game.Board.HasAvailableTurns() ) {
                UnityContext.AddUpdateCallback(Update);
            } else {
                Game.Surrender(this);
            }
        }

        public override void EndTurn() {
            base.EndTurn();

            UnityContext.RemoveUpdateCallback(Update);
        }

        void OnPawnReadyToPromote(PawnReadyToPromote ev) {
            if ( ev.Pawn.Color != Color ) {
                return;
            }
            var rand    = Random.Range(0, 4);
            var newRank = FigureType.Unknown;
            switch ( rand ) {
                case 0: {
                    newRank = FigureType.Bishop;
                    break;
                }
                case 1: {
                    newRank = FigureType.Knight;
                    break;
                }
                case 2: {
                    newRank = FigureType.Rook;
                    break;
                }
                case 3: {
                    newRank = FigureType.Queen;
                    break;
                }
            }
            Debug.Assert(newRank != FigureType.Unknown);
            Game.Board.PromotePawn(ev.Pawn, newRank);
        }

        void Update() {
            var availableTurns = Game.Board.GetAvailableTurns();
            Debug.Assert(availableTurns.Count > 0);
            var keys = availableTurns.Keys; // unpleasant allocation
            Figure key = null;
            var count = 0;
            var index = Random.Range(0, keys.Count);
            foreach ( var k in keys ) {
                if ( count == index ) {
                    key = k;
                    break;
                }
                ++count;
            }
            Debug.Assert(key != null);
            var turns = availableTurns[key];
            var turn = turns[Random.Range(0, turns.Count)];
            Game.TryMove(key.Coords, turn);
        }
    }
}
