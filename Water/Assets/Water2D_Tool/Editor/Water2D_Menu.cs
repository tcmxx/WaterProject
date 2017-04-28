using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Water2DTool
{
    public class Water2D_Menu
    {
        [MenuItem("GameObject/2D Water/Create 2D Water With 2D Collider %w", false, 0)]
        static void MenuAddWater2D_Collider2D()
        {
            GameObject obj = CreateWater2D(true);
            Selection.activeGameObject = obj;
            EditorGUIUtility.PingObject(obj);
        }

        [MenuItem("GameObject/2D Water/Create 2D Water With 3D Collider %#w", false, 0)]
        static void MenuAddWater2D_Collider3D()
        {
            GameObject obj = CreateWater2D(false);
            Selection.activeGameObject = obj;
            EditorGUIUtility.PingObject(obj);
        }

        static GameObject CreateWater2D(bool collider2D)
        {
            GameObject obj = new GameObject("New Water2D");
            Water2D_Tool water = obj.AddComponent<Water2D_Tool>();
            obj.AddComponent<Water2D_Simulation>();

            water.Add(new Vector2(0, 3));
            water.Add(new Vector2(0, -3));
            water.Add(new Vector2(-5, 0));
            water.Add(new Vector2(5, 0));

            if (!collider2D)
                water.use3DCollider = true;

            water.SetDefaultMaterial();
            water.RecreateWaterMesh();

            obj.transform.position = GetSpawnPos();

            return obj;
        }

        static Vector3 GetSpawnPos()
        {
            Plane plane = new Plane(new Vector3(0, 0, -1), 0);
            float dist = 0;
            Vector3 result = new Vector3(0, 0, 0);
            Ray ray = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f));
            if (plane.Raycast(ray, out dist))
            {
                result = ray.GetPoint(dist);
            }
            return new Vector3(result.x, result.y, 0);
        }
    }
}
