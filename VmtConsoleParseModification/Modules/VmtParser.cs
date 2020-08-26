using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using VmtConsoleParseModification.Templates;

namespace VmtConsoleParseModification.Modules
{
    public class VmtParser
    {
        public static string NormalizePath(string FilePath)
        {
            return Path.GetFullPath(new Uri(FilePath).LocalPath)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                       .ToLower();
                       //.ToUpperInvariant();
        }

        public static VmtTemplate ParseVmtFile(string FilePath)
        {
            string MaterialType = "VertexlitGeneric";
            var MaterialFields = new List<VmtTemplate.VmtField>();
            var VmtObject = new VmtTemplate();
            VmtObject.IsBrokenFile = true;
            VmtObject.SetPath(FilePath);

#if DEBUG
            Console.WriteLine("File - " + FilePath);
#endif

            using (var ReadingFile = new StreamReader(FilePath))
            {
                try
                {
                    string FileContent = ReadingFile.ReadToEnd();
                    FileContent = Regex.Replace(FileContent.Replace("\t", " "), @"[ ]{2,}", " ");

                    int StartSubContent = -1;
                    int EndSubContent = -1;

                    for (int i = 0; i < FileContent.Length; i++)
                    {
                        if (FileContent[i] == '{' && StartSubContent == -1)
                        {
                            StartSubContent = i;
                            break;
                        }
                    }

                    for (int i = FileContent.Length - 1; i >= 0; i--)
                    {
                        if (FileContent[i] == '}' && EndSubContent == -1)
                        {
                            EndSubContent = i;
                            break;
                        }
                    }

                    MaterialType = FileContent.Trim().Substring(0, StartSubContent - 1).Trim();
                    MaterialType = MaterialType.Replace("\"", string.Empty);

                    string SubContent = FileContent.Substring(StartSubContent + 1, EndSubContent - StartSubContent - 1);
                    string[] SubContentSplit = SubContent.Split(Environment.NewLine);

                    for (int i = 0; i < SubContentSplit.Length; i++)
                    {
                        string Filed = SubContentSplit[i].Trim();

                        if (Filed.Replace(" ", "") != string.Empty)
                        {
                            Filed = Filed.Replace("\"", string.Empty);
                            string[] KeyAndValue = Filed.Split(' ', 2);

                            MaterialFields.Add(new VmtTemplate.VmtField(KeyAndValue[0], KeyAndValue[1]));
                        }
                    }
                /*
                #if DEBUG
                                Console.WriteLine("Type - " + MaterialType);
                #endif

                #if DEBUG
                                foreach (var Filed in MaterialFields)
                                    Console.WriteLine(Filed.Key + " - " + Filed.Value);
                #endif
                */
                }
                catch
                {
                    return VmtObject;
                }
        }

            VmtObject.MaterialType = MaterialType;
            VmtObject.Values = MaterialFields;
            VmtObject.IsBrokenFile = false;

            return VmtObject;
        }
    }
}
