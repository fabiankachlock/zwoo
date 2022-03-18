const { WebSocketServer } = require('ws');
const readline = require('readline').createInterface({
    input: process.stdin,
    output: process.stdout
});

const wss = new WebSocketServer({ port: 9072 });

let senderFunc = () => { }

wss.on('connection', function connection(ws) {
    console.log('connected')
    senderFunc = (str) => ws.send(str);
});

readline.on('line', (input) => {
    const args = input.split(' ');
    if (args.length < 2) return
    if (args[0] === 'send') {
        const msg = createExampleMessage(parseInt(args[1], 10) ?? 0, ...args.slice(2));
        console.log('sending: %s', msg);
        senderFunc(msg)
    }
})

const exampleMessages = {
    [100]: {
        name: 'test-player',
        wins: 3
    },
    [101]: {
        name: 'test-player',
        wins: 3
    },
    [102]: {
        name: 'test-player',
        wins: 3
    },
    [103]: {
        name: 'test-player',
        wins: 3
    },
    [104]: {
        message: 'test-message'
    },
    [105]: {
        message: 'test-message',
        name: 'test-player',
        role: 'my-role'
    },
    [110]: {},
    [111]: {},
    [112]: {},
    [113]: {},
    [114]: {},
    [115]: {},
    [200]: {
        setting: 'my-setting',
        value: true
    },
    [201]: {
        setting: 'my-setting',
        value: true
    },
    [202]: {},
    [300]: {},
    [301]: {},
    [302]: {},
    [303]: {},
    [304]: {
        type: 'x',
        symbol: 'y'
    },
    [305]: {},
    [306]: {
        type: 'x',
        symbol: 'y'
    },
    [307]: {
        type: 'x',
        symbol: 'y'
    },
    [308]: {
        type: 'x',
        symbol: 'y'
    },
    [399]: {
        name: 'test-player',
        wins: 3
    },
    [400]: {
        message: 'some-error'
    },
    [433]: {
        message: 'some-error'
    },
    [434]: {
        message: 'some-error'
    },
}

function createExampleMessage(code, ...args) {
    const exampleMessage = { ...exampleMessages[code] };
    for (const arg of args) {
        const [key, value] = arg.split(':');
        exampleMessage[key] = value;
    }
    return code.toString().padStart(3, '0') + ',' + JSON.stringify(exampleMessage);
}
