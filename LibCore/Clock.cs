namespace CagrsLib.LibCore
{
    public class Clock
    {
        private float _past;

        private float _timer;
        
        private bool _enable;

        private bool _timerSingle;

        private TimerMode _mode;
        
        public void StartWithTimer(float timer, TimerMode mode)
        {
            _timer = timer;
            _mode = mode;
            Start();
        }

        public void Start()
        {
            _enable = true;
        }

        public void Stop()
        {
            _enable = false;
        }

        public void Reset()
        {
            _past = 0;
            _timerSingle = false;
        }

        public float Get()
        {
            return _past;
        }

        public bool IsOutTime()
        {
            return _timerSingle;
        }

        public void Update(float deltaTime)
        {
            _timerSingle = false;
            
            if (_enable)
            {
                _past += deltaTime;
            }

            if (_timer > 0 && _past >= _timer)
            {
                _timerSingle = true;

                switch (_mode)
                {
                    case TimerMode.Stop:
                        Stop();
                        break;
                    case TimerMode.Reset:
                        Stop();
                        Reset();
                        break;
                    case TimerMode.Restart:
                        Stop();
                        Reset();
                        Start();
                        break;
                    case TimerMode.Keep:
                        break;
                }
            }
        }

        public enum TimerMode
        {
            Stop, Reset, Restart, Keep
        }
    }
}