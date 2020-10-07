using Exiled.API.Interfaces;

namespace ArithFeather.Points
{
	public sealed class Config : IConfig
	{
		public bool IsEnabled { get; set; } = true;

		public bool EditMode { get; set; } = false;
	}
}
