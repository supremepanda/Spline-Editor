using UnityEngine;

namespace Extensions.Other
{
    public class EditorDebug : MonoBehaviour
    {
//-------Public Variables-------//


//------Serialized Fields-------//


//------Private Variables-------//



#region UNITY_METHODS


#endregion


#region PUBLIC_METHODS
        

        public static void Log(object obj)
        {
#if UNITY_EDITOR
            Debug.Log(obj);
#endif
        }

        public static void Log(object obj, GameObject gObj)
        {
#if UNITY_EDITOR
            Debug.Log(obj, gObj);
#endif
        }

        public static void LogWarning(object obj)
        {
#if UNITY_EDITOR
            Debug.LogWarning(obj);
#endif
        }

        public static void LogWarning(object obj, GameObject gObj)
        {
#if UNITY_EDITOR
            Debug.LogWarning(obj, gObj);
#endif
        }

        public static void LogError(object obj)
        {
#if UNITY_EDITOR
            Debug.LogError(obj);
#endif
        }

        public static void LogError(object obj, GameObject gObj)
        {
#if UNITY_EDITOR
            Debug.LogError(obj, gObj);
#endif
        }

        
#endregion


#region PRIVATE_METHODS

#endregion
        
    }
}