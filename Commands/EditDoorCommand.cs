using Rocket.API;
using Rocket.Unturned.Chat;
using System.Collections.Generic;
using UnityEngine;

namespace DoorPlugin
{
    public class EditDoorCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "EditDoor";

        public string Help => "/EditDoor [Perms]";

        public string Syntax => "";

        public List<string> Aliases => new List<string> {"editdoor", "Dedit" };

        public List<string> Permissions => new List<string> { "D.Edit" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            EditData(DoorPlugin.Raycast(caller), command,caller);
        }
        public  void EditData(Transform transform, string[] Perms, IRocketPlayer caller)
        {
            var Exsists = DoorPlugin.Instance.Configuration.Instance.conf.Find(c => new Vector3 { x = c.transform.x, y = c.transform.y, z = c.transform.z } == transform.parent.parent.position);
            if (Exsists != null)
            {
                DoorPlugin.Instance.DeleteData(caller);
                DoorPlugin.Instance.Configuration.Instance.SaveDataForEdit(transform.parent.parent, Perms, caller);
                UnturnedChat.Say(caller, DoorPlugin.Instance.Translations.Instance.Translate("DoorEdited"));
            } else
            {
                UnturnedChat.Say (caller,DoorPlugin.Instance.Translations.Instance.Translate("NoExists"),Color.red);
            }
        }
    }
}
