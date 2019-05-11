using SDG.Unturned;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rocket.API.Commands;
using Rocket.API.Player;
using Rocket.Core.I18N;

namespace DoorPlugin
{
    public class DoorCommand : ICommand
    {
        

        public string Name => "Door";

        public string Help => "You Can Use /Door Or Just Punch The Door With Left Click";

        public string Syntax => "";

        public string[] Aliases => new string[]  {"D", "door", "d"};

        public List<string> Permissions => new List<string> { "D.Door" };

        public async Task ExecuteAsync(IPlayer caller, string[] command)
        {
            var raycast = DoorPlugin.Raycast(caller);
            if(raycast != null)
            {
                if (raycast.GetComponent<InteractableDoorHinge>() != null)
                {
                    DoorPlugin.Instance.Execute(caller);
                }
                else
                {
                    caller.User.SendLocalizedMessageAsync(DoorPlugin.Instance.Translations.Instance.Translate("NoDoor"), UnityEngine.Color.red);
                }

            }    
        }      
    }
}
