export const CreateUseHook = <T>(setup: () => T): (() => T) => {
  const value = setup();
  return () => value;
};
