const { WebSocketServer } = require('ws');
const readline = require('readline').createInterface({
  input: process.stdin,
  output: process.stdout
});

const wss = new WebSocketServer({ port: 9071 });

let senderFunc = () => {
  return;
};

wss.on('connection', function connection(ws) {
  console.log('connected');
  ws.on('message', msg => console.log(`received: ${msg}`));
  senderFunc = str => ws.send(str);
});

readline.on('line', input => {
  const args = input.split(' ');
  if (args.length < 2) return;
  if (args[0] === 'send') {
    const msg = createExampleMessage(parseInt(args[1], 10) ?? 0, ...args.slice(2));
    console.log('sending: %s', msg);
    senderFunc(msg);
  }
});

const exampleMessages = {
  [100]: {
    username: 'test-player',
    wins: 3
  },
  [101]: {
    username: 'test-player',
    wins: 3
  },
  [102]: {
    username: 'test-player',
    wins: 3
  },
  [103]: {
    username: 'test-player',
    wins: 3
  },
  [104]: {
    message: 'test-message'
  },
  [105]: {
    message: 'test-message',
    username: 'test-player',
    role: 3
  },
  [108]: {},
  [109]: {
    players: [
      {
        username: 'test-batch-p',
        wins: 0,
        role: 2
      },
      {
        username: 'test-batch-s',
        wins: 0,
        role: 3
      }
    ]
  },
  [110]: {},
  [111]: {},
  [112]: {},
  [113]: {},
  [114]: {},
  [115]: {},
  [200]: {
    setting: 'my-setting',
    value: 1
  },
  [201]: {
    setting: 'my-setting',
    value: 0
  },
  [202]: {},
  [203]: {
    settings: [
      {
        setting: 'maxPlayers',
        value: 5
      },
      {
        setting: 'maxDraw',
        value: 108
      },
      {
        setting: 'maxCardsOnHand',
        value: 108
      },
      {
        setting: 'initialCards',
        value: 7
      }
    ]
  },
  [210]: {},
  [300]: {},
  [301]: {},
  [302]: {},
  [303]: {},
  [304]: {
    type: 2,
    symbol: 2
  },
  [305]: {},
  [306]: {
    type: 2,
    symbol: 2
  },
  [307]: {
    type: 2,
    symbol: 2
  },
  [308]: {
    pileTop: {
      type: 2,
      symbol: 2
    },
    activePlayer: 'test1',
    activePlayerCardAmount: 1,
    lastPlayer: 'test2',
    lastPlayerCardAmount: 1
  },
  [310]: {},
  [311]: {
    hand: [
      {
        type: 2,
        symbol: 2
      }
    ]
  },
  [312]: {},
  [313]: {
    players: [
      {
        username: 'test1',
        cards: 4,
        isActivePlayer: true
      },
      {
        username: 'test2',
        cards: 5,
        isActivePlayer: false
      }
    ]
  },
  [314]: {},
  [315]: {
    type: 2,
    symbol: 2
  },
  [316]: {
    type: 1
  },
  [317]: {},
  [399]: {
    username: 'test-player',
    wins: 3
  },
  [400]: {
    message: 'some-error'
  },
  [420]: {},
  [433]: {
    message: 'some-error'
  },
  [434]: {
    message: 'some-error'
  }
};

function createExampleMessage(code, ...args) {
  const exampleMessage = { ...exampleMessages[code] };
  for (const arg of args) {
    const [key, value] = arg.split(':');
    exampleMessage[key] = value;
  }
  return code.toString().padStart(3, '0') + ',' + JSON.stringify(exampleMessage);
}
