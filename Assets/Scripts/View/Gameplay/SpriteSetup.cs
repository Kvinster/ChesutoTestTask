using UnityEngine;

using System;
using System.Collections.Generic;

using Chesuto.Chess;

namespace Chesuto.Gameplay.View {
    [CreateAssetMenu(fileName = "SpriteSetup", menuName = "Create SpriteSetup")]
    public sealed class SpriteSetup : ScriptableObject {
        [Serializable]
        public sealed class FigureSprite {
            #if UNITY_EDITOR
            [HideInInspector]
            public string Name;
            #endif
            public FigureType FigureType = FigureType.Unknown;
            public Sprite     WhiteBoardSprite;
            public Sprite     BlackBoardSprite;
        }

        public Sprite             WhiteCellBackground;
        public Sprite             BlackCellBackground;
        public List<FigureSprite> FigureSprites       = new List<FigureSprite>();

        void OnValidate() {
            #if UNITY_EDITOR
            foreach ( var fs in FigureSprites ) {
                fs.Name = fs.FigureType.ToString();
            }
            #endif
        }

        public Sprite GetFigureBoardSprite(FigureType figureType, ChessColor color) {
            foreach ( var figureSprite in FigureSprites ) {
                if ( figureSprite.FigureType == figureType ) {
                    switch ( color ) {
                        case ChessColor.Black: return figureSprite.BlackBoardSprite;
                        case ChessColor.White: return figureSprite.WhiteBoardSprite;
                        default: {
                            Debug.LogErrorFormat(this, "Unsupported ChessColor '{0}'", color.ToString());
                            return null;
                        }
                    }
                } 
            }
            return null;
        }
    }
}
