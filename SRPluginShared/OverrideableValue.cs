using System;

namespace SRPlugin
{
    public class OverrideableValue<T>
    {
        private bool wasSet;
        private Action<T> setter;
        private Func<T> originalValueGetter;
        private T defaultSetValue;

        private T _originalValue;
        private bool _originalValueFetched;

        public OverrideableValue(
            Func<T> originalValueGetter,
            Action<T> setter,
            T defaultSetValue = default
        )
        {
            this.originalValueGetter = originalValueGetter;
            this.setter = setter;
            this.defaultSetValue = defaultSetValue;
        }

        private void FetchOriginalValue()
        {
            if (!_originalValueFetched)
            {
                _originalValueFetched = true;
                _originalValue = originalValueGetter();
            }
        }

        public void Reset()
        {
            FetchOriginalValue();
            if (!wasSet)
            {
                return;
            }

            wasSet = false;
            setter(GetOriginal());
        }

        public void Set(T value)
        {
            FetchOriginalValue();
            wasSet = true;
            setter(value);
        }

        public void SetDefault()
        {
            FetchOriginalValue();
            Set(defaultSetValue);
        }

        public T GetOriginal()
        {
            FetchOriginalValue();
            return _originalValue;
        }
    }
}
