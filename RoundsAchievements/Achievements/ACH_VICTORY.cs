using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnboundLib.GameModes;
using UnityEngine;

public class ACH_VICTORY : ACH
{
    public new const string AchievementName = "ACH_VICTORY";

    public new void AddListener(RoundsAchievements roundsAchievements)
    {
        GameModeManager.AddHook(GameModeHooks.HookGameEnd, OnGameEnd);
    }

    private void Unload()
    {
        GameModeManager.RemoveHook(GameModeHooks.HookGameEnd, OnGameEnd);
    }

    public IEnumerator OnGameEnd(IGameModeHandler handler)
    {
        if (!RoundsAchievements.NotUnlockedAchievements.Contains(AchievementName))
        {
            Unload();
            return new WaitForSecondsRealtime(0.1f);
        }

        int[] winners = handler.GetGameWinners();
        IEnumerable<Player> localPlayers = PlayerManager.instance.players.Where(p => p.data.view.IsMine);

        Player[] enumerable = localPlayers as Player[] ?? localPlayers.ToArray();

        foreach (int winnerTeam in winners)
        {
            if (enumerable.Any(player => player.teamID == winnerTeam))
            {
                RoundsAchievements.UnlockAchievement(AchievementName);
                Unload();
                break;
            }
        }
        
        return new WaitForSecondsRealtime(0.1f);
    }
}