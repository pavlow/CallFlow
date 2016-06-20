using System;

namespace CallFlowManager.UI.Business
{
    public class DataServiceFactory
    {
        private static readonly Lazy<IDataService> DataService = new Lazy<IDataService>(() => new PowerShellDataService());

        public static IDataService Get()
        {
            return DataService.Value;
        }
    }
}