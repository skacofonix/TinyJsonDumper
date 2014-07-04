using System;
using System.Collections.Generic;
using System.Diagnostics;
using TinyJsonDumper.Loggable;

namespace TinyJsonDumper.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            DoSomething();
        }

        private static void DoSomething()
        {
            var instance = new MyClass
            {
                Date = DateTime.Now,
                Name = "Yannick",
                Order = 42,
                stringArray = new string[] { "1", "2", "3" },
                stringList = new List<string> { "4", "5", "6" },
                Blop = DateTime.Now,
                Dico = new Dictionary<int, string> { { 1, "one" }, { 2, "two" }, { 3, "three" } },
                MyEnum = MyEnum.THREE
            };
            var instance2 = new MyClass
            {
                Date = DateTime.Now.AddHours(1),
                Name = "Yannick2",
                Order = 43,
                stringArray = new string[] { "10", "20", "30" },
                stringList = new List<string> { "40", "50", "60" },
                CustomClass = instance
            };
            var instance3 = new MyClass
            {
                Date = DateTime.Now.AddHours(2),
                Name = "Yannick3",
                Order = 44,
                stringArray = new string[] { "10", "20", "30" },
                stringList = new List<string> { "40", "50", "60" },
                CustomClass = instance2
            };
            var instance4 = new MyClass
            {
                Date = DateTime.Now.AddHours(3),
                Name = "Yannick4",
                Order = 45,
                stringArray = new string[] { "10", "20", "30" },
                stringList = new List<string> { "40", "50", "60" },
                CustomClass = instance3
            };

            var output = instance3.DumpLoggable(true);

            if (output != null)
                Debug.WriteLine(output);
        }
    }
}
