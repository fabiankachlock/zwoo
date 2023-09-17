import { computed, Ref, ref, watch } from 'vue';

import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

const serializeState = (state: Record<string, unknown>): string => {
  return btoa(JSON.stringify(state));
};

const deserializeState = (state: string): Record<string, unknown> => {
  return JSON.parse(atob(state));
};

export const useUserDefaults = <T>(key: string, defaultValue: T): Ref<T> => {
  const config = useConfig();
  const state = computed(() => config.get(ZwooConfigKey.UserDefaults));
  const stateObject = deserializeState(config.get(ZwooConfigKey.UserDefaults) || btoa('{}'));
  const value = ref(key in stateObject ? (stateObject[key] as T) : defaultValue) as Ref<T>;

  watch(state, newState => {
    const stateObject = deserializeState(newState);
    value.value = stateObject[key] as T;
  });

  watch(value, newValue => {
    const stateObject = deserializeState(config.get(ZwooConfigKey.UserDefaults) || btoa('{}'));
    stateObject[key] = newValue;
    config.set(ZwooConfigKey.UserDefaults, serializeState(stateObject));
  });

  return value;
};
