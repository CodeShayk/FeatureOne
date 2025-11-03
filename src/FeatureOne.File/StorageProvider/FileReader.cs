using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;

namespace FeatureOne.File.StorageProvider
{
    internal class FileReader : IFileReader
    {
        private readonly FileConfiguration configuration;

        public FileReader(FileConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public FileState Read()
        {
            if (string.IsNullOrWhiteSpace(configuration?.FilePath))
                throw new ArgumentNullException(nameof(configuration.FilePath));

            var values = new FileState() { Hash = string.Empty, Records = Array.Empty<FileRecord>() };

            if (System.IO.File.Exists(configuration.FilePath) == false)
                return values;

            var readAttempts = 0;
            while (true)
            {
                try
                {
                    var content = System.IO.File.ReadAllText(configuration.FilePath);
                    var list = new List<FileRecord>();

                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        var dictionary = JsonSerializer.Deserialize<Dictionary<string, JsonObject>>(content);

                        if (dictionary == null)
                            return new FileState() { Hash = ComputeMd5Hash(content), Records = list.ToArray() };

                        foreach (var item in dictionary)
                            list.Add(new FileRecord(item.Key, item.Value["toggle"].ToString()));
                    }

                    return new FileState() { Records = list.ToArray() };
                }
                catch (Exception ex)
                {
                    readAttempts++;
                    if (readAttempts >= 3)
                        throw new Exception("The file is in an invalid format", ex);

                    // Please be aware that this particular number is an empirical one - it helps to avoid
                    // System.IO.IOException issue stating that "Another process is locking the file".
                    // System.Runtime.Caching.HostFileChangeMonitor that we are using to track file
                    // updates is not strictly deterministic so we have to work-around it.
                    Thread.SpinWait(3000 * readAttempts);
                }
            }
        }

        private static string ComputeMd5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sBuilder = new StringBuilder();

                for (var i = 0; i < data.Length; i++)
                    sBuilder.Append(data[i].ToString("x2"));

                return sBuilder.ToString();
            }
        }
    }
}