using UnityEngine;

namespace SplineEditor.Controller
{
    public struct CatmullRomPoint
    {
        public Vector3 Position;
        public Vector3 Tangent;
        public Vector3 Normal;
        public float DistanceToStart;
        public float NormalizedValue;

        public CatmullRomPoint(Vector3 position, Vector3 tangent, Vector3 normal, float normalizedValue, float distanceToStart)
        {
            Position = position;
            Tangent = tangent;
            Normal = normal;
            NormalizedValue = normalizedValue;
            DistanceToStart = distanceToStart;
        }
    }
}


