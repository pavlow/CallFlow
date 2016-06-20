using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CallFlowManager.UI.Models;
using Microsoft.Win32;
using System.Windows;
// using Xceed.Wpf.Toolkit;

namespace CallFlowManager.UI.Common
{
    public static class FileHelper
    {
        public static string LogFileSelector()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Custom Log files (*.clg)|*.clg";
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "logs";

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            else return null;
        }

        public static string CsvFileSelector()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            else return null;
        }

        public static string AudioFileSelector()
        {
            //configure save file dialog box
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Audio files (*.wma)|*.wma|Audio files (*.wav)|*.wav|All files (*.*)|*.*";

            // Show dialog box
            Nullable<bool> result = openFileDialog.ShowDialog();

            // Process dialogue results
            if (result == true)
            {
                //Return dialogue result
                return openFileDialog.FileName;
            }
            else return null;

            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "Audio files (*.wma)|*.wma|Audio files (*.wav)|*.wav|All files (*.*)|*.*";

            //if (openFileDialog.ShowDialog() == true)
            //{
            //    return openFileDialog.FileName;
            //}
            //else return null;

            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "Audio files (*.wma)|*.wma|Audio files (*.wav)|*.wav|All files (*.*)|*.*";

            //if (openFileDialog.ShowDialog() == true)
            //{
            //    return openFileDialog.FileName;
            //}
            //else return null;
        }

        public static string OpenLicenseFile(string infile)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Licensing file (license.lic)|*.lic";
            openFileDialog.Title = "Open Licensing File";
            if (openFileDialog.ShowDialog() == false)
            {
                MessageBox.Show(String.Format("Licensing file {0} not found", infile));
                return null;
            }

            try
            {
                if (File.Exists(infile))
                {
                    File.Delete(infile);
                }

                File.Copy(openFileDialog.FileName, infile);
            }
            catch (Exception) {}
            
            return openFileDialog.FileName;
        }

        public static Config ReadConfig(string fileName)
        {
            ConfigService wfConfig = ConfigService.Instance;

            FileStream fs = null;
            try
            {
                if (File.Exists(fileName))
                {
                    fs = new FileStream(fileName, FileMode.Open);
                    var bf = new BinaryFormatter();
                    
                    wfConfig.Config = (Config)bf.Deserialize(fs);
                    fs.Close();
                }
                else
                {
                    MessageBox.Show("License data not found");
                }
            }

            catch (Exception)
            {
                MessageBox.Show("Error of reading license data");
                if (fs != null)
                    fs.Close();
                var serialiseObj = wfConfig.Config;
                Serialize(serialiseObj, fileName);
            }
            return wfConfig.Config;
        }

        /// <summary>
        /// Serialize a object to file
        /// </summary>
        /// <param name="serialObj">The collection for serialize</param>
        /// <param name="fileName">Name of a file for save</param>
        public static void Serialize(Object serialObj, string fileName)
        {
            // Open a stream for save in file 
            var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            var bf = new BinaryFormatter();
            bf.Serialize(fs, serialObj);
            fs.Close();
        }

        public static string FileToString(string infile)
        {
            string fileName = infile;
            if (!File.Exists((fileName)))
            {
                return null;
            }

            StreamReader stream = File.OpenText(infile);
            string content = stream.ReadToEnd();
            stream.Close();
            return content;
        }
    }
}



