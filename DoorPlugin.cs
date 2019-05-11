using Rocket.API;
using Rocket.Core.Plugins;
using SDG.Unturned;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Rocket.API.Player;
using Rocket.Core.Logging;
using UnityEngine;
using Rocket.Unturned.Player;

namespace DoorPlugin
{
    public class DoorPlugin : Plugin<Config>
    {
        #region Loading/Unloading
        protected override async Task OnActivate()
        {
            
            Logger.LogInformation("DoorPlugin Loaded ❤️ Joosep & Fixed by educatalan02", System.ConsoleColor.Blue);
            Logger.Log("Version: " + Assembly.GetName().Version);
        }
        protected override async Task OnDeactivate()
        {
            Instance = null;
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

        private void UnturnedPlayerEvents_OnPlayerUpdateGesture(UnturnedPlayer player, Rocket.Unturned.Events.UnturnedPlayerEvents.PlayerGesture gesture)
        {
            Transform raycast = Raycast(player);

            if (gesture.Equals(Rocket.Unturned.Events.UnturnedPlayerEvents.PlayerGesture.PunchLeft) && raycast != null && Instance.Configuration.Instance.OpenOnHit == true
             )
            {
                if(Raycast(player).GetComponent<InteractableDoorHinge>() != null)
                {
                    Execute(player);
                }    
            }
        }
        //Opens The Bloody Door
        public void Execute(IPlayer caller)
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
                    UnturnedChat.Say(caller, Instance.Translations.Instance.Translate("NoPerms"));
                }
            }
        }

        //Checks If The Player Has One Of The Required Permissions
        public static bool CheckPermissions(IPlayer caller, List<string> perms)
        {
            if (perms.Count > 0)
            {
                foreach (var i in perms)
                {
                    foreach (var t in caller.GetPermissions())
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

        public void DeleteData(Transform transform, string[] permissions, IPlayer rocketPlayer)
        {
            var i = Instance.Configuration.Instance.conf.Find(c => new Vector3 { x = c.transform.x, y = c.transform.y, z = c.transform.z } == Raycast(rocketPlayer).parent.parent.position);
            if(i != null)
            {
                Instance.Configuration.Instance.conf.Remove(i);
                Instance.Configuration.Save();
            } else
            {
                UnturnedChat.Say(rocketPlayer, Instance.Translations.Instance.Translate("NoExists"));
            }
            
        }

        public static bool ShouldOpen(Transform transform)
        {
            if (transform.GetComponent<InteractableDoorHinge>() != null)
            {
                transform = transform.parent.parent;
                if (transform.GetComponent<InteractableDoor>().isOpen)
                {
                    return false;
                }
                else
                {
                    return true;
                }
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
        public static Transform Raycast(IPlayer rocketPlayer)
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
