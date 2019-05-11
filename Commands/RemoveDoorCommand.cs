using SDG.Unturned;
using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Player;
using Rocket.Core.I18N;

namespace DoorPlugin
{
    public class RemoveDoorCommand : Command
    {
        

        public string Name => "RemoveDoor";

        public string Help => "/RemoveDoor";

        public string Syntax => null;

        public List<string> Aliases => new List<string> { "removedoor", "Rdoor" };

        public List<string> Permissions => new List<string> { "D.Removedoor" };

        public void ExecuteAsync(IPlayer caller, ICommandContext commandContext)
        {
            var raycast = DoorPlugin.Raycast(caller);
            if(raycast != null)
            {
                if (raycast.GetComponent<InteractableDoorHinge>() != null)
                {
                    DoorPlugin.Instance.DeleteData(raycast.parent.parent, command, caller);
                    caller.User.SendLocalizedMessageAsync(DoorPlugin.Instance.Translations.Instance.Translate("DoorRemoved"));
                }
                else
                {
                    caller.User.SendLocalizedMessageAsync(DoorPlugin.Instance.Translations.Instance.Translate("NoDoor"), UnityEngine.Color.red);
                }

            }  
        }
    }
}
