using System;
using System.IO;
using Newtonsoft.Json;

namespace NeroLib {
    public class LibConfiguration {
        [JsonIgnore]
        public static string FileName { get; set; } = "./config/libConfiguration.json";
        public string ConnectionString { get; set; }= "";
        public string FFLogsKey { get; set; } = "";

        public static void EnsureExists() {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            if (!File.Exists(file)) {
                Console.WriteLine($"File Path: {FileName}\n Full Path: {file}");
                string path = Path.GetDirectoryName(file);
                if(!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                
                var config = new LibConfiguration();

                Console.WriteLine("Please enter the path to the users.db: ");
                string conString = Console.ReadLine();
                config.ConnectionString = conString;

                Console.WriteLine("Please enter your fflogs api key: ");
                string FflogsKey = Console.ReadLine();
                config.FFLogsKey = FflogsKey;

                config.SaveJson();
            }
        }
        public void SaveJson() {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            File.WriteAllText(file, ToJson());
        }

        public static LibConfiguration Load() {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            return JsonConvert.DeserializeObject<LibConfiguration>(File.ReadAllText(file));
        }

        public string ToJson()
            => JsonConvert.SerializeObject(this, Formatting.Indented);

    }
}