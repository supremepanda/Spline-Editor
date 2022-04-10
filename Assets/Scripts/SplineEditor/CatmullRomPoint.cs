using UnityEngine;

namespace SplineEditor
{
    public struct CatmullRomPoint
    {
        public Vector3 Position;
        public Vector3 Tangent;
        public Vector3 Normal;

        public CatmullRomPoint(Vector3 position, Vector3 tangent, Vector3 normal)
        {
            Position = position;
            Tangent = tangent;
            Normal = normal;
        }
    }
}


