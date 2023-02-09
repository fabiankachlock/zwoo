/* eslint-disable */
const icons = [
  'akar-icons:box',
  'akar-icons:check',
  'mdi:chevron-left',
  'gg:close',
  'akar-icons:arrow-left',
  'akar-icons:arrow-right',
  'akar-icons:chevron-down',
  'bi:mic-mute-fill',
  'bi:mic-fill',
  'akar-icons:copy',
  'iconoir:nav-arrow-down',
  'iconoir:nav-arrow-up',
  'iconoir:share-android',
  'akar-icons:crown',
  'iconoir:delete-circled-outline',
  'carbon:settings',
  'gg:menu-grid-o',
  'bytesize:external',
  'ri:moon-fill',
  'ri:sun-fill',
  'mdi:fullscreen-exit',
  'mi:home',
  'mdi:eye-outline',
  'mdi:eye-off-outline',
  'carbon:magnify',
  'teenyicons:file-x-outline',
  'icon-park-outline:sort-amount-down',
  'uis:bars',
  'fluent:checkmark-circle-32-regular',
  'fluent:layer-20-filled',
  'iconoir:scan-qr-code',
  'iconoir:refresh',
  'akar-icons:cross',
  'iconoir:lock-key',
  'fluent:window-new-16-regular',
  'fluent:square-arrow-forward-32-regular',
  'fluent:ribbon-star-20-regular',
  'iconoir:nav-arrow-left',
  'mdi:login-variant',
  'ic:outline-add-box',
  'mdi:trophy-outline',
  'mdi:arrow-right-bold-box-outline',
  'akar-icons:info',
  'material-symbols:block',
  'iconoir:system-restart',
  'mdi:logout-variant',
  'iconoir:eye-alt',
  'iconoir:play-outline',
  'teenyicons:send-outline',
  'mdi:fullscreen',
  'mdi:fullscreen-exit',
  'mdi:cloud-sync-outline',
  'mdi:cloud-off-outline',
  'fluent:bot-add-20-regular',
  'fluent:bot-24-regular',
  'material-symbols:help-outline-rounded',
  'mdi:email-outline'
];

const { SVG, Collection } = require('@iconify/json-tools');
const fs = require('fs');
const path = require('path');

const reducedIcons = icons
  .map(fullIcon => fullIcon.split(':'))
  .reduce(
    (all, icon) => ({
      ...all,
      [icon[0]]: [...(all[icon[0]] ? all[icon[0]] : []), icon[1]]
    }),
    {}
  );

console.log(`starting icon build process`);
console.log(`cleaning output folder...`);

const iconsDir = path.join(__dirname, 'icons');
if (fs.existsSync(iconsDir)) {
  fs.rmSync(iconsDir, { recursive: true });
}
fs.mkdirSync(iconsDir);

const iconifyPath = path.join(__dirname, '..', '..', '..', 'node_modules', '@iconify', 'json');
let count = 0;

console.log(`detected ${Object.keys(reducedIcons).length} icon sets`);
for (const iconCollection in reducedIcons) {
  let collection = new Collection();
  collection.loadIconifyCollection(iconCollection, iconifyPath);

  for (const icon of reducedIcons[iconCollection]) {
    count++;
    const fileName = `${iconCollection}__${icon}.js`;
    const svgData = collection.getIconData(icon);
    console.log(`building icon ${icon} of ${iconCollection} into ${fileName}`);
    const svg = new SVG(svgData);
    const code = `export default '${svg.getSVG({})}'`;
    fs.writeFileSync(path.join(iconsDir, fileName), code, 'utf-8');
  }
}

console.log(`built ${count} icons`);
console.log(`finished`);

if (!process.argv.includes('--view')) {
  process.exit(0);
}

console.log('generating html overview');

const html = `<html><head><title>zwoo icons</title><style>
body { padding: 0 1rem; }
div.collection { display: grid; grid-template-columns: auto auto auto; grid-gap: 4px; margin-bottom: 1rem; }
div.row { display: flex; flex-direction: row-reverse; justify-content: flex-end; align-items: center; }
svg { width: 2rem; height: 2rem; margin-right: 1rem; display: block; }
p, h1, h2 { font-family: 'Segoe UI', sans-serif;  margin: 0; }
p { font-size: 1.5rem; }
h1 { font-size: 3rem; }
h2 { font-size: 2rem; margin-top: 2rem; margin-bottom: 1rem; border-bottom: 1px solid black; }
</style></head><body><h1>Zwoo Icons</h1>
{body}</body></html>
`;

const createIconRow = (name, svg) => {
  return `<div class="row"><p>${name}</p>${svg}</div>`;
};

const createIconCollection = (name, html) => {
  return `<h2>${name}</h2>
<div class="collection">${html}</div>
`;
};

const collections = [];

for (const iconCollection in reducedIcons) {
  let collection = new Collection();
  collection.loadIconifyCollection(iconCollection, iconifyPath);
  const rows = [];

  for (const icon of reducedIcons[iconCollection]) {
    const svgData = collection.getIconData(icon);
    const svg = new SVG(svgData);
    rows.push(createIconRow(icon, svg.getSVG({})));
  }
  collections.push(createIconCollection(iconCollection, rows.join('')));
}

fs.writeFileSync(path.join(__dirname, 'icons.html'), html.replace('{body}', collections.join('')));
