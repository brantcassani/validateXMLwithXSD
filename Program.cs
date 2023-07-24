using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Windows.Forms;

class Program
{
    [STAThread]
    static void Main()
    {
        try
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Selecione o arquivo ZIP";
            openFileDialog.Filter = "Arquivos ZIP (*.zip)|*.zip";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string zipFilePath = openFileDialog.FileName;

                openFileDialog.Reset();
                openFileDialog.Title = "Selecione o arquivo XSD";
                openFileDialog.Filter = "Arquivos XSD (*.xsd)|*.xsd";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string xsdFilePath = openFileDialog.FileName;

                    using (var zip = System.IO.Compression.ZipFile.Open(zipFilePath, System.IO.Compression.ZipArchiveMode.Read))
                    {
                        foreach (var entry in zip.Entries)
                        {
                            if (Path.GetExtension(entry.FullName).Equals(".xml", StringComparison.OrdinalIgnoreCase))
                            {
                                using (var streamReader = new StreamReader(entry.Open()))
                                {
                                    string xmlContent = streamReader.ReadToEnd();

                                    XmlReaderSettings settings = new XmlReaderSettings();
                                    settings.ValidationType = ValidationType.Schema;
                                    settings.Schemas.Add(null, xsdFilePath);
                                    settings.ValidationEventHandler += new ValidationEventHandler(ValidationEventHandler);

                                    using (XmlReader reader = XmlReader.Create(new StringReader(xmlContent), settings))
                                    {
                                        while (reader.Read()) { }
                                    }

                                    Console.WriteLine($"O arquivo '{entry.FullName}' foi validado com sucesso.");
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro: {ex.Message}");
        }

        Console.WriteLine("Pressione qualquer tecla para sair...");
        Console.ReadKey();
    }

    static void ValidationEventHandler(object sender, ValidationEventArgs e)
    {
        Console.WriteLine($"Erro de validação: {e.Message}");
    }
}
