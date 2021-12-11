using Player;
using ShootCommon.Signals;
using UnityEngine;
using Zenject;

namespace Common
{
    public class CommonInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            
            MessageBrokerInstaller.Install(Container);
            SignalBusInstaller.Install(Container);
            SignalsInstaller.Install(Container);
            
            PlayerInstaller.Install(Container);
        }
    }
}