using Deblockle.UI;
using FawesomeLab.GameCore.Manager;
using FawesomeLab.Reactive;
using UnityEngine;

namespace Deblockle.Managers
{
    public class InputManager : SingleManager<InputManager>
    {
        public ReactiveBool mouseButton = new ();
        public ReactiveBool mouseButtonUp = new ();
        public ReactiveBool mouseButtonDown = new ();
        public ReactiveValue<Vector2> mousePosition = new (Vector2.zero);
        
        public SwipeInfo CurrentSwipe;
        public bool SwipeStarted => CurrentSwipe.Length > 5;

        public override void Awake()
        {
            base.Awake();
            Init();
        }

        public override void Init()
        {
            CurrentSwipe = new SwipeInfo();
        }

        public void StopSwipe()
        {
            CurrentSwipe.SwipeStart.Value = mousePosition.Value;
            CurrentSwipe.SwipeEnd.Value = mousePosition.Value;
        }

        private void Update()
        {
            if(mouseButton.Value != Input.GetMouseButton(0))
                mouseButton.Value = Input.GetMouseButton(0);
            
            if(mouseButtonUp.Value != Input.GetMouseButtonUp(0)) 
                mouseButtonUp.Value = Input.GetMouseButtonUp(0);
            
            if(mouseButtonDown.Value != Input.GetMouseButtonDown(0)) 
                mouseButtonDown.Value = Input.GetMouseButtonDown(0);

            if (mousePosition.Value != new Vector2(Input.mousePosition.x, Input.mousePosition.y))
            {
                mousePosition.Value = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
            
            if (mouseButton.Value)
            {
                CurrentSwipe.SwipeEnd.Value = mousePosition.Value;
            }
            
            if (mouseButtonDown.Value)
            {
                CurrentSwipe.SwipeStart.Value = mousePosition.Value;
                CurrentSwipe.SwipeEnd.Value = mousePosition.Value;
            }

            if (mouseButtonUp.Value)
            {
                CurrentSwipe.SwipeStart.Value = mousePosition.Value;
                CurrentSwipe.SwipeEnd.Value = mousePosition.Value;
            }
        }
    }
}