using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Deblockle.Managers;
using FawesomeLab.GameCore.Manager;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

namespace Managers
{
    public class JSBackend : SingleManager<JSBackend>
    {
        public string PlayerAccount => playerAccount;
        public string EnemyPlayerAccount => enemyPlayerAccount;
        
        private string playerAccount;
        private string enemyPlayerAccount;
        private int playerId;

        private Dictionary<string, Type> callbacks = new();

        [DllImport("__Internal")] private static extern void callGetAcc();

        [DllImport("__Internal")] private static extern void callGetPlayers();

        [DllImport("__Internal")] private static extern void callDepositWNear();

        // [DllImport("__Internal")] private static extern void callMakeMove(int fromX, int fromY, int toX, int toY);



        public void ApplyMove(int fromX, int fromY, int toX, int toY)
        {
            Debug.Log($"From {new Vector2(fromX, fromY)}, To {new Vector2(toX, toY)}");
        }

        public async void StartGame()
        {
            callDepositWNear();

            // var command = new JSCommand<Null, Null, Null>((input) => { callDepositWNear(); }, 
            //         new JSCallbackWrapper<Null, Null>(new Task<Null>(() => null)))
            //     .Run(null);
            //
            // await UniTask.WaitWhile(() => command.IsFinished == false);
        }
        
        public void ReceiveCallback(string callbackKey)
        {
            Debug.Log(callbackKey);
            // callbacks[callbackKey]?.Invoke();
        }
        
        
        public void ReceiveInfo(string info)
        {
            var output = JsonConvert.DeserializeObject<StringGameData>(info);

            if (output == null)
            {
                return;
            }
            
            CubePlacer.I.UpdateCubesInfo(
                output.board.Select(cube => new CubeDataPack(
                    IsPlayer(int.Parse(cube.player)), 
                    int.Parse(cube.forwardSide), 
                    int.Parse(cube.rightSide), 
                    int.Parse(cube.upSide), 
                    new Vector3(int.Parse(cube.positionX), 0, int.Parse(cube.positionY) * 1.1f)))
                    .ToArray());

            var phase = output.phase switch
            {
                "Roll" => GameStateEnum.Roll,
                "Hop" => GameStateEnum.Hop,
                "End" => GameStateEnum.End,
                _ => GameStateEnum.Roll
            };

            var targetPlayerID = playerId.ToString();
            var player = output.activePlayer == targetPlayerID ? GamePlayerEnum.Player : GamePlayerEnum.Enemy;
            
            GameProvider.I.SetNewState(phase, player);
        }

        public void SetPlayers(string[] players)
        {
            if (players[0] == null || players[1] == null)
            {
                return;
            }
            
            if (players[0] == playerAccount)
            {
                enemyPlayerAccount = players[1];
                playerId = 1;
            }
            else
            {
                enemyPlayerAccount = players[0];
                playerId = 2;
            }
        }

        public void SetPlayerAccount(string account)
        {
            playerAccount = account;
        }

        public bool IsPlayer(int byId)
        {
            return playerId == byId;
        }

        public override void Init(){}
    }
}

public class StringCubeData
{
    public string positionX;
    public string positionY;
    public string player;
    public string upSide;
    public string rightSide;
    public string forwardSide;
}

public class StringGameData
{
    public string phase;
    public string activePlayer;
    public string firstPlayer;
    public string secondPlayer;
    public StringCubeData[] board;
}