// eslint-disable-next-line no-undef
const http = require('http');
// eslint-disable-next-line no-undef
const httpProxy = require('http-proxy');

var proxy = new httpProxy.createProxyServer({
  target: 'http://zwoo.igd20.de:8000/',
  changeOrigin: true
});

//var proxyServer = http.createServer((req, res) => {
//  proxy.web(req, res);
//});
//
//proxyServer.on('upgrade', (req, socket, head) => {
//  console.log('WS: ' + req.method + ' ' + req.url);
//  socket.on('close', d => console.log('ws-close: error:', d));
//  socket.on('connect', () => console.log('ws-connect'));
//  socket.on('data', d => console.log('ws-data: ', d));
//  socket.on('drain', () => console.log('ws-drain'));
//  socket.on('end', () => console.log('ws-end'));
//  socket.on('error', d => console.log('ws-error: ', d));
//  socket.on('lookup', (err, addr, fam, host) => console.log('ws-lookup: ', { err, addr, fam, host }));
//  socket.on('ready', () => console.log('ws-ready'));
//  socket.on('timeout', () => console.log('ws-timeout'));
//  proxy.ws(req, socket, head);
//});

//proxy.on('open', proxySocket => {
//  console.log('WS: OPEN');
//  proxySocket.on('close', d => console.log('ws-proxy-close: error:', d));
//  proxySocket.on('connect', () => console.log('ws-proxy-connect'));
//  proxySocket.on('data', d => console.log('ws-proxy-data: ', d));
//  proxySocket.on('drain', () => console.log('ws-proxy-drain'));
//  proxySocket.on('end', () => console.log('ws-proxy-end'));
//  proxySocket.on('error', d => console.log('ws-proxy-error: ', d));
//  proxySocket.on('lookup', (err, addr, fam, host) => console.log('ws-proxy-lookup: ', { err, addr, fam, host }));
//  proxySocket.on('ready', () => console.log('ws-proxy-ready'));
//  proxySocket.on('timeout', () => console.log('ws-proxy-timeout'));
//});

proxy.on('error', (err, req, res) => {
  console.log('ERR: ' + req.method + ' ' + req.url + ' ' + err);
});

proxy.on('proxyReq', (proxyReq, req, res) => {
  console.log('PROXY: ' + req.method + ' ' + req.url);
});

proxy.on('proxyRes', (proxyRes, req, res) => {
  console.log('RESP: ' + req.method + ' ' + req.url + ' ' + proxyRes.statusCode);
  console.log('proxyRes', JSON.stringify(proxyRes.headers, true, 2));
  proxyRes.headers['Access-Control-Allow-Origin'] = 'http://localhost:8080';
  proxyRes.headers['Access-Control-Allow-Headers'] = 'Content-Type, cookie';
  proxyRes.headers['Access-Control-Allow-Credentials'] = 'true';
  proxyRes.headers['Access-Control-Request-Method'] = 'GET, OPTIONS, POST, PUT, HEAD, DELETE, CONNECT, TRACE, PATCH';

  // rewrite cookie domain
  if (proxyRes.headers['set-cookie']) {
    let oldCookies = [];
    const newCookies = [];
    if (Array.isArray(proxyRes.headers['set-cookie'])) {
      oldCookies = proxyRes.headers['set-cookie'];
    } else if (typeof proxyRes.headers['set-cookie'] === 'string') {
      oldCookies = [proxyRes.headers['set-cookie']];
    }

    for (const cookie of oldCookies) {
      newCookies.push(cookie.replace('zwoo.igd20.de', 'localhost'));
    }

    proxyRes.headers['set-cookie'] = newCookies;
  }
});

console.log('started!');
proxy.listen(8005);
