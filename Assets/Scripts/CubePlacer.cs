using System.Linq;
using Deblockle.Gameplay;
using FawesomeLab.GameCore.Manager;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Deblockle.Managers
{
    public class CubePlacer : SingleManager<CubePlacer>
    {
        [FoldoutGroup("Reference")] [SerializeField] private GameCube[] cubes = new GameCube[8];

        public override void Awake()
        {
            base.Awake();
            Init();
        }

        public override void Init()
        {
        }

        public GameCube GetCubeByRotator(CubeRotator rotator)
        {
            return cubes.FirstOrDefault(cube => cube.CubeRotator == rotator);
        }

        public bool IsTileFree(Vector3 pos)
        {
            return cubes.All(cube => (Vector3.Distance(cube.CubeRotator.transform.position, pos) > 0.25f));
        }
        

        public void UpdateCubesInfo(CubeDataPack[] newInfo)
        {
            for (var i = 0; i < newInfo.Length; i++)
            {
                if (i >= cubes.Length)
                {
                    break;
                }
                
                cubes[i].UpdateData(newInfo[i]);
            }
        }
    }
}