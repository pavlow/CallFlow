using System.Collections.Generic;

namespace CallFlowManagerv2
{
    

    public static class Globals
    {
        public static Dictionary<string, string> TimeZones = new Dictionary<string, string>
        {
            {"(GMT-12:00) International Date Line West", "Dateline Standard Time"},
            {"(GMT-11:00) Midway Island, Samoa", "Samoa Standard Time"},
            {"(GMT-10:00) Hawaii", "Hawaiian Standard Time"},
            {"(GMT-09:00) Alaska", "Alaskan Standard Time"},
            {"(GMT-08:00) Pacific Time (US and Canada); Tijuana", "Pacific Standard Time"},
            {"(GMT-07:00) Mountain Time (US and Canada)", "Mountain Standard Time"},
            {"(GMT-07:00) Chihuahua, La Paz, Mazatlan", "Mexico Standard Time 2"},
            {"(GMT-07:00) Arizona", "U.S. Mountain Standard Time"},
            {"(GMT-06:00) Central Time (US and Canada", "Central Standard Time"},
            {"(GMT-06:00) Saskatchewan", "Canada Central Standard Time"},
            {"(GMT-06:00) Guadalajara, Mexico City, Monterrey", "Mexico Standard Time"},
            {"(GMT-06:00) Central America", "Central America Standard Time"},
            {"(GMT-05:00) Eastern Time (US and Canada)", "Eastern Standard Time"},
            {"(GMT-05:00) Indiana (East)", "U.S. Eastern Standard Time"},
            {"(GMT-05:00) Bogota, Lima, Quito", "S.A. Pacific Standard Time"},
            {"(GMT-04:00) Atlantic Time (Canada)", "Atlantic Standard Time"},
            {"(GMT-04:00) Caracas, La Paz", "S.A. Western Standard Time"},
            {"(GMT-04:00) Santiago", "Pacific S.A. Standard Time"},
            {"(GMT-03:30) Newfoundland and Labrador", "Newfoundland and Labrador Standard Time"},
            {"(GMT-03:00) Brasilia", "E. South America Standard Time"},
            {"(GMT-03:00) Buenos Aires, Georgetown", "S.A. Eastern Standard Time"},
            {"(GMT-03:00) Greenland", "Greenland Standard Time"},
            {"(GMT-02:00) Mid-Atlantic", "Mid-Atlantic Standard Time"},
            {"(GMT-01:00) Azores", "Azores Standard Time"},
            {"(GMT-01:00) Cape Verde Islands", "Cape Verde Standard Time"},
            {"(GMT) Greenwich Mean Time: Dublin, Edinburgh, Lisbon, London", "GMT Standard Time"},
            {"(GMT) Casablanca, Monrovia", "Greenwich Standard Time"},
            {"(GMT+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague", "Central Europe Standard Time"},
            {"(GMT+01:00) Sarajevo, Skopje, Warsaw, Zagreb", "Central European Standard Time"},
            {"(GMT+01:00) Brussels, Copenhagen, Madrid, Paris", "Romance Standard Time"},
            {"(GMT+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna", "W. Europe Standard Time"},
            {"(GMT+01:00) West Central Africa", "W. Central Africa Standard Time"},
            {"(GMT+02:00) Bucharest", "E. Europe Standard Time"},
            {"(GMT+02:00) Cairo", "Egypt Standard Time"},
            {"(GMT+02:00) Helsinki, Kiev, Riga, Sofia, Tallinn, Vilnius", "FLE Standard Time"},
            {"(GMT+02:00) Athens, Istanbul, Minsk", "GTB Standard Time"},
            {"(GMT+02:00) Jerusalem", "Israel Standard Time"},
            {"(GMT+02:00) Harare, Pretoria", "South Africa Standard Time"},
            {"(GMT+03:00) Moscow, St. Petersburg, Volgograd", "Russian Standard Time"},
            {"(GMT+03:00) Kuwait, Riyadh", "Arab Standard Time"},
            {"(GMT+03:00) Nairobi", "E. Africa Standard Time"},
            {"(GMT+03:00) Baghdad", "Arabic Standard Time"},
            {"(GMT+03:30) Tehran", "Iran Standard Time"},
            {"(GMT+04:00) Abu Dhabi, Muscat", "Arabian Standard Time"},
            {"(GMT+04:00) Baku, Tbilisi, Yerevan", "Caucasus Standard Time"},
            {"(GMT+04:30) Kabul", "Transitional Islamic State of Afghanistan Standard Time"},
            {"(GMT+05:00) Ekaterinburg", "Ekaterinburg Standard Time"},
            {"(GMT+05:00) Islamabad, Karachi, Tashkent", "West Asia Standard Time"},
            {"(GMT+05:30) Chennai, Kolkata, Mumbai, New Delhi", "India Standard Time"},
            {"(GMT+05:45) Kathmandu", "Nepal Standard Time"},
            {"(GMT+06:00) Astana, Dhaka", "Central Asia Standard Time"},
            {"(GMT+06:00) Sri Jayawardenepura", "Sri Lanka Standard Time"},
            {"(GMT+06:00) Almaty, Novosibirsk", "N. Central Asia Standard Time"},
            {"(GMT+06:30) Yangon Rangoon", "Myanmar Standard Time"},
            {"(GMT+07:00) Bangkok, Hanoi, Jakarta", "S.E. Asia Standard Time"},
            {"(GMT+07:00) Krasnoyarsk", "North Asia Standard Time"},
            {"(GMT+08:00) Beijing, Chongqing, Hong Kong SAR, Urumqi", "China Standard Time"},
            {"(GMT+08:00) Kuala Lumpur, Singapore", "Singapore Standard Time"},
            {"(GMT+08:00) Taipei", "Taipei Standard Time"},
            {"(GMT+08:00) Perth", "W. Australia Standard Time"},
            {"(GMT+08:00) Irkutsk, Ulaanbaatar", "North Asia East Standard Time"},
            {"(GMT+09:00) Seoul", "Korea Standard Time"},
            {"(GMT+09:00) Osaka, Sapporo, Tokyo", "Tokyo Standard Time"},
            {"(GMT+09:00) Yakutsk", "Yakutsk Standard Time"},
            {"(GMT+09:30) Darwin", "A.U.S. Central Standard Time"},
            {"(GMT+09:30) Adelaide", "Cen. Australia Standard Time"},
            {"(GMT+10:00) Canberra, Melbourne, Sydney", "A.U.S. Eastern Standard Time"},
            {"(GMT+10:00) Brisbane", "E. Australia Standard Time"},
            {"(GMT+10:00) Hobart", "Tasmania Standard Time"},
            {"(GMT+10:00) Vladivostok", "Vladivostok Standard Time"},
            {"(GMT+10:00) Guam, Port Moresby", "West Pacific Standard Time"},
            {"(GMT+11:00) Magadan, Solomon Islands, New Caledonia", "Central Pacific Standard Time"},
            {"(GMT+12:00) Fiji Islands, Kamchatka, Marshall Islands", "Fiji Islands Standard Time"},
            {"(GMT+12:00) Auckland, Wellington", "New Zealand Standard Time"},
            {"(GMT+13:00) Nuku'alofa", "Tonga Standard Time"}
        };

