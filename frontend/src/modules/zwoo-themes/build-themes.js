/* eslint-disable */
const fs = require('node:fs');
const path = require('node:path');
const chroma = require('chroma-js');

// --- Constants & Types ---
const TARGET_DIR = 'themes';
const THEME_CONFIG_FILENAME = 'themes.json';
const THEME_OUT_FILENAME = 'themes.css';

const VARIANT_BASE = '$base';
const VARIANT_HOVER = 'hover';
const VARIANT_TEXT = 'text';
const VARIANT_CONTRAST_TEXT = 'contrast-text';
const VARIANT_INVERRSE = 'inverse';

/**
 * @typedef {keyof typeof THEME_CONFIG} Color
 */

/**
 * @typedef {typeof VARIANT_BASE | typeof VARIANT_HOVER | typeof VARIANT_TEXT | typeof VARIANT_CONTRAST_TEXT | typeof VARIANT_INVERRSE} BuilderVariant
 */

/**
 * @typedef {Object} ThemeColor
 * @property {chroma.Color} light
 * @property {chroma.Color} dark
 * @property {chroma.Color} lightHighContrast
 * @property {chroma.Color} darkHighContrast
 */

/**
 * @typedef {'color' | 'highContrast' | keyof ThemeColor} JsonThemeColorConfigKey
 */

/**
 * @typedef {Record<JsonThemeColorConfigKey, string | undefined> & { overrides: Record<string, JsonColorConfig>}} JsonColorConfig
 */

/**
 * @typedef {Record<Color, JsonColorConfig} JsonThemeConfig
 */

/**
 * @typedef {Record<BuilderVariant, ThemeColor>} ThemeColorVariants
 */

/**
 * @typedef {Record<Color, ThemeColorVariants> & { "$name": string, "$inherits": boolean }} Theme
 */

/**
 * @typedef {Object} ColorContext
 * @property {Color} key
 * @property {BuilderVariant} variant
 * @property {boolean} isDark
 * @property {boolean} isHighContrast
 */

const THEME_CONFIG = {
  bg: [VARIANT_BASE, VARIANT_HOVER, VARIANT_INVERRSE],
  surface: [VARIANT_BASE, VARIANT_HOVER, VARIANT_INVERRSE],
  alt: [VARIANT_BASE, VARIANT_HOVER, VARIANT_INVERRSE],
  text: [VARIANT_BASE, 'secondary'],
  primary: [VARIANT_BASE, VARIANT_HOVER, VARIANT_TEXT, VARIANT_CONTRAST_TEXT],
  secondary: [VARIANT_BASE, VARIANT_HOVER, VARIANT_TEXT, VARIANT_CONTRAST_TEXT],
  success: [VARIANT_BASE, VARIANT_HOVER, VARIANT_TEXT, VARIANT_CONTRAST_TEXT],
  error: [VARIANT_BASE, VARIANT_HOVER, VARIANT_TEXT, VARIANT_CONTRAST_TEXT],
  warning: [VARIANT_BASE, VARIANT_HOVER, VARIANT_TEXT, VARIANT_CONTRAST_TEXT],
  border: [VARIANT_BASE, 'light', 'divider']
};

/**
 * A mapping of known color adapters
 * @type {Record<string, (color: chroma.Color, context: ColorContext, theme: Theme) => chroma.Color>}
 */
const variantAdapter = {
  [VARIANT_HOVER]: (color, { isHighContrast }) => {
    const lighness = color.get('oklch.l');
    let change = lighness > 0.4 ? -0.11 : 0.07;
    change = isHighContrast ? change * 1.5 : change;
    return color.set('oklch.l', lighness + change);
  }
};

/**
 * A mapping of known color adapters that run after the whole theme has been parsed
 * @type {Record<string, (color: chroma.Color, context: ColorContext, theme: Theme) => chroma.Color>}
 */
