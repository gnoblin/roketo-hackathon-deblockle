using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Deblockle.Managers;
using Deblockle.UI;
using FawesomeLab.GameCore.Manager;
using FawesomeLab.UIBuilder.Core;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Managers
{
    public class JSBackend : SingleManager<JSBackend>
    {
        public string EnemyPlayerAccount => enemyPlayerAccount;
        
        private string enemyPlayerAccount;
        private int playerId;
        private string playerWallet;

        private List<string> callbacks = new();

        [DllImport("__Internal")] private static extern void callGetPlayers();
        [DllImport("__Internal")] private static extern void callGetInfo();
        [DllImport("__Internal")] private static extern void callGetAcc();

        [DllImport("__Internal")] private static extern void callDepositWNear();
        
        [DllImport("__Internal")] private static extern void startStream();

        [DllImport("__Internal")] private static extern void callMakeMove(int fromX, int fromY, int toX, int toY);


        private Action<string[]> onPlayersReceived;


        public void ApplyMove(int fromX, int fromY, int toX, int toY)
        {
            Debug.Log($"From {new Vector2(fromX, fromY)}, To {new Vector2(toX, toY)}");
            callMakeMove(fromX, fromY, toX, toY);
        }

        private async void Start()
        {
#if UNITY_EDITOR
            UIManager.I.GetWindow<EnterScreen>().Hide();
            // return;
#endif
            var ui = UIManager.I.GetWindow<EnterScreen>();
            ui.ShowLoading();

            callGetAcc();
            await UniTask.WaitWhile(() => playerWallet == string.Empty);
            callGetPlayers();
            
            ui.ShowDeposit();
            ui.OnDepositButtonPressed += callDepositWNear;
            ui.OnStreamButtonPressed += startStream;
        }

        public void ReceivePlayers(string[] players)
        {
            if (players[0] != null && players[0] == playerWallet)
            {
                playerId = 1;
            }
            else if (players[1] != null && players[1] == playerWallet)
            {
                playerId = 2;
            }
        }

        public void ReceiveWallet(string wallet)
        {
            playerWallet = wallet;
        }

        [Button]
        public void ShowDepositButton()
        {
            Debug.Log("UNITY Show Deposit Button");
            UIManager.I.GetWindow<EnterScreen>().ShowDeposit();
        }

        
        [Button]
        public void ShowStreamButton()
        {
            Debug.Log("UNITY Show Stream Button");
            UIManager.I.GetWindow<EnterScreen>().ShowStream();
        }

        [Button]
        public void ReceiveInfo(string info)
        {
            StringGameData output;
            try
            {
                output = JsonConvert.DeserializeObject<StringGameData>(info);
            }
            catch (Exception e)
            {
                return;
            }

            UIManager.I.Hide<EnterScreen>();

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
                    new Vector3(int.Parse(cube.positionX), 0, int.Parse(cube.positionY))))
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