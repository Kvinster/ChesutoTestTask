using UnityEngine;
using UnityEditor;

using Chesuto.Gameplay.View;

namespace Chesuto.Editor {
    [CustomEditor(typeof(CellView))]
    public sealed class CellViewEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            var target = this.target as CellView;
            if ( !target ) {
                return;
            }
            var enabled = GUI.enabled;
            GUI.enabled = false;
            DrawDefaultInspector();
            GUI.enabled = enabled;
        }
    }
}
