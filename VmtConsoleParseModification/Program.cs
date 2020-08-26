using System;
using System.Collections.Generic;
using System.IO;
using VmtConsoleParseModification.Modules;
using VmtConsoleParseModification.Templates;

namespace VmtConsoleParseModification
{
    class Program
    {
        static void Main(string[] args)
        {
            bool IsExit = false;

            while (!IsExit)
            {
                Console.WriteLine("  VMT Files Parser");

                Console.WriteLine("  1 - Change the value of one file");
                Console.WriteLine("  2 - Change the value of all files in a directory");
                Console.WriteLine("  0 - Exit");

                Console.Write("> ");
                string command = Console.ReadLine();

                Console.Clear();

                if (long.TryParse(command, out _))
                    switch (Convert.ToInt64(command))
                    {
                        case 1:
                            VmtFileEditor();
                            break;
                        case 2:
                            VmtDirectoryEditor();
                            break;
                        case 0:
                            IsExit = true;
                            break;
                    }

                Console.Clear();
            }
        }

        private static void VmtFileEditor()
        {
            Console.WriteLine("  Write to vmt file path:");
            Console.Write("> ");
            string VmtFilePath = Console.ReadLine();

            VmtTemplate VmtObject = VmtParser.ParseVmtFile(VmtFilePath);

            if (VmtObject.IsBrokenFile)
            {
                Console.WriteLine("File is broken!");
                Console.WriteLine("\n  Press any button to continue...");
                Console.ReadLine();
                return;
            }

            bool IsExit = false;
            while (!IsExit)
            {
                Console.Clear();

                Console.WriteLine("  1 - Add value");
                Console.WriteLine("  2 - Delete value");
                Console.WriteLine("  3 - View the contents of the future file");
                Console.WriteLine("  4 - Save");
                Console.WriteLine("  0 - Back");

                Console.Write("> ");
                string command = Console.ReadLine();

                if (long.TryParse(command, out _))
                    switch (Convert.ToInt64(command))
                    {
                        case 1:
                            var Field = NewVmtField();
                            if (VmtObject.Values.Exists(x => x.Key == Field.Key))
                            {
                                VmtTemplate.VmtField GetField = VmtObject.Values.Find(x => x.Key == Field.Key);
                                GetField.Key = Field.Key;
                                GetField.Value = Field.Value;
                            }
                            else
                                VmtObject.Values.Add(Field);

                            break;
                        case 2:
                            Console.WriteLine("  Write the key (Example: $basetexture):");
                            Console.Write("> ");
                            string Key = Console.ReadLine();
                            RemoveValueOnVmtObject(ref VmtObject, Key);

                            break;
                        case 3:
                            ViewVmtObjectContent(VmtObject);
                            break;
                        case 4:
                            Console.WriteLine("  Write the output directory path:");
                            Console.Write("> ");
                            string OutputDirectory = Console.ReadLine();

                            if (!Directory.Exists(OutputDirectory))
                                Directory.CreateDirectory(OutputDirectory);

                            string FileName = Path.GetFileName(VmtObject.FilePath);

                            using(var WriterFile = new StreamWriter(Path.Combine(OutputDirectory, FileName)))
                            {
                                string FileContent = VmtTemplateToFileContent(VmtObject);
                                WriterFile.Write(FileContent);
                            }

                            break;
                        case 0:
                            IsExit = true;
                            break;
                    }
            }

            
        }

