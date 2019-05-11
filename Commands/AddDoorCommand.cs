using System;
using Rocket.API;
using SDG.Unturned;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rocket.API.Commands;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.I18N;

namespace DoorPlugin 
{
    public class AddDoorCommand : ICommand {
        

        public string Name => "AddDoor";
        

        public string Syntax => "/adddoor [perms]";

        public string[] Aliases => new string[] { "adddoor", "AD" };

        public string Summary => "Adds a door";

        public List<string> Permissions => new List<string> { "D.Adddoor" };

        public bool SupportsUser(IUser user) => true;

        public IChildCommand[] ChildCommands => null;

        public async Task ExecuteAsync(ICommandContext command, IPlayer caller)
        {
            var raycast = DoorPlugin.Raycast(caller);
            if(raycast != null)
            {
                if (raycast.GetComponent<InteractableDoorHinge>() != null)
                {
                    DoorPlugin.Instance.Configuration.Instance.SaveData(raycast.parent.parent, command, caller);
                    string Permissions = "";
                    foreach (var item in command)
                    {
                        Permissions += item + ", ";
                    }
                    caller.User.SendLocalizedMessageAsync(DoorPlugin.Instance.Translations.Instance.Translate("DoorAdded") + Permissions);
                }
                else
                {
                    caller.User.SendLocalizedMessageAsync(DoorPlugin.Instance.Translations.Instance.Translate("NoDoor"), UnityEngine.Color.red);
                }

            }   
        }
    }
}
