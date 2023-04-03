using UnityEngine;

namespace SplineEditor.Controller.CatmullRomCalc
{
    public struct CatmullRomPoint
    {
        public Vector3 Position;
        public Vector3 Tangent;
        public Vector3 Normal;
        public Vector3 Binormal;
        public float DistanceToStart;
        public float NormalizedValue;

        public CatmullRomPoint(Vector3 position, Vector3 tangent, Vector3 normal, Vector3 binormal, float normalizedValue, float distanceToStart)
        {
            Position = position;
            Tangent = tangent;
            Normal = normal;
            Binormal = binormal;
            NormalizedValue = normalizedValue;
            DistanceToStart = distanceToStart;
        }
    }
}


