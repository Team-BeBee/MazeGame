using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// 벽을 일정 시간 동안 통과 가능하게 만드는 컴포넌트
public class PassableWall : MonoBehaviour
{
    [Header("투명도 설정")]
    [Range(0.05f, 0.9f)]
    [SerializeField] private float transparentAlpha = 0.35f;

    private readonly List<Collider> cachedColliders = new List<Collider>();
    private readonly List<Renderer> cachedRenderers = new List<Renderer>();
    private readonly List<MaterialSnapshot> materialSnapshots = new List<MaterialSnapshot>();

    private bool snapshotsReady;
    private Coroutine activeCoroutine;
    private bool isPassable;

    private void Awake()
    {
        EnsureComponents();
    }

    public void MakePassable(float duration)
    {
        MakePassable(duration, transparentAlpha);
    }

    public void MakePassable(float duration, float alpha)
    {
        if (duration <= 0f)
        {
            return;
        }

        transparentAlpha = Mathf.Clamp01(alpha);

        EnsureComponents();
        if (!isPassable)
        {
            CacheMaterialSnapshots();
        }
        ApplyPassableState(true);

        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }

        activeCoroutine = StartCoroutine(PassableRoutine(duration));
    }

    private IEnumerator PassableRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        ApplyPassableState(false);
        activeCoroutine = null;
    }

    private void EnsureComponents()
    {
        if (cachedColliders.Count == 0 || cachedColliders.Exists(collider => collider == null))
        {
            cachedColliders.Clear();
            cachedColliders.AddRange(GetComponentsInChildren<Collider>(true));
        }

        if (cachedRenderers.Count == 0 || cachedRenderers.Exists(renderer => renderer == null))
        {
            cachedRenderers.Clear();
            cachedRenderers.AddRange(GetComponentsInChildren<Renderer>(true));
        }
    }

    private void CacheMaterialSnapshots()
    {
        materialSnapshots.Clear();

        foreach (Renderer renderer in cachedRenderers)
        {
            if (renderer == null)
            {
                continue;
            }

            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                Material material = materials[i];
                if (material == null)
                {
                    continue;
                }

                materialSnapshots.Add(MaterialSnapshot.FromMaterial(renderer, material, i));
            }
        }

        snapshotsReady = true;
    }

    private void ApplyPassableState(bool passable)
    {
        foreach (Collider collider in cachedColliders)
        {
            if (collider != null)
            {
                collider.enabled = !passable;
            }
        }

        if (passable)
        {
            isPassable = true;
            if (snapshotsReady)
            {
                foreach (MaterialSnapshot snapshot in materialSnapshots)
                {
                    SetMaterialTransparent(snapshot.Material, transparentAlpha);
                }
            }
            else
            {
                foreach (Renderer renderer in cachedRenderers)
                {
                    if (renderer == null)
                    {
                        continue;
                    }

                    foreach (Material material in renderer.materials)
                    {
                        SetMaterialTransparent(material, transparentAlpha);
                    }
                }
            }
        }
        else
        {
            foreach (MaterialSnapshot snapshot in materialSnapshots)
            {
                snapshot.Restore();
            }

            isPassable = false;
            snapshotsReady = false;
        }
    }

    private void SetMaterialTransparent(Material material, float alpha)
    {
        if (material == null)
        {
            return;
        }

        if (material.HasProperty("_Mode"))
        {
            material.SetFloat("_Mode", 3f);
        }

        if (material.HasProperty("_Surface"))
        {
            material.SetFloat("_Surface", 1f);
        }

        if (material.HasProperty("_Blend"))
        {
            material.SetFloat("_Blend", 0f);
        }

        if (material.HasProperty("_SrcBlend"))
        {
            material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
        }

        if (material.HasProperty("_DstBlend"))
        {
            material.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
        }

        if (material.HasProperty("_ZWrite"))
        {
            material.SetFloat("_ZWrite", 0f);
        }

        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int)RenderQueue.Transparent;

        if (material.HasProperty("_Color"))
        {
            Color color = material.color;
            color.a = alpha;
            material.color = color;
        }
        else if (material.HasProperty("_BaseColor"))
        {
            Color color = material.GetColor("_BaseColor");
            color.a = alpha;
            material.SetColor("_BaseColor", color);
        }
    }

    private struct MaterialSnapshot
    {
        public Renderer Renderer;
        public Material Material;
        public int MaterialIndex;
        public bool HasColor;
        public Color Color;
        public bool HasBaseColor;
        public Color BaseColor;
        public bool HasMode;
        public float Mode;
        public bool HasSurface;
        public float Surface;
        public bool HasBlend;
        public float Blend;
        public bool HasSrcBlend;
        public float SrcBlend;
        public bool HasDstBlend;
        public float DstBlend;
        public bool HasZWrite;
        public float ZWrite;
        public bool AlphaTestKeyword;
        public bool AlphaBlendKeyword;
        public bool AlphaPremultiplyKeyword;
        public int RenderQueue;

        public static MaterialSnapshot FromMaterial(Renderer renderer, Material material, int materialIndex)
        {
            MaterialSnapshot snapshot = new MaterialSnapshot
            {
                Renderer = renderer,
                Material = material,
                MaterialIndex = materialIndex,
                HasColor = material.HasProperty("_Color"),
                HasBaseColor = material.HasProperty("_BaseColor"),
                HasMode = material.HasProperty("_Mode"),
                HasSurface = material.HasProperty("_Surface"),
                HasBlend = material.HasProperty("_Blend"),
                HasSrcBlend = material.HasProperty("_SrcBlend"),
                HasDstBlend = material.HasProperty("_DstBlend"),
                HasZWrite = material.HasProperty("_ZWrite"),
                AlphaTestKeyword = material.IsKeywordEnabled("_ALPHATEST_ON"),
                AlphaBlendKeyword = material.IsKeywordEnabled("_ALPHABLEND_ON"),
                AlphaPremultiplyKeyword = material.IsKeywordEnabled("_ALPHAPREMULTIPLY_ON"),
                RenderQueue = material.renderQueue
            };

            if (snapshot.HasColor)
            {
                snapshot.Color = material.color;
            }

            if (snapshot.HasBaseColor)
            {
                snapshot.BaseColor = material.GetColor("_BaseColor");
            }

            if (snapshot.HasMode)
            {
                snapshot.Mode = material.GetFloat("_Mode");
            }

            if (snapshot.HasSurface)
            {
                snapshot.Surface = material.GetFloat("_Surface");
            }

            if (snapshot.HasBlend)
            {
                snapshot.Blend = material.GetFloat("_Blend");
            }

            if (snapshot.HasSrcBlend)
            {
                snapshot.SrcBlend = material.GetFloat("_SrcBlend");
            }

            if (snapshot.HasDstBlend)
            {
                snapshot.DstBlend = material.GetFloat("_DstBlend");
            }

            if (snapshot.HasZWrite)
            {
                snapshot.ZWrite = material.GetFloat("_ZWrite");
            }

            return snapshot;
        }

        public void Restore()
        {
            if (Material == null)
            {
                return;
            }

            if (HasColor)
            {
                Material.color = Color;
            }

            if (HasBaseColor)
            {
                Material.SetColor("_BaseColor", BaseColor);
            }

            if (HasMode)
            {
                Material.SetFloat("_Mode", Mode);
            }

            if (HasSurface)
            {
                Material.SetFloat("_Surface", Surface);
            }

            if (HasBlend)
            {
                Material.SetFloat("_Blend", Blend);
            }

            if (HasSrcBlend)
            {
                Material.SetFloat("_SrcBlend", SrcBlend);
            }

            if (HasDstBlend)
            {
                Material.SetFloat("_DstBlend", DstBlend);
            }

            if (HasZWrite)
            {
                Material.SetFloat("_ZWrite", ZWrite);
            }

            SetKeyword("_ALPHATEST_ON", AlphaTestKeyword);
            SetKeyword("_ALPHABLEND_ON", AlphaBlendKeyword);
            SetKeyword("_ALPHAPREMULTIPLY_ON", AlphaPremultiplyKeyword);
            Material.renderQueue = RenderQueue;
        }

        private void SetKeyword(string keyword, bool enabled)
        {
            if (enabled)
            {
                Material.EnableKeyword(keyword);
            }
            else
            {
                Material.DisableKeyword(keyword);
            }
        }
    }
}
