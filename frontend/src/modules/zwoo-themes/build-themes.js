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
 * @typedef {Record<Color, ThemeColorVariants> & { "$name": string }} Theme
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
  [VARIANT_HOVER]: (color, ctx) => {
    // const luminance = color.luminance();
    // return color.luminance(luminance > 0.4 ? luminance - 0.03 : luminance + 0.03);
    // const lighness = color.get('hsl.l');
    // return color.set('hsl.l', lighness > 0.4 ? lighness - 0.06 : lighness + 0.07);
    const lighness = color.get('oklch.l');
    return color.set('oklch.l', lighness > 0.4 ? lighness - 0.11 : lighness + 0.07);
  }
};

/**
 * A mapping of known color adapters that run after the whole theme has been parsed
 * @type {Record<string, (color: chroma.Color, context: ColorContext, theme: Theme) => chroma.Color>}
 */
const defferedVariantAdapter = {
  [VARIANT_INVERRSE]: (_color, { key, variant, isDark, isHighContrast }, theme) => {
    return theme[key][VARIANT_BASE][modifierToKey(!isDark, isHighContrast)];
  }
  // [VARIANT_TEXT]: (color, { isDark, isHighContrast }, theme) => {
  //   const desiredBg = theme.bg.$base[modifierToKey(isDark, isHighContrast)];
  //   const desiredContrast = isHighContrast ? 4.5 : 7;
  //   console.log(color.hex());
  //   while (chroma.contrast(desiredBg, color) < desiredContrast) {
  //     console.log(chroma.contrast(desiredBg, color), color.hex());
  //     const lighness = color.get('hsl.l');
  //     color = color.set('hsl.l', lighness > 0.4 ? lighness + 0.06 : lighness - 0.06);
  //   }
  //   return color;
  // }
  // [VARIANT_CONTRAST_TEXT]: (color, { isDark, isHighContrast }, theme) => {
  //   const desiredBg = theme.bg.$base[modifierToKey(isDark, isHighContrast)];
  //   const desiredContrast = isHighContrast ? 4.5 : 8;
  //   while (chroma.contrast(desiredBg, color) < desiredContrast) {
  //     // might want to use set(hsl.l, x)
  //     color = color.darken(0.01);
  //   }
  //   return color;
  // },
};

// --- UTILITIES ---
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

  return chroma(color);
}

/**
 * Transforms a json theme definitions into a full css theme
 * @param {string} name
 * @param {JsonThemeConfig} config
 * @returns {Theme} the created theme
 */
function buildTheme(name, config) {
  const modifiers = [
    [false, false],
    [true, false],
    [false, true],
    [true, true]
  ];
  const currentTheme = {
    $name: name
  };
  console.log('building ' + name);
  console.log(`  with modifiers: ${modifiers.map((a, b) => modifierToKey(a, b)).join(', ')}`);

  // for all defined theme colors
  for (const colorDef in THEME_CONFIG) {
    if (!config[colorDef]) {
      console.warn(`Invalid config: theme '${name}' has no defintion for color ${colorDef}`);
      continue;
    }

    currentTheme[colorDef] = {};
    // for every color variant defined for that color
    for (const variant of THEME_CONFIG[colorDef]) {
      const currentVariant = {};
      currentTheme[colorDef][variant] = currentVariant;

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
 *
 * @param {Theme} theme
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

  const themes = [];
  for (const themeName in config) {
    const newTheme = buildTheme(themeName, config[themeName]);
    const processed = postprocessTheme(newTheme);
    themes.push(processed);
  }
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
    .join();
  fs.writeFileSync(path.join(__dirname, THEME_OUT_FILENAME), bundle);
  console.log('done');
}
main();
