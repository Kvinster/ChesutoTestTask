using System;

using UnityEngine;

namespace Chesuto.Chess.Figures {
    public static class FigureFactory {
        public static Figure FromChar(char c) {
            var color = char.IsUpper(c) ? ChessColor.White : ChessColor.Black;
            var uniC  = char.ToLower(c);
            switch ( uniC ) {
                case 'x': return null;
                case 'p': return new Pawn(color);
                case 'r': return new Rook(color);
                case 'n': return new Knight(color);
                case 'b': return new Bishop(color);
                case 'q': return new Queen(color);
                case 'k': return new King(color);
                default: {
                    Debug.LogErrorFormat("Unsupported character '{0}'", c);
                    return null;
                }
            }
        }

        public static Figure FromType(FigureType type, ChessColor color) {
            switch ( type ) {
                case FigureType.Pawn:  return new Pawn(color);
                case FigureType.Rook:  return new Rook(color);
                case FigureType.Knight: return new Knight(color);
                case FigureType.Bishop: return new Bishop(color);
                case FigureType.Queen:  return new Queen(color);
                case FigureType.King:   return new King(color);
                default: {
                    Debug.LogErrorFormat("Unsupported type '{0}'", type.ToString());
                    return null;
                }
            }
        }
    }
}
