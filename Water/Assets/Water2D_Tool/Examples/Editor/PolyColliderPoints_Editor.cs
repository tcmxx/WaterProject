using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Water2DTool
{
    // Will display the indexes of a Polygon Collider 2D in the Scene View.
    [CustomEditor(typeof(PolyColliderPoints))]
    public class PolyColliderPoints_Editor : Editor
    {
        private PolygonCollider2D poly;
        private Vector2[] polyPoints;

        private void OnSceneGUI()
        {
            PolyColliderPoints polyCollider = (PolyColliderPoints)target;

            poly = polyCollider.GetComponent<PolygonCollider2D>();
            polyPoints = poly.points;

            if (polyCollider.showIndices)
            {
                for (int i = 0; i < polyPoints.Length; i++)
                {
                    Vector3 labelPos = polyCollider.transform.TransformPoint(polyPoints[i]);
                    Handles.color = Color.white;
                    Handles.Label(labelPos, " " + i);
                    Handles.color = new Color(1, 1, 1, 0);
                }
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
