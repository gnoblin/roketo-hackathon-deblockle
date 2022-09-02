using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FawesomeLab.UIBuilder.AddOns.Button;
using FawesomeLab.UIBuilder.Core;
using FawesomeLab.UIBuilder.Core.UIElements;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Deblockle.UI
{
    public class EnterScreen : ScreenView
    {
        [FoldoutGroup("Reference")] [SerializeField] private ButtonView depositButton;
        [FoldoutGroup("Reference")] [SerializeField] private ButtonView streamButton;

        [FoldoutGroup("Reference")] [SerializeField] private View loadingIcon;


        public event Action OnDepositButtonPressed
        {
            add => depositButton.OnClickEvent += value;
            remove => depositButton.OnClickEvent -= value;
        }
        
        public event Action OnStreamButtonPressed
        {
            add => streamButton.OnClickEvent += value;
            remove => streamButton.OnClickEvent -= value;
        }
        
        private void Start()
        {
            loadingIcon.Rect
                .DORotate(new Vector3(0, 0, 359), 1.5f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1);

            OnHideEndEvent += () =>
            {
                depositButton.ShowImmediately();
                streamButton.ShowImmediately();
                loadingIcon.HideImmediately();
            };
        }

        public void ShowLoading()
        {
            loadingIcon.Show();
            depositButton.Hide();
            streamButton.Hide();
        }

        public void ShowDeposit()
        {
            depositButton.Show();
            loadingIcon.Hide();
            streamButton.Hide();
        }

        public void ShowStream()
        {
            streamButton.Show();
            loadingIcon.Hide();
            depositButton.Hide();
        }
    }
}