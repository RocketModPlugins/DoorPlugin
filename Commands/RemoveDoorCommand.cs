using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using System.Collections.Generic;

namespace DoorPlugin
{
    public class RemoveDoorCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "RemoveDoor";

        public string Help => "/RemoveDoor";

        public string Syntax => null;

        public List<string> Aliases => new List<string> { "removedoor", "Rdoor" };

        public List<string> Permissions => new List<string> { "D.Removedoor" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var raycast = DoorPlugin.Raycast(caller);
            if(raycast != null)
            {
                if (raycast.GetComponent<InteractableDoorHinge>() != null)
                {
                    DoorPlugin.Instance.DeleteData(caller);
                    UnturnedChat.Say(caller, DoorPlugin.Instance.Translations.Instance.Translate("DoorRemoved"));
                }
                else
                {
                    UnturnedChat.Say(caller, DoorPlugin.Instance.Translations.Instance.Translate("NoDoor"), UnityEngine.Color.red);
                }

            }  
        }
    }
}
