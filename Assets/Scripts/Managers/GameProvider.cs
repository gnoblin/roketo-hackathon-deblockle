using System;
using FawesomeLab.GameCore.Manager;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Managers
{
    public class GameProvider : SingleManager<GameProvider>
    {
        public GameStateEnum GameState => gameState;
        public GamePlayerEnum CurrentPlayer => currentPlayer;

        public event Action<GameProvider> OnStateChanged;
        
        [SerializeField] [ReadOnly] private GamePlayerEnum currentPlayer;
        [SerializeField] [ReadOnly] private GameStateEnum gameState;
        
        public override void Init()
        {
            
        }

        public void SetNewState(GameStateEnum newState, GamePlayerEnum newPlayer)
        {
            OnStateChanged?.Invoke(this);
            
            currentPlayer = newPlayer;
            gameState = newState;
        }
    }

    public enum GameStateEnum
    {
        Roll,
        Hop,
        End
    }

    public enum GamePlayerEnum
    {
        Player,
        Enemy
    }
}