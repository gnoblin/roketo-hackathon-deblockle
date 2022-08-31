using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Deblockle.Actions
{
    public abstract class CubeAction
    {
        public abstract UniTask Process(Vector3 processForPosition, Transform cube);
    }
}