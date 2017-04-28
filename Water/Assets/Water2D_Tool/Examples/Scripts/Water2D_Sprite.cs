using UnityEngine;
using System.Collections;

namespace Water2DTool
{
    // A simple class for creating sprites.
    [RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
    public class Water2D_Sprite : MonoBehaviour
    {

        private float width = 1.0f;
        private float height = 1.0f;

        private int hVerts = 1;
        private int vVerts = 1;

        private float scaleX = 1f;
        private float scaleY = 1f;

        private Water2D_Mesh dMesh;
        private Vector2 unitsPerUV = Vector2.one;
        private Mesh mesh;

        public float pixelsPerUnit = 100;
        public int widthSegments = 1;
        public int heightSegments = 1;

        private Water2D_Mesh DMesh
        {
            get
            {
                if (dMesh == null)
                    dMesh = new Water2D_Mesh();
                return dMesh;
            }
        }

        public void RebuildMesh()
        {
            DMesh.Clear();

            if (GetComponent<Renderer>().sharedMaterial == null)
                return;

            unitsPerUV.x = GetComponent<Renderer>().sharedMaterial.mainTexture.width / pixelsPerUnit;
            unitsPerUV.y = GetComponent<Renderer>().sharedMaterial.mainTexture.height / pixelsPerUnit;

            width = unitsPerUV.x;
            height = unitsPerUV.y;

            hVerts = widthSegments + 1;
            vVerts = heightSegments + 1;

            scaleX = width / widthSegments;
            scaleY = height / heightSegments;

            float startX = -unitsPerUV.x / 2f;
            float startY = -unitsPerUV.y / 2f;

            for (int y = 0; y < vVerts; y++)
            {
                for (int x = 0; x < hVerts; x++)
                {
                    Vector3 vertPos = new Vector3(x * scaleX + startX, y * scaleY + startY, 0.0f);
                    float U = (vertPos.x / width) + 0.5f;
                    float V = y == 0 ? 0 : (scaleY * y) / height;
                    DMesh.AddVertex(vertPos, 0.0f, new Vector2(U, V));
                }
            }

            DMesh.GenerateTriangles(widthSegments, heightSegments, hVerts, true);

            mesh = GetComponent<MeshFilter>().sharedMesh;
            string name = GetMeshName();
            if (mesh == null || mesh.name != name)
            {
                mesh = GetComponent<MeshFilter>().sharedMesh = new Mesh();
                mesh.name = name;
            }

            DMesh.Build(ref mesh);
        }

        private string GetMeshName()
        {
            return "SpriteMesh" + gameObject.GetInstanceID();
        }
    }
}
