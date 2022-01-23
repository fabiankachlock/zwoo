import { Ref, ref, watch } from 'vue';

export enum SWIPE_DIRECTION {
  left,
  right,
  up,
  down
}

type Point = {
  x: number;
  y: number;
};

const getTouch = (list: TouchList) => {
  if (list.length > 0) {
    return list[0];
  }
  return undefined;
};

export const useSwipeGesture = (
  element: Ref<HTMLElement | undefined>,
  handler: (from: Point, to: Point) => void,
  direction: SWIPE_DIRECTION,
  threshold = 50
): void => {
  const startPosition = ref<Point | undefined>(undefined);
  const endPosition = ref<Point | undefined>(undefined);

  const handleTouchStart = (event: TouchEvent): void => {
    const touch = getTouch(event.changedTouches);
    if (!startPosition.value && !endPosition.value && touch) {
      startPosition.value = {
        x: touch.screenX,
        y: touch.screenY
      };
    }
  };

  const handleTouchEnd = (event: TouchEvent): void => {
    const touch = getTouch(event.changedTouches);
    if (startPosition.value && !endPosition.value && touch) {
      endPosition.value = {
        x: touch.screenX,
        y: touch.screenY
      };
      evaluateSwipe();
      startPosition.value = undefined;
      endPosition.value = undefined;
    }
  };

  const evaluateSwipe = (): void => {
    if (startPosition.value && endPosition.value) {
      const xDistance = Math.abs(startPosition.value.x - endPosition.value.x);
      const yDistance = Math.abs(startPosition.value.y - endPosition.value.y);
      if (direction === SWIPE_DIRECTION.up && (startPosition.value.y - threshold < endPosition.value.y || xDistance > yDistance)) return;
      else if (direction === SWIPE_DIRECTION.down && (startPosition.value.y + threshold > endPosition.value.y || xDistance > yDistance)) return;
      else if (direction === SWIPE_DIRECTION.left && (startPosition.value.x - threshold < endPosition.value.x || xDistance < yDistance)) return;
      else if (direction === SWIPE_DIRECTION.right && (startPosition.value.x + threshold > endPosition.value.x || xDistance < yDistance)) return;
      handler(startPosition.value, endPosition.value);
    }
  };

  watch(element, (curr, prev) => {
    if (prev) {
      prev.removeEventListener('touchstart', handleTouchStart);
      prev.removeEventListener('touchend', handleTouchEnd);
    }
    if (curr) {
      curr.removeEventListener('touchstart', handleTouchStart);
      curr.removeEventListener('touchend', handleTouchEnd);
      curr.addEventListener('touchstart', handleTouchStart, false);
      curr.addEventListener('touchend', handleTouchEnd, false);
    }
  });
};
