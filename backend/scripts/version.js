const exec = require("child_process").execSync;
const fs = require("fs");
const read = fs.readFileSync;
const write = fs.writeFileSync;
const path = require("path");

const paths = [
  "/Zwoo.Backend/Version.cs",
  "/Zwoo.Backend.LocalServer/Version.cs",
  "/Zwoo.Dashboard/Version.cs",
];

const gitHash = exec("git rev-parse --short HEAD", { encoding: "utf8" }).trim();

const versionRegex = /const string VERSION = "(.*)";/;
const replaceHashRegex = /const string HASH = ".*";/;

for (const relPath of paths) {
  const absolutePath = path.join(__dirname, "..", relPath);
  const [, project] = relPath.split("/");
  const content = read(absolutePath).toString();
  const version = content.match(versionRegex)[1];

  console.log("Setup version for", project, ":", version, "(" + gitHash + ")");
  const newContent = content.replace(
    replaceHashRegex,
    'const string HASH = "' + version + '";'
  );
  write(absolutePath, newContent);
}

console.log("done!");
