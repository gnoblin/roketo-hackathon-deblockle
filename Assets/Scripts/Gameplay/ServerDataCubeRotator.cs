using Deblockle.Gameplay;
using DG.Tweening;
using FawesomeLab.Helpers.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay
{
    public class ServerDataCubeRotator : MonoBehaviour
    {
        [SerializeField] private CubeRotator cubeRotator;
        [SerializeField] private Transform cube;

        [Button]
        public void RotateToPos(Vector2 pos)
        {
            var relativePos = (new Vector3(pos.x - 4f, 0, pos.y - 4f) - cube.position.SetY(0)).normalized;
            Debug.DrawRay(cube.position, cube.position + relativePos * 1.1f);
            cubeRotator.SetPos(new Vector2(relativePos.x, relativePos.z) * 1.1f);

            DOTween.To(
                    () => cubeRotator.Progress, 
                    value => cubeRotator.Progress = value, 1, 1)
                .SetEase(Ease.InOutSine);
        }
    }
}