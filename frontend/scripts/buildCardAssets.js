const { join, basename } = require('path');
const fs = require('fs-extra');

// --- CONSTANTS ---
const DATA_PREFIX = 'data:image/svg+xml;base64, ';
const ASSETS_DIR = join(__dirname, '..', 'assets');
const CARDS_DIR = join(ASSETS_DIR, 'cards');
const CARDS_SOURCES = join(CARDS_DIR, 'raw');
const OUT_DIR = join(CARDS_DIR, 'dist');
const PUBLIC_DIR = join(__dirname, '..', 'public');
const THEME_CONFIG_FILENAME = 'zwoo-theme.json';

const VARIANT_DARK = 'dark';
const VARIANT_LIGHT = 'light';
const VARIANT_AUTO = '@auto';

const CARD_LAYER_SEPARATOR = '_';
const MAX_THEME_PREVIEWS = 6;
const DEFAULT_CARD_PREVIEWS = ['back_u', 'front_1_1', 'front_2_a', 'front_3_b', 'front_4_d', 'front_5_e'];

// --- DATA_MODEL ---
/**
 * An example theme configuration.
 * - contains all default values, used as fallback
 */
const BaseThemeConfig = {
  name: 'the theme name', // required
  description: 'a theme description', // optional
  author: 'the theme author', // optional
  isMultiLayer: true, // optional,
  variants: ['a', 'list', 'of', 'variants'], // required
  imageType: 'svg+xml', // optional,
  previews: [], // optional
  overrides: {
    // optional
    cardFront: 'front',
    cardBack: 'back',
    layerWildcard: '$',
    encoding: 'base64',
    imageDataPrefix: DATA_PREFIX
  },
  // internal
  _dir: '', // the theme directory,
  _files: {
    // output file names
    previews: [],
    '[variant]': ''
  },
  _sources: {
    // all theme card paths
    '[variant]': {
      back: [],
      font: []
    }
  }
};

// --- UTILITIES ---
/**
 * extract the filename of a file path
 * @param {string} path the file path
 * @returns {string} fileName
 */
function filePathToFilaName(path) {
  return basename(path);
}

/**
 * get the file name without the file extension
 * @param {string} path the file path
 * @returns {string} filename
 */
function fileNameWithoutExtension(path) {
  const parts = filePathToFilaName(path).split('.');
  parts.pop();
  return parts.join('.');
}

/**
 * utility function for writing json files synchronously
 * @param {string} path the files path
 * @param {Record<string, any>} data the data to be converted to json
 * @returns {Promise<number>} size of the files in byte
 */
async function writeJSONFile(path, data) {
  if (!fs.existsSync(path)) {
    await fs.createFile(path);
  } else {
    await fs.remove(path);
    await fs.createFile(path);
  }
  const dataContent = JSON.stringify(data, null, 2);
  await fs.writeFile(path, dataContent);
  return dataContent.length;
}

/**
 *  combine an array of object into a single object
 * @param {Object[]} objects
 * @returns {Object} the combined object
 */
function combineToObject(objects) {
  return objects.reduce((acc, curr) => ({ ...acc, ...curr }), {});
}

/**
 * delete a key from an object
 * @param {Object} obj the target object
 * @param {string} key the key to be deleted
 * @returns {Object} the object without the key
 */
function objectWithoutKey(obj, key) {
  const newObject = { ...obj };
  delete newObject[key];
  return newObject;
}

/**
 * format a file size in bytes
 * @param {number} size the file size in bytes
 * @returns {string} the formatted size
 */
function formatFileSize(size) {
  if (size < 1_024) {
    return size.toString() + 'B';
  } else if (size < 1_024 * 1_024) {
    return (size / 1_024).toFixed(2).toString() + 'KB';
  } else if (size < 1_024 * 1_024 * 1_024) {
    return (size / (1_024 * 1_024)).toFixed(2).toString() + 'MB';
  }
}

/**
 * get all unique items of an array
 * @param {any[]} arr the array
 * @returns {any[]} array
 */
function unique(arr) {
  return Array.from(new Set(arr));
}

// --- CARD_THEME_UTILITIES ---
/**
 * generate an output filename for a sprite sheet
 * @param {string} theme the themes name
 * @param {string} variant the variant
 * @param {boolean} isPreview whether it is a preview file
 * @returns {string} the filename
 */
function createThemeFileName(theme, variant, isPreview) {
  return theme + '.' + variant + (isPreview ? '.preview' : '') + '.json';
}

