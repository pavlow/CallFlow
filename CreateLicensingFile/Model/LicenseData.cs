using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CreateLicensingFile.Common;

namespace CreateLicensingFile.Model
{
    public class LicenseData : PropertyChangedBase
    {
        #region Fields

        private const int TrialDaysDefault = 30;

        private string _personName;
        private string _companyName;
        private string _eMail;
        private DateTime _expDate = DateTime.Today.AddDays(TrialDaysDefault);
        private int _trialDays = TrialDaysDefault;

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
                    ExpDate = DateTime.Today.AddDays(_trialDays);
                    OnPropertyChanged("TrialDays");
                    OnPropertyChanged("ExpDate");
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
