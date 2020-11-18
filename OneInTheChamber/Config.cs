using Exiled.API.Interfaces;

namespace OneInTheChamber
{
	public class Config : IConfig
	{
		public bool IsEnabled { get; set; } = true;
		public string StartingBroadcastMessage { get; set; } = "One In The Chamber. Starting in $time second(s)";
		public string EliminationBroadcastMessage { get; set; } = "%user has been eliminated. $count players remain.";
		public float MaxDuration { get; set; } = 600f;
		public float RespawnTime { get; set; } = 5f;
		public int ScoreToWin { get; set; } = 21;
		public int Lives { get; set; } = 3;
		public float UspKillRange { get; set; } = 4f;
		public bool OnlyUseSurface { get; set; } = true;
		public bool DisableDecontamination { get; set; } = false;
		public bool InstantDecontamination { get; set; } = false;
	}
}