const defferedVariantAdapter = {
  [VARIANT_INVERRSE]: (_color, { key, variant, isDark, isHighContrast }, theme) => {
    return theme[key][VARIANT_BASE][modifierToKey(!isDark, isHighContrast)];
  },
  [VARIANT_TEXT]: (color, { key, variant, isDark, isHighContrast }, theme) => {
    let lighness = color.get('oklch.l');
    const desiredBg = getClosestColor(
      [
        theme.bg[VARIANT_BASE][modifierToKey(isDark, isHighContrast)],
        theme.surface[VARIANT_BASE][modifierToKey(isDark, isHighContrast)],
        theme.alt[VARIANT_BASE][modifierToKey(isDark, isHighContrast)]
      ],
      color
    );
    if (!desiredBg) return color;

    const desiredContrast = isHighContrast ? 8 : 4;
    const direction = chroma.contrast(desiredBg, color.set('oklch.l', lighness + 0.01)) > chroma.contrast(desiredBg, color) ? 1 : -1;
    let failSave = 0;

    while (chroma.contrast(desiredBg, color) < desiredContrast && lighness >= 0 && lighness <= 1) {
      lighness = color.get('oklch.l');
      color = color.set('oklch.l', lighness + 0.02 * direction);
      if (failSave++ > 1000) {
        console.error(
          `${theme.$name} color ${key} in variant ${variant} does not meet contrast resitrictions for ${modifierToKey(isDark, isHighContrast)}`
        );
        break;
      }
    }
    return color;
  },
  [VARIANT_CONTRAST_TEXT]: (color, { isHighContrast }, theme) => {
    const lightText = theme.text[VARIANT_BASE][modifierToKey(false, isHighContrast)];
    const darkText = theme.text[VARIANT_BASE][modifierToKey(true, isHighContrast)];
    if (!lightText || !darkText) return color;

    if (chroma.contrast(lightText, color) > chroma.contrast(darkText, color)) {
      return lightText;
    }
    return darkText;
  }
};

// --- UTILITIES ---
/**
 * Returns the color with the minimum contrast to the target color
 *
 * @param {chroma.Color[]} colors a list of colors to choose from
 * @param {chroma.Color} target the atregt color
 */
function getClosestColor(colors, target) {
  let minimumContrast = Infinity;
  let returnColor = colors[0];

  colors
    .filter(col => !!col)
    .forEach(color => {
      const contrast = chroma.contrast(target, color);
      if (contrast < minimumContrast) {
        minimumContrast = contrast;
        returnColor = color;
      }
    });

  return returnColor;
}

/**
 * Returns the color object key for the given modifiers
 * @param {boolean} isDark
 * @param {boolean} isHighContrast
 * @returns {keyof ThemeColor}
 */
function modifierToKey(isDark, isHighContrast) {
  if (isDark && isHighContrast) {
    return 'darkHighContrast';
  } else if (isDark) {
    return 'dark';
  } else if (isHighContrast) {
    return 'lightHighContrast';
  }
  return 'light';
}

/**
 * Fallback chains:
 * dark -> color -> darkHighContrast -> highcontrast
 * light -> color -> lightHighContrast -> highcontrast
 * darkHighContrast -> highcontrast -> dark -> color
 * lightHighContrast -> highcontrast -> light -> color
 */

/**
 * Returns the color object fallback key for the given modifiers
 * @param {boolean} isDark
 * @param {boolean} isHighContrast
 * @returns {JsonThemeColorConfigKey[]} the fallback key for a color
 */
function modifierToFallbackKeys(isDark, isHighContrast) {
  return [
    // 1. same kind
    isHighContrast ? 'highContrast' : 'color',
    // 2. same theme
    modifierToKey(isDark, !isHighContrast),
    // 3. other kind
    isHighContrast ? 'color' : 'highContrast'
  ];
}

/**
 * Returns a list of possible colors object keys for the given modifiers
 * @param {boolean} isDark
 * @param {boolean} isHighContrast
 * @returns {JsonThemeColorConfigKey[]}
 */
