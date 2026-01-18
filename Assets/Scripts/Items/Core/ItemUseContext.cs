using UnityEngine;

namespace MazeGame.Items.Core
{
    // 아이템 사용 시 전달되는 컨텍스트
    public struct ItemUseContext
    {
        public Transform PlayerTransform;
        public Camera PlayerCamera;
        public GameObject PlayerObject;

        public ItemUseContext(Transform playerTransform, Camera playerCamera, GameObject playerObject)
        {
            PlayerTransform = playerTransform;
            PlayerCamera = playerCamera;
            PlayerObject = playerObject;
        }
    }
}
