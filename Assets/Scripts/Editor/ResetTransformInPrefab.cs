using UnityEditor;
using UnityEngine;

namespace WaterSort
{
    public class ResetTransformInPrefab : AssetPostprocessor
    {
        void OnPostprocessPrefab (GameObject o)
        {
            o.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }
}