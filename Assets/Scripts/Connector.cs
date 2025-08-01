using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Gmtk2025
{
    public enum ConnectorType
    {
        Unknown,
        Swap,
        Launch,
        Binary,
        Countdown,
        Portal
    }
    
    // TODO
    // Connectors can't be placed too close to each other
    public abstract class Connector : Placeable
    {
        public virtual ConnectorType Type => ConnectorType.Unknown;
        public virtual int IntValue => 0;

        public PlacedLoop LoopA => _firstLoop;
        public PlacedLoop LoopB => _attachedLoop;

        [SerializeField] private SpriteRenderer _ghostSprite;
        [SerializeField] private Transform _realVisuals;
        
        [SerializeField] protected PlacedLoop _firstLoop;

        [SerializeField] protected PlacedLoop _attachedLoop;

        [SerializeField] private Color _invalidColor = new Color(1,0, 0, 0.7f);
        [SerializeField] private Color _validColor = new Color(0,1, 0, 0.9f);
        
        private List<SnapPoint> _availableSnapPoints = new();
        
        public class SnapPoint
        {
            public Vector2 WorldPos;
            public PlacedLoop Loop;
        }
        
        public void SetLoops(PlacedLoop parent, PlacedLoop child)
        {
            _firstLoop = parent;
            _attachedLoop = child;
        }

        public void AttachLoop(PlacedLoop loop)
        {
            _attachedLoop = loop;
            _firstLoop.Connect(this, loop);
            loop.Connect(this, _firstLoop);
        }
        
        public override void SetAsGhost(float value)
        {
            _ghostSprite.gameObject.SetActive(true);
            _realVisuals.gameObject.SetActive(false);
            DetectAllSnapPoints();
        }
        
        public override void StopBeingAGhost()
        {
            _ghostSprite.gameObject.SetActive(false);
            _realVisuals.gameObject.SetActive(true);
        }
        
        public override void MoveTo(Vector3 worldPos)
        {
            SnapPoint closestSnap = null;
            float minDist = 1f;

            foreach (SnapPoint snap in _availableSnapPoints)
            {
                float dist = Vector2.Distance(snap.WorldPos, worldPos);

                if (dist < minDist)
                {
                    closestSnap = snap;
                    minDist = dist;
                }
            }

            if (closestSnap != null)
            {
                // TODO check if in range of the level
                transform.position = closestSnap.WorldPos;
                _ghostSprite.color = _validColor;
                CanPlace = true;
            }
            else
            {
                transform.position = worldPos;
                _ghostSprite.color = _invalidColor;
                CanPlace = false;
            }
        }
        
        public virtual void OnProjectilePassed(Projectile projectile, PlacedLoop currentLoop)
        {

        }

        public virtual void SetParameter(int number)
        {
            
        }

        private int GetSnapPointCount(float radius)
        {
            if (radius < 1) return 4;
            if (radius < 2) return 8;
            if (radius < 3) return 12;
            if (radius < 4) return 16;
            return 24;
        }
        
        
        
        
        private void DetectAllSnapPoints()
        {
            _availableSnapPoints.Clear();
            var level = FindFirstObjectByType<LevelController>();

            foreach (PlacedLoop loop in level.AllLoops)
            {
                int snapCount = GetSnapPointCount(loop.Radius);
                float tolerance = 0.4f / snapCount;
                
                
                for (int i = 0; i < snapCount; i++)
                {
                    float loopSpace = (1f / snapCount) * i;
                    bool isTaken = false;
                    
                    foreach (PlacedLoop.ConnectorInfo info in loop.Connectors)
                    {
                        isTaken |= Mathf.Abs(loopSpace - info.Offset) < tolerance;
                    }

                    if (isTaken == false)
                    {
                        _availableSnapPoints.Add(new SnapPoint()
                        {
                            Loop = loop,
                            WorldPos = loop.LoopSpaceToPosition(loopSpace)
                        });
                    }
                }
            }
        }
    }
}