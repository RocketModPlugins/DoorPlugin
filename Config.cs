using Rocket.API;
using Rocket.Unturned.Chat;
using System.Collections.Generic;
using UnityEngine;

namespace DoorPlugin
{
    public class Config : IRocketPluginConfiguration
    {
        //Config
        public int OpenDistance { get; set; }
        public bool OpenOnHit { get; set; }
        public List<Data> conf = new List<Data>();
        

        public void SaveData(Transform transform, string[] permissions, IRocketPlayer caller)
        {
            var find = conf.Find(c => new Vector3 { x = c.transform.x, y = c.transform.y, z = c.transform.z } == transform.position);
            if (find == null)
            {
                    conf.Add(new Data { Permissions = new List<string>(permissions), transform = transform.position });
                DoorPlugin.Instance.Configuration.Save();
            }
            else
            {
               UnturnedChat.Say(caller,DoorPlugin.Instance.Translations.Instance.Translate("AExsists"),Color.red);
            }
        }
        public void SaveDataForEdit(Transform transform, string[] permissions, IRocketPlayer _)
        {
          conf.Add(new Data { Permissions = new List<string>(permissions), transform = transform.position });
          DoorPlugin.Instance.Configuration.Save();  
        }


        public void LoadDefaults()
        {
            OpenDistance = 2;
            OpenOnHit = true;
            conf.Add(new Data { Permissions = null, transform = new Vector3 { x = 0, y = 0, z = 0 } });
            
        }


        public class Data
        {
            public Vector3 transform;
            public List<string> Permissions;
        }
    }
}
