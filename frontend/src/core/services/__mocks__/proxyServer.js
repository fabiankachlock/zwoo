// eslint-disable-next-line no-undef
const http = require('http');
// eslint-disable-next-line no-undef
const httpProxy = require('http-proxy');

var proxy = new httpProxy.createProxyServer({
  target: 'http://zwoo.igd20.de:8000/',
  followRedirects: true,
  ws: true
});

var proxyServer = http.createServer((req, res) => {
  // fix origin
  req.headers['origin'] = 'https://zwoo.igd20.de';

  proxy.web(req, res);
});

proxyServer.on('upgrade', (req, socket, head) => {
  // fix origin or connections will be rejected with 403
  req.headers['origin'] = 'https://zwoo.igd20.de';

  console.log('WS: ' + req.method + ' ' + req.url);
  proxy.ws(req, socket, head);
});

proxy.on('open', proxySocket => {
  console.log('WS: OPEN');
  proxySocket.on('close', d => console.log('ws-proxy-close: error:', d));
  proxySocket.on('connect', () => console.log('ws-proxy-connect'));
  proxySocket.on('data', d => console.log('ws-proxy-data: ', d.toString()));
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

proxy.on('close', () => {
  console.log('WS: CLOSE');
});

proxy.on('proxyReq', (_proxyReq, req) => {
  console.log('PROXY: ' + req.method + ' ' + req.url);
});

proxy.on('proxyRes', (proxyRes, req) => {
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
proxyServer.listen(8005);
