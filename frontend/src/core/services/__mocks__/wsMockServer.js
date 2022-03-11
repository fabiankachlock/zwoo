const { WebSocketServer } = require('ws');
const readline = require('readline').createInterface({
    input: process.stdin,
    output: process.stdout
});

const wss = new WebSocketServer({ port: 9072 });

let senderFunc = () => { }

wss.on('connection', function connection(ws) {
    senderFunc = (str) => ws.send(str);
    ws.on('message', function message(data) {
        ws.send(data.toString())
    });
});

readline.on('line', (input) => {
    const args = input.split(' ');
    if (args.length < 2) return
    if (args[0] === 'send') {
        const msg = createExampleMessage(args[1], ...args.slice(2));
        console.log('sending: %s', msg);
        senderFunc(msg)
    }
})

const exampleMessages = {
    [001]: {
        test: 'testxxx'
    }
}

function createExampleMessage(code, ...args) {
    const exampleMessage = { ...exampleMessages[code] };
    for (const arg of args) {
        const [key, value] = arg.split(':');
        exampleMessage[key] = value;
    }
    return JSON.stringify(exampleMessage);
}
