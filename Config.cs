using Exiled.API.Interfaces;

namespace Points
{
	public sealed class Config : IConfig
	{
		public bool IsEnabled { get; set; } = true;

		public bool EditMode { get; set; } = false;
	}
}
