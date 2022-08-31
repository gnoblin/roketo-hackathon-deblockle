using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FawesomeLab.UIBuilder.AddOns.Button;
using FawesomeLab.UIBuilder.Core;
using FawesomeLab.UIBuilder.Core.UIElements;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Deblockle.UI
{
    public class EnterScreen : ScreenView
    {
        [FoldoutGroup("Reference")] [SerializeField] private ButtonView loginButton;
        [FoldoutGroup("Reference")] [SerializeField] private ButtonView secondButton;

        [FoldoutGroup("Reference")] [SerializeField] private View loadingIcon;
        
        private void Start()
        {
            loginButton.OnClickEvent += async () =>
            {
                OnAnyButtonPressed();
                
#if UNITY_EDITOR
                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(1f, 2f)));
#else

#endif
                
                UIManager.I.ShowImmediately<InputScreen>(isSolo: false);
                UIManager.I.Show<GameScreen>(isSolo: false);
                Hide();
            };

            secondButton.OnClickEvent += async () =>
            {
                OnAnyButtonPressed();

#if UNITY_EDITOR
                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(1f, 2f)));
#else

#endif
                
                UIManager.I.ShowImmediately<InputScreen>(isSolo: false);
                UIManager.I.Show<GameScreen>(isSolo: false);
                Hide();
            };

            loadingIcon.Rect
                .DORotate(new Vector3(0, 0, 359), 1.5f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1);

            async void OnAnyButtonPressed()
            {
                loginButton.Hide();
                secondButton.Hide();
                await UniTask.Delay(TimeSpan.FromSeconds(0.25f));
                loadingIcon.Show();
            }

            OnHideEndEvent += () =>
            {
                loginButton.ShowImmediately();
                secondButton.ShowImmediately();
                loadingIcon.HideImmediately();
            };
        }
    }
}