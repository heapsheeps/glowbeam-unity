using System;

namespace GlowBeam
{
    [Serializable]
    public class Id
    {
        public Guid id { get; private set; }
        
        private Action _onChanged;
        public Action OnChanged
        {
            get { return _onChanged; }
            set
            {
                _onChanged = value;
                //MarkChanged(); // Generally OnChanged is initialized right after object is created, so it's fine to leave this commented
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public Id()
        {
            id = Guid.NewGuid();
        }

        protected void MarkChanged()
        {
            _onChanged?.Invoke();
        }
    }
}
