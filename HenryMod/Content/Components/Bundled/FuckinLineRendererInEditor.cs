using System;
using UnityEngine;

namespace MoffeinPilot.Content.Components {
    public class FuckinLineRendererInEditor : MonoBehaviour {
        [SerializeField]
        private Transform[] pointTransforms;

        [SerializeField]
        private LineRenderer lineRenderer;

        private Vector3[] points = new Vector3[0];

        void Reset() {
            lineRenderer = GetComponent<LineRenderer>();
        }

        void LateUpdate() {

            if (points.Length != pointTransforms.Length) {
                Array.Resize(ref points, pointTransforms.Length);
            }
            for (int i = 0; i < pointTransforms.Length; i++) {
                points[i] = pointTransforms[i].position;
            }
            lineRenderer?.SetPositions(points);
        }
    }

}
