namespace ShootCommon.Signals
{
    public class Signal: ISignal
    {
    }

    public class Signal<TValue> : ISignal<TValue>
    {
        public TValue Value { get; set; }

        public Signal()
        {
        }
        
        public Signal(TValue value) => Value = value;
    }
}