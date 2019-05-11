using System.Collections.Generic;
using System.Threading.Tasks;
using Rocket.API.Player;
using Rocket.Core.I18N;
using UnityEngine;

namespace DoorPlugin
{
    public class Config
    {
        //Config
        public int OpenDistance { get; set; }
        public bool OpenOnHit { get; set; }
        public List<Data> conf = new List<Data>();
        

        public async Task SaveData(Transform transform, string[] permissions, IPlayer caller)
        {
            var find = conf.Find(c => new Vector3 { x = c.transform.x, y = c.transform.y, z = c.transform.z } == transform.position);
            if (find == null)
            {
                    conf.Add(new Data { Permissions = new List<string>(permissions), transform = transform.position });
                DoorPlugin.Instance.Configuration.Save();
            }
            else
            {
               caller.User.SendLocalizedMessageAsync(DoorPlugin.Instance.Translations.Instance.Translate("AExsists"),Color.red);
            }
        }
        public async Task SaveDataForEdit(Transform transform, string[] permissions, IPlayer caller)
        {
          conf.Add(new Data { Permissions = new List<string>(permissions), transform = transform.position });
          DoorPlugin.Instance.Configuration.Save();  
        }


        public async Task LoadDefaults()
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
