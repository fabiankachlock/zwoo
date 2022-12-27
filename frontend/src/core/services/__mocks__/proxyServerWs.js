// eslint-disable-next-line no-undef
const httpProxy = require('http-proxy');

const proxy = httpProxy.createServer({
  target: 'ws://zwoo.igd20.de:8000',
  ws: true
});

proxy.on('close', () => {
  console.log('WS: CLOSE');
});

proxy.on('open', proxySocket => {
  console.log('WS: OPEN');
  proxySocket.on('close', d => console.log('ws-proxy-close: error:', d));
  proxySocket.on('connect', () => console.log('ws-proxy-connect'));
  proxySocket.on('data', d => console.log('ws-proxy-data: ', d));
  proxySocket.on('drain', () => console.log('ws-proxy-drain'));
  proxySocket.on('end', () => console.log('ws-proxy-end'));
  proxySocket.on('error', d => console.log('ws-proxy-error: ', d));
  proxySocket.on('lookup', (err, addr, fam, host) => console.log('ws-proxy-lookup: ', { err, addr, fam, host }));
  proxySocket.on('ready', () => console.log('ws-proxy-ready'));
  proxySocket.on('timeout', () => console.log('ws-proxy-timeout'));
});

proxy.on('error', (err, req) => {
  console.log('ERR: ' + req.method + ' ' + req.url + ' ' + err);
});

console.log('started!');
proxy.listen(8006);
