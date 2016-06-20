using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using CallFlowManager.UI.Models;
using CallFlowManager.UI.ViewModels.Queues;

namespace CallFlowManager.UI.Business
{
    public class HolidayService
    {
        //private PsQueries _dataRetriever;
        private ICollection<PSObject> _csHolidays;
        public IList<CsHolidayGroup> HolidayGroups { get; private set; }
        public IList<CsHoliday> Holidays { get; private set; }

        public HolidayService(PsQueries dataRetriever)
        {
            //_dataRetriever = dataRetriever;
            HolidayGroups = new List<CsHolidayGroup>();
            Holidays = new List<CsHoliday>();
        }

        public void ProcessPsHolidays(ICollection<PSObject> csHolidays)
        {
            _csHolidays = csHolidays;
            HolidayGroups.Clear();

            //_dataRetriever.GetCsHolidays();

            if (_csHolidays != null)
            {
                foreach (dynamic holidayGrp in _csHolidays)
                {
                    if (holidayGrp != null)
                    {
                        var hol = new List<CsHoliday>();
                        foreach (dynamic holidayList in holidayGrp.HolidayList)
                        {
                            if (holidayList != null)
                            {
                                if (holidayGrp.Name != null && holidayList.Name != null && holidayList.StartDate != null &&
                                    holidayList.EndDate != null)
                                {
                                    hol.Add(new CsHoliday(holidayGrp.Name.ToString(), holidayList.Name.ToString(),
                                        holidayList.StartDate.ToString(), holidayList.EndDate.ToString()));
                                }
                            }
                        }

                        var newHolGroup = new CsHolidayGroup(holidayGrp.Name, holidayGrp.OwnerPool.ToString(), holidayGrp.Identity.InstanceId.ToString(), hol);
                        //newHolGroup.Holidays = hol;
                        HolidayGroups.Add(newHolGroup);
                    }
                }
            }
        }


        //private void HolGroupAdd(string name, string ownerPool, string identity)
        //{
        //    var match = HolidayGroups.Where(p => (p.Name == name));
        //    var matchCount = match.Count();

        //    //if (matchCount == 0)
        //    //{
        //        var newHolGroup = new CsHolidayGroup(name, ownerPool, identity);
        //        newHolGroup.Holidays = new List<CsHoliday>();
        //        LoadHolidays(newHolGroup);
        //        HolidayGroups.Add(newHolGroup);
        //    //}
        //    //else if (matchCount > 1)
        //    //{
        //        //MessageBox.Show("Error determining if item already exisits - more than 1 result returned");
        //    //}
        //    //else if (matchCount == 1)
        //    //{
        //        //MessageBox.Show("Already exists");
        //    //}
        //}

        //private void LoadHolidays(CsHolidayGroup group)
        //{
        //    var holFilter =
        //        _csHolidays.Where(
        //                    x => x.Members["Name"].Value.ToString() == group.Name)
        //                    .ToList();

        //    if (holFilter != null)
        //    {
        //        foreach (dynamic hol in holFilter)
        //        {
        //            if (hol != null)
        //            {
        //                //TO BE COMPLETED
        //                if (hol.HolidayList != null)
        //                {
        //                    foreach (var holList in hol.HolidayList)
        //                    {
        //                        if (holList.Name != null && holList.StartDate != null &&
        //                            holList.EndDate != null)
        //                        {
        //                            group.Holidays.Add(new CsHoliday(group.Name.ToString(), holList.Name.ToString(), holList.StartDate.ToString(), holList.EndDate.ToString()));
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //private void HolAdd(string parent, string name, string startDate, string endDate)
        //{
        //    var match = Holidays.Where(p => p.Parent == parent && p.Name == name);

        //    var matchCount = match.Count();

        //    if (matchCount == 0)
        //    {
        //        Holidays.Add(new CsHoliday(parent, name, startDate, endDate));
        //        //holBindingSource.ResetBindings(false);
        //    }
        //    else if (matchCount > 1)
        //    {
        //        //MessageBox.Show("Error determining if item already exisits - more than 1 result returned");
        //    }
        //    else if (matchCount == 1)
        //    {
        //        //ISSUE WITH LOGIC HERE - MESSAGE BOX KEEPS POPPING!
        //        //MessageBox.Show("Already exists");
        //    }
        //}
    }
}

//Filter out Business Hours that contain GUID - these are system created for customised business hours on a queue by queue basis
//Regex regex = new Regex(regexBusHoursGroupNames);
//Match match = regex.Match(bhour.Name);

//Those that dont contain a GUID are deemed as user created
//if (!match.Success)
//{
//HolGroupAdd(hol.Name, hol.OwnerPool.ToString(), hol.Identity.InstanceId.ToString());

//HolAddFromSfB(hol.Name);
//metroGridBusHoursGroups.Rows.Add();
//metroGridBusHoursGroups.Rows[i].Cells["dataGridViewBusHoursGroups_Name"].Value = bhour.Name;
//i++;
//}

