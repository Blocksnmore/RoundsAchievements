using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnboundLib.GameModes;
using UnityEngine;

namespace RoundsAchievements.Achievements
{
    public class ACH_THREE : ACH
    {
        public new const string AchievementName = "ACH_THREE";
        private int _winCount = 0;

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

            Boolean didLocalUserWin = false;

            foreach (int winnerTeam in winners)
            {
                if (enumerable.Any(player => player.teamID == winnerTeam))
                {
                    _winCount++;
                    didLocalUserWin = true;

                    if (_winCount == 3)
                    {
                        RoundsAchievements.UnlockAchievement(AchievementName);
                        Unload();
                    }

                    break;
                }
            }

            if (!didLocalUserWin)
            {
                _winCount = 0;
            }

            return new WaitForSecondsRealtime(0.1f);
        }
    }
}