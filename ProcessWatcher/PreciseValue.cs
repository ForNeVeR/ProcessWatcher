namespace ProcessWatcher
{
    public class PreciseValue<T> where T : class
    {
        public T? Value { get; }
        public bool IsPrecise { get; }

        internal PreciseValue(T? value, bool isPrecise)
        {
            Value = value;
            IsPrecise = isPrecise;
        }
    }

    public class PreciseValue
    {
        public static PreciseValue<T> Precise<T>(T? value) where T : class => new PreciseValue<T>(value, true);
        public static PreciseValue<T> Imprecise<T>(T? value) where T : class => new PreciseValue<T>(value, false);
    }
}