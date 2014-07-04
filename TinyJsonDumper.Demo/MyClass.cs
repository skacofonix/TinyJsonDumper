using System;
using System.Collections.Generic;
using TinyJsonDumper.Loggable;

namespace TinyJsonDumper.Demo
{
    [Loggable]
    public class MyClass
    {
        public string Name { get; set; }

        public int Order { get; set; }

        public List<string> stringList { get; set; }

        public string[] stringArray { get; set; }

        public DateTime Date { get; set; }

        public MyClass CustomClass { get; set; }

        public DateTime? Blop { get; set; }

        public Dictionary<int,string> Dico { get; set; }

        public MyEnum MyEnum { get; set; }
    }
}
