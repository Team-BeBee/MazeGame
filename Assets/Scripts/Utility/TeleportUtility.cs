using UnityEngine;
using System.Collections.Generic;

public static class TeleportUtility
{
    private static Transform player;
    private static List<Vector3> walkableCells = new List<Vector3>();

    public static void RegisterPlayer(Transform p)
    {
        player = p;
    }

    public static void RegisterWalkableCells(List<Vector3> cells)
    {
        walkableCells = cells;
    }

    public static void TeleportPlayerRandom()
    {
        if (player == null)
        {
            Debug.LogWarning("[TeleportUtility] Player not registered.");
            return;
        }

        if (walkableCells == null || walkableCells.Count == 0)
        {
            Debug.LogWarning("[TeleportUtility] No walkable cells registered.");
            return;
        }

        // 유효한 셀을 무작위로 선택
        Vector3 target = walkableCells[Random.Range(0, walkableCells.Count)];

        // CharacterController가 있으면 순간 텔레포트 동안 비활성화
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
            controller.enabled = false;

        player.position = target + Vector3.up * 1f; // 바닥 클리핑을 피하기 위한 오프셋

        if (controller != null)
            controller.enabled = true;

        Debug.Log("[TeleportUtility] Player teleported to " + target);
    }
}
