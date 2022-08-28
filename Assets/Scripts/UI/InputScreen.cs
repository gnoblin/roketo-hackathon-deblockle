using System;
using Deblockle.Gameplay;
using Deblockle.Managers;
using FawesomeLab.UIBuilder.Core.UIElements;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Deblockle.UI
{
    public class InputScreen : ScreenView
    {
        [FoldoutGroup("Preference")] [SerializeField] private LayerMask cubeLayerMask;
        
        public event Action<CubeRotator> OnCubeSelected;
        public event Action<CubeRotator> OnCubeUnselected;
        
        public CubeRotator CurrentSelectedCube => currentSelectedCube;
      
        private CubeRotator currentSelectedCube;
        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;

            // Cube selecting
            InputManager.I.mouseButtonDown.OnChangedEvent += (old, v) =>
            {
                if (v == false)
                    return;

                var ray = mainCamera.ScreenPointToRay(InputManager.I.mousePosition.Value);
                if (Physics.Raycast(ray, out var hit, 100, cubeLayerMask))
                {
                    if (hit.transform.TryGetComponent(out CubeRotator cubeRotator))
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