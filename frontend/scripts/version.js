const exec = require('child_process').execSync;
const fs = require('fs');
const read = fs.readFileSync;
const write = fs.writeFileSync;

const createEnvVar = (name, val) => name + '=' + val + '\n';

const gitHash = exec('git rev-parse --short HEAD', { encoding: 'utf8' }).trim();
const packageJson = read('package.json');
const version = JSON.parse(packageJson).version;

const VERSION = 'VUE_APP_VERSION';
const HASH = 'VUE_APP_VERSION_HASH';
const replaceVersionRegex = /VUE_APP_VERSION=.*\n/;
const replaceHashRegex = /VUE_APP_VERSION_HASH=.*\n/;

console.log('Setup version:', version, '(' + gitHash + ')');

let env = read('env/.env').toString();

env = env.replace(replaceVersionRegex, createEnvVar(VERSION, version));
env = env.replace(replaceHashRegex, createEnvVar(HASH, gitHash));

write('env/.env', env);

console.log('done!');
