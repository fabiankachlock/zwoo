import { ZRPMessage } from '@/core/services/zrp/zrpTypes';
import { watch } from 'vue';
import { useGameEvents } from '../events';
import { ZRPMatcher } from './zrpMatcher';

export const useWatchGameEvents = <T>(matcher: ZRPMatcher<T>, handler: (msg: ZRPMessage<T>) => void) => {
  const gameEvents = useGameEvents();
  return watch(
    () => gameEvents.lastEvent,
    msg => {
      console.log(msg);
      if (matcher.matches(msg as ZRPMessage<T>)) {
        handler(msg as ZRPMessage<T>);
      }
    }
  );
};
