using System;
using UnityEditor;
using UnityEngine;

public class ModelColliderSuffixPostprocessor : AssetPostprocessor
{
    private const string TargetRootPath = "Assets/Art/";

    void OnPostprocessModel(GameObject root)
    {
        if (root == null)
        {
            return;
        }

        if (!assetPath.StartsWith(TargetRootPath, StringComparison.Ordinal))
        {
            return;
        }

        Transform[] transforms = root.GetComponentsInChildren<Transform>(true);
        foreach (Transform transform in transforms)
        {
            if (transform == null)
            {
                continue;
            }

            GameObject target = transform.gameObject;
            if (target.GetComponent<Collider>() != null)
            {
                continue;
            }

            // 이름 접미사 규칙에 따라 콜라이더 타입을 결정한다.
            if (target.name.EndsWith("_NC", StringComparison.Ordinal))
            {
                continue;
            }

            if (target.name.EndsWith("_BC", StringComparison.Ordinal))
            {
                BoxCollider boxCollider = target.AddComponent<BoxCollider>();
                boxCollider.isTrigger = false;
                continue;
            }

            if (target.name.EndsWith("_SC", StringComparison.Ordinal))
            {
                SphereCollider sphereCollider = target.AddComponent<SphereCollider>();
                sphereCollider.isTrigger = false;
                continue;
            }

            if (target.name.EndsWith("_CC", StringComparison.Ordinal))
            {
                CapsuleCollider capsuleCollider = target.AddComponent<CapsuleCollider>();
                capsuleCollider.isTrigger = false;
                continue;
            }

            if (target.name.EndsWith("_MC", StringComparison.Ordinal))
            {
                MeshCollider meshCollider = target.AddComponent<MeshCollider>();
                meshCollider.isTrigger = false;
                meshCollider.convex = false;
            }
        }
    }
}
