using UnityEngine;
using DG.Tweening;

namespace Gmtk2025
{
    public class CameraLevelFramer : MonoBehaviour
    {
        [SerializeField] private Camera _cam;

        [SerializeField] private LevelController _level;
        
        public void ZoomOutPressed()
        {
            DOTween.Kill(_cam);
            float orthoSize = Mathf.Max(5, Mathf.Ceil(_cam.orthographicSize + 1));
            _cam.DOOrthoSize(orthoSize, 1f).SetEase(Ease.OutExpo);
        }

        public void ZoomInPressed()
        {
            DOTween.Kill(_cam);
            float orthoSize = Mathf.Max(5, Mathf.Floor(_cam.orthographicSize - 1));
            _cam.DOOrthoSize(orthoSize, 1f).SetEase(Ease.OutExpo);
        }

        public void SnapFrameLevel()
        {
            Rect levelRect = _level.GetLevelBounds();

            Vector3 center = levelRect.center;
            center.z = _cam.transform.position.z;

            DOTween.Kill(_cam.transform);
            _cam.transform.position = center;
            
            DOTween.Kill(_cam);
            _cam.orthographicSize = Mathf.Max(5, levelRect.height + 4);
        }

        public void FramePressed()
        {
            Rect levelRect = _level.GetLevelBounds();
            
            /*
            Vector3 tl = new Vector3(levelRect.x, levelRect.y, 0);
            Vector3 tr = new Vector3(levelRect.x + levelRect.width, levelRect.y);
            Vector3 bl = new Vector3(levelRect.x, levelRect.y + levelRect.height, 0);
            Vector3 br = new Vector3(levelRect.x + levelRect.width, levelRect.y + levelRect.height);
            
            Debug.DrawLine(tl, tr, Color.red, 5);
            Debug.DrawLine(bl, br, Color.red, 5);
            Debug.DrawLine(bl, tl, Color.red, 5);
            Debug.DrawLine(tr, br, Color.red, 5);
            */
            
            Vector3 center = levelRect.center;
            center.z = _cam.transform.position.z;

            DOTween.Kill(_cam.transform);
            _cam.transform.DOMove(center, 1f).SetEase(Ease.OutExpo);
            
            DOTween.Kill(_cam);
            float orthoSize = Mathf.Max(5, levelRect.height + 4);
            _cam.DOOrthoSize(orthoSize, 1f).SetEase(Ease.OutExpo);
        }
    }
}