        private static void VmtDirectoryEditor()
        {
            bool IsSearchSubDir = true;

            Console.WriteLine("  Write to directory path:");
            Console.Write("> ");
            string VmtDirectoryPath = Console.ReadLine();
            VmtDirectoryPath = VmtParser.NormalizePath(VmtDirectoryPath);

            Console.WriteLine("  Default: Yes. You can leave the field blank.");
            Console.WriteLine("  Search in sub-directories? [ Y/n ]:");
            Console.Write("> ");
            string ReadSearchSubDir = Console.ReadLine();

            if (ReadSearchSubDir.Trim().ToLower() == "n")
                IsSearchSubDir = false;

            string[] VmtFiles;

            if (IsSearchSubDir)
                VmtFiles = Directory.GetFiles(VmtDirectoryPath, "*.vmt", SearchOption.AllDirectories);
            else
                VmtFiles = Directory.GetFiles(VmtDirectoryPath, "*.vmt", SearchOption.TopDirectoryOnly);

            List<VmtTemplate> VmtObjectList = new List<VmtTemplate>();

            foreach (string VmtFilePath in VmtFiles)
            {
                VmtTemplate VmtEmptyObject = VmtParser.ParseVmtFile(VmtFilePath);
                VmtEmptyObject.LocalDirectoryPath = VmtEmptyObject.DirectoryPath.Replace(VmtDirectoryPath, string.Empty);
                VmtObjectList.Add(VmtEmptyObject);
            }

            bool IsExit = false;
            while (!IsExit)
            {
                Console.Clear();

                Console.WriteLine("  1 - Add value");
                Console.WriteLine("  2 - Delete value");
                Console.WriteLine("  3 - Save");
                Console.WriteLine("  0 - Back");

                Console.Write("> ");
                string command = Console.ReadLine();

                if (long.TryParse(command, out _))
                    switch (Convert.ToInt64(command))
                    {
                        case 1:
                            var Field = NewVmtField();

                            for (int i = 0; i < VmtObjectList.Count; i++)
                            {
                                VmtTemplate VmtObject = VmtObjectList[i];

                                if (VmtObject.IsBrokenFile)
                                    continue;

                                try
                                {
                                    if (VmtObject.Values.Exists(x => x.Key == Field.Key))
                                    {
                                        VmtTemplate.VmtField GetField = VmtObject.Values.Find(x => x.Key == Field.Key);
                                        GetField.Key = Field.Key;
                                        GetField.Value = Field.Value;
                                    }
                                    else
                                        VmtObject.Values.Add(Field);
                                }
                                catch(Exception ex)
                                {
                                    Console.WriteLine("  Modification error:\n" + ex);
                                    Console.WriteLine("-------------------------------");
                                    Console.WriteLine("  File Content:");
                                    VmtTemplateToFileContent(VmtObject);
                                }
                            }

                            break;
                        case 2:
                            Console.WriteLine("  Write the key (Example: $basetexture):");
                            Console.Write("> ");
                            string Key = Console.ReadLine();

                            for (int i = 0; i < VmtObjectList.Count; i++)
                            {
                                VmtTemplate VmtObject = VmtObjectList[i];

                                if (VmtObject.IsBrokenFile)
                                    continue;

                                RemoveValueOnVmtObject(ref VmtObject, Key);
                                VmtObjectList[i] = VmtObject;
                            }

                            break;
                        case 3:
                            Console.WriteLine("  Write the output directory path:");
                            Console.Write("> ");
                            string OutputDirectory = Console.ReadLine();

                            if (!Directory.Exists(OutputDirectory))
                                Directory.CreateDirectory(OutputDirectory);

                            foreach (VmtTemplate VmtObject in VmtObjectList)
                            {
                                string FileName = Path.GetFileName(VmtObject.FilePath);

                                Console.WriteLine(VmtObject.LocalDirectoryPath + @"\" + Path.GetFileName(VmtObject.FilePath));

                                string OutputSubDirectory = Path.Combine(OutputDirectory, VmtObject.LocalDirectoryPath);

                                if (!Directory.Exists(OutputSubDirectory))
                                    Directory.CreateDirectory(OutputSubDirectory);

                                if (VmtObject.IsBrokenFile)
                                {
                                    string NewFilePath = Path.Combine(OutputSubDirectory, FileName);

                                    if (!File.Exists(NewFilePath))
                                        File.Copy(VmtObject.FilePath, NewFilePath);

                                    Console.WriteLine("--------------------------------");
                                    Console.WriteLine("  File is broken - " + VmtObject.FilePath);
                                    Console.WriteLine("  The file will be copied but not modified.");
                                    Console.WriteLine("--------------------------------");
                                }
                                else
                                {
                                    string OutputFIlePath = Path.Combine(OutputSubDirectory, FileName);

                                    using (var WriterFile = new StreamWriter(OutputFIlePath))
                                    {
                                        string FileContent = VmtTemplateToFileContent(VmtObject);
                                        WriterFile.Write(FileContent);
                                    }
                                }
                            }

                            Console.WriteLine("\n  Press any button to continue...");
                            Console.ReadLine();

                            break;
                        case 0:
                            IsExit = true;
                            break;
                    }
            }
        }

        private static VmtTemplate.VmtField NewVmtField()
        {
            Console.WriteLine("  Write the key (Example: $basetexture):");
            Console.Write("> ");
            string Key = Console.ReadLine();

            Console.WriteLine("  Write the value:");
            Console.Write("> ");
            string Value = Console.ReadLine();

            return new VmtTemplate.VmtField(Key, Value);
        }

        private static void RemoveValueOnVmtObject(ref VmtTemplate VmtObject, string Key)
        {
            VmtObject.Values.RemoveAll(x => x.Key == Key);
        }

        private static void ViewVmtObjectContent(VmtTemplate VmtObject)
        {
            Console.Clear();

            if (VmtObject.IsBrokenFile)
            {
                Console.WriteLine("File is broken!");
                Console.WriteLine("\n  Press any button to continue...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine($"  \"{VmtObject.MaterialType}\"");
                Console.WriteLine("  {");

                foreach (var Field in VmtObject.Values)
                    Console.WriteLine($"      \"{Field.Key}\" \"{Field.Value}\"");

                Console.WriteLine("  }");

                Console.WriteLine("\n  Press any button to continue...");
                Console.ReadLine();
            }
        }

        private static string VmtTemplateToFileContent(VmtTemplate VmtObject)
        {
            string NewFileContent = $"\"{VmtObject.MaterialType}\"\n";
            NewFileContent += "{\n";

            foreach (var Field in VmtObject.Values)
                NewFileContent += $"    \"{Field.Key}\" \"{Field.Value}\"\n";

            NewFileContent += "}\n";

            NewFileContent = NewFileContent.Replace("\n", Environment.NewLine);

            return NewFileContent;
        }
    }
}
