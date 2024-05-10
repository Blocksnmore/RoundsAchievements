using System.Collections;
using BepInEx;
using RoundsAchievements.Achievements;
using Steamworks;
using UnboundLib;

namespace RoundsAchievements
{
	[BepInDependency("com.willis.rounds.unbound")]
	[BepInPlugin(ModID, ModName, ModVersion)]
	[BepInProcess("Rounds.exe")]
	public class RoundsAchievements : BaseUnityPlugin
	{
		public const string ModID = "one.wavestudios.bloxs.roundsachievements";
		public const string ModName = "RoundsAchievements";
		public const string ModVersion = "1.0.0";
		
		public static readonly string[] AllAchievements =
		{
			"ACH_ACE",
			"ACH_THREE",
			"ACH_IMPENETRABLE",
			"ACH_DESTROYER",
			"ACH_BEEKEEPER",
			"ACH_EXPENSIVE",
			"ACH_TITAN",
			"ACH_HEALER",
			"ACH_HYPERSONIC",
			"ACH_SHIELDMASTERY",
			"ACH_VICTORY"
		};

		public static readonly ArrayList NotUnlockedAchievements = new ArrayList();

		private void Awake()
		{
			UnityEngine.Debug.Log("RoundsAchievements loading");

			Unbound.RegisterCredits(
				"RoundsAchievements",
				new[] { "Bloxs" },
				new[] { "Github" },
				new[] { "https://github.com/Blocksnmore" }
			);
		}

		private void Start()
		{
			Unbound.RegisterClientSideMod("one.wavestudios.bloxs.roundsachievements");
			UnityEngine.Debug.Log("RoundsAchievements started");
			if (SteamManager.Initialized)
			{
				CSteamID userID = SteamUser.GetSteamID();

				foreach (string achievement in AllAchievements)
				{
					if (!SteamUserStats.GetUserAchievement(userID, achievement, out bool achieved))
					{
						UnityEngine.Debug.Log("Failed to get achievement " + achievement);
					}
					else if (!achieved)
					{
						NotUnlockedAchievements.Add(achievement);
					}
				}

				UnityEngine.Debug.Log("Not unlocked achievements: " + NotUnlockedAchievements.Count + "/" +
				                      AllAchievements.Length);
			}
			else
			{
				UnityEngine.Debug.Log("Steam not initialized");
			}

			foreach (ACH achievementHandler in new ACH[]
			         {
				         new ACH_ACE(),
				         new ACH_BEEKEEPER(),
				         new ACH_DESTROYER(),
				         new ACH_EXPENSIVE(),
				         new ACH_HEALER(),
				         new ACH_HYPERSONIC(),
				         new ACH_IMPENETRABLE(),
				         new ACH_SHIELDMASTERY(),
				         new ACH_THREE(),
				         new ACH_TITAN(),
				         new ACH_VICTORY()
			         })
			{
				if (NotUnlockedAchievements.Contains(achievementHandler.AchievementName))
				{
					achievementHandler.AddListener(this);
				}
			}
		}

		public static void UnlockAchievement(string achievement)
		{
			if (SteamManager.Initialized)
			{
				NotUnlockedAchievements.Remove(achievement);
				SteamUserStats.SetAchievement(achievement);
				SteamUserStats.StoreStats();
			}
		}
	}
}