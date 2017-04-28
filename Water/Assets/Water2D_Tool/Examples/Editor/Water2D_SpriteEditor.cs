using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditorInternal;

namespace Water2DTool
{
    [CustomEditor(typeof(Water2D_Sprite))]
    public class Water2D_SpriteEditor : Editor
    {

        bool showVisuals = true;

        void OnEnable()
        {
            Water2D_Sprite sprite = (Water2D_Sprite)target;
            if (sprite.GetComponent<MeshFilter>().sharedMesh == null)
                sprite.RebuildMesh();

        }

        // Inspector Fields
        public override void OnInspectorGUI()
        {
            Water2D_Sprite foliage2D = (Water2D_Sprite)target;
            CustomInspector(foliage2D);
        }

        //------------------------------------------------------------------------------

        private void CustomInspector(Water2D_Sprite water2D_sprite)
        {
            showVisuals = EditorGUILayout.Foldout(showVisuals, "Visual Properties");

            if (showVisuals)
            {
                EditorGUI.indentLevel = 1;

                water2D_sprite.pixelsPerUnit = Mathf.Clamp(EditorGUILayout.FloatField("Pixels Per Unit", water2D_sprite.pixelsPerUnit), 1, 768);
                water2D_sprite.widthSegments = Mathf.Clamp(EditorGUILayout.IntField("With Segments", water2D_sprite.widthSegments), 1, 100);
                water2D_sprite.heightSegments = Mathf.Clamp(EditorGUILayout.IntField("Height Segments", water2D_sprite.heightSegments), 1, 100);

#if !(UNITY_4_2 || UNITY_4_1 || UNITY_4_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_1 || UNITY_3_0)
                Type utility = Type.GetType("UnityEditorInternal.InternalEditorUtility, UnityEditor");
                if (utility != null)
                {
                    PropertyInfo sortingLayerNames = utility.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
                    if (sortingLayerNames != null)
                    {
                        string[] layerNames = sortingLayerNames.GetValue(null, null) as string[];
                        string currName = water2D_sprite.GetComponent<Renderer>().sortingLayerName == "" ? "Default" : water2D_sprite.GetComponent<Renderer>().sortingLayerName;
                        int nameID = EditorGUILayout.Popup("Sorting Layer", Array.IndexOf(layerNames, currName), layerNames);

                        water2D_sprite.GetComponent<Renderer>().sortingLayerName = layerNames[nameID];

                    }
                    else
                    {
                        water2D_sprite.GetComponent<Renderer>().sortingLayerID = EditorGUILayout.IntField("Sorting Layer", water2D_sprite.GetComponent<Renderer>().sortingLayerID);
                    }
                }
                else
                {
                    water2D_sprite.GetComponent<Renderer>().sortingLayerID = EditorGUILayout.IntField("Sorting Layer", water2D_sprite.GetComponent<Renderer>().sortingLayerID);
                }
                water2D_sprite.GetComponent<Renderer>().sortingOrder = EditorGUILayout.IntField("Order in Layer", water2D_sprite.GetComponent<Renderer>().sortingOrder);
#endif

            }
            EditorGUI.indentLevel = 0;

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
                water2D_sprite.RebuildMesh();
            }
        }
    }
}
