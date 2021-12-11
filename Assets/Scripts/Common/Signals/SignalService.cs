using System;
using System.Collections.Generic;
using UniRx;
using Zenject;

namespace ShootCommon.Signals
{
    public class SignalService: ISignalService
    {
        
        private Dictionary<Type, ISignal> _cache = new Dictionary<Type, ISignal>();

        private IMessageBroker _messageBroker;
        
        [Inject]
        public void Init(
            IMessageBroker messageBroker
            )
        {
            _messageBroker = messageBroker;
        }
        
        [Inject]
        public void Init(ISignal[] declaredCache)
        {
            for (var i = 0; i != declaredCache.Length; i++)
            {
                var signal = declaredCache[i];
                _cache[signal.GetType()] = signal;
            }
        }

        public void Publish<TSignal>(TSignal signal = null)
            where TSignal: class, ISignal, new()
        {
            _messageBroker.Publish(signal ?? new TSignal());
        }

        public IObservable<TSignal> Receive<TSignal>() 
            where TSignal : ISignal
        {
            return _messageBroker.Receive<TSignal>();
        }
    }
}