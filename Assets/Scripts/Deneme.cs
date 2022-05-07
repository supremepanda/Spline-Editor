using Extensions.Other;
using SplineEditor.PathFollowing.Positioner;
using UnityEngine;

namespace DefaultNamespace
{
    public class Deneme : MonoBehaviour
    {
//-------Public Variables-------//


//------Serialized Fields-------//


//------Private Variables-------//


#region UNITY_METHODS


#endregion


#region PUBLIC_METHODS

        public void SplineFollower(SplineFollower follower)
        {
            EditorDebug.Log(follower.ToString());
        }
#endregion


#region PRIVATE_METHODS

#endregion

    }
}


