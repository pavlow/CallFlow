using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using CreateLicensingFile.Common;
using CreateLicensingFile.Helper;
using CreateLicensingFile.Model;

namespace CreateLicensingFile.ViewModel
{
    public class MainViewModel : PropertyChangedBase
    {
        #region Fields

        private const string LicenseFolder = "License";

        private string _privateKeyXML;
        private string _publicKeyXML;
        private LicenseData _currentLicense;


        //private ICommand _loadKeyPublicCommand;
        //private ICommand _createKeyPairsCommand;

        #endregion

        public MainViewModel()
        {
            LoadKeyPublicCommand = new RelayCommand(_ => LoadKeyPublic());
            LoadKeyPrivateCommand = new RelayCommand(_ => LoadKeyPrivate());
            CreateKeyPairsCommand = new RelayCommand(_ => CreateKeyPairs(LicenseFolder));
            CreateLicenseCommand = new RelayCommand(_ => CreateLicense(LicenseFolder));
            CurrentLicense = new LicenseData();

            ValidateLicenseCommand = new RelayCommand(_ => ValidateLicense());

            _privateKeyXML = "<RSAKeyValue><Modulus>1Ij5dC8CcvP55RmzXwaiQFvTYhVtXYHsseOQX7snuVan/uuVFxlnKRSbWtxRZacq6L2yAmLBddojzlAjmJoXsedq9V1rLZnPITCWuCcsWnEHNEOeltW2tLILdEhWObJuYQmiUyWOT7YDww68XmLxaS4TqmNA/cNQuYcg59W0K9YRQ1ZIgroqea2duetvSMNc5XPEpdH5/JoH7tOJzMX5nWNB6bvk9hV0LNoFSVsMF89b57uapPutNKEEsM42X6q3p/XTDsXwMuc/u9sMEbIFm1jPvQXykXJ8Etve2o05jWf1Fr21VZN1tIb3N/apWSDc7QPEh/Ew0M4PiOwa98n6SQ==</Modulus><Exponent>AQAB</Exponent><P>7ZPxqG780NT7en3cM64WpQ756mzwPb7I6tG7sJKScfD6OchoLnjNqh5A6LD7k6GsI66pthGoDI8FyjEjmt5M4Qfu2Qv4gVwwW85E+d9N7EskxidBd5ZB1siuHbLjBieize4gILn8IfJ+8ASf7mj6KAMjwe4g9cqSIRMar5Luvoc=</P><Q>5QPqayoNv/18lROuT7B/kmMhf6SN+v9f9r4HxLCOBDAOosZmTVVNo3xtCSKJ6PuaeRlfOlAJXhr0HFLpJRMY9fMWafXMz+FMgv+8AKTxm1fXsU0cGMQjYQ0ahsEgsR2cu0eglr6J1bEf8KF4XbBVpgiwa/bjwSQx9/RR2mPPZK8=</Q><DP>o9ujFAmgoyQSYpSGGLrccXFX22DRfbSlfJe1PP6KtL2Ax/O6LzswlkGRgSv4CAW85UtlT+bzod3PH3ou1XS3T4maKecAua/sxpUIq9JhOzVEosqWc7WVqV72ABPOUwNpXWD8lbXQ+VT4SfDWoaeTyZ8IvYvQ/dV+XgOAJ1JPCec=</DP><DQ>gvRJQHwsnqMxcQFvBGx1llyPFDEhauxKkIo7YPJgUu6z/8oZR5FGsvx8UOPqHkp5G1QxyuNnZ+NbwTJ71NJlzIXzRCXktt4q5SGc/HVVHrINSXpBXbwSSvW2PGYE7qJzM/gSUQClSoddLzj13gTuBOIlF6d2tsOeux1/DLVUK0s=</DQ><InverseQ>stMyhdLMChzBpQa8B3uBrq6In3qARi4O8BfSFFLVnLuAnE6wN6MUEcu52ZBvJ2zXZSm7EDlfCQrxpQWNsfyjvfmCmuX+jFGrgqS6OsKxchHkiR6YCQuk1VBbgAamyd2AKMfeM2bYbJz+0LwbSPDZPPTEjbKFSJbui4LnKV3kqzM=</InverseQ><D>WL2oip5mQ2EjYklM38yBSqYMqNORio2JKu19hrs6ZuSL2KC865wrurdj9pgvVGZsQCmUffKGHJMsiCRl+H2+jwW1XyXWbn/eQFltVQ7mMg0nmOby/L/XPbNpnO/nMMgT1miTYmw384hoIbWxaSee7O/UPjybElnPBugpESPlyrcgQ7EPDtPHME3oQWvWlTysenDVwUEHWu8bum2YZbC6Kd4Em+atFYgZOsc0iOrWsa8PPkj3VolXJ1WH/9/3dytDNHJtc0BjHs2AE1AOLCU1+9Fy+0NzXtEnQOkaZV3dDAv0n0iDJx1PGfgOFHHoNZEwyU15dCWQ7pSCXKfK/8jV0Q==</D></RSAKeyValue>";
        }

