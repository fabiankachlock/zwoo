import { useConfig } from '../../config';
import { Shortcut } from '../types';

export const ToggleDarkModeShortcut: Shortcut<KeyboardEvent> = {
  type: 'keyboard',
  execute(event) {
    if (event.ctrlKey && event.key === 'd') {
      const config = useConfig();
      config.setDarkMode(!config.useDarkMode);
    }
  }
};

export const FullScreenShortcut: Shortcut<KeyboardEvent> = {
  type: 'keyboard',
  execute(event) {
    if (event.ctrlKey && event.key === 'f') {
      const config = useConfig();
      config.setFullScreen(!config.useFullScreen);
    }
  }
};

export const QuickMenuShortcut: Shortcut<KeyboardEvent> = {
  type: 'keyboard',
  execute(event) {
    if (event.ctrlKey && event.key === 'q') {
      const config = useConfig();
      config.setQuickMenu(!config.showQuickMenu);
    }
  }
};