/**
 * Validate a provided theme configuration
 * @param {Record<string, any>} config the themes config
 * @returns {boolean} whether it is valid
 */
function validateThemeConfig(config) {
  return 'name' in config && 'variants' in config;
}

/**
 * do some computation on a theme variants
 * optionally add some auto supported themes (as @auto)
 * @param {string[]} variants the themes variants
 * @returns {string[]} the computed variants
 */
function computeThemeVariants(variants) {
  if (variants.includes(VARIANT_DARK) && variants.includes(VARIANT_LIGHT)) {
    return [...variants, VARIANT_AUTO];
  }
  return variants;
}

/**
 * compute a themes previews
 * cut of everything over max len or add defaults
 * @param {string[]} previews
 * @returns {string[]} computed previews
 */
function computeThemePreviews(previews) {
  return (previews ?? []).length === 0 ? DEFAULT_CARD_PREVIEWS.slice() : previews.slice(0, MAX_THEME_PREVIEWS - 1);
}

/**
 * Create a object with theme themes name as keys and der variants as sub keys
 * @param {(typeof BaseThemeConfig)[]} themes the list of themes
 * @param {(name: string, variant: string) => any} transformer
 * @returns
 */
function toThemesObjectWithVariants(themes, transformer) {
  return combineToObject(
    themes.map(theme => ({
      [theme.name]: theme.variants.reduce((acc, variant) => ({ ...acc, [variant]: transformer(theme.name, variant) }), {})
    }))
  );
}

/**
 * get the needed layers for a card
 * @param {string} card the card descriptor
 * @param {string} layerWildCard the wildcard to replace for layers
 * @returns {string[]} the needed layers
 */
function resolveLayersForCard(card, layerWildCard) {
  const firstLayer = card.replace(
    new RegExp(CARD_LAYER_SEPARATOR + '.' + CARD_LAYER_SEPARATOR),
    CARD_LAYER_SEPARATOR + layerWildCard + CARD_LAYER_SEPARATOR
  );
  const secondLayer = card.replace(new RegExp(CARD_LAYER_SEPARATOR + '.$'), CARD_LAYER_SEPARATOR + layerWildCard);
  return [firstLayer, secondLayer];
}

// --- THEME_RESOLVING ---
/**
 * search for all themes in the /assets/cards/raw folder
 * @returns {typeof BaseThemeConfig} an list of all themes
 */
async function findThemes() {
  const allThemes = await fs.readdir(CARDS_SOURCES);
  const themes = [];
  for (const theme of allThemes) {
    try {
      const configPath = join(CARDS_SOURCES, theme, THEME_CONFIG_FILENAME);
      if (!fs.existsSync(configPath)) throw 'not-found';
      const configuration = await fs.readFile(configPath);
      const parsedConfiguration = JSON.parse(configuration.toString());
      if (!validateThemeConfig(parsedConfiguration)) throw 'invalid-config';
      themes.push({
        ...parsedConfiguration,
        _dir: theme
      });
    } catch (e) {
      if (e instanceof SyntaxError) {
        console.error('Invalid theme configuration in ' + theme + ' theme');
        console.trace(e);
        throw new Error('Invalid theme configuration');
      } else if (e === 'not-found') {
        console.error('No theme configuration found in ' + theme);
        console.error('Make sure to include a ' + THEME_CONFIG_FILENAME + 'in your theme directory');
        throw new Error('Theme configuration not found');
      } else if (e === 'invalid-config') {
        console.error('Invalid theme configuration of  ' + theme);
        throw new Error('Invalid theme configuration');
      } else {
        console.trace(e);
      }
    }
  }
  return themes;
}

/**
 * Get all sources files for a theme
 * @param {typeof BaseThemeConfig} themeConfig the provided theme
 * @returns {typeof BaseThemeConfig} themeConfig extended with source information
 */
