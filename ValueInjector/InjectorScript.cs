namespace CagrsLib.ValueInjector
{
    public abstract class InjectorScript
    {
        private bool _isActive = true;

        private string _guid;

        private Injectable _bindInjectable;
        
        public abstract void OnBind();
        
        public abstract void InjectorUpdate(float deltaTime);
        public abstract void InjectorFixedUpdate(float deltaTime);
        public abstract void InjectorLateUpdate(float deltaTime);
        
        public abstract void OnUnbind();

        public abstract object OnInject(string name, object obj);

        public abstract void OnInvoke(string invokeName, object[] parameters);

        public abstract string GetDestruction();

        public void ActiveInjector()
        {
            _isActive = true;
        }

        public void DeactiveInjector()
        {
            _isActive = false;
        }

        public void Bind(Injectable injectable)
        {
            _bindInjectable = injectable;
            _guid = injectable.BindInjectorScript(this);
        }

        public virtual void Unbind()
        {
            if (_bindInjectable != null)
            {
                _bindInjectable.UnbindInjectorScript(_guid);
                OnUnbind();
            }
        }

        public bool IsActive()
        {
            return _isActive;
        }
    }

    public abstract class LifeInjectorScript : InjectorScript
    {
        private bool _isPause;
        
        private float _lifeSeconds;

        private bool _isFixedUpdate;

        protected LifeInjectorScript(float lifeSeconds, bool isFixedUpdate = true, bool isPause = true)
        {
            _lifeSeconds = lifeSeconds;
            _isFixedUpdate = isFixedUpdate;

            _isPause = isPause;
        }

        public override void InjectorUpdate(float deltaTime)
        {
            if (!_isFixedUpdate && !_isPause)
            {
                _lifeSeconds -= deltaTime;
                OutTimeCheck();
            }
        }

        public override void InjectorFixedUpdate(float deltaTime)
        {
            if (_isFixedUpdate && !_isPause)
            {
                _lifeSeconds -= deltaTime;
                OutTimeCheck();
            }
        }

        private void OutTimeCheck()
        {
            if (_lifeSeconds <= 0)
            {
                base.Unbind();
            }
        }

        public void AddTime(float seconds)
        {
            _lifeSeconds += seconds;
        }
        
        public void RemoveTime(float seconds)
        {
            _lifeSeconds -= seconds;
        }
        
        public void ResetTime(float seconds)
        {
            _lifeSeconds = seconds;
        }
        
        public void Pause()
        {
            _isPause = true;
        }
        
        public void Break()
        {
            _isPause = false;
        }

        public override void Unbind()
        {
            _lifeSeconds = 0;
        }

        public override void OnBind()
        {
        }

        public override void OnUnbind()
        {
        }

        public override void InjectorLateUpdate(float deltaTime)
        {
        }

        public override string GetDestruction()
        {
            return $"Life : {_lifeSeconds}";
        }
    }

    public abstract class SimpleInjectorScript : InjectorScript
    {
        public override void OnBind()
        {
        }

        public override void InjectorUpdate(float deltaTime)
        {
        }

        public override void InjectorFixedUpdate(float deltaTime)
        {
        }

        public override void InjectorLateUpdate(float deltaTime)
        {
        }
        
        public override void OnUnbind()
        {
        }

        public override string GetDestruction()
        {
            return "";
        }
    }
}
