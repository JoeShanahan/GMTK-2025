using UnityEngine;

namespace Gmtk2025.Connectors
{
    public class CountdownSwapper : Connector
    {
        [SerializeField] private int _maxValue = 3;
        [SerializeField] private int _currentValue;
        [SerializeField] private Transform[] _counters;

        public override ConnectorType Type => ConnectorType.Countdown;
        public override int IntValue => _maxValue;
        
        public override void SetParameter(int number)
        {
            _maxValue = number;
        }
        
        private void Start()
        {
            if (_counters.Length != _maxValue)
            {
                Debug.LogError($"Wrong number of counter objects for CountdownSwapper! {_counters.Length} != {_maxValue}", gameObject);
            }

            _currentValue = _maxValue;
        }

        private void SetCounters()
        {
            for (int i = 0; i < _maxValue; i++)
            {
                _counters[i].gameObject.SetActive(i < _currentValue);
            }
        }
        
        public override void OnProjectilePassed(Projectile projectile, PlacedLoop currentLoop)
        {
            if (_currentValue == 0)
            {
                _currentValue = _maxValue;
                PlacedLoop toLoop = currentLoop == _loopA ? _loopB : _loopA;
                projectile.SwapBetweenLoops(currentLoop, toLoop);
            }
            else
            {
                _currentValue--;
            }
            
            SetCounters();
        }
    }
}
