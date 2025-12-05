using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject doorPrefab;
    public float cellSize = 1f;

    private MazeGrid grid;
    private readonly List<Vector3> walkableCells = new List<Vector3>();

    public void Render(MazeGrid grid)
    {
        if (grid == null)
        {
            return; // 렌더링할 그리드가 없을 때는 아무 것도 하지 않음
        }

        this.grid = grid;
        walkableCells.Clear();

        // 이전에 생성된 타일들을 모두 정리
        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child);
        }

        foreach (Transform child in children)
        {
            Destroy(child.gameObject);
        }

        List<Vector2Int> pathCells = new List<Vector2Int>();

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                MazeCell cell = grid.Cells[x, y];
                GameObject prefab = cell.Type == CellType.Wall ? wallPrefab : floorPrefab;
                Vector3 position = new Vector3(x * cellSize, 0f, y * cellSize);

                if (prefab != null)
                {
                    Instantiate(prefab, position, Quaternion.identity, transform);
                }

                if (cell.Type == CellType.Path)
                {
                    pathCells.Add(new Vector2Int(x, y));
                    walkableCells.Add(position);
                }
            }
        }

        // 텔레포트용 이동 가능 위치 등록
        TeleportUtility.RegisterWalkableCells(new List<Vector3>(walkableCells));

        // 출구 문 배치 (골문)
        Vector2Int goalPos = new Vector2Int(grid.Width - 2, grid.Height - 2);
        if (grid.InBounds(goalPos.x, goalPos.y) && doorPrefab != null)
        {
            Vector3 goalPosition = new Vector3(goalPos.x * cellSize, 0f, goalPos.y * cellSize);
            GameObject goalDoor = Instantiate(doorPrefab, goalPosition, Quaternion.identity, transform);
            GoalDoor goalDoorComponent = goalDoor.GetComponent<GoalDoor>() ?? goalDoor.AddComponent<GoalDoor>();

            goalDoorComponent.doorType = DoorType.Goal;
        }

        // 랜덤 가짜 문 배치
        if (doorPrefab != null && pathCells.Count > 0)
        {
            System.Random rng = new System.Random();
            HashSet<Vector2Int> used = new HashSet<Vector2Int> { goalPos };
            int fakeDoorCount = Math.Max(1, pathCells.Count / 8);

            for (int i = 0; i < fakeDoorCount; i++)
            {
                Vector2Int pos = pathCells[rng.Next(pathCells.Count)];
                int guard = 0;

                // 출구와 중복되지 않는 위치를 찾음
                while (used.Contains(pos) && guard < pathCells.Count)
                {
                    pos = pathCells[rng.Next(pathCells.Count)];
                    guard++;
                }

                if (used.Contains(pos))
                {
                    continue;
                }

                used.Add(pos);
                Vector3 spawnPosition = new Vector3(pos.x * cellSize, 0f, pos.y * cellSize);
                GameObject fakeDoor = Instantiate(doorPrefab, spawnPosition, Quaternion.identity, transform);
                FakeDoor fakeDoorComponent = fakeDoor.GetComponent<FakeDoor>() ?? fakeDoor.AddComponent<FakeDoor>();

                fakeDoorComponent.doorType = DoorType.Fake;

                if (PenaltyManager.Instance != null)
                {
                    PenaltyManager.Instance.RegisterFakeDoor(fakeDoorComponent);
                }
            }
        }
    }
}
