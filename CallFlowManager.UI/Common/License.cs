using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using CallFlowManager.UI.Models;
using CallFlowManager.UI.ViewModels.License;
using CallFlowManager.UI.Views;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;

namespace CallFlowManager.UI.Common
{
    public static class License
    {
        public const string DomainUrl = "https://theucguys.com";

        // This unique ID is used by the API Manager to find the right software
        public const string SoftwareTitleFull = "Call Flow Manager";

        public const string SoftwareTitleTrial = "Call Flow Manager - 30 day trial";

        public const string FileConfig = "Config.dat";

        public const int TrialPeriod = 30;

        public static string Instance;

        public static string CurrentVersion;
        public static string SoftwareVersion;

        static ConfigService config = ConfigService.Instance;

        public static bool IsValid { get; set; }

        public static bool Validate()
        {
            bool isValid = false;

            if (!CheckForInternetConnection())
            {
                MessageBox.Show("An internet connection is required to validate your licence. Please make sure that you can access https://theucguys.com, and try again.", "No Internet connection");
                return false;
            }

            Instance = SystemInfo.GetSystemInfo("CallFlow");

            config.Config = FileHelper.ReadConfig(FileConfig);

            if (Request("status"))
            {
                return true;
            }
            else
            {
                var licenseViewModel = new LicenseViewModel();
                var dialogResult = false;

                var window = new LicenseWindow
                {
                    DataContext = licenseViewModel
                };

                licenseViewModel.RequestCloseDialogEvent += (sender, args) =>
                {

                    window.Dispatcher.BeginInvoke((Action)window.Close);
                };

                licenseViewModel.DialogResultTrueEvent += (sender, args) => dialogResult = true;
                window.ShowDialog();

                if (dialogResult)
                {
                    isValid = true;
                }
            }

            return isValid;
        }

        public static void SaveConfig()
        {
            FileHelper.Serialize(config.Config, License.FileConfig);
        }

        /// <summary>
        /// Creates request to API for activation or to check of status (validation) 
        /// </summary>
        /// <param name="requestParameters"></param>
        /// <returns></returns>
        public static bool Request(string requestParameters)
        {
            var request = BuildRequest(requestParameters);

            var client = new RestClient(DomainUrl);
            var response = client.Execute(request);
            var isSuccess = ParseResponse(response);

            return isSuccess;
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead(DomainUrl))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private static bool ParseResponse(IRestResponse response)
        {
            JsonDeserializer deserializer = new JsonDeserializer();
            Dictionary<string, string> reponseBody = deserializer.Deserialize<Dictionary<string, string>>(response);

            var isSuccess = false;

            #region Set status and results

            // Set status
                if (reponseBody.ContainsKey("status_check"))
                {
                    if (reponseBody["status_check"].ToLower() == "inactive")
                    {
                        config.Config.IsActive = false;
                        SaveConfig();
                    }
                    else
                    {
                        if (reponseBody["status_check"].ToLower() == "active")
                        {
                            var statusExtra = reponseBody["status_extra"];
                            var statusExtraArray = JsonConvert.DeserializeObject<StatusExtra>(statusExtra);
                            //var softwareVersion = statusExtraArray.current_version;
                            //var x = softwareVersion;
                            if (statusExtraArray.product_id == SoftwareTitleFull)
                            {
                                isSuccess = true;
                            }
                            else
                            {
                                TimeSpan ts = statusExtraArray.activation_time.AddDays(TrialPeriod) - DateTime.Now;
                                int tsDays = ts.Days;
                                if (statusExtraArray.product_id == SoftwareTitleTrial && tsDays > 0)
                                {
                                    MessageBox.Show("The test period will end in " + tsDays + " days");
                                    isSuccess = true;
                                }
                                else
                                {
                                    MessageBox.Show("Trial period is over");
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (reponseBody.ContainsKey("activated"))
                    {
                        if (reponseBody["activated"].ToLower() == "true")
                        {
                            config.Config.IsActive = true;
                            isSuccess = true;
                            SaveConfig();
                        }
                    }
                    else
                    {
                        if (reponseBody.ContainsKey("deactivated"))
                        {
                            if (reponseBody["deactivated"].ToLower() == "true")
                            {
                                config.Config.IsActive = false;
                                isSuccess = true;
                                SaveConfig();
                            }
                        }
                    }
                }

            #endregion
            return isSuccess;
        }

        private static RestRequest BuildRequest(string requestParameters)
        {
            string productId = TextHelper.ConvertSpaceToPlus(config.Config.SoftwareTitle);

            var strRequest = String.Concat("?wc-api=am-software-api&request=", requestParameters, "&email=", config.Config.Email.Trim(), "&licence_key=", config.Config.LicenseKey.Trim(), "&product_id=", productId, "&instance=", Instance);//, "&software_version=", SoftwareVersion, "&current_version=", CurrentVersion);

            var request = new RestRequest(strRequest, Method.GET);
            request.RequestFormat = RestSharp.DataFormat.Json;
            return request;
        }
    }

    [JsonObject]
    public class StatusExtra
    {
        public string order_key;

        public string instance;

        public string product_id;

        public DateTime activation_time;

        public string activation_active;

        public string activation_domain;

        public string software_version;
        
        public string current_version;

        public string status_check;
    }
}