function getColorKeys(isDark, isHighContrast) {
  return [modifierToKey(isDark, isHighContrast), ...modifierToFallbackKeys(isDark, isHighContrast)];
}

/**
 * Returns the value of a property of an object with a given key or one of its fallbacks
 * @param {Record<string, string>} obj the target object
 * @param {string} props a list of possible property keys
 * @returns {string} the first property whose key is present
 */
function getObjectProperty(obj, props) {
  if (typeof obj === 'string') {
    return obj;
  }

  for (const prop of props) {
    if (prop in obj) return obj[prop];
  }
  return undefined;
}

/**
 * Creates the full css varaible name for an color
 * @param {ColorContext} context
 * @returns {string} the name
 */
function colorToCssVariableName(context) {
  if (context.variant === VARIANT_BASE) {
    return context.key;
  }
  return `${context.key}-${context.variant}`;
}

/**
 * Creates the css rule group selector
 * @param {boolean} isDark
 * @param {boolean} isHighContrast
 * @returns {string} the selector
 */
function modifierContextToGroupName(isDark, isHighContrast) {
  return `.${isDark ? 'dark' : 'light'}${isHighContrast ? '.highContrast' : ''}`;
}

/**
 * Iterates over all colors in all variants and modifications of theme
 * @param {Theme} theme
 * @param {(context: ColorContext & { color?: chroma.Color}) => void} handler
 */
function loopOverTheme(theme, handler) {
  // for all defined theme colors
  for (const colorDef in THEME_CONFIG) {
    // for every color variant defined for that color
    for (const variant of THEME_CONFIG[colorDef]) {
      // for every modifier combination
      for (const [isDark, isHighContrast] of [
        [false, false],
        [true, false],
        [false, true],
        [true, true]
      ]) {
        handler({
          isDark,
          isHighContrast,
          key: colorDef,
          variant,
          color: theme[colorDef]?.[variant]?.[modifierToKey(isDark, isHighContrast)]
        });
      }
    }
  }
}

// --- Building Themes ---
/**
 * Get color value definiton of a theme
 * @param {JsonColorConfig} config the colors theme config
 * @param {ColorContext} context
 * @param {Theme} theme
 * @returns
 */
function getThemeColor(config, context, theme) {
  let target = config;
  let isOverride = false;

  if (context.variant !== VARIANT_BASE && config.overrides && config.overrides[context.variant]) {
    target = config.overrides[context.variant];
    isOverride = true;
  }

  const color = getObjectProperty(target, getColorKeys(context.isDark, context.isHighContrast));

  if (!isOverride) {
    // omly apply default transformations if the color isnt explicitly overriten
    const adapter = variantAdapter[context.variant];
    if (adapter) {
      return adapter(chroma(color), context, theme);
    }
  }

  return color ? chroma(color) : undefined;
}

/**
 * Transforms a json theme definitions into a full css theme
 * @param {string} name
 * @param {boolean} inherits
 * @param {JsonThemeConfig} config
 * @returns {Theme} the created theme
 */
function buildTheme(name, inherits, config) {
  const modifiers = [
    [false, false],
    [true, false],
    [false, true],
    [true, true]
  ];
  const currentTheme = {
    $name: name,
    $inherits: inherits
  };
  console.log('building ' + name);
  console.log(`  with modifiers: ${modifiers.map((a, b) => modifierToKey(a, b)).join(', ')}`);

  // for all defined theme colors
  for (const colorDef in THEME_CONFIG) {
    currentTheme[colorDef] = {};
    // for every color variant defined for that color
    for (const variant of THEME_CONFIG[colorDef]) {
      const currentVariant = {};
      currentTheme[colorDef][variant] = currentVariant;

      if (!config[colorDef]) {
        if (!config['$inherits']) {
          console.warn(`Invalid config: theme '${name}' has no defintion for color ${colorDef}`);
        }
        continue;
      }

      // for every modifier combination
      for (const [isDark, isHighContrast] of modifiers) {
        const colorKey = modifierToKey(isDark, isHighContrast);
        const color = getThemeColor(
          config[colorDef],
          {
            variant,
            isDark,
            isHighContrast,
            key: colorDef
          },
          currentTheme
        );

        currentVariant[colorKey] = color;
      }
    }
  }

  return currentTheme;
}

