using Rocket.Core.Plugins;
using SDG.Unturned;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Rocket.API.DependencyInjection;
using Rocket.API.Permissions;
using Rocket.API.Player;
using Rocket.Core.I18N;
using Rocket.Core.Logging;
using UnityEngine;
using Rocket.Unturned.Player;

namespace DoorPlugin
{
    public class DoorPlugin : Plugin<Config>
    {
        private IPermissionProvider _permissionProvider;

        public DoorPlugin(IDependencyContainer dependencyContainer, IPermissionProvider _permissionProvider) : base("Door Plugin", dependencyContainer)
        {
            this._permissionProvider = _permissionProvider;
        }
        
        
        #region Loading/Unloading
        protected override async Task OnActivate(bool isFromReload)
        {
            
            Logger.LogInformation("DoorPlugin Loaded ❤️ Joosep & Fixed by educatalan02", System.ConsoleColor.Blue);
            Logger.Log("Version: " + Assembly.GetName().Version);
        }
        protected override async Task OnDeactivate()
        {
            Logger.LogInformation("DoorPlugin Unloaded ❤️ Joosep", System.ConsoleColor.Blue);
            
        }
        public override TranslationList DefaultTranslations => new TranslationList
        {
            {"NoPerms", "DoorPlugin: You Don't Have Enough Permissions" },
            {"AExsists", "DoorPlugin: The Door Already Exists" },
            {"NoExists", "DoorPlugin: The Door Doesn't Exist" },
            {"NoDoor", "DoorPlugin: This Is Not A Door" },
            {"DoorAdded", "DoorPlugin: Door Added With These Perms : " },
            {"DoorRemoved", "DoorPlugin : Door Removed" },
            {"DoorEdited", "DoorPlugin : Door Edited" }

        };
        #endregion

        private async Task UnturnedPlayerEvents_OnPlayerUpdateGesture(UnturnedPlayer player, Rocket.Unturned.Events.UnturnedPlayerEvents.PlayerGesture gesture)
        {
            Transform raycast = await Raycast(player);

            if (gesture.Equals(Rocket.Unturned.Events.UnturnedPlayerEvents.PlayerGesture.PunchLeft) && raycast != null && Instance.Configuration.Instance.OpenOnHit == true
             )
            {
                if(Raycast(player).GetComponent<InteractableDoorHinge>() != null)
                {
                    ExecuteAsync(player);
                }    
            }
        }
        //Opens The Bloody Door
        public async Task ExecuteAsync(IPlayer caller)
        {
            var RaycastPos = Raycast(caller).parent.parent.position;
            var Exsists = Instance.Configuration.Instance.conf.Exists(c => new Vector3 { x = c.transform.x, y = c.transform.y, z = c.transform.z } == RaycastPos);
            if (Exsists != false)
            {
                var Item = Instance.Configuration.Instance.conf.Find(c => new Vector3 { x = c.transform.x, y = c.transform.y, z = c.transform.z } == RaycastPos);
                if (CheckPermissions(caller, Item.Permissions))
                {
                    OpenDoor(Raycast(caller).parent.parent, ShouldOpen(Raycast(caller)));
                }
                else
                {
                    caller.User.SendLocalizedMessageAsync(caller, Instance.Translations.Instance.Translate("NoPerms"));
                }
            }
        }

        //Checks If The Player Has One Of The Required Permissions
        public async Task <bool> CheckPermissions(IPlayer caller, List<string> perms, IPermissionProvider _permissionProvider)
        {
            if (perms.Count > 0)
            {
                foreach (var i in perms)
                {
                    foreach (var t in await _permissionProvider.GetGrantedPermissionsAsync(caller.User))
                    {
                        if (t.Name == i)
                        {
                            return true;
                        }
                    }
                }
            } else { return true; }
            return false;
        }

        public async Task DeleteData(Transform transform, string[] permissions, IPlayer rocketPlayer)
        {
            var i = Instance.Configuration.Instance.conf.Find(c => new Vector3 { x = c.transform.x, y = c.transform.y, z = c.transform.z } == Raycast(rocketPlayer).parent.parent.position);
            if(i != null)
            {
                Instance.Configuration.Instance.conf.Remove(i);
                Instance.Configuration.Save();
            } else
            {
                rocketPlayer.User.SendLocalizedMessageAsync(Instance.Translations.Instance.Translate("NoExists"));
            }
            
        }

        public static async Task<bool> ShouldOpen(Transform transform)
        {
            if (transform.GetComponent<InteractableDoorHinge>() != null)
            {
                transform = transform.parent.parent;
                return !transform.GetComponent<InteractableDoor>().isOpen;
            }
            return true;
        }
        #region DoorPackets
        public static void OpenDoor(Transform transform, bool ShouldOpen)
        {
            byte x;
            byte y;
            BarricadeRegion r;
            ushort index;
            ushort plant;
            

            if (BarricadeManager.tryGetInfo(transform, out x, out y,out plant, out index, out r))
            {

                BarricadeManager.instance.channel.send("askToggleDoor", ESteamCall.ALL, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[] {
                        x,
                        y,
                        plant,
                        index,
                        ShouldOpen

                    });
                BarricadeManager.instance.channel.send("tellToggleDoor", ESteamCall.ALL, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[] {
                        x,
                        y,
                        plant,
                        index,
                        ShouldOpen

                    });
            }
        }
        #endregion
        public static async Task<Transform> Raycast(IPlayer rocketPlayer)
        {
            RaycastHit hit;
            UnturnedPlayer player = (UnturnedPlayer)rocketPlayer;
            if (Physics.Raycast(player.Player.look.aim.position, player.Player.look.aim.forward, out hit, DoorPlugin.Instance.Configuration.Instance.OpenDistance, RayMasks.BARRICADE_INTERACT))
            {
                Transform transform = hit.transform;


                return transform;
            }
            return null;
        }
    }
}
