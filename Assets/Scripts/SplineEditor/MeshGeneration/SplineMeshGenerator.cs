using Sirenix.OdinInspector;
using SplineEditor.Controller.CatmullRomCalc;
using UnityEngine;

namespace SplineEditor.MeshGeneration
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class SplineMeshGenerator : MonoBehaviour
    {
        [SerializeField, Required] private CatmullRom _spline;
        [SerializeField, Required] private Material _material;
        [SerializeField] private Vector3 _meshRotation;
        [SerializeField] private float _width = 1f;
        [SerializeField, Required] private MeshRenderer _meshRenderer;
        [SerializeField, Required] private MeshFilter _meshFilter;
        private Mesh _mesh;
        private Vector3[] _vertices;
        private Vector2[] _uv;
        private int[] _triangles;

        [Button]
        public void Generate()
        {
            _mesh = new Mesh();
            _meshFilter.mesh = _mesh;
            _meshRenderer.material = _material;

            int numVertices = _spline.SplinePoints.Length * 2;
            int numTriangles = (_spline.SplinePoints.Length - 1) * 6;
            _vertices = new Vector3[numVertices];
            _uv = new Vector2[numVertices];
            _triangles = new int[numTriangles];

            for (int i = 0; i < _spline.SplinePoints.Length; i++)
            {
                Vector3 point = _spline.SplinePoints[i].Position;
                Vector3 tangent = _spline.SplinePoints[i].Tangent.normalized;
                Matrix4x4 rotation = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(_meshRotation), Vector3.one);
                Vector3 normal =  (rotation * _spline.SplinePoints[i].Normal).normalized;
                Vector3 binormal = Vector3.Cross(tangent, normal).normalized;

                Vector3 v1 = point - binormal * _width / 2f;
                Vector3 v2 = point + binormal * _width / 2f;

                _vertices[i * 2] = v1;
                _vertices[i * 2 + 1] = v2;
                _uv[i * 2] = new Vector2(0, i);
                _uv[i * 2 + 1] = new Vector2(1, i);

                if (i < _spline.SplinePoints.Length - 1)
                {
                    int triangleIndex = i * 6;
                    _triangles[triangleIndex] = i * 2;
                    _triangles[triangleIndex + 1] = i * 2 + 1;
                    _triangles[triangleIndex + 2] = i * 2 + 2;
                    _triangles[triangleIndex + 3] = i * 2 + 2;
                    _triangles[triangleIndex + 4] = i * 2 + 1;
                    _triangles[triangleIndex + 5] = i * 2 + 3;
                }
            }

            _mesh.vertices = _vertices;
            _mesh.uv = _uv;
            _mesh.triangles = _triangles;
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
        }

        private void Reset()
        {
            _spline = GetComponent<CatmullRom>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshFilter = GetComponent<MeshFilter>();
        }
    }
}