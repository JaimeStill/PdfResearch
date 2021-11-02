using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iText.Forms;
using iText.Kernel.Pdf;

namespace PdfResearch
{
    public class PdfGenerator
    {
        string src;
        string dest;

        public PdfGenerator(string dest = "ssn.pdf")
        {
            this.src = GetResource("ssn.pdf");
            this.dest = Path.Combine(Environment.CurrentDirectory, dest);
        }

        public Task Generate(Record record) => Task.Run(() =>
        {
            var doc = new PdfDocument(new PdfReader(src), new PdfWriter(this.dest));
            var form = PdfAcroForm.GetAcroForm(doc, false);
            var fields = form.GetFormFields();

            foreach (var prop in record.Properties)
            {
                var matches = fields.Where(f => f.Key.Contains(prop.Map));

                if (matches.Count() > 1)
                {
                    matches = matches.Skip(1);

                    var match = matches.First();

                    var type = match.Key.Split('.').Last();

                    int i;
                    var isIndexed = int.TryParse(type, out i);

                    if (isIndexed)
                    {
                        foreach (var m in matches.OrderBy(x => x.Key))
                        {
                            i = int.Parse(m.Key.Split('.').Last());

                            form.GetField(m.Key)
                                .SetValue(prop.Value.ElementAt(i).ToString());
                        }
                    }
                    else
                    {
                        match = matches.FirstOrDefault(m =>
                        {
                            type = m.Key.Split('.').Last();

                            return prop.Value.ToLower() == type.ToLower();
                        });

                        if (match.Key is not null)
                            form.GetField(match.Key)
                                .SetValue("Yes", true);
                    }
                }
                else if (matches.Count() > 0)
                {
                    var match = matches.First();

                    form.GetField(match.Key)
                        .SetValue(prop.Value, true);
                }
            }

            doc.Close();
        });

        string GetResource(string resource) => $@"{AppDomain.CurrentDomain.BaseDirectory}files\{resource}";
    }
}