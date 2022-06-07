import { useConfig } from '../../config';
import { Shortcut } from '../types';

export const ToggleDarkModeShortcut: Shortcut<KeyboardEvent> = {
  id: 'toggle-darkmode',
  type: 'keyboard',
  keyCombination: 'd',
  execute(event) {
    if (event.key === 'd') {
      const config = useConfig();
      config.setDarkMode(!config.useDarkMode);
    }
  }
};

export const FullScreenShortcut: Shortcut<KeyboardEvent> = {
  id: 'toggle-fullscreen',
  type: 'keyboard',
  keyCombination: 'f',
  execute(event) {
    if (event.key === 'f') {
      const config = useConfig();
      config.setFullScreen(!config.useFullScreen);
    }
  }
};

export const QuickMenuShortcut: Shortcut<KeyboardEvent> = {
  id: 'toggle-quickmenu',
  type: 'keyboard',
  keyCombination: 'q',
  execute(event) {
    if (event.key === 'q') {
      const config = useConfig();
      config.setQuickMenu(!config.showQuickMenu);
    }
  }
};
