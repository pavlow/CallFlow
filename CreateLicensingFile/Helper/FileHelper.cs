using System.IO;
using Microsoft.Win32;

namespace CreateLicensingFile.Helper
{
    public static class FileHelper
    {
        public static string XmlFileSelector(bool publicFile)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (publicFile)
            {
                openFileDialog.Filter = "Public key file (key_public*.xml)|*.xml";
            }
            else
            {
                openFileDialog.Filter = "Private key file (key_private*.xml)|*.xml";
            }
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            else return null;
        }

        public static void StringToFile(string outfile, string data)
        {
            StreamWriter outStream = System.IO.File.CreateText(outfile);
            outStream.Write(data);
            outStream.Close();
        }

        public static string FileToString(string infile)
        {
            StreamReader stream = System.IO.File.OpenText(infile);
            string content = stream.ReadToEnd();
            stream.Close();
            return content;
        }
    }
}
