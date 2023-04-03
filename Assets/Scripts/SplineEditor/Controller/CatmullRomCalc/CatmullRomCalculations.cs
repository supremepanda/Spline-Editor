using UnityEngine;

namespace SplineEditor.Controller
{
    public class CatmullRomCalculations : MonoBehaviour
    {
//-------Public Variables-------//


//------Serialized Fields-------//


//------Private Variables-------//


#region UNITY_METHODS


#endregion


#region PUBLIC_METHODS
        public static Vector3 CalculatePosition(Vector3 start, Vector3 end, Vector3 tanPoint1, Vector3 tanPoint2, float t)
        {
            // Hermite curve formula:
            // (2t^3 - 3t^2 + 1) * p0 + (t^3 - 2t^2 + t) * m0 + (-2t^3 + 3t^2) * p1 + (t^3 - t^2) * m1
            Vector3 position = (2.0f * t * t * t - 3.0f * t * t + 1.0f) * start
                               + (t * t * t - 2.0f * t * t + t) * tanPoint1
                               + (-2.0f * t * t * t + 3.0f * t * t) * end
                               + (t * t * t - t * t) * tanPoint2;
            return position;
        }

        public static Vector3 CalculateTangent(Vector3 start, Vector3 end, Vector3 tanPoint1, Vector3 tanPoint2, float t)
        {
            // p'(t) = (6t² - 6t)p0 + (3t² - 4t + 1)m0 + (-6t² + 6t)p1 + (3t² - 2t)m1
            Vector3 tangent = (6 * t * t - 6 * t) * start
                              + (3 * t * t - 4 * t + 1) * tanPoint1
                              + (-6 * t * t + 6 * t) * end
                              + (3 * t * t - 2 * t) * tanPoint2;
            return tangent.normalized;
        }
        
        public static Vector3 NormalFromTangent(Vector3 tangent)
        {
            return Vector3.Cross(tangent, Vector3.up).normalized / 2;
        }
        
        public static Vector3 BinormalFromTangentAndNormal(Vector3 tangent, Vector3 normal)
        {
            return Vector3.Cross(tangent, normal).normalized;
        }
#endregion


#region PRIVATE_METHODS

#endregion

    }
}


