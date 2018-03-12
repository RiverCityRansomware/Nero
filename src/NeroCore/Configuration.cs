using System;
using System.IO;
using Newtonsoft.Json;

namespace NeroCore {
    public class Configuration {
        [JsonIgnore]
        public static string FileName { get; set; } = "./config/configuration.json";
        public ulong[] Owners { get; set; }
        public string Prefix { get; set; } = "!n ";
        public string Token { get; set; } = "";
        
        public string FFLogsKey { get; set; } = "";

        public static void EnsureExists() {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            if (!File.Exists(file)) {
                Console.WriteLine($"File Path: {FileName}\n Full Path: {file}");
                string path = Path.GetDirectoryName(file);
                if(!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                
                var config = new Configuration();

                Console.WriteLine("Please Enter your discord Token: ");
                string DiscordToken = Console.ReadLine();
                config.Token = DiscordToken;

                config.SaveJson();
            }
        }
        public void SaveJson() {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            File.WriteAllText(file, ToJson());
        }

        public static Configuration Load() {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(file));
        }

        public string ToJson()
            => JsonConvert.SerializeObject(this, Formatting.Indented);

    }
}