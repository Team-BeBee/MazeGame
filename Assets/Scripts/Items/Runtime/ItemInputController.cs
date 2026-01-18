using System.Collections.Generic;
using UnityEngine;
using MazeGame.Items.Core;

namespace MazeGame.Items.Runtime
{
    // 아이템 입력 처리
    public class ItemInputController : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField] private QuickSlotManager quickSlotManager;
        [SerializeField] private Camera playerCamera;

        [Header("입력 차단 컴포넌트")]
        [SerializeField] private List<MonoBehaviour> inputBlockers = new List<MonoBehaviour>();

        [Header("입력 설정")]
        [SerializeField] private bool allowSelection = true;
        [SerializeField] private bool allowUse = true;

        private void Reset()
        {
            quickSlotManager = GetComponent<QuickSlotManager>();
        }

        private void Update()
        {
            if (quickSlotManager == null)
            {
                return;
            }

            if (allowSelection && !IsSelectionBlocked())
            {
                HandleSelectionInput();
            }

            if (allowUse && !IsUseBlocked())
            {
                HandleUseInput();
            }
        }

        private bool IsSelectionBlocked()
        {
            foreach (MonoBehaviour blocker in inputBlockers)
            {
                if (blocker is ItemInputBlocker inputBlocker && inputBlocker.IsItemSelectionBlocked())
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsUseBlocked()
        {
            foreach (MonoBehaviour blocker in inputBlockers)
            {
                if (blocker is ItemInputBlocker inputBlocker && inputBlocker.IsItemUseBlocked())
                {
                    return true;
                }
            }

            return false;
        }

        private void HandleSelectionInput()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                quickSlotManager.SetSelectedIndex(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                quickSlotManager.SetSelectedIndex(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                quickSlotManager.SetSelectedIndex(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                quickSlotManager.SetSelectedIndex(3);
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0.01f)
            {
                quickSlotManager.CycleSelection(1);
            }
            else if (scroll < -0.01f)
            {
                quickSlotManager.CycleSelection(-1);
            }
        }

        private void HandleUseInput()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            Transform playerTransform = transform;
            Camera contextCamera = playerCamera != null ? playerCamera : Camera.main;
            ItemUseContext context = new ItemUseContext(playerTransform, contextCamera, playerTransform.gameObject);
            quickSlotManager.UseSelected(context);
        }
    }
}