async function searchThemeSources(themeConfig) {
  const themeSources = {};
  const cardKinds = {
    back: join(CARDS_SOURCES, themeConfig._dir, themeConfig.overrides?.cardBack ?? BaseThemeConfig.overrides.cardBack),
    front: join(CARDS_SOURCES, themeConfig._dir, themeConfig.overrides?.cardFront ?? BaseThemeConfig.overrides.cardFront)
  };
  for (const [kind, kindDirectory] of Object.entries(cardKinds)) {
    for (const variant of themeConfig.variants) {
      if (!fs.existsSync(join(kindDirectory, variant))) {
        console.error('Cannot find directory for ' + variant + ' variant of theme ' + themeConfig.name);
        throw new Error('Cannot find directory ' + variant);
      }
      if (!themeSources[variant]) {
        themeSources[variant] = {};
      }
      const sprites = await fs.readdir(join(kindDirectory, variant));
      themeSources[variant][kind] = sprites.filter(sprite => !sprite.startsWith('.'));
    }
  }

  const generateFileName = isPreview => (name, variant) => createThemeFileName(name, variant, isPreview);
  return {
    ...themeConfig,
    _sources: themeSources,
    _files: {
      previews: toThemesObjectWithVariants([themeConfig], generateFileName(true))[themeConfig.name],
      ...toThemesObjectWithVariants([themeConfig], generateFileName(false))[themeConfig.name]
    }
  };
}

// --- META_FILES ---
/**
 * Create the meta.json and sourcemap.json files
 * @param {(typeof BaseThemeConfig)[]} themes list of all themes
 */
async function createMetaFiles(themes) {
  const data = {
    themes: themes.map(t => t.name),
    configs: combineToObject(
      themes.map(theme => ({
        [theme.name]: {
          name: theme.name ?? '',
          description: theme.description ?? '',
          author: theme.author ?? '',
          isMultiLayer: theme.isMultiLayer,
          variants: computeThemeVariants(theme.variants),
          previews: computeThemePreviews(theme.previews)
        }
      }))
    ),
    variants: combineToObject(
      themes.map(theme => ({
        [theme.name]: computeThemeVariants(theme.variants)
      }))
    ),
    files: {
      previews: {
        ...combineToObject(themes.map(theme => ({ [theme.name]: theme._files.previews })))
      },
      ...combineToObject(themes.map(theme => ({ [theme.name]: objectWithoutKey(theme._files, 'previews') })))
    }
  };

  writeJSONFile(join(OUT_DIR, 'meta.json'), data);
  writeJSONFile(join(OUT_DIR, 'sourcemap.json'), combineToObject(themes.map(t => ({ [t.name]: t._sources }))));
}

// --- GENERATE_SPRITE_SHEETS ---
/**
 * Create a json sprite sheet single layer a theme
 * @param {string[]} files an array with all the themes source file paths
 * @param {string} encoding teh target data encoding
 * @param {string} dataPrefix the html image 'data:' prefix
 * @returns
 */
async function createSingleLayerCardSpriteSheet(files, encoding, dataPrefix) {
  const spriteData = {};
  for (const file of files) {
    const spriteName = fileNameWithoutExtension(file);
    const buffer = await fs.readFile(file);
    const content = buffer.toString(encoding);
    spriteData[spriteName] = dataPrefix + content;
  }
  return spriteData;
}

/**
 * Create a json sprite sheet multi layer a theme
 * @param {string[]} files an array with all the themes source file paths
 * @param {string} encoding teh target data encoding
 * @param {string} dataPrefix the html image 'data:' prefix
 * @param {string} customWildcardCharacter the file name card wildcard char
 * @returns
 */
async function createMultiLayerCardSpriteSheet(files, encoding, dataPrefix, customWildcardCharacter) {
  const spriteData = {};
  for (const file of files) {
    const spriteName = fileNameWithoutExtension(file);
    const validSpriteName = spriteName.replaceAll(customWildcardCharacter, BaseThemeConfig.overrides.layerWildcard);
    const buffer = await fs.readFile(file);
    const content = buffer.toString(encoding);
    spriteData[validSpriteName] = dataPrefix + content;
  }
  return spriteData;
}

// --- BUILDING ---
let outFiles = 0;
let sumSize = 0;
/**
 * Create all sprite sheets for a theme
 * @param {typeof BaseThemeConfig} theme the target theme
 */
