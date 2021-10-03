import { ObjectDirective } from 'vue';
import './Tooltip.css';

export const Tooltip: ObjectDirective<HTMLElement, string> = {
  mounted(el, binding) {
    el.classList.add('zwoo--tooltip');
    el.setAttribute('data-text', binding.value);
  }
};
