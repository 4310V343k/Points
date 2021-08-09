using HarmonyLib;

namespace Points.Internal
{
	[HarmonyPatch(typeof(HostItemSpawner), "Spawn")]
	internal static class LoadSpawnPointPatch
	{
		internal static event Points.LoadSpawnPoints OnLoadSpawnPoints;

		private static void Prefix()
		{
			OnLoadSpawnPoints?.Invoke();
		}
	}
}
