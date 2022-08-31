using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Deblockle.Gameplay;
using Deblockle.Managers;
using FawesomeLab.Helpers.Extensions;
using FawesomeLab.UIBuilder.Core.UIElements;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Deblockle.UI
{
    public class InputScreen : ScreenView
    {
        [FoldoutGroup("Preference")] [SerializeField] private LayerMask cubeLayerMask;
        [FoldoutGroup("Preference")] [SerializeField] private LayerMask everythingLayerMask;

        public event Action<CubeRotator> OnCubeSelected;
        public event Action<CubeRotator> OnCubeUnselected;

        public CubeRotator CurrentSelectedCube => currentSelectedCube;

        private Func<int, KeyValuePair<Vector3, Transform>> GetGlobalViewportMousePosition;

        private CubeRotator currentSelectedCube;
        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;

            GetGlobalViewportMousePosition = (layerMask) =>
            {
                var ray = mainCamera.ScreenPointToRay(InputManager.I.mousePosition.Value);
                return Physics.Raycast(ray, out var hit, 100, layerMask) 
                    ? new KeyValuePair<Vector3, Transform>(hit.point, hit.transform) 
                    : new KeyValuePair<Vector3, Transform>();
            };

            // Cube selecting
            InputManager.I.mouseButtonDown.OnChangedEvent += (old, v) =>
            {
                if (v == false)
                    return;

                var (_, cubeTransform) = GetGlobalViewportMousePosition.Invoke(cubeLayerMask);
                if (cubeTransform != null && cubeTransform.TryGetComponent(out CubeRotator cubeRotator))
                {
                    if (CubePlacer.I.GetCubeByRotator(cubeRotator).IsBlockedInput == false)
                    {
                        SelectCube(cubeRotator);
                    }
                }
            };

            // Cube unselecting
            InputManager.I.mouseButtonUp.OnChangedEvent += (old, v) =>
            {
                if (v == false)
                    return;
                UnSelectCube();
            };
        }

        public async UniTask<Vector3> WaitSelectionByTherePositions(List<Vector3> positions)
        {
            await UniTask.WaitUntil(() =>
            {
                if (InputManager.I.mouseButtonDown.Value == false)
                {
                    return false;
                }
                var mousePos = GetGlobalViewportMousePosition.Invoke(everythingLayerMask).Key;
                return positions.Any(pos => Vector3.Distance(pos.SetY(0), mousePos.SetY(0)) < 0.5f);
            });

            var mousePos = GetGlobalViewportMousePosition.Invoke(everythingLayerMask).Key;
            
            foreach (var pos in positions)
            {
                if (Vector3.Distance(pos.SetY(0), mousePos.SetY(0)) < 0.5f)
                {
                    return pos;
                }
            }

            return positions[0];
        }

        public void SelectCube(CubeRotator cube)
        {
            currentSelectedCube = cube;
            OnCubeSelected?.Invoke(cube);
        }

        public void UnSelectCube()
        {
            if (currentSelectedCube != null)
            {
                currentSelectedCube = null;
                OnCubeUnselected?.Invoke(currentSelectedCube);
            }
        }
    }
}