const http = require('http');

const requestListener = function (req, res) {
  if (req.url.startsWith('/version')) {
    res.writeHead(200, { 'Access-Control-Allow-Origin': 'http://localhost:15761' });
    res.end('1.0.0-beta.12');
    return;
  }
  res.writeHead(404, { 'Access-Control-Allow-Origin': 'http://localhost:15761' });
  res.end('not found');
};

const server = http.createServer(requestListener);
server.listen(8000);
