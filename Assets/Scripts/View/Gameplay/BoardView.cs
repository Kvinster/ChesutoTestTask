using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

using Chesuto.Chess;
using Chesuto.Events;
using Chesuto.Manager;
using Chesuto.Starter;

namespace Chesuto.Gameplay.View {
    [RequireComponent(typeof(RectTransform))]
    public sealed class BoardView : MonoBehaviour {
        public GameObject CellPrefab;
        public CellView[] CellViews;

        GameManager    _gameManager;
        FigureViewPool _figureViewPool;

        void OnDestroy() {
            if ( _gameManager != null ) {
                _gameManager.StateChangedEvent -= OnStateChanged;
            }
            
            EventManager.Unsubscribe<GameStarted>(OnGameStarted);
            EventManager.Unsubscribe<ChessFigureMoved>(OnChessFigureMoved);
            EventManager.Unsubscribe<ChessFigureCaptured>(OnChessFigureCaptured);
            EventManager.Unsubscribe<ChessFigureRemoved>(OnChessFigureRemoved);
            EventManager.Unsubscribe<ChessFigureSummoned>(OnChessFigureAdded);
            EventManager.Unsubscribe<PawnPromoted>(OnPawnPromoted);
        }

        public void Init(GameStarter gameStarter) {
            _gameManager    = gameStarter.GameManager;
            _figureViewPool = gameStarter.FigureViewPool;

            _gameManager.StateChangedEvent += OnStateChanged;

            foreach ( var cellView in CellViews ) {
                cellView.CommonInit(_gameManager, _figureViewPool);
            }
            
            EventManager.Subscribe<GameStarted>(OnGameStarted);
            EventManager.Subscribe<ChessFigureMoved>(OnChessFigureMoved);
            EventManager.Subscribe<ChessFigureCaptured>(OnChessFigureCaptured);
            EventManager.Subscribe<ChessFigureRemoved>(OnChessFigureRemoved);
            EventManager.Subscribe<ChessFigureSummoned>(OnChessFigureAdded);
            EventManager.Subscribe<PawnPromoted>(OnPawnPromoted);
        }

        void OnStateChanged(GameManager.State newState) {
            switch ( newState  ) {
                case GameManager.State.Idle: {
                    DeselectAll();
                    break;
                }
                case GameManager.State.CellSelected: {
                    DeselectAll();
                    foreach ( var cellView in CellViews ) {
                        if ( cellView.Coords == _gameManager.SelectedCell.Coords ) {
                            cellView.SetSelected(true);
                            if ( cellView.CurFigure != null ) {
                                var turns = _gameManager.GetAvailableTurns(cellView.CurFigure);
                                if ( turns != null ) {
                                    foreach ( var turn in turns ) {
                                        var turnCellView = GetCell(turn);
                                        turnCellView.SetHighlighted(true);
                                    }
                                }
                            }
                        }
                    }
                    break;
                }
                case GameManager.State.SelectingCell_Recruitment: {
                    var color = _gameManager.Game.CurPlayer.Color; // law of Demeter? Never heard of it >_>
                    var yMin  = (color == ChessColor.White) ? 0 : 4;
                    var yMax  = (color == ChessColor.White) ? 3 : 7;
                    for ( var y = yMin; y <= yMax; ++y ) {
                        for ( var x = 0; x < 8; ++x ) {
                            var cellView = GetCell(new ChessCoords(x, y));
                            if ( cellView.IsFree ) {
                                cellView.SetHighlighted(true);
                            }
                        }
                    }
                    break;
                }
                case GameManager.State.SelectingPawn_ClaymoreOfRush: {
                    var color = _gameManager.Game.CurPlayer.Color.Opposite(); // and again?!
                    foreach ( var cellView in CellViews ) {
                        if ( !cellView.IsFree && (cellView.CurFigure.Type == FigureType.Pawn) &&
                             (cellView.CurFigure.Color == color) ) {
                            cellView.SetHighlighted(true);
                        }
                    }
                    break;
                }
                case GameManager.State.PromotingPawn: {
                    DeselectAll();
                    break;
                }
                default: {
                    Debug.LogErrorFormat("Unsupported state '{0}'", newState.ToString());
                    break;
                }
            }
        }

        void DeselectAll() {
            foreach ( var cellView in CellViews ) {
                cellView.SetSelected(false);
            }
        }

