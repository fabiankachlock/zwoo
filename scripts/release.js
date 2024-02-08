const fs = require("fs");
const path = require("path");
const child_process = require("child_process");

const newVersion = process.argv[2];
if (newVersion === undefined) {
  console.log("please enter a new version");
  process.exit(1);
}

const versionFiles = [
  {
    path: "/frontend/package.json",
    regex: /"version": "(.*)"/,
    replace: '"version": "###"',
  },
  {
    path: "/frontend/src-tauri/tauri.conf.json",
    regex: /"version": "(.*)"/,
    replace: '"version": "###"',
  },
  {
    path: "/frontend/src-tauri/Cargo.toml",
    regex: /version = "(.*)"/,
    replace: 'version = "###"',
  },
  {
    path: "/frontend/src-tauri/Cargo.lock",
    regex: /"zwoo"\nversion = "(.*)"/,
    replace: '"zwoo"\nversion = "###"',
  },
  {
    path: "/backend/Zwoo.Backend/Globals.cs",
    regex: /const string VERSION = "(.*)";/,
    replace: 'const string VERSION = "###";',
  },
  {
    path: "/backend/Zwoo.Dashboard/Data/Globals.cs",
    regex: /const string VERSION = "(.*)";/,
    replace: 'const string VERSION = "###";',
  },
];

for (const file of versionFiles) {
  const content = fs
    .readFileSync(path.join(__dirname, "..", file.path))
    .toString();
  const newContent = content.replace(
    file.regex,
    file.replace.replace("###", newVersion)
  );
  fs.writeFileSync(path.join(__dirname, "..", file.path), newContent);
}

const frontendPath = path.join(__dirname, "..", "frontend");
child_process.execSync(`cd ${frontendPath} && yarn setup:version`);
child_process.execSync("git add -A");
child_process.execSync(`git commit -m "release: v${newVersion}"`);
child_process.execSync(`git tag v${newVersion}`);
