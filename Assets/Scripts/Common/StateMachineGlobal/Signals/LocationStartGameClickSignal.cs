using ShootCommon.Signals;

namespace Packages.Common.StateMachineGlobal.Signals
{
    public class LocationStartGameClickSignal : Signal
    {
        public string LocationId;
        public string MissionId;
        public string LevelId;
        public int LevelIndex;
    }
}