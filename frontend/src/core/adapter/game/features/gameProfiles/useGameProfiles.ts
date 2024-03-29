import { defineStore } from 'pinia';
import { ref } from 'vue';

// import { ZRPCoder } from '@/core/domain/zrp/zrpCoding';
import { GameProfileGroup, ZRPOPCode } from '@/core/domain/zrp/zrpTypes';

// import { LocalGameProfileManager } from '@/core/services/gameProfileCache/LocalGameProfileManager';
import { MonolithicEventWatcher } from '../../util/MonolithicEventWatcher';
import { useGameEventDispatch } from '../../util/useGameEventDispatch';

const gameProfilesWatcher = new MonolithicEventWatcher(ZRPOPCode.AllGameProfiles);

export type GameProfile = {
  id: string;
  name: string;
  group: GameProfileGroup;
};

export const useGameProfiles = defineStore('game-profiles', () => {
  const profiles = ref<GameProfile[]>([]);
  const dispatchEvent = useGameEventDispatch();
  // let profileManager: LocalGameProfileManager;

  const _receiveMessage: (typeof gameProfilesWatcher)['_msgHandler'] = msg => {
    if (msg.code === ZRPOPCode.AllGameProfiles) {
      profiles.value = msg.data.profiles;
      // profileManager.syncProfiles(ZRPCoder.encodePayload(msg.data.profiles.filter(p => p.group === GameProfileGroup.User)));
    }
  };

  const safeToProfile = (name: string) => {
    dispatchEvent(ZRPOPCode.SaveToProfile, { name });
    // update local profiles
    loadProfiles();
  };

  const updateGameProfile = (id: string) => {
    dispatchEvent(ZRPOPCode.UpdateGameProfile, { id });
  };

  const applyProfile = (id: string) => {
    dispatchEvent(ZRPOPCode.ApplyGameProfile, { id });
  };

  const deleteProfile = (id: string) => {
    profiles.value = profiles.value.filter(p => p.id !== id);
    dispatchEvent(ZRPOPCode.DeleteGameProfile, { id });
  };

  const loadProfiles = () => {
    dispatchEvent(ZRPOPCode.GetAllGameProfiles, {});
  };

  const setup = () => {
    loadProfiles();
  };

  const reset = () => {
    profiles.value = [];
  };

  gameProfilesWatcher.onOpen(setup);
  gameProfilesWatcher.onMessage(_receiveMessage);
  gameProfilesWatcher.onReset(() => {
    reset();
    setup();
  });
  gameProfilesWatcher.onClose(() => {
    reset();
  });

  return {
    profiles,
    safeToProfile,
    updateGameProfile,
    applyProfile,
    loadProfiles,
    deleteProfile,
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: async () => {
      // profileManager = await LocalGameProfileManager.create();
    }
  };
});
