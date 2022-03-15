using UnityEngine;

namespace Extensions.Other
{
    public static class LayerMaskExtension
    {
        public static bool Contains(this LayerMask mask, int layer)
        {
            return ((1 << layer) & mask) != 0;
        }
        
        public static int GetLayerFromLayerMask(this LayerMask layerMask)
        {
            var val = (uint)layerMask.value;
            if (val  == 0)
                return -1;
            for (var i = 0; i < 32; i++)
            {
                if((val & (1<<i)) != 0)
                    return i;
            }
            return -1;
        }
    }
}


