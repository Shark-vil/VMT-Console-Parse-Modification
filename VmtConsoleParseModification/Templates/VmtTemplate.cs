using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VmtConsoleParseModification.Modules;

namespace VmtConsoleParseModification.Templates
{
    public class VmtTemplate
    {
        public class VmtField
        {
            public string Key { get; set; }
            public string Value { get; set; }

            public VmtField(string Key, string Value)
            {
                this.Key = Key;
                this.Value = Value;
            }
        }

        public string FilePath { get; set; }
        public string DirectoryPath { get; set; }
        public string LocalDirectoryPath = "";
        public bool IsBrokenFile { get; set; }
        public string MaterialType { get; set; }
        public List<VmtField> Values { get; set; }

        public VmtTemplate() { }

        public VmtTemplate(string FilePath, string MaterialType, List<VmtField> Values)
        {
            SetPath(FilePath);

            this.MaterialType = MaterialType;
            this.Values = Values;
        }

        public void SetPath(string FilePath)
        {
            this.FilePath = VmtParser.NormalizePath(FilePath);
            this.DirectoryPath = VmtParser.NormalizePath(Path.GetDirectoryName(this.FilePath));
        }
    }
}
