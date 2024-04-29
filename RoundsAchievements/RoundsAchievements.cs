using System.Collections;
using BepInEx;
using Steamworks;
using UnboundLib;

[BepInDependency("com.willis.rounds.unbound")]
[BepInPlugin("one.wavestudios.bloxs.roundsachievements", "RoundsAchievements", "1.0.0")]
[BepInProcess("Rounds.exe")]
public class RoundsAchievements : BaseUnityPlugin
{
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

			UnityEngine.Debug.Log("Not unlocked achievements: " + NotUnlockedAchievements.Count + "/" + AllAchievements.Length);
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
