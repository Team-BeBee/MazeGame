using UnityEngine;
using MazeGame.Items.Core;
using MazeGame.Items.Data;

namespace MazeGame.Items.Effects
{
    // 바라보는 벽을 일정 시간 통과 가능하게 만드는 아이템 효과
    [CreateAssetMenu(
        menuName = "MazeGame/Items/Effects/Eraser Effect",
        fileName = "EraserEffect"
    )]
    public class EraserEffect : ItemEffectBase
    {
        [Header("레이캐스트 설정")]
        [SerializeField] private float maxDistance = 4f;
        [SerializeField] private LayerMask wallLayerMask;

        [Header("효과 설정")]
        [SerializeField] private float passableDuration = 10f;
        [Range(0.05f, 0.9f)]
        [SerializeField] private float transparentAlpha = 0.35f;

        public override bool TryExecute(ItemUseContext context)
        {
            if (context.PlayerCamera == null)
            {
                return false;
            }

            if (!TryResolveWallMask(out int mask))
            {
                return false;
            }

            Ray ray = new Ray(context.PlayerCamera.transform.position, context.PlayerCamera.transform.forward);
            if (!Physics.Raycast(ray, out RaycastHit hit, maxDistance, mask))
            {
                return false;
            }

            PassableWall passableWall = hit.collider.GetComponentInParent<PassableWall>();
            if (passableWall == null)
            {
                return false;
            }

            passableWall.MakePassable(passableDuration, transparentAlpha);
            return true;
        }

        private bool TryResolveWallMask(out int mask)
        {
            mask = wallLayerMask.value;
            if (mask != 0)
            {
                return true;
            }

            int passableLayer = LayerMask.NameToLayer("PassableWall");
            if (passableLayer < 0)
            {
                Debug.LogWarning("[EraserEffect] PassableWall 레이어가 없습니다. 레이어 설정을 확인하세요.");
                return false;
            }

            mask = 1 << passableLayer;
            return true;
        }
    }
}
