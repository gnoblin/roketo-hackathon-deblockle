using Sirenix.OdinInspector;
using UnityEngine;

namespace Deblockle.Gameplay
{
    public class CubeRotator : MonoBehaviour
    {
        [SerializeField] private Transform cube;

        public float Progress
        {
            get => progress;
            set
            {
                progress = value;
                if (isPosSet == false)
                {
                    return;
                }

                //end dragging 
                if (value > 0.99f)
                {
                    SetPositionToTarget();
                    return;
                }

                if (value < 0.5f)
                {
                    // 0.0 to 0.5
                    cube.position = Vector3.Lerp(startPos, middlePos, value*2);
                    // cube.eulerAngles = Vector3.Lerp(startRot, middleRot, value*2);

                    cube.eulerAngles = startRot;
                    cube.Rotate(rotateDir * 90 * value, Space.World);
                }
                else
                {
                    // 0.5 to 1.0
                    var posDiff = endPos - middlePos + Vector3.up * heightOffset/4;
                    cube.position = Vector3.Lerp(middlePos, endPos + posDiff, value-0.5f);
                    // cube.eulerAngles  = Vector3.Lerp(middleRot, startRot + rotateDir * 135, value-0.5f);
                    
                    cube.eulerAngles = startRot;
                    cube.Rotate(rotateDir * 90 * value, Space.World);
                }
            }
        }
        
        [SerializeField] private float heightOffset = 0.25f;

        private Vector3 startPos;
        private Vector3 startRot;
    
        private Vector3 middlePos;
        private Vector3 middleRot;

        private Vector3 endPos;
        private Vector3 endRot;

        private Vector3 rotateDir;
        private float progress;
        private bool isPosSet;

        [Button]
        public void GlobalRotate()
        {
            cube.Rotate(new Vector3(0, 0, 90), Space.World);
        }

        [Button]
        public void LocalRotate()
        {
            cube.Rotate(new Vector3(0, 0, 90), Space.Self);
        }
    
    
        [Button]
        public void SetPos(Vector2 offset)
        {
            Progress = 0;
            isPosSet = true;

            //pos
            startPos = cube.position;
            endPos = startPos + new Vector3(offset.x, 0, offset.y);
            middlePos = (startPos + endPos)/2 + Vector3.up * heightOffset;

            //rot
            rotateDir = Vector3.Cross(Vector3.up, (endPos - startPos).normalized);
            
            startRot = cube.eulerAngles;
            // middleRot = startRot + rotateDir * 45;
            // endRot = startRot + rotateDir * 90;
        }

        public void ReturnCube()
        {
            Progress = 0;
            if (isPosSet)
            {
                cube.position = startPos;
                cube.eulerAngles = startRot;
            }
        }
        
        private void SetPositionToTarget()
        {
            if (isPosSet == false)
            {
                return;
            }
            cube.position = endPos;
            // cube.eulerAngles = endRot;
            
            cube.eulerAngles = startRot;
            cube.Rotate(rotateDir * 90, Space.World);

            startPos = Vector3.zero;
            startRot = Vector3.zero;
            middlePos = Vector3.zero;
            middleRot = Vector3.zero;
            endPos = Vector3.zero;
            endRot = Vector3.zero;
            rotateDir = Vector3.zero;
            isPosSet = false;
            Progress = 0;
        }
    }
}
