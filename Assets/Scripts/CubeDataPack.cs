using UnityEngine;

namespace Deblockle.Managers
{
    public class CubeDataPack
    {
        public CubeDataPack(bool isMyCube, int forwardSide, int rightSide, int upSide, Vector3 position)
        {
            this.IsMyCube = isMyCube;
            this.ForwardSide = forwardSide;
            this.RightSide = rightSide;
            this.UpSide = upSide;
            this.Position = position;
        }

        public Vector3 Position { get; private set; }
        public int UpSide{ get; private set; }
        public int RightSide{ get; private set; }
        public int ForwardSide{ get; private set; }
        public bool IsMyCube{ get; private set; }
    }
}