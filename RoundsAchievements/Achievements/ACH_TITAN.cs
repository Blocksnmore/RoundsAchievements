using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnboundLib.GameModes;
using UnityEngine;

public class ACH_TITAN : ACH
{
    public new const string AchievementName = "ACH_TITAN";

    public new void AddListener(RoundsAchievements roundsAchievements)
    {
        GameModeManager.AddHook(GameModeHooks.HookPlayerPickEnd, OnCardCollect);
    }

    private void Unload()
    {
        GameModeManager.RemoveHook(GameModeHooks.HookPlayerPickEnd, OnCardCollect);
    }

    private IEnumerator OnCardCollect(IGameModeHandler handler)
    {
        if (!RoundsAchievements.NotUnlockedAchievements.Contains(AchievementName))
        {
            Unload();
            return new WaitForSecondsRealtime(0.1f);
        }
        
        IEnumerable<Player> localPlayers = PlayerManager.instance.players.Where(p => p.data.view.IsMine);

        Player[] enumerable = localPlayers as Player[] ?? localPlayers.ToArray();

        foreach (Player player in enumerable)
        {
            float playerHealth = player.data.health;
            
            if (playerHealth >= 1000)
            {
                RoundsAchievements.UnlockAchievement(AchievementName);
                Unload();
                break;
            }
        }
        
        return new WaitForSecondsRealtime(0.1f);
    }
}