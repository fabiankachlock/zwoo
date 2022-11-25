export type TransitionProperty = 'left' | 'top' | 'right' | 'bottom' | 'width' | 'height';

export type AnimateFunc = (options: {
  element: HTMLElement;
  target: HTMLElement;
  initialState?: Record<TransitionProperty, number>;
  transitionProperties: TransitionProperty[];
  duration: number;
  timingFunction?: string;
  unsetProperties?: string[];
  unmount?: boolean;
}) => Promise<void>;

const animationContainerId = '__animation_container';

const getNumber = (pxString: string) => {
  const onlyNumbers = pxString.substring(0, pxString.length - 1);
  return parseFloat(onlyNumbers);
};

const baseStyles = 'position: fixed; pointer-events: none; cursor: not-allowed; ';

const buildStyleString = (
  properties: TransitionProperty[],
  values: Record<TransitionProperty, number>,
  inherited: string,
  duration: number,
  timingFunction: string,
  unset: string[]
): string => {
  let styles = `${inherited} ${baseStyles}`;
  for (const prop of properties) {
    styles += `${prop}: ${values[prop as TransitionProperty]}px; `;
  }
  styles += `transition: all ${duration}ms ${timingFunction}; `;
  styles += `transition-property: ${properties.join(', ')}; `;
  for (const prop of unset) {
    styles += `${prop}: unset !important; `;
  }
  return styles;
};

const animate: AnimateFunc = async ({ element, target, duration, transitionProperties, initialState, timingFunction, unsetProperties, unmount }) => {
  const targetRect = target.getBoundingClientRect();
  const sourceRect = element.getBoundingClientRect();
  const animationContainer = document.getElementById(animationContainerId);
  const sourceStyle = element.getAttribute('style') ?? '';
  const computedSourceStyle = getComputedStyle(element);
  const computedTargetStyle = getComputedStyle(target);
  const leftOffset = getNumber(computedSourceStyle.marginLeft) + getNumber(computedSourceStyle.paddingLeft);
  const topOffset = getNumber(computedSourceStyle.marginTop) + getNumber(computedSourceStyle.paddingTop);
  const rightOffset = getNumber(computedSourceStyle.marginRight) + getNumber(computedSourceStyle.paddingRight);
  const bottomOffset = getNumber(computedSourceStyle.marginBottom) + getNumber(computedSourceStyle.paddingBottom);

  if (unmount) {
    element.parentNode?.removeChild(element);
    animationContainer?.appendChild(element);
  }

  if (initialState) {
    element.setAttribute(
      'style',
      buildStyleString(transitionProperties, initialState, sourceStyle, duration, timingFunction ?? 'ease-in-out', unsetProperties ?? [])
    );
  } else {
    element.setAttribute(
      'style',
      buildStyleString(
        transitionProperties,
        {
          left: sourceRect.left + leftOffset,
          top: sourceRect.top + topOffset,
          right: sourceRect.right + rightOffset,
          bottom: sourceRect.bottom + bottomOffset,
          width: getNumber(computedSourceStyle.width),
          height: getNumber(computedSourceStyle.height)
        },
        sourceStyle,
        duration,
        timingFunction ?? 'ease-in-out',
        unsetProperties ?? []
      )
    );
  }
  setTimeout(() => {
    element.setAttribute(
      'style',
      buildStyleString(
        transitionProperties,
        {
          left: targetRect.left + leftOffset,
          top: targetRect.top + topOffset,
          right: targetRect.right + rightOffset,
          bottom: targetRect.bottom + bottomOffset,
          width: getNumber(computedTargetStyle.width),
          height: getNumber(computedTargetStyle.height)
        },
        sourceStyle,
        duration,
        timingFunction ?? 'ease-in-out',
        unsetProperties ?? []
      )
    );
  }, 10);

  await new Promise(res => setTimeout(() => res({}), duration));
  if (unmount) {
    animationContainer?.removeChild(element);
  }
};

export const useAnimate = (): AnimateFunc => animate;
