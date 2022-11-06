import { reactive, Ref, ref, toRefs, watch } from 'vue';

const StateObject = reactive(JSON.parse(atob(localStorage.getItem('zwoo:ud') ?? btoa('{}'))));

watch(StateObject, newValue => {
  localStorage.setItem('zwoo:ud', btoa(JSON.stringify(newValue)));
});

export const useUserDefaults = <T>(key: string, defaultValue: T): Ref<T> => {
  const refs = toRefs(StateObject);
  const value: Ref<T> = key in refs ? refs[key] : ref(defaultValue);

  watch(value, newValue => {
    StateObject[key] = newValue;
  });

  return value;
};
