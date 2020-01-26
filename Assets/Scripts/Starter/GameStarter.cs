using UnityEngine;

using System.Collections.Generic;

using Chesuto.Cards;
using Chesuto.Manager;
using Chesuto.Gameplay.View;

namespace Chesuto.Starter {
    public sealed class GameStarter : MonoBehaviour {
        public SpriteSetup    SpriteSetup;
        public BoardView      BoardView;
        public FigureViewPool FigureViewPool;
        public DeckPreset     DeckPreset;

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

            GameManager.StartGame(DeckPreset);
        }
    }
}
