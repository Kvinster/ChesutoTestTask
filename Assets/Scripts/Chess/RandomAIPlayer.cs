using Chesuto.Cards;
using Chesuto.Utils;

using UnityEngine;

namespace Chesuto.Chess {
    // ReSharper disable once InconsistentNaming
    public sealed class RandomAIPlayer : GenericPlayer {
        public RandomAIPlayer(ChessColor color, Deck deck, Game game) : base(color, deck, game) { }
        
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
