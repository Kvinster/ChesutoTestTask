﻿using UnityEngine;
using UnityEngine.UI;

using Chesuto.Chess;
using Chesuto.Chess.Figures;
using Chesuto.Events;
using Chesuto.Starter;

namespace Chesuto.View.UI {
    public sealed class PromotionMenuController : GameComponent {
        public GameObject MenuRoot;
        [Space]
        public Button KnightButton;
        public Button BishopButton;
        public Button RookButton;
        public Button QueenButton;

        Board _board;

        Pawn _pawn;

        void OnDestroy() {
            EventManager.Unsubscribe<PawnReadyToPromote>(OnPawnReadyToPromote);
        }

        public override void Init(GameStarter gameStarter) {
            _board = gameStarter.GameManager.Board;

            KnightButton.onClick.AddListener(() => OnFigureButtonClick(FigureType.Knight));
            BishopButton.onClick.AddListener(() => OnFigureButtonClick(FigureType.Bishop));
            RookButton.onClick.AddListener(() => OnFigureButtonClick(FigureType.Rook));
            QueenButton.onClick.AddListener(() => OnFigureButtonClick(FigureType.Queen));

            Hide();
            
            EventManager.Subscribe<PawnReadyToPromote>(OnPawnReadyToPromote);
        }

        void OnFigureButtonClick(FigureType figureType) {
            if ( _board.PromotePawn(_pawn, figureType) ) {
                _pawn = null;
                Hide();
            }
        }

        void Show() {
            MenuRoot.SetActive(true);
        }

        void Hide() {
            MenuRoot.SetActive(false);
        }

        void OnPawnReadyToPromote(PawnReadyToPromote ev) {
            _pawn = ev.Pawn;
            Show();
        }
    }
}