        public static Dictionary<string, string> Languages = new Dictionary<string, string>
        {
            {"Catalan (Spain)", "ca-ES"},
            {"Danish (Denmark)", "da-DK"},
            {"German (Germany)", "de-DE"},
            {"English (Australia)", "en-AU"},
            {"English (Canada)", "en-CA"},
            {"English (United Kingdom)", "en-GB"},
            {"English (India)", "en-IN"},
            {"English (United States)", "en-US"},
            {"Spanish (Spain)", "es-ES"},
            {"Spanish (Mexico)", "es-MX"},
            {"Finnish (Finland)", "fi-FI"},
            {"French (Canada)", "fr-CA"},
            {"French (France)", "fr-FR"},
            {"Italian (Italy)", "it-IT"},
            {"Japanese (Japan)", "ja-JP"},
            {"Korean (Korea)", "ko-KR"},
            {"Norwegian, Bokmal (Norway)", "nb-NO"},
            {"Dutch (Netherlands)", "nl-NL"},
            {"Polish (Poland)", "pl-PL"},
            {"Portuguese (Brazil)", "pt-BR"},
            {"Portuguese (Portugal)", "pt-PT"},
            {"Russian (Russia)", "ru-RU"},
            {"Swedish (Sweden)", "sv-SE"},
            {"Chinese (People’s Republic of China)", "zh-CN"},
            {"Chinese (Hong Kong SAR)", "zh-HK"},
            {"Chinese (Taiwan)", "zh-TW"}
        };


        public static Dictionary<string, string> WfDestinations = new Dictionary<string, string>
        {
            {"User", "TransferToUri"},
            {"Voicemail", "TransferToVoicemailUri"},
            {"Number", "TransferToPstn"},
            {"Disconnect", "Terminate"}

        };

        public static Dictionary<string, string> GrpRoutingMethods = new Dictionary<string, string>
        {
            {"Longest Idle", "LongestIdle"},
            {"Parallel", "Parallel"},
            {"Round Robin", "RoundRobin"},
            {"Serial", "Serial"},
            {"Attendant", "Attendant"}
        };


        public static Dictionary<string, string> DaysOfWeek = new Dictionary<string, string>
        {
            {"Mon - Fri", "MonFri"},
            {"Mon - Sat", "MonSat"},
            {"Mon - Sun", "MonSun"},
            {"Monday", "Monday"},
            {"Tuesday", "Tuesday"},
            {"Wednesday", "Wednesday"},
            {"Thursday", "Thursday"},
            {"Friday", "Friday"},
            {"Saturday", "Saturday"},
            {"Sunday", "Sunday"}
        };

