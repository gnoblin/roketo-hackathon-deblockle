using Cysharp.Threading.Tasks;
using Deblockle.Gameplay;
using Deblockle.Managers;
using Deblockle.UI;
using DG.Tweening;
using FawesomeLab.Helpers.Extensions;
using FawesomeLab.UIBuilder.Core;
using UnityEngine;

namespace Deblockle.Actions
{
    public class SideMoveAction : CubeAction
    {
        public override async UniTask Process(Vector3 processForPosition, Transform cube)
        {
            var positions = FieldProcessor.I.GetPositionByAction(processForPosition, ActionType.SideMove);
            foreach (var pos in positions)
            {  
                FieldProcessor.I.SelectPos(pos);
            }

            var result = await UIManager.I.GetWindow<InputScreen>().WaitSelectionByTherePositions(positions);
            FieldProcessor.I.RemoveSelection();

            // distance / speed
            var moveTime = Vector3.Distance(cube.position.SetY(0), result.SetY(0)) / 4;
            var seq = DOTween.Sequence()
                .Append(cube
                    .DOMove(cube.position + Vector3.up, 0.15f)
                    .SetEase(Ease.InSine))
                .Append(cube
                    .DOMove(result + Vector3.up, moveTime)
                    .SetEase(Ease.InOutSine))
                .Append(cube
                    .DOMove(result, 0.15f)
                    .SetEase(Ease.OutSine))
                .AsyncWaitForCompletion();
            await seq;
        }
    }
}