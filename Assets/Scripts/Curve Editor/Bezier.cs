using UnityEngine;

namespace Curve_Editor
{
    public class Bezier : MonoBehaviour
    {
//-------Public Variables-------//


//------Serialized Fields-------//


//------Private Variables-------//


#region UNITY_METHODS


#endregion


#region PUBLIC_METHODS

        public static Vector3 QuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            var l1 = Vector3.Lerp(p0, p1, t);
            var l2 = Vector3.Lerp(p1, p2, t);
            return Vector3.Lerp(l1, l2, t);
        }

        public static Vector3 CubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            var q1 = QuadraticBezier(p0, p1, p2, t);
            var q2 = QuadraticBezier(p1, p2, p3, t);
            return Vector3.Lerp(q1, q2, t);
        }
#endregion


#region PRIVATE_METHODS

#endregion

    }
}


