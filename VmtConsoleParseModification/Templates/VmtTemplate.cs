using System;
using System.Collections.Generic;
using System.Text;

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

        public string MaterialType { get; set; }
        public List<VmtField> Values { get; set; }

        public VmtTemplate(string MaterialType, List<VmtField> Values)
        {
            this.MaterialType = MaterialType;
            this.Values = Values;
        }
    }
}
