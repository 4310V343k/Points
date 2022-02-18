namespace Points
{
    using Exiled.API.Interfaces;

    public sealed class Config : IConfig
    {
        public bool Debug { get; set; } = false;
        public bool EditMode { get; set; } = false;
        public bool IsEnabled { get; set; } = true;
    }
}