        public static Dictionary<string, string> IvrDtmfKeys = new Dictionary<string, string>
        {
            {"1", "1"},
            {"2", "2"},
            {"3", "3"},
            {"4", "4"},
            {"5", "5"},
            {"6", "6"},
            {"7", "7"},
            {"8", "8"},
            {"9", "9"},
            {"0", "0"}
        };

        public static Dictionary<string, string> TimeHours = new Dictionary<string, string>
        {
            {"00", "00"},
            {"01", "01"},
            {"02", "02"},
            {"03", "03"},
            {"04", "04"},
            {"05", "05"},
            {"06", "06"},
            {"07", "07"},
            {"08", "08"},
            {"09", "09"},
            {"10", "10"},
            {"11", "11"},
            {"12", "12"},
            {"13", "13"},
            {"14", "14"},
            {"15", "15"},
            {"16", "16"},
            {"17", "17"},
            {"18", "18"},
            {"19", "19"},
            {"20", "20"},
            {"21", "21"},
            {"22", "22"},
            {"23", "23"},

        };

        public static Dictionary<string, string> TimeMinutes = new Dictionary<string, string>
        {
            {"00", "00"},
            {"01", "01"},
            {"02", "02"},
            {"03", "03"},
            {"04", "04"},
            {"05", "05"},
            {"06", "06"},
            {"07", "07"},
            {"08", "08"},
            {"09", "09"},
            {"10", "10"},
            {"11", "11"},
            {"12", "12"},
            {"13", "13"},
            {"14", "14"},
            {"15", "15"},
            {"16", "16"},
            {"17", "17"},
            {"18", "18"},
            {"19", "19"},
            {"20", "20"},
            {"21", "21"},
            {"22", "22"},
            {"23", "23"},
            {"24", "24"},
            {"25", "25"},
            {"26", "26"},
            {"27", "27"},
            {"28", "28"},
            {"29", "29"},
            {"30", "30"},
            {"31", "31"},
            {"32", "32"},
            {"33", "33"},
            {"34", "34"},
            {"35", "35"},
            {"36", "36"},
            {"37", "37"},
            {"38", "38"},
            {"39", "39"},
            {"40", "40"},
            {"41", "41"},
            {"42", "42"},
            {"43", "43"},
            {"44", "44"},
            {"45", "45"},
            {"46", "46"},
            {"47", "47"},
            {"48", "48"},
            {"49", "49"},
            {"50", "50"},
            {"51", "51"},
            {"52", "52"},
            {"53", "53"},
            {"54", "54"},
            {"55", "55"},
            {"56", "56"},
            {"57", "57"},
            {"58", "58"},
            {"59", "59"}

        };

        public static List<string> BusHoursDays(string days)
        {
            List<string> arrayOfDays = new List<string>();

            switch (days)
            {
                case "MonFri":
                    arrayOfDays.Add("Monday");
                    arrayOfDays.Add("Tuesday");
                    arrayOfDays.Add("Wednesday");
                    arrayOfDays.Add("Thursday");
                    arrayOfDays.Add("Frinday");
                    return arrayOfDays;
                case "MonSat":
                    arrayOfDays.Add("Monday");
                    arrayOfDays.Add("Tuesday");
                    arrayOfDays.Add("Wednesday");
                    arrayOfDays.Add("Thursday");
                    arrayOfDays.Add("Friday");
                    arrayOfDays.Add("Saturday");
                    return arrayOfDays;
                case "MonSun":
                    arrayOfDays.Add("Monday");
                    arrayOfDays.Add("Tuesday");
                    arrayOfDays.Add("Wednesday");
                    arrayOfDays.Add("Thursday");
                    arrayOfDays.Add("Friday");
                    arrayOfDays.Add("Saturday");
                    arrayOfDays.Add("Sunday");
                    return arrayOfDays;
                case "Monday":
                    arrayOfDays.Add("Monday");
                    return arrayOfDays;
                case "Tuesday":
                    arrayOfDays.Add("Tuesday");
                    return arrayOfDays;
                case "Wednesday":
                    arrayOfDays.Add("Wednesday");
                    return arrayOfDays;
                case "Thursday":
                    arrayOfDays.Add("Thursday");
                    return arrayOfDays;
                case "Friday":
                    arrayOfDays.Add("Friday");
                    return arrayOfDays;
                case "Saturday":
                    arrayOfDays.Add("Saturday");
                    return arrayOfDays;
                case "Sunday":
                    arrayOfDays.Add("Sunday");
                    return arrayOfDays;
                default:
                    return null;
            }
        }
    }
}
