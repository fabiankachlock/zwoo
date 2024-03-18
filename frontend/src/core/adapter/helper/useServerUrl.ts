export const useServerUrl = (id: string) => {
  return `${window.location.protocol}://${window.location.hostname}/join/${id}`;
};
