type GregRuntimeHooks = {
  mod_on_init?: () => void;
  mod_on_update?: (deltaTime: number) => void;
  mod_on_shutdown?: () => void;
};

let elapsed = 0;
const heartbeatSeconds = 10;

export const hooks: GregRuntimeHooks = {
  mod_on_init: () => {
    console.log("[gregCore][Sysadmin][TS] initialized");
  },
  mod_on_update: (deltaTime) => {
    elapsed += deltaTime;
    if (elapsed >= heartbeatSeconds) {
      elapsed = 0;
      console.log("[gregCore][Sysadmin][TS] heartbeat");
    }
  },
  mod_on_shutdown: () => {
    console.log("[gregCore][Sysadmin][TS] shutdown");
  }
};
