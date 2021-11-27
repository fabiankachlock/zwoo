import { ObjectDirective } from 'vue';
import './Tooltip.css';

export const Tooltip: ObjectDirective<HTMLElement, string> = {
  mounted(el, binding) {
    el.classList.add('zwoo--tooltip');
    if (binding.modifiers['underline']) {
      el.classList.add('tooltip__underline');
    }
    el.setAttribute('data-text', binding.value);
  },
  updated(el, binding) {
    el.setAttribute('data-text', binding.value);
  }
};

