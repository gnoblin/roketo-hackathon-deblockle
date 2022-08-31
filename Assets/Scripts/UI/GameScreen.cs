using Deblockle.Gameplay;
using Deblockle.Managers;
using FawesomeLab.UIBuilder.Core;
using FawesomeLab.UIBuilder.Core.UIElements;
using Managers;
using UnityEngine;

namespace Deblockle.UI
{
    public class GameScreen : ScreenView
    {
        private CubeRotator currentCube;
        private SwipeDirection currentDirection;

        private void Start()
        {
            var input = UIManager.I.GetWindow<InputScreen>();

            input.OnCubeSelected += selectedCube =>
            {
                ReleaseCube();
                currentCube = selectedCube;
            };

            input.OnCubeUnselected += unSelectedCube => ReleaseCube();
        }

        private void Update()
        {
            if (GameProvider.I.CurrentPlayer != GamePlayerEnum.Player)
            {
                return;
            }
         
            if (InputManager.I.mouseButton.Value == false)
            {
                return;
            }
            
            // Debug.Log($"Swipe {InputManager.I.CurrentSwipe.Direction} {InputManager.I.CurrentSwipe.Length}");
            var swipe = InputManager.I.CurrentSwipe;

            if (InputManager.I.SwipeStarted && currentCube != null)
            {
                if (swipe.Direction != currentDirection)
                {
                    currentCube.ReturnCube();
                    currentCube.SetPos(swipe.Vector);
                    currentDirection = swipe.Direction;
                }

                var progress = Mathf.Clamp(swipe.Length / 200, 0, 1f);
                currentCube.Progress = progress;

                if (progress > 0.99f)
                {
                    ReleaseCube();
                }
            }
        }

        private void ReleaseCube()
        {
            if (currentCube == null)
            {
                return;
            }
            
            
            
            InputManager.I.StopSwipe();
            currentDirection = SwipeDirection.Unknown;
            currentCube.Progress = currentCube.Progress > 0.4f ? 1f : 0f;
            currentCube = null;
            UIManager.I.GetWindow<InputScreen>().UnSelectCube();
        }
    }
}