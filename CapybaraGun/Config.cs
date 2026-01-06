using Exiled.API.Interfaces;

namespace CapybaraGun
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        public Handler CapybaraGun { get; set; } = new();
    }
}