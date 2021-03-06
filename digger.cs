using System;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Text;


class Digger
{
    // Conversion labels for file size
    static readonly string[] SizeSuffixes = {"bytes", "KB", "MB", "GB", "TB", "PB"};


    static IEnumerable<string> EnumerateFilesRecursively(string path)
    {
        return Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories);
    }


    private static string FormatByteSize(long byteSize)
    {
        decimal dvalue = (decimal)byteSize;

        int i= 0;
        while (Math.Round(dvalue) >= 1000)
        {
            dvalue /= 1024;
            i++;
        }

        return $"{dvalue.ToString("0.00")} {SizeSuffixes[i]}";
    }


    static XDocument CreateReport(IEnumerable<string> files)
    {
        XDocument doc = new XDocument();
        XElement root = new XElement("Root");
        XElement filesList = new XElement("Files");

        foreach (var file in files) 
        {
            string size = FormatByteSize(new FileInfo(file).Length);
            string extension = Path.GetExtension(file);

            // Check if Extension Exists
            bool hasExtension = (
                from ext in filesList.Elements("File").Elements("FileExtension")
                where ext.Value == extension
                select ext
            ).Any();

            if (hasExtension)
            {
                // Get the Element
                XElement temp = (
                    from f in filesList.Elements("File")
                    where f.Element("FileExtension").Value == extension
                    select f 
                ).Single();

                /* TODO:
                Increment the Count 
                Increment the Size
                */

                //Console.WriteLine(temp.Element("Size").Value);
            }
            else
            {
                filesList.Add( new XElement("File", 
                    new XElement ("FileExtension", $"{extension}"),
                    new XElement("Size", $"{size}"),
                    new XElement("Count", 0))
                );
            }
        }
        
        root.Add(filesList);
        doc.Add(root);


        Console.WriteLine(doc);
        return doc;
    }


    private static void Main(string[] args)
    {
        // Inputs 
        Console.WriteLine("Enter Path: ");
        string directory = Console.ReadLine();
        Console.WriteLine("Enter output file: ");
        string outputFile= Console.ReadLine();

        // Init our digger
        Digger FileDigger = new Digger();

        // Create file iterator
        IEnumerable<string> files = Digger.EnumerateFilesRecursively(directory);

        // Create Report 
        XDocument doc = Digger.CreateReport(files);

        // Create and write to html file
        using (FileStream fs = new FileStream(outputFile, FileMode.Create))
        {
            using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
            {
                w.WriteLine(doc);
            }
        }


    }
}

