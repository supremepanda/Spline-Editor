using Vector3 = UnityEngine.Vector3;

namespace Extensions.Other
{
    public static class Vector3Extension
    {
        public static Vector3 SetX(this Vector3 v, float x)
        {
            return new Vector3(x, v.y, v.z);
        }

        public static Vector3 SetY(this Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }

        public static Vector3 SetZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }

        public static Vector3 AddX(this Vector3 v, float x)
        {
            return v + new Vector3(x, 0f, 0f);
        }

        public static Vector3 AddY(this Vector3 v, float y)
        {
            return v + new Vector3(0f, y, 0f);
        }

        public static Vector3 AddZ(this Vector3 v, float z)
        {
            return v + new Vector3(0f, 0f, z);
        }
    }
}