async function buildTheme(theme) {
  console.log('building ' + theme.name);
  console.log(' found', theme.variants.length, 'variants (' + theme.variants.join(', ') + ')');
  const themeEncoding = theme.overrides?.encoding ?? BaseThemeConfig.overrides.encoding;
  const themDataPrefix = theme.overrides?.imageDataPrefix ?? `data:image/${theme.imageType ?? BaseThemeConfig.imageType};${themeEncoding},`;

  for (const variant of theme.variants) {
    const fileName = theme._files[variant];
    const previewFileName = theme._files.previews[variant];
    console.log('   building ' + theme.name + '[' + variant + '] into ' + fileName);
    const buildStart = process.hrtime();
    const basePath = join(CARDS_SOURCES, theme.name);

    // collect all theme files
    let files = [
      ...theme._sources[variant]['back'].map(fileName =>
        join(basePath, theme.overrides?.cardBack ?? BaseThemeConfig.overrides.cardBack, variant, fileName)
      )
    ];
    const frontFiles = theme._sources[variant]['front'].map(fileName =>
      join(basePath, theme.overrides?.cardFront ?? BaseThemeConfig.overrides.cardFront, variant, fileName)
    );

    // check multi layer files
    if (theme.isMultiLayer) {
      for (const file of frontFiles) {
        if (!fileNameWithoutExtension(file).includes(theme.overrides?.layerWildcard ?? BaseThemeConfig.overrides.layerWildcard)) {
          console.error(file + ' should be part of a multi layer theme, but has no wildcard');
          throw new Error('Multi-Layer sprite file without wildcard');
        }
      }
    }

    files = files.concat(frontFiles);
    console.log('     found', files.length, 'file(s)');
    console.log('     building...');

    // create theme
    const layerWildcard = theme.overrides?.layerWildcard ?? BaseThemeConfig.overrides.layerWildcard;
    const sheetData = theme.isMultiLayer
      ? await createMultiLayerCardSpriteSheet(files, themeEncoding, themDataPrefix, layerWildcard)
      : await createSingleLayerCardSpriteSheet(files, themeEncoding, themDataPrefix);

    const fileSize = await writeJSONFile(join(OUT_DIR, fileName), sheetData);
    outFiles += 1;
    sumSize += fileSize;

    const buildEnd = process.hrtime(buildStart);
    console.log('     %s[%s] successfully build (%s) - %s - %dms', theme.name, variant, fileName, formatFileSize(fileSize), buildEnd[1] / 1000000);

    // create preview
    const previewStart = process.hrtime();
    console.log('     building preview...');
    const previews = computeThemePreviews(theme.previews);
    console.log('     selected previews cards: ' + previews.join(', '));

    const previewImages = theme.isMultiLayer ? unique(previews.map(card => resolveLayersForCard(card)).flat()) : previews;

    const previewData = combineToObject(previewImages.map(card => ({ [card]: sheetData[card] })));
    const previewFileSize = await writeJSONFile(join(OUT_DIR, previewFileName), previewData);
    outFiles += 1;
    sumSize += previewFileSize;

    const previewEnd = process.hrtime(previewStart);
    console.log(
      '     %s[%s] successfully build (%s) - %s - %dms',
      theme.name,
      variant,
      previewFileName,
      formatFileSize(previewFileSize),
      previewEnd[1] / 1000000
    );
  }
}

// --- MAIN ---
/**
 * Create sprite sheets for all themes
 * @param {(typeof BaseThemeConfig)[]} themes a list of themes
 */
async function buildCards(themes) {
  for (const theme of themes) {
    await buildTheme(theme);
    console.log('  done');
  }
  console.log('done');
  console.log('build', outFiles, 'sprite maps');
}

(async () => {
  const allStart = process.hrtime();
  console.log('build card assets');
  // clean build output directory
  if (fs.existsSync(OUT_DIR)) {
    await fs.rm(OUT_DIR, { recursive: true });
  }
  await fs.mkdir(OUT_DIR);

  console.log('scanning directories for build files');

  // search all themes and source files
  const scanStart = process.hrtime();
  const themes = await findThemes();
  const themesWithSources /*: (typeof BaseThemeConfig)[] */ = [];
  for (const theme of themes) {
    themesWithSources.push(await searchThemeSources(theme));
  }
  const scanEnd = process.hrtime(scanStart);
  console.log('found %d themes (took %dms)', themes.length, scanEnd[1] / 1000000);
  console.log('start build process');

  // write meta information files
  await createMetaFiles(themesWithSources);
  // build theme assets
  await buildCards(themesWithSources);

  // copy files to public dir
  console.log('cleaning target folder');
  const targetDirectory = join(PUBLIC_DIR, 'assets');
  if (fs.existsSync(targetDirectory)) {
    await fs.rm(targetDirectory, { recursive: true });
  }
  console.log('copying build output');
  await fs.copy(OUT_DIR, targetDirectory);
  const allEnd = process.hrtime(allStart);
  console.log('done - %s - %dms', formatFileSize(sumSize), allEnd[1] / 1000000);
})();
