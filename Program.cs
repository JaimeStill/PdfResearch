using System;
using System.Threading.Tasks;

namespace PdfResearch
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var person = Person.Generate();
            var record = Record.Generate(person);
            var generator = new PdfGenerator();
            await generator.Generate(record);
        }
    }
}