        public LicenseData CurrentLicense
        {
            get { return _currentLicense; }
            set
            {
                if (value != _currentLicense)
                {
                    _currentLicense = value;
                    OnPropertyChanged("CurrentLicense");
                }
            }
        }

        public string PrivateKeyXML
        {
            get { return _privateKeyXML; }
            set
            {
                if (_privateKeyXML != value)
                {
                    _privateKeyXML = value;
                    OnPropertyChanged("PrivateKeyXML");
                }
            }
        }

        public string PublicKeyXML
        {
            get { return _publicKeyXML; }
            set
            {
                if (_publicKeyXML != value)
                {
                    _publicKeyXML = value;
                    OnPropertyChanged("PublicKeyXML");
                }
            }
        }

        public ICommand LoadKeyPublicCommand { get; private set; }

        public ICommand LoadKeyPrivateCommand { get; private set; }

        public ICommand CreateKeyPairsCommand { get; private set; }

        public ICommand CreateLicenseCommand { get; private set; }

        public ICommand ValidateLicenseCommand { get; private set; }

        public void LoadKeyPrivate()
        {
            string file = FileHelper.XmlFileSelector(false);
            StreamReader stream = System.IO.File.OpenText(file);
            PrivateKeyXML = stream.ReadToEnd();
        }

        public void LoadKeyPublic()
        {
            string file = FileHelper.XmlFileSelector(true);
            StreamReader stream = System.IO.File.OpenText(file);
            PublicKeyXML = stream.ReadToEnd();
        }

        public void CreateKeyPairs(string directory)
        {
            string dateTime = DateTime.Now.ToString("yyyy-MM-dd hh_mm_ss");

            RSACryptoServiceProvider key = new RSACryptoServiceProvider(2048);

            PrivateKeyXML = key.ToXmlString(true);
            PublicKeyXML = key.ToXmlString(false);

            FileHelper.StringToFile(Path.Combine(directory, string.Concat("key_public ", dateTime, ".xml")), PublicKeyXML);
            FileHelper.StringToFile(Path.Combine(directory, string.Concat("key_private ", dateTime, ".xml")), PrivateKeyXML);
        }

        public void CreateLicense(string directory)
        {
            if (string.IsNullOrEmpty(PrivateKeyXML))
            {
                MessageBox.Show("Create or open a private/public key pair file");
                return;
            }
            StringBuilder licenseContent = new StringBuilder();
            licenseContent.Append("<license>");
            licenseContent.AppendFormat("<personName>{0}</personName>", CurrentLicense.PersonName);
            licenseContent.AppendFormat("<companyName>{0}</companyName>", CurrentLicense.CompanyName);
            licenseContent.AppendFormat("<eMail>{0}</eMail>", CurrentLicense.EMail);
            licenseContent.AppendFormat("<expDate>{0}</expDate>", CurrentLicense.ExpDate.Date.ToString("yyyy-MM-dd"));
            licenseContent.AppendFormat("<trialDays>{0}</trialDays>", CurrentLicense.TrialDays.ToString());
            licenseContent.Append("</license>");
            var lc = licenseContent.ToString();
            XmlDocument fileContent = License.SignXmlDocument(lc, PrivateKeyXML);

            FileHelper.StringToFile(Path.Combine(LicenseFolder, "license.lic"), fileContent.OuterXml);

            MessageBox.Show("Done");

        }

        public void ValidateLicense()
        {
            if (string.IsNullOrEmpty(PublicKeyXML))
            {
                MessageBox.Show("Create or open a public key pair file");
                return;
            }

            string licenseContent = FileHelper.FileToString(Path.Combine(LicenseFolder, "license.lic"));

            bool valid = License.VerifyXmlDocument(PublicKeyXML, licenseContent);

            if (valid)
            {
                MessageBox.Show("License is valid");
            }
            else
            {
                MessageBox.Show("License file was modified");
            }

        }



    }
}
