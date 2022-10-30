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
  'bytesize-external',
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
  'teenyicons:send-outline'
];

const fs = require('fs');
const path = require('path');
// eslint-disable-next-line no-undef
const iconsDir = path.join(__dirname, 'icons');
if (fs.existsSync(iconsDir)) {
  fs.rmdirSync(iconsDir, { recursive: true });
}
fs.mkdirSync(iconsDir);

for (const iconDescription of icons) {
  const [prefix, icon] = iconDescription.split(':');
  const fileContent = `import icon from '@iconify-icons/${prefix}/${icon}';\nexport default icon;`;
  fs.writeFileSync(path.join(iconsDir, `${iconDescription}.js`), fileContent);
}
