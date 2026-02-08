using MazeGame.Items.Core;
using MazeGame.Items.Data;
using UnityEngine;

namespace MazeGame.Items.Effects
{
    [CreateAssetMenu(
        menuName = "MazeGame/Items/Effects/Speed Modifier Effect",
        fileName = "SpeedModifierEffect"
    )]
    public class SpeedModifierEffect : ItemEffectBase
    {
        [Header("Speed Modifier")]
        [SerializeField] private float addDelta = 0f;
        [SerializeField] private float mulDelta = 1f;

        public override bool TryExecute(ItemUseContext context)
        {
            if (context.PlayerObject == null)
                return false;

            SpeedModifierManager manager = context.PlayerObject.GetComponent<SpeedModifierManager>();
            if (manager == null)
                return false;

            manager.AddModifier(addDelta, mulDelta);
            return true;
        }
    }
}
