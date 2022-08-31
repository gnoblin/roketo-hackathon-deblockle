using System;
using System.Collections.Generic;
using System.Linq;
using Deblockle.Gameplay;
using FawesomeLab.GameCore.Manager;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Deblockle.Managers
{
    public class FieldProcessor : SingleManager<FieldProcessor>
    {
        [FoldoutGroup("Reference")] [OdinSerialize] private Transform[,] fieldPoints = new Transform[7,7];
        // [FoldoutGroup("Reference")] [OdinSerialize] private Dictionary<GameCube, Transform> cubesInfo = new();
        [FoldoutGroup("Reference")] [SerializeField] private List<ParticleSystem> selectionFX = new();

        // public void UpdateCubesData()
        // {
        //     var info = new Dictionary<GameCube, Transform>(cubesInfo);
        //     foreach (var (cube, _) in cubesInfo)
        //     {
        //         var minDist = Mathf.Infinity;
        //         var p = fieldPoints[0, 0];
        //         
        //         foreach (var point in fieldPoints)
        //         {
        //             var dist = Vector3.Distance(point.position, cube.BodyPos); 
        //             if (dist < minDist)
        //             {
        //                 minDist = dist;
        //                 p = point;
        //             }
        //             else
        //             {
        //                 info[cube] = point;
        //             }
        //         }
        //         info[cube] = p;
        //     }
        //     cubesInfo = info;
        // }

        public List<Vector3> GetPositionByAction(Vector3 cubePos, ActionType actionType)
        {
            switch (actionType)
            {
                case ActionType.ThreeMoves:
                case ActionType.SideMove:
                    var sidePosition = new List<Vector3>();
                    var positionsToCheck = new List<Vector2Int> {new (1, 0), new (-1, 0), new (0, 1), new (0, -1)};
                    foreach (var pos in positionsToCheck)
                    {
                        var p = GetPosWithOffset(cubePos, pos);
                        if(p != null) sidePosition.Add(p.Value);
                    }
                    return ValidatePositions(sidePosition);
                
                case ActionType.DiagonalMove:
                    var diagonalPositions = new List<Vector3>();
                    var positionsToCheckD = new List<Vector2Int> {new (1, 1), new (-1, -1), new (-1, 1), new (1, -1)};
                    foreach (var pos in positionsToCheckD)
                    {
                        var p = GetPosWithOffset(cubePos, pos);
                        if(p != null) diagonalPositions.Add(p.Value);
                    }
                    return ValidatePositions(diagonalPositions);
                
                case ActionType.MoveToEndOfField:
                    break;
                case ActionType.Unknown:
                case ActionType.Star:
                case ActionType.Stop:
                default:
                    break;
            }

            return new List<Vector3>();
        }

        private List<Vector3> ValidatePositions(IEnumerable<Vector3> positions)
        {
            return positions.Where(pos => CubePlacer.I.IsTileFree(pos)).ToList();
        }

        public void SelectPos(Vector3 pos)
        {
            foreach (var fx in selectionFX)
            {
                if (fx != null && fx.gameObject.activeInHierarchy == false)
                {
                    fx.transform.position = pos;
                    fx.gameObject.SetActive(true);
                    fx.Play();
                    break;
                }
            }
        }

        public void RemoveSelection()
        {
            foreach (var fx in selectionFX)
            {
                fx?.Stop();
                fx?.gameObject.SetActive(false);
            }
        }

        private Vector3? GetPosWithOffset(Vector3 pos, Vector2Int offset)
        {
            for (var x = 0; x < fieldPoints.GetLength(0); x++)
            {
                for (var y = 0; y < fieldPoints.GetLength(1); y++)
                {
                    if (Vector3.Distance(fieldPoints[x,y].position, pos) < 0.2f)
                    {
                        return GetPosWithOffset(x, y, offset);
                    }
                }
            }
            return null;
        }
        
        private Vector3? GetPosWithOffset(int x, int y, Vector2Int offset)
        {
            var targetPosition = new Vector2Int(x + offset.x, y + offset.y);

            if (targetPosition.x >= 0 && targetPosition.x < fieldPoints.GetLength(0) &&
                targetPosition.y >= 0 && targetPosition.y < fieldPoints.GetLength(1))
            {
                return fieldPoints[targetPosition.x, targetPosition.y].position;
            }
            return null;
        }

        public override void Init() {}
    }
}