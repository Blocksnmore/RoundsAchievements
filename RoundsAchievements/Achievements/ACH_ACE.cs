using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnboundLib.GameModes;
using UnityEngine;

public class ACH_ACE : ACH
{
    public new const string AchievementName = "ACH_ACE";
    private Boolean _isEligible = true;
    private int _points = 0;
    private int _teamID = -1;
    
    public new void AddListener(RoundsAchievements roundsAchievements)
    {
        GameModeManager.AddHook(GameModeHooks.HookGameStart, GameStart);
        GameModeManager.AddHook(GameModeHooks.HookPointEnd, GamePoint);    
    }

    private void Unload()
    {
        GameModeManager.RemoveHook(GameModeHooks.HookGameStart, GameStart);
        GameModeManager.RemoveHook(GameModeHooks.HookPointEnd, GamePoint);
    }

    private IEnumerator GameStart(IGameModeHandler handler)
    {
        _isEligible = true;
        _points = 0;
        _teamID = -1;
        return new WaitForSecondsRealtime(0.1f);
    }

    private IEnumerator GamePoint(IGameModeHandler handler)
    {
         if (!_isEligible || !RoundsAchievements.NotUnlockedAchievements.Contains(AchievementName))
         {
             Unload();
             
             return new WaitForSecondsRealtime(0.1f);
         }
            
         IEnumerable<Player> localPlayers = PlayerManager.instance.players.Where(p => p.data.view.IsMine);

         Player[] enumerable = localPlayers as Player[] ?? localPlayers.ToArray();
            
         int[] winners = handler.GetPointWinners();
            
         if (winners.Length != 1)
         {
             // No clue how this would work, so I'm not going to handle it - Bloxs
             _isEligible = false;
         }
         else
         {

             if (enumerable.Count() != 1)
             {
                 if (_teamID != -1)
                 {
                     if (winners[0] == _teamID)
                     {
                         _points++;
                     }
                     else
                     {
                         _isEligible = false;
                     }
                 }
                 else
                 {
                     _teamID = enumerable.First().teamID;
                     _points++;
                     
                     if (_points >= 5)
                     {
                         RoundsAchievements.UnlockAchievement(AchievementName);
                         Unload();
                         _isEligible = false;
                     }
                 }
             }
             else
             {

                 Player localPlayer = enumerable.First();
                 if (winners[0] == localPlayer.teamID)
                 {
                     _points++;

                     if (_points >= 5)
                     {
                         RoundsAchievements.UnlockAchievement(AchievementName);
                         Unload();
                         _isEligible = false;
                     }
                 }
                 else
                 {
                     _isEligible = false;
                 }
             }
         }

         return new WaitForSecondsRealtime(0.1f);
    }
}
