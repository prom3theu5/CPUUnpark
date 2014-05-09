using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnparkCPU
{
    public class RegData
    {
        public int Number { get; set; }
        public string Path { get; set; }
        public string ValueName { get; set; }
        public int Value { get; set; }
        public YesNoStatus IsParked
        {
            get
            {
                if (Value == 0)
                    return YesNoStatus.No;
                return YesNoStatus.Yes;
            }
        }

        public RegData()
        {
        }

        public RegData(int number, string path, string valueName, int val)
        {
            Number = number;
            Path = path;
            ValueName = valueName;
            Value = val;
        }
    }
}
