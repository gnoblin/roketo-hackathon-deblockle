using System;
using System.Collections.Generic;
using System.Linq;
using Deblockle.Managers;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Deblockle.Gameplay
{
    public class GameCube : SerializedMonoBehaviour
    {
        [FoldoutGroup("Reference")] [SerializeField] private Material myCubeMaterial;
        [FoldoutGroup("Reference")] [SerializeField] private Material enemyCubeMaterial;
        [FoldoutGroup("Reference")] [SerializeField] private List<MeshRenderer> renderers = new();
        [FoldoutGroup("Reference")] [SerializeField] private CubeActionsPack actionsPack;

        [FoldoutGroup("Reference")] [SerializeField] private SpriteRenderer upSide;
        [FoldoutGroup("Reference")] [SerializeField] private SpriteRenderer downSide;
        [FoldoutGroup("Reference")] [SerializeField] private SpriteRenderer rightSide;
        [FoldoutGroup("Reference")] [SerializeField] private SpriteRenderer leftSide;
        [FoldoutGroup("Reference")] [SerializeField] private SpriteRenderer forwardSide;
        [FoldoutGroup("Reference")] [SerializeField] private SpriteRenderer backwardSide;
        
        [FoldoutGroup("Reference")] [SerializeField] private CubeRotator cubeBody;

        public CubeRotator CubeRotator => cubeBody;
        public event Action<ActionType> OnRotationEnd;
        public bool IsBlockedInput => isBlockedInput;
        
        private bool isBlockedInput;
        private int currentActionType;

        private void Start()
        {
            // cubeBody.OnRotationEnd += () =>
            // {
            //     OnRotationEnd?.Invoke(GetSign());
            // };
        }

        [Button]
        private ActionType GetSign()
        {
            return actionsPack.GetAction(currentActionType);
        }

        public async void UpdateData(CubeDataPack cubeDataPack)
        {
            // CubeRotator.SetPos((transform.position - cubeDataPack.Position).normalized * 1.1f);
            //
            // await DOTween.To(
            //         () => CubeRotator.Progress, 
            //         value => CubeRotator.Progress = value, 1, 1)
            //     .SetEase(Ease.InOutSine)
            //     .AsyncWaitForCompletion();
            
            transform.position = (cubeDataPack.Position - new Vector3(4, 0, 4)) * 1.1f;
            transform.eulerAngles = Vector3.zero;
            currentActionType = cubeDataPack.UpSide;

            upSide.sprite       = actionsPack.GetIcon(cubeDataPack.UpSide);
            downSide.sprite     = actionsPack.GetIcon(GetMirroredValue(cubeDataPack.UpSide));
            rightSide.sprite    = actionsPack.GetIcon(cubeDataPack.RightSide);
            leftSide.sprite     = actionsPack.GetIcon(GetMirroredValue(cubeDataPack.RightSide));
            forwardSide.sprite  = actionsPack.GetIcon(cubeDataPack.ForwardSide);
            backwardSide.sprite = actionsPack.GetIcon(GetMirroredValue(cubeDataPack.ForwardSide));

            foreach (var ren in renderers)
            {
                ren.material = cubeDataPack.IsMyCube ? myCubeMaterial : enemyCubeMaterial;
            }

            isBlockedInput = !cubeDataPack.IsMyCube;
            
            int GetMirroredValue(int v)
            {
                return v switch {1 => 6, 2 => 5, 3 => 4, 4 => 3, 5 => 2, 6 => 1, _ => 0};
            }
        }
    }
}