using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Chesuto.Chess;
using Chesuto.Manager;

namespace Chesuto.View {
    public sealed class CellView : MonoBehaviour {
        public Image Background;
        
        public int X;
        public int Y;

        Cell           _cell;
        GameManager    _gameManager;
        FigureViewPool _figureViewPool;

        bool _isCommonInit;
        
        FigureView _figureView;
        
        EventTrigger _eventTrigger;
        
        public ChessCoords Coords => new ChessCoords(X, Y);

        public bool IsFree => _figureView == null;
        
        public Figure CurFigure => IsFree ? null : _figureView.Figure;

        void Reset() {
            Background = GetComponentInChildren<Image>();
        }
        
        void Start() {
            _eventTrigger = GetComponent<EventTrigger>();
            if ( _eventTrigger ) {
                var entry = new EventTrigger.Entry() {
                    eventID = EventTriggerType.PointerUp
                };
                entry.callback.AddListener(_ => OnClick());
                _eventTrigger.triggers.Add(entry);
            }
        }

        public void CommonInit(GameManager gameManager, FigureViewPool figureViewPool) {
            _gameManager    = gameManager;
            _figureViewPool = figureViewPool;

            _isCommonInit = true;
        }

        public void Init(Cell cell) {
            if ( !_isCommonInit ) {
                Debug.LogError("CellView must be CommonInit before Init!");
                return;
            }

            if ( !IsFree ) {
                _figureViewPool.Put(_figureView);
                _figureView = null;
            }
            
            _cell = cell;

            if ( !_cell.IsEmpty ) {
                _figureView = _figureViewPool.Get(x => x.Init(_cell.CurFigure));
                InitFigureView();
            }
        }

        public void SetFigure(FigureView figureView) {
            if ( !IsFree ) {
                _figureViewPool.Put(_figureView);
            }
            _figureView = figureView;
            InitFigureView();
        }

        public FigureView Free() {
            if ( IsFree ) {
                Debug.LogErrorFormat("CellView '{0}' is already free", Coords);
                return null;
            }
            var figureView = _figureView;
            _figureView = null;
            return figureView;
        }

        public void SetSelected(bool isSelected) {
            if ( Background ) {
                Background.color = isSelected ? Color.gray : Color.white;
            }
        }

        public void SetHighlighted(bool isHighlighted) {
            if ( Background ) {
                Background.color = isHighlighted
                    ? (IsFree ? Color.cyan : Color.red)
                    : Color.white;
            }
        }

        void InitFigureView() {
            if ( _figureView ) {
                var figureViewTrans = _figureView.transform;
                figureViewTrans.SetParent(transform);
                figureViewTrans.localPosition = Vector3.zero;
            }
        }

        void OnClick() {
            _gameManager.OnCellClick(Coords);
        }
    }
}
