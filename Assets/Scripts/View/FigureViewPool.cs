using UnityEngine;

using System;
using System.Collections.Generic;

namespace Chesuto.View {
    public sealed class FigureViewPool : MonoBehaviour {
        public GameObject       FigureViewPrefab;
        public List<FigureView> FigureViews      = new List<FigureView>();

        SpriteSetup _spriteSetup;

        public void Init(SpriteSetup spriteSetup) {
            _spriteSetup = spriteSetup;
            
            foreach ( var figureView in FigureViews ) {
                figureView.CommonInit(_spriteSetup);
                Deinit(figureView);
            }
        }

        public FigureView Get(Action<FigureView> init) {
            if ( FigureViews.Count > 0 ) {
                var figureView = FigureViews[0];
                FigureViews.RemoveAt(0);
                figureView.gameObject.SetActive(true);
                init?.Invoke(figureView);
                return figureView;
            } else {
                var figureViewGO = Instantiate(FigureViewPrefab, Vector3.zero, Quaternion.identity, null);
                var figureView   = figureViewGO.GetComponent<FigureView>();
                if ( figureView ) {
                    figureView.CommonInit(_spriteSetup);
                    Deinit(figureView);
                    figureView.gameObject.SetActive(true);
                    init?.Invoke(figureView);
                    return figureView;
                }
                Debug.LogError("Can't create new FigureView — no FigureView component on prefab");
                return null;
            }
        }

        public void Put(FigureView figureView) {
            if ( FigureViews.Contains(figureView) ) {
                Debug.LogError("Pool already contains this FigureView");
                return;
            }
            Deinit(figureView);
            FigureViews.Add(figureView);
        }

        void Deinit(FigureView figureView) {
            var figureViewTrans = figureView.transform;
            figureViewTrans.SetParent(transform);
            figureViewTrans.localPosition = Vector3.zero;
            figureView.gameObject.SetActive(false);
            figureView.Deinit();
        }

        [ContextMenu("Find FigureViews")]
        void FindFigureViews() {
            GetComponentsInChildren(FigureViews);
        }
    }
}