        void MoveFigure(ChessCoords start, ChessCoords end) {
            var startCell = GetCell(start);
            if ( startCell == null ) {
                return;
            }
            if ( startCell.IsFree ) {
                Debug.LogErrorFormat("Cell '{0}' is free", start);
                return;
            }
            var endCell = GetCell(end);
            if ( endCell == null ) {
                return;
            }
            var startFigure = startCell.Free();
            endCell.SetFigure(startFigure);
        }

        void OnGameStarted(GameStarted ev) {
            foreach ( var cellView in CellViews ) {
                cellView.Init(_gameManager.GetCell(cellView.Coords));
            }
        }

        void OnChessFigureMoved(ChessFigureMoved ev) {
            MoveFigure(ev.Start, ev.End);
        }

        void OnChessFigureCaptured(ChessFigureCaptured ev) {
            foreach ( var cellView in CellViews ) {
                if ( cellView.CurFigure == ev.Figure ) {
                    cellView.SetFigure(null);
                    break;
                }
            }
        }

        void OnChessFigureRemoved(ChessFigureRemoved ev) {
            foreach ( var cellView in CellViews ) {
                if ( cellView.CurFigure == ev.Figure ) {
                    cellView.SetFigure(null);
                    break;
                }
            }
        }

        void OnChessFigureAdded(ChessFigureSummoned ev) {
            foreach ( var cellView in CellViews ) {
                cellView.SetHighlighted(false);
            }
            var figureCellView = GetCell(ev.Figure.Coords);
            if ( (figureCellView == null) || !figureCellView.IsFree ) {
                Debug.LogErrorFormat("Can't add FigureView on cell {0}", ev.Figure.Coords);
                return;
            }
            figureCellView.SetFigure(_figureViewPool.Get(figureView => figureView.Init(ev.Figure)));
        }

        void OnPawnPromoted(PawnPromoted ev) {
            var cellView = GetCell(ev.Coords);
            if ( (cellView == null) || cellView.IsFree ) {
                return;
            }
            cellView.SetFigure(_figureViewPool.Get(figureView => figureView.Init(ev.NewFigure)));
        }

        CellView GetCell(ChessCoords coords) {
            if ( !coords.IsValid ) {
                return null;
            }
            foreach ( var cellView in CellViews ) {
                if ( cellView.Coords == coords ) {
                    return cellView;
                }
            }
            Debug.LogErrorFormat("CellView for coords '{0}' not found", coords);
            return null;
        }
        
        [ContextMenu("Create Cells")]
        void CreateCells() {
            #if UNITY_EDITOR
            for ( var i = 0; i < transform.childCount; ++i ) {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
            
            CellViews = new CellView[8 * 8];

            var spriteSetup = Resources.Load("SpriteSetup") as SpriteSetup;
            if ( !spriteSetup ) {
                Debug.LogError("Can't find SpriteSetup", this);
                return;
            }
            
            var rect       = (transform as RectTransform).rect;
            var halfWidth  = rect.width / 2f;
            var halfHeight = rect.height / 2f;
            var cellSize   = new Vector2(rect.width / 8f, rect.height / 8f);
            for ( var i = 0; i < 8; ++i ) {
                for ( var j = 0; j < 8; ++j ) {
                    var cellGo = PrefabUtility.InstantiatePrefab(CellPrefab, transform) as GameObject;
                    cellGo.name = $"Cell ({i},{j})";
                    var cellView = cellGo.GetComponent<CellView>();
                    cellView.X = i;
                    cellView.Y = j;
                    var cellViewTrans = cellGo.GetComponent<RectTransform>();
                    cellViewTrans.SetParent(transform);
                    cellViewTrans.sizeDelta = cellSize;
                    cellViewTrans.localPosition = new Vector3(
                        -halfWidth + cellSize.x * (i + 0.5f),
                        -halfHeight + cellSize.y * (j + 0.5f)
                    );
                    var cellViewImage = cellGo.GetComponent<Image>();
                    cellViewImage.sprite = ((i + j) % 2 == 0)
                        ? spriteSetup.BlackCellBackground
                        : spriteSetup.WhiteCellBackground;
                    cellViewImage.SetNativeSize();

                    EditorUtility.SetDirty(cellGo);

                    CellViews[i * 8 + j] = cellView;
                }
            }
            #endif
        }
    }
}