/**
 *  Caculate some colors that cant be caculated initially
 *
 * @param {Theme} theme the theme to work with
 */
function postprocessTheme(theme) {
  loopOverTheme(theme, ({ key, variant, isDark, isHighContrast, color }) => {
    if (!color) {
      return;
    }
    const colorKey = modifierToKey(isDark, isHighContrast);
    const adapter = defferedVariantAdapter[variant];
    if (adapter) {
      theme[key][variant][colorKey] = adapter(
        color,
        {
          isDark,
          isHighContrast,
          key,
          variant
        },
        theme
      );
    }
    theme[key][variant][colorKey] = theme[key][variant][colorKey];
  });
  return theme;
}

// --- Converting Themes to CSS ---
/**
 *
 * @param {Theme} theme
 * @returns {string} a block of css
 */
function renderTheme(theme) {
  const modifierGroups = {
    [modifierContextToGroupName(false, false)]: [],
    [modifierContextToGroupName(false, true)]: [],
    [modifierContextToGroupName(true, false)]: [],
    [modifierContextToGroupName(true, true)]: []
  };
  const themeSelector = `[data-theme='${theme.$name}']`;
  let css = '';

  loopOverTheme(theme, ({ key, variant, isDark, isHighContrast, color }) => {
    if (!color) {
      return;
    }

    modifierGroups[modifierContextToGroupName(isDark, isHighContrast)].push([
      colorToCssVariableName({
        isDark,
        isHighContrast,
        key,
        variant
      }),
      theme[key][variant][modifierToKey(isDark, isHighContrast)]
    ]);
  });

  for (const group in modifierGroups) {
    css += `${themeSelector}${group} {\n`;
    css += modifierGroups[group]
      .flatMap(([name, color]) => [
        `\t--color-${name}: ${color.get('rgb.r')} ${color.get('rgb.g')} ${color.get('rgb.b')};`,
        `\t--color-${name}-hex: ${color.hex()};`
      ])
      .join('\n');
    css += '\n}\n';
  }
  return css;
}

function main() {
  console.log('build color themes');
  // clean build output directory
  if (fs.existsSync(TARGET_DIR)) {
    fs.rmSync(TARGET_DIR, { recursive: true });
  }
  fs.mkdirSync(TARGET_DIR);

  const config = JSON.parse(fs.readFileSync(path.join(__dirname, THEME_CONFIG_FILENAME)).toString());

  console.log('running theme builders...');
  let themes = Object.keys(config).map(themeName => buildTheme(themeName, config[themeName]['$inherits'], config[themeName]));

  console.log('running inheritations...');
  for (const theme of themes) {
    const parentTheme = themes.find(otherTheme => otherTheme.$name === theme.$inherits);
    if (parentTheme) {
      console.log(`inheriting missing colors of ${theme.$name} from ${parentTheme.$name}`);
      for (const color in THEME_CONFIG) {
        if (!config[theme.$name]?.[color]) {
          theme[color] = parentTheme[color];
        }
      }
    }
  }

  console.log('running post processing...');
  themes = themes.map(postprocessTheme);

  console.log(`processed ${themes.length} themes`);
  console.log('creating css files');

  for (const theme of themes) {
    const css = renderTheme(theme);
    fs.writeFileSync(path.join(__dirname, TARGET_DIR, theme.$name + '.css'), css);
  }

  console.log('creating bundle file');
  const bundle = themes
    .map(theme => theme.$name + '.css')
    .map(fileName => `@import './${TARGET_DIR}/${fileName}';\n`)
    .join('');
  fs.writeFileSync(path.join(__dirname, THEME_OUT_FILENAME), bundle);
  console.log('done');
}
main();
