using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mailer
{
    public class ApplicationConfig
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int MaxMessages { get; set; }
    }

    public class ApplicationData
    {
        private static ApplicationData _instance;
        private ApplicationConfig _config;
        public string relFilePath = "config.json";

        private ApplicationData()
        {
            Read();
        }

        private void Read()
        {
            if (File.Exists(relFilePath))
            {
                _config = JsonConvert.DeserializeObject<ApplicationConfig>(File.ReadAllText(relFilePath));
            }
            else
            {
                Write();
                Read();
            }
        }

        private void Write()
        {
            if (_config == null) 
            {
                string json = JsonConvert.SerializeObject(new ApplicationConfig { Email = "", Password = "", MaxMessages = 30 });
                File.WriteAllText(relFilePath, json);
            }
            else
            {
                string json = JsonConvert.SerializeObject(_config);
                File.WriteAllText(relFilePath, json);
            }
        }

        public static ApplicationConfig GetConfig()
        {
            if (_instance == null) 
            {
                _instance = new ApplicationData();
            }
            return _instance._config;
        }

        public static void Save()
        {
            if (_instance == null) _instance = new ApplicationData();
            _instance.Write();
        }
    }
}
