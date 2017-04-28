using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Water2DTool
{
    [CustomEditor(typeof(Water2D_Tool))]
    public class Water2D_ToolEditor : Editor
    {
        private Texture2D texDot;
        private List<int> selectedPoints = new List<int>();
        private bool prevChanged = false;
        private bool showVisuals = true;
        private bool showCollider = true;
        private static float pathScale = 1f;

        void OnEnable()
        {
			texDot = GetGizmo("water2D_circle.png");
            selectedPoints.Clear();
            Water2D_Tool water2D_Tool = (Water2D_Tool)target;
            if (water2D_Tool.GetComponent<MeshFilter>().sharedMesh == null)
                water2D_Tool.RecreateWaterMesh();
        }

        void OnSceneGUI()
        {
            Water2D_Tool water2D_Tool = (Water2D_Tool)target;

            EditorUtility.SetSelectedWireframeHidden(water2D_Tool.gameObject.GetComponent<Renderer>(), water2D_Tool.showMesh);

            GUIStyle iconStyle = new GUIStyle();
            iconStyle.alignment = TextAnchor.MiddleCenter;

            // Setup undoing things
            Undo.RecordObject(target, "Handles Position");

            if (Event.current.type == EventType.repaint)
            {
                DrawRectangle(water2D_Tool);
                DrawFlowDirection(water2D_Tool);
            }

            // Draw and interact with the handles.
            UpdateHandles(water2D_Tool, iconStyle);

            if (Event.current.type == EventType.repaint && water2D_Tool.use3DCollider)
                DrawOverlapSphereGizmo(water2D_Tool);

            // Update everything that relies on the handles, if the GUI changed.
            if (GUI.changed)
            {
                water2D_Tool.RecreateWaterMesh();
                EditorUtility.SetDirty(target);
                prevChanged = true;
            }
            else if (Event.current.type == EventType.used)
            {
                if (prevChanged == true)
                {
                    water2D_Tool.RecreateWaterMesh();
                }
                prevChanged = false;
            }
        }

        public override void OnInspectorGUI()
        {
            Undo.RecordObject(target, "Modified Inspector");

            Water2D_Tool water2D = (Water2D_Tool)target;

            // Render the custom inspector fields.
            CustomInspector(water2D);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
                water2D.RecreateWaterMesh();
            }

            if (Event.current.type == EventType.ValidateCommand)
            {
                switch (Event.current.commandName)
                {
                    case "UndoRedoPerformed":
                        water2D.RecreateWaterMesh();
                        break;
                }
            }

            // A button for updating the Transform Position of the water object.
            if (GUILayout.Button("Center Position"))
                water2D.ReCenterPivotPoint();

        }

        private void DrawFlowDirection(Water2D_Tool water)
        {
            Handles.color = Color.white;
            Water2D_Simulation sim = water.GetComponent<Water2D_Simulation>();
            float angleInDeg = 0;

            if (sim.waterFlow)
            {
                if (sim.useAngles)
                {
                    angleInDeg = sim.flowAngle;
                    Handles.ArrowCap(0, water.transform.position, Quaternion.Euler(new Vector3(angleInDeg, 90f, 0)), 1.5f);
                }
            }
        }

        private void DrawOverlapSphereGizmo(Water2D_Tool water)
        {
            UnityEditor.Handles.color = Color.blue;
            Vector3[] vertices = water.GetComponent<MeshFilter>().sharedMesh.vertices;
            Water2D_Simulation water2D_Sim = water.GetComponent<Water2D_Simulation>();
            int surfaceVertsCount = water.surfaceVertsCount / 2; ;

            for (int i = 0; i < surfaceVertsCount; i++)
            {
                Vector3 vertexGlobalPos = water.transform.TransformPoint(vertices[i + surfaceVertsCount]);
                UnityEditor.Handles.DrawWireDisc(new Vector3(vertexGlobalPos.x, vertexGlobalPos.y, vertexGlobalPos.z + water.boxColliderZOffset), Vector3.up, water2D_Sim.overlapSphereRadius);
                UnityEditor.Handles.DrawWireDisc(new Vector3(vertexGlobalPos.x, vertexGlobalPos.y, vertexGlobalPos.z + water.boxColliderZOffset), Vector3.forward, water2D_Sim.overlapSphereRadius);
            }
        }

        //------------------------------------------------------------------------------

        private void CustomInspector(Water2D_Tool water)
        {
            showVisuals = EditorGUILayout.Foldout(showVisuals, "Visual Properties");

            if (showVisuals)
            {
                EditorGUI.indentLevel = 1;
                InspectorBox(10, () =>
                {
                    water.showMesh = !EditorGUILayout.Toggle(new GUIContent("Show Mesh", "Shows or hides the mesh of the water in the scene view"), !water.showMesh);

                    water.verticalTiling = EditorGUILayout.Toggle(new GUIContent("Vertical Tiling", "When enabled will tile the texture horizontally "
                        + "if the water height is greater than the max water height that can be created with the current texture."), water.verticalTiling);

                    water.pixelsPerUnit = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Pixels To Units", "The number of pixels in 1 Unity unit."), water.pixelsPerUnit), 1, 768);

                    water.segmentsPerUnit = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Segments To Units", "The number of horizontal segments "
                        + "that should fit in 1 Unity unit."), water.segmentsPerUnit), 0, 100);

                    Type utility = Type.GetType("UnityEditorInternal.InternalEditorUtility, UnityEditor");
                    if (utility != null)
                    {
                        PropertyInfo sortingLayerNames = utility.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
                        if (sortingLayerNames != null)
                        {
                            string[] layerNames = sortingLayerNames.GetValue(null, null) as string[];
                            string currName = water.GetComponent<Renderer>().sortingLayerName == "" ? "Default" : water.GetComponent<Renderer>().sortingLayerName;
                            int nameID = EditorGUILayout.Popup("Sorting Layer", Array.IndexOf(layerNames, currName), layerNames);

                            water.GetComponent<Renderer>().sortingLayerName = layerNames[nameID];

                        }
                        else
                        {
                            water.GetComponent<Renderer>().sortingLayerID = EditorGUILayout.IntField("Sorting Layer", water.GetComponent<Renderer>().sortingLayerID);
                        }
                    }
                    else
                    {
                        water.GetComponent<Renderer>().sortingLayerID = EditorGUILayout.IntField("Sorting Layer", water.GetComponent<Renderer>().sortingLayerID);
                    }
                    water.GetComponent<Renderer>().sortingOrder = EditorGUILayout.IntField("Order in Layer", water.GetComponent<Renderer>().sortingOrder);

                    water.handleScale = EditorGUILayout.Slider(new GUIContent("Handle Scale", "Sets the scale of the water handles."), water.handleScale, 0.1f, 10f);
                    pathScale = water.handleScale;

                    water.cubeWater = EditorGUILayout.Toggle(new GUIContent("2.5D Water", "Adds a horizontal mesh at the top of the vertical water mesh."), water.cubeWater);
                    if (water.cubeWater)
                    {
                        water.zSegments = Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Z Segments", "The number of segments, the horizontal water mesh will have."), water.zSegments), 1, 100);

                        if (water.handlesPosition.Count == 4)
                        {
                            water.handlesPosition.Add(new Vector3(water.handlesPosition[0].x, water.handlesPosition[0].y, water.zSize));
                        }
                    }

                    if (!water.cubeWater && water.handlesPosition.Count > 4)
                        water.handlesPosition.RemoveAt(4);

                    EditorGUILayout.LabelField(new GUIContent("Current Water Area", "The value of this field is not used in any calculations. "
                        + "When creating an animation that animates the area of the water, use this value as a guide to see how the water area changes between two positions."), new GUIContent(water.curentWaterArea.ToString()));

                });
            }

            EditorGUI.indentLevel = 0;

            Water2D_Simulation water2D_Sim = water.GetComponent<Water2D_Simulation>();

            if (water2D_Sim.waterType == Water2D_Type.Dynamic)
            {
                showCollider = EditorGUILayout.Foldout(showCollider, "Collider");

                if (showCollider && water.createCollider)
                {
                    EditorGUI.indentLevel = 1;
                    InspectorBox(10, () =>
                    {
                        if (water.createCollider)
                        {
                            water.colliderOffset = EditorGUILayout.FloatField(new GUIContent("Collider Top Offset", "Offsets the position of the top edge of the water collider. "
                                + "This is done so that objects that are a little above the waterline are detected."), water.colliderOffset);

                            if (water.use3DCollider)
                            {
                                water.boxColliderZOffset = EditorGUILayout.FloatField(new GUIContent("Box Collider Z Offset", "Offsets the center of the box collider on the Z axis."), water.boxColliderZOffset);
                                water.boxColliderZSize = EditorGUILayout.FloatField(new GUIContent("Box Collider Z Size", "The size of the box collider on the Z axis."), water.boxColliderZSize);
                            }
                        }

                    });
                }

                EditorGUI.indentLevel = 0;
            }
        }

        private void UpdateHandles(Water2D_Tool water2D, GUIStyle iconStyle)
        {
            Quaternion inv = Quaternion.Inverse(water2D.transform.rotation);

            Handles.color = new Color(1, 1, 1, 0);
            Vector3 global;

            for (int i = 0; i < water2D.handlesPosition.Count; i++)
            {
                Vector3 pos = water2D.transform.position + Vector3.Scale(new Vector3(water2D.handlesPosition[i].x, water2D.handlesPosition[i].y, water2D.handlesPosition[i].z), water2D.transform.localScale);

                if (IsVisible(pos))
                {
                    SetScale(pos, texDot, ref iconStyle);
                    Handles.Label(pos, new GUIContent(texDot), iconStyle);
                }

                if (i < 2)
                    global = Handles.Slider(pos, Vector3.up, HandleScale(pos), Handles.CubeCap, 0);
                else
                {
                    if (i == 4)
                        global = Handles.Slider(pos, Vector3.forward, HandleScale(pos), Handles.CubeCap, 0);
                    else
                        global = Handles.Slider(pos, Vector3.right, HandleScale(pos), Handles.CubeCap, 0);

                }

                if (global != pos)
                {
                    selectedPoints.Clear();
                    selectedPoints.Add(i);

                    Vector3 local = inv * (global - water2D.transform.position);

                    Vector3 relative = new Vector3(local.x / water2D.transform.localScale.x, local.y / water2D.transform.localScale.y, local.z / water2D.transform.localScale.z) - water2D.handlesPosition[i];

                    water2D.handlesPosition[selectedPoints[0]] += relative;

                    if (i == 0 || i == 1)
                    {
                        float distance = Mathf.Abs(water2D.handlesPosition[0].y - water2D.handlesPosition[1].y);
                        water2D.handlesPosition[2] = new Vector2(water2D.handlesPosition[2].x, water2D.handlesPosition[0].y - distance / 2);
                        water2D.handlesPosition[3] = new Vector2(water2D.handlesPosition[3].x, water2D.handlesPosition[0].y - distance / 2);

                        if(water2D.cubeWater)
                            water2D.handlesPosition[4] = new Vector3(water2D.handlesPosition[0].x, water2D.handlesPosition[0].y, water2D.handlesPosition[4].z);

                    }

                    if (i == 2 || i == 3)
                    {
                        float distance = Mathf.Abs(water2D.handlesPosition[2].x - water2D.handlesPosition[3].x);
                        water2D.handlesPosition[0] = new Vector2(water2D.handlesPosition[2].x + distance / 2, water2D.handlesPosition[0].y);
                        water2D.handlesPosition[1] = new Vector2(water2D.handlesPosition[2].x + distance / 2, water2D.handlesPosition[1].y);


                        if (water2D.cubeWater)
                            water2D.handlesPosition[4] = new Vector3(water2D.handlesPosition[0].x, water2D.handlesPosition[0].y, water2D.handlesPosition[4].z);
                    }

                    if (i == 4)
                    {
                        float distance = Mathf.Abs(water2D.handlesPosition[0].z - water2D.handlesPosition[4].z);
                        water2D.zSize = distance;
                    }
                }
            }

            water2D.curentWaterArea = Mathf.Abs(water2D.handlesPosition[2].x - water2D.handlesPosition[3].x) * Mathf.Abs(water2D.handlesPosition[0].y - water2D.handlesPosition[1].y);
        }

        private void DrawRectangle(Water2D_Tool water2D)
        {
            Handles.color = Color.white;

            for (int i = 2; i < 4; i++)
            {
                Vector3 pos = water2D.transform.position + water2D.transform.rotation * Vector3.Scale(new Vector3(water2D.handlesPosition[i].x, water2D.handlesPosition[0].y, 0), water2D.transform.localScale);
                Vector3 pos2 = water2D.transform.position + water2D.transform.rotation * Vector3.Scale(new Vector3(water2D.handlesPosition[i].x, water2D.handlesPosition[1].y, 0), water2D.transform.localScale);
                Handles.DrawLine(pos, pos2);
            }

            for (int i = 0; i < 2; i++)
            {
                Vector3 pos = water2D.transform.position + water2D.transform.rotation * Vector3.Scale(new Vector3(water2D.handlesPosition[2].x, water2D.handlesPosition[i].y, 0), water2D.transform.localScale);
                Vector3 pos2 = water2D.transform.position + water2D.transform.rotation * Vector3.Scale(new Vector3(water2D.handlesPosition[3].x, water2D.handlesPosition[i].y, 0), water2D.transform.localScale);
                Handles.DrawLine(pos, pos2);
            }
        }

        public static float GetCameraDist(Vector3 aPt)
        {
            return Vector3.Distance(SceneView.lastActiveSceneView.camera.transform.position, aPt);
        }

        public bool IsVisible(Vector3 aPos)
        {
            Transform t = SceneView.lastActiveSceneView.camera.transform;
            if (Vector3.Dot(t.forward, aPos - t.position) > 0)
                return true;
            return false;
        }

        public void SetScale(Vector3 aPos, Texture aIcon, ref GUIStyle aStyle, float aScaleOverride = 1)
        {
            float max = (Screen.width + Screen.height) / 2;
            float dist = SceneView.lastActiveSceneView.camera.orthographic ? SceneView.lastActiveSceneView.camera.orthographicSize / 0.5f : GetCameraDist(aPos);

            float div = (dist / (max / 160));
            float mul = pathScale * aScaleOverride;

            aStyle.fixedWidth = (aIcon.width / div) * mul;
            aStyle.fixedHeight = (aIcon.height / div) * mul;
        }

        public static float HandleScale(Vector3 aPos)
        {
            float dist = SceneView.lastActiveSceneView.camera.orthographic ? SceneView.lastActiveSceneView.camera.orthographicSize / 0.45f : GetCameraDist(aPos);
            return Mathf.Min(0.4f * pathScale, (dist / 5.0f) * 0.4f * pathScale);
        }

        public void InspectorBox(int aBorder, System.Action inside, int aWidthOverride = 0, int aHeightOverride = 0)
        {
            Rect r = EditorGUILayout.BeginHorizontal(GUILayout.Width(aWidthOverride));
            if (aWidthOverride != 0)
            {
                r.width = aWidthOverride;
            }
            GUI.Box(r, GUIContent.none);
            GUILayout.Space(aBorder);
            if (aHeightOverride != 0)
                EditorGUILayout.BeginVertical(GUILayout.Height(aHeightOverride));
            else
                EditorGUILayout.BeginVertical();
            GUILayout.Space(aBorder);
            inside();
            GUILayout.Space(aBorder);
            EditorGUILayout.EndVertical();
            GUILayout.Space(aBorder);
            EditorGUILayout.EndHorizontal();
        }

        private static Texture2D GetGizmo(string aFileName)
        {
            Texture2D tex = AssetDatabase.LoadAssetAtPath("Assets/Water2D_Tool/Gizmos/" + aFileName, typeof(Texture2D)) as Texture2D;
            if (tex == null)
            {
                tex = EditorGUIUtility.whiteTexture;
                Debug.Log("Couldn't load Gizmo texture. This happened most likely because you moved Water2D_Tool "
                    + "from the Assets folder to a subfolder. Click on this message to edit the path to the texture" + aFileName);
            }
            return tex;
        }
    }
}
