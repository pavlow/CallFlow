using CallFlowManager.UI;

namespace CallFlowManager.UI.Business
{
    public class GroupsServiceBeforeTidyUp
    {
        private readonly PsQueries _dataRetriever;

        public GroupsServiceBeforeTidyUp(PsQueries dataRetriever)
        {
            _dataRetriever = dataRetriever;
        }

        public void Load()
        {
            _dataRetriever.GetCsUsers();
            //Tidy up removed this, re-enable for role back
            //_dataRetriever.UpdateUsersList();
            _dataRetriever.GetCsGroups();
        }
    }
}
