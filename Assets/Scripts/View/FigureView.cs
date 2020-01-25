using UnityEngine;
using UnityEngine.UI;

using Chesuto.Chess;

namespace Chesuto.View {
    [RequireComponent(typeof(Image))]
    public sealed class FigureView : MonoBehaviour {
        SpriteSetup _spriteSetup;
        Image       _image;
        bool        _isCommonInit;

        public Figure Figure { get; private set; }

        public void CommonInit(SpriteSetup spriteSetup) {
            if ( _isCommonInit ) {
                return;
            }
            _image = GetComponent<Image>();
            _spriteSetup = spriteSetup;

            _isCommonInit = true;
        }

        public void Init(Figure figure) {
            if ( !_isCommonInit ) {
                Debug.LogError("FigureView must be CommonInit before Init is called");
                return;
            }
            Figure       = figure;
            _image.sprite = _spriteSetup.GetFigureBoardSprite(Figure.Type, Figure.Color);
        }

        public void Deinit() {
            if ( !_isCommonInit ) {
                Debug.LogError("FigureView must be CommonInit before Deinit is called");
                return;
            }
            Figure       = null;
            _image.sprite = null;
        }
    }
}
