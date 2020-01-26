using UnityEngine;

using System.Collections.Generic;

using Chesuto.Cards;
using Chesuto.Controller;
using Chesuto.Manager;
using Chesuto.Gameplay.View;

namespace Chesuto.Starter {
    public sealed class GameStarter : MonoBehaviour {
        public SpriteSetup            SpriteSetup;
        public BoardView              BoardView;
        public FigureViewPool         FigureViewPool;
        public DeckPresetSerializable DeckPreset;

        public GameManager GameManager { get; private set; }

        void OnDestroy() {
            GameManager?.Deinit();
        }

        void Start() {
            FigureViewPool.Init(SpriteSetup);
            
            GameManager = new GameManager();
            BoardView.Init(this);

            var gameComps = new HashSet<GameComponent>(GameComponent.Instances);
            foreach ( var gameComp in gameComps ) {
                gameComp.Init(this);
            }

            GameManager.StartGame(GameController.Instance.Deck == null
                ? Deck.FromPreset(DeckPreset.DeckPreset)
                : GameController.Instance.Deck);
        }
    }
}
