const { keyStores, connect, WalletConnection, Contract, utils } = nearApi
const blockleContractAddress = 'deblockle-v2.hawthorne.testnet';
const streamingAddress = 'streaming-r-v2.dcversus.testnet';
const wrapAddress = 'wrap.testnet';
const nearConfig = {
    networkId: 'testnet',
    nodeUrl: 'https://rpc.testnet.near.org',
    contractAddress: blockleContractAddress,
    walletUrl: 'https://wallet.testnet.near.org',
    helperUrl: 'https://helper.testnet.near.org'
}

let near
let walletConnection
let blockleContract
let wrapContract
const ONE_NEAR = 1000000000000000000000000;
const pricePerGame = 3;
const logOut = async () => {
    await walletConnection.signOut()
}

const requestSignIn = async () => {
    await walletConnection.requestSignIn(
        blockleContractAddress
    )
}

const setPlayerAccount = async() => {
    try {
    window.unityInst.SendMessage('JS Backend', 'SetPlayerAccount', walletConnection.getAccountId());
    }
    catch (e) {
        console.log(e.message);
    }
}
const isSignIn = async () => {
    const res = await walletConnection.isSignedIn()
    return res
}
const showDepositButton = async() =>{
    window.unityInst.SendMessage('JS Backend', 'ShowDepositButton');
}

const showStreamButton = async() => {
    window.unityInst.SendMessage('JS Backend', 'ShowStreamButton');
}
async function periodically(func, interval, ...args) {
    try {
      await func(...args);
  
    const timeout = setTimeout(
      () => {
        clearTimeout(timeout);
        periodically(func, interval, ...args);
      },
      interval,
    );
    } catch (err) {
      console.error('periodically error: ', err);
    }
}

const connectToNear = async () => {
    near = await connect({
        deps: {
            keyStore: new keyStores.BrowserLocalStorageKeyStore()
        },
        ...nearConfig
    })

    walletConnection = new WalletConnection(near)

    blockleContract = new Contract(walletConnection.account(), blockleContractAddress, {
        viewMethods: ['status', 'get_game', 'first_player', 'streaming_id', 'second_player', 'game_state', 'cube_state', 'check_winner'],
        changeMethods: ['start', 'pass_move', 'make_move', 'reset', 'connect_streaming_contract'],
        sender: walletConnection.getAccountId()
    })

    wrapContract = new Contract(walletConnection.account(), 'wrap.testnet', {
        viewMethods: ['ft_balance_of', 'storage_balance_of'],
        changeMethods: ['near_deposit', 'ft_transfer_call'],
        sender: walletConnection.getAccountId()
    })

    console.log(await isSignIn())
}

const getInfo = async () => {
    const players = await getPlayer();
    
    if (players.first_player == null || players.second_player == null) {
        console.log("one of players is null");
        const a = "ss";
        window.unityInst.SendMessage('JS Backend', 'ReceiveInfo', a);
        return
    };
    const gameInfo = await blockleContract.game_state();
    console.log(gameInfo);
    const infoObj = {};
    infoObj.board = [];
    gameInfo.board.forEach(element => {
        const cubeObj = {
            positionX: element.position.x,
            positionY: element.position.y,
            player: element.player,
            upSide: element.direction.up,
            rightSide: element.direction.right,
            forwardSide: element.direction.front
        }
        infoObj.board.push(cubeObj)
    });

    infoObj.phase = gameInfo.phase;
    infoObj.activePlayer = gameInfo.active_player
    console.log(infoObj);
    window.unityInst.SendMessage('JS Backend', 'ReceiveInfo', JSON.stringify(infoObj));
    return gameInfo;
}

const reset = async () => {
    await blockleContract.reset();
}

const getPlayer = async () => {
    const info = await blockleContract.status();
    console.log(info);
    return info;
}

const startGame = async () => {
    await blockleContract.start({
        args: {},
        gas: "300000000000000"
    })
    periodically(getInfo, 3000);
}

const makeMove = async (fromX, fromY, toX, toY) => {
    const move = await blockleContract.make_move({ args:{"from_x": fromX, "from_y": fromY, "to_x": toX, "to_y": toY },gas: "300000000000000"});
    return move;
}

const passMove = async () => {
    const pass = await blockleContract.passMove();
}

const registerStreamingContract = async() => {
    await blockleContract.connect_streaming_contract({"streaming_id": streamingAddress});
}

const isEnoughWNearForGame = async() => {
    const balance = await getWNearBalance();
    console.log(utils.format.formatNearAmount(balance),pricePerGame);
    console.log(utils.format.formatNearAmount(balance) >= utils.format.parseNearAmount(pricePerGame.toString()));
    return utils.format.formatNearAmount(balance) >= pricePerGame;
}
const getWNearBalance = async () => {
    const balance = await wrapContract.ft_balance_of({ 'account_id': walletConnection.account().accountId });
    return balance;
}
const depositWNear = async () => {
    const isStorageAvailable = await wrapContract.storage_balance_of({ 'account_id': walletConnection.account().accountId });
    if (isStorageAvailable === null) {
        console.log(isStorageAvailable);
        const storageDeposit = await wrapContract.near_deposit({
            args: {},
            gas: "300000000000000",
            amount: "1250000000000000000000"
        });
    }
    const balance = await getWNearBalance();
    console.log(balance);
    console.log(utils.format.parseNearAmount(pricePerGame.toString()));
    console.log();
    if (utils.format.formatNearAmount(balance) < pricePerGame) {
        const res = await wrapContract.near_deposit({
            args: {},
            gas: "300000000000000",
            amount: utils.format.parseNearAmount(pricePerGame.toString())
        })
    }

    return balance;
}

const startStream = async () => {
    const message = {
        tokens_per_sec: "10000"
    };
    console.log(message);
    console.log(blockleContractAddress);
    const stream = wrapContract.ft_transfer_call(
        {
            args: {
                "receiver_id": blockleContractAddress,
                "amount": utils.format.parseNearAmount(pricePerGame.toString()),
                msg: JSON.stringify(message)
            },
            amount: "1",
            gas: "300000000000000"
        }
    );

    console.log(stream);

}
connectToNear()