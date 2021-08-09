using Exiled.API.Interfaces;

namespace Points
{
    public sealed class Config : IConfig
    {
        public bool EditMode { get; set; } = false;
        public bool IsEnabled { get; set; } = true;
    }
}