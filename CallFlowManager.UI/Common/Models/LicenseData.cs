using System;
using System.Xml.Serialization;
using CallFlowManager.UI.Common;

namespace CallFlowManager.UI.Models
{
    public class LicenseData : PropertyChangedBase
    {
        #region Fields

        private string _personName;
        private string _companyName;
        private string _eMail;
        private DateTime _expDate;
        private int _trialDays;

        #endregion // Fields

        #region Properties

        public string PersonName
        {
            get { return _personName; }
            set
            {
                if (value != _personName)
                {
                    _personName = value;
                    OnPropertyChanged("PersonName");
                }
            }
        }

        public int TrialDays
        {
            get { return _trialDays; }
            set
            {
                if (value != _trialDays)
                {
                    _trialDays = value;
                    OnPropertyChanged("TrialDays");
                }
            }
        }

        public string CompanyName
        {
            get { return _companyName; }
            set
            {
                if (value != _companyName)
                {
                    _companyName = value;
                    OnPropertyChanged("CompanyName");
                }
            }
        }

        [XmlAttribute]
        public string EMail
        {
            get { return _eMail; }
            set
            {
                if (value != _eMail)
                {
                    _eMail = value;
                    OnPropertyChanged("EMail");
                }
            }
        }

        public DateTime ExpDate
        {
            get { return _expDate; }
            set
            {
                if (value != _expDate)
                {
                    _expDate = value;
                    OnPropertyChanged("ExpDate");
                }
            }
        }

        #endregion // Properties
    }
}
