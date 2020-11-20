using System;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace msfsJSONgen
{
    class Program
    {
        static void Main(string[] args)
        {
            createManifest(args);
        }

        static void createManifest(string[] args)
        {
            string TargetFolder = "";

            if (args.Length > 0)
            {
                if (Directory.Exists(args[0]))
                    TargetFolder = args[0] + "\\";
                else
                {
                    Console.WriteLine("Target folder does not exists: " + args[0]);
                    Thread.Sleep(3000);
                    return;
                }
            }

            if (string.IsNullOrEmpty(TargetFolder))
            {
                if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\legacy-vcockpits-instruments"))
                {
                    TargetFolder = AppDomain.CurrentDomain.BaseDirectory + "\\legacy-vcockpits-instruments\\";
                }
                else
                {
                    Console.WriteLine("Target folder not set, pass it as argument, or place EXE near to folder 'legacy-vcockpits-instruments'");
                    Thread.Sleep(3000);
                    return;
                }
            }

            string json = "";

            if (!File.Exists(TargetFolder + "manifest.json"))
            {
                Manifest manifest = new Manifest(new Dependencies[] { new Dependencies("fs-base-ui", "0.1.10") }, "CORE", "MSFS Legacy Importer Instruments", "MSFS Legacy Importer", "Alex Marko", "1.0.0", "1.0.0", new ReleaseNotes(new Neutral("", "")));
                json = JsonConvert.SerializeObject(manifest, Newtonsoft.Json.Formatting.Indented);

                try { File.WriteAllText(TargetFolder + "manifest.json", json); }
                catch { Console.WriteLine("Can't write into file " + TargetFolder + "manifest.json"); Thread.Sleep(3000); }
            }

            // GENERATE LAYOUT FILE
            scanTargetFolder(TargetFolder);
        }

        static int scanTargetFolder(string TargetFolder)
        {
            int i = 0;

            if (TargetFolder != "")
            {
                Content[] array = new Content[10000000];

                // ADD MANIFEST AT THE TOP

                string[] parentDirectory = new string[] { TargetFolder };
                string[] subDirectories = Directory.GetDirectories(TargetFolder, "*", SearchOption.AllDirectories).Where(x => !x.Contains("\\.")).ToArray();
                subDirectories = parentDirectory.Concat(subDirectories).ToArray();

                foreach (var subdir in subDirectories)
                {
                    string folderName = subdir.Split('\\').Last().ToLower().Trim();
                    if (folderName.Length > 0 && folderName[0] != '.')
                    {
                        var txtFiles = Directory.EnumerateFiles(subdir, "*.*", SearchOption.TopDirectoryOnly);
                        foreach (string currentFile in txtFiles)
                        {
                            if (Path.GetFileName(currentFile)[0] != '.' && Path.GetExtension(currentFile).ToLower() != "json" && Path.GetExtension(currentFile).ToLower() != "exe"
                                && Path.GetExtension(currentFile).ToLower() != "zip" && Path.GetExtension(currentFile).ToLower() != "rar" && Path.GetExtension(currentFile).ToLower() != "7z")
                            {
                                FileInfo info = new System.IO.FileInfo(currentFile);
                                array[i] = new Content(currentFile.Replace(TargetFolder, "").Replace("\\", "/").Trim('/'), info.Length, info.LastWriteTimeUtc.ToFileTimeUtc());

                                i++;
                            }
                        }
                    }
                }

                // CLEAR UNUSED ARRAY ITEMS
                Content[] truncArray = new Content[i];
                Array.Copy(array, truncArray, truncArray.Length);

                // ENCODE AND SAVE JSON
                ParentContent obj = new ParentContent(truncArray);
                string json = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);

                try { File.WriteAllText(TargetFolder + "\\layout.json", json); }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return 0;
                }
            }

            return i;
        }

        public class Dependencies
        {
            public string name { get; set; }
            public string package_version { get; set; }
            public Dependencies(string Name, string Package_version)
            {
                name = Name;
                package_version = Package_version;
            }
        }

        public class Manifest
        {
            public Dependencies[] dependencies { get; set; }
            public string content_type { get; set; }
            public string title { get; set; }
            public string manufacturer { get; set; }
            public string creator { get; set; }
            public string package_version { get; set; }
            public string minimum_game_version { get; set; }
            public ReleaseNotes release_notes { get; set; }
            public Manifest(Dependencies[] Dependencies, string Content_type, string Title, string Manufacturer, string Creator,
                string Package_version, string Minimum_game_version, ReleaseNotes Release_notes)
            {
                dependencies = Dependencies;
                content_type = Content_type;
                title = Title;
                manufacturer = Manufacturer;
                creator = Creator;
                package_version = Package_version;
                minimum_game_version = Minimum_game_version;
                release_notes = Release_notes;
            }
        }

        public class ReleaseNotes
        {
            public Neutral neutral { get; set; }
            public ReleaseNotes(Neutral Neutral)
            {
                neutral = Neutral;
            }
        }

        public class Neutral
        {
            public string LastUpdate { get; set; }
            public string OlderHistory { get; set; }
            public Neutral(string lastUpdate, string olderHistory)
            {
                LastUpdate = lastUpdate;
                OlderHistory = olderHistory;
            }
        }

        public class ParentContent
        {
            public Content[] content { get; set; }
            public ParentContent(Content[] Content)
            {
                content = Content;
            }
        }

        public class Content
        {
            public string path { get; set; }
            public long size { get; set; }
            public long date { get; set; }
            public Content(string Path, long Size, long Date)
            {
                path = Path;
                size = Size;
                date = Date;
            }
        }

    }
}
