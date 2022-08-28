using FawesomeLab.Reactive;
using UnityEngine;

namespace Deblockle.UI
{
    public class SwipeInfo
    {
        public ReactiveValue<Vector2> SwipeStart = new(Vector2.zero);
        public ReactiveValue<Vector2> SwipeEnd = new(Vector2.zero);

        public Vector2 Vector
        {
            get
            {
                return Direction switch
                {
                    SwipeDirection.Unknown => new Vector2(0, 0),
                    SwipeDirection.Left =>  new Vector2(-1.1f, 0),
                    SwipeDirection.Right => new Vector2(1.1f, 0),
                    SwipeDirection.Up =>    new Vector2(0, 1.1f),
                    SwipeDirection.Down =>  new Vector2(0, -1.1f),
                };
            }
        }

        public SwipeDirection Direction
        {
            get
            {
                var verticalOffset = SwipeEnd.Value.y - SwipeStart.Value.y;
                var horizontalOffset = SwipeEnd.Value.x - SwipeStart.Value.x;
                
                // Center/Unknown
                if(Mathf.Abs(verticalOffset) < 5f && Mathf.Abs(horizontalOffset) < 5f)
                {
                    return SwipeDirection.Unknown;
                }
                
                if (Mathf.Abs(verticalOffset) > Mathf.Abs(horizontalOffset))
                {
                    //vertical
                    return verticalOffset > 0 ? SwipeDirection.Up : SwipeDirection.Down;
                }
                //horizontal
                return horizontalOffset > 0 ? SwipeDirection.Right : SwipeDirection.Left;
            }
        }

        public float Length => Vector2.Distance(SwipeStart, SwipeEnd);
    }

    public enum SwipeDirection
    {
        Unknown,
        Left,
        Right,
        Up,
        Down
    }
}