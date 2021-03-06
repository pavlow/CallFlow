﻿using System.Collections.Generic;
using System.Linq;
using CallFlowManager.UI.Models;

namespace CallFlowManager.UI.Business
{
    public class HolidayServiceBeforePsTidyUp
    {
        private PsQueries _dataRetriever;
        public IList<CsHolidayGroup> HolidayGroups { get; private set; }
        public IList<CsHoliday> Holidays { get; private set; }

        public HolidayServiceBeforePsTidyUp(PsQueries dataRetriever)
        {
            _dataRetriever = dataRetriever;
            HolidayGroups = new List<CsHolidayGroup>();
            Holidays = new List<CsHoliday>();
        }

        public void LoadHolidaysInternal()
        {
            _dataRetriever.GetCsHolidays();

            if (_dataRetriever.Holidays != null)
            {
                foreach (dynamic hol in _dataRetriever.Holidays)
                {
                    if (hol != null)
                    {
                        //Filter out Business Hours that contain GUID - these are system created for customised business hours on a queue by queue basis
                        //Regex regex = new Regex(regexBusHoursGroupNames);
                        //Match match = regex.Match(bhour.Name);

                        //Those that dont contain a GUID are deemed as user created
                        //if (!match.Success)
                        //{
                        HolGroupAdd(hol.Name, hol.OwnerPool.ToString(), hol.Identity.ToString());

                        //HolAddFromSfB(hol.Name);
                        //metroGridBusHoursGroups.Rows.Add();
                        //metroGridBusHoursGroups.Rows[i].Cells["dataGridViewBusHoursGroups_Name"].Value = bhour.Name;
                        //i++;
                        //}

                    }
                }
            }
        }

        private void HolGroupAdd(string name, string ownerPool, string identity)
        {
            var match = HolidayGroups.Where(p => (p.Name == name));
            var matchCount = match.Count();

            if (matchCount == 0)
            {
                var newHolGroup = new CsHolidayGroup(name, ownerPool, identity, null);
                newHolGroup.Holidays = new List<CsHoliday>();
                LoadHolidays(newHolGroup);
                HolidayGroups.Add(newHolGroup);
            }
            else if (matchCount > 1)
            {
                //MessageBox.Show("Error determining if item already exisits - more than 1 result returned");
            }
            else if (matchCount == 1)
            {
                //MessageBox.Show("Already exists");
            }
        }

        private void LoadHolidays(CsHolidayGroup group)
        {
            var holFilter =
                _dataRetriever.Holidays.Where(
                            x => x.Members["Name"].Value.ToString() == group.Name)
                            .ToList();

            if (holFilter != null)
            {
                foreach (dynamic hol in holFilter)
                {
                    if (hol != null)
                    {
                        //TO BE COMPLETED
                        if (hol.HolidayList != null)
                        {
                            foreach (var holList in hol.HolidayList)
                            {
                                if (holList.Name != null && holList.StartDate != null &&
                                    holList.EndDate != null)
                                {
                                    group.Holidays.Add(new CsHoliday(group.Name.ToString(), holList.Name.ToString(), holList.StartDate.ToString(), holList.EndDate.ToString()));
                                }
                            }
                        }
                    }
                }
            }
        }

        private void HolAdd(string parent, string name, string startDate, string endDate)
        {
            var match = Holidays.Where(p => p.Parent == parent && p.Name == name);

            var matchCount = match.Count();

            if (matchCount == 0)
            {
                Holidays.Add(new CsHoliday(parent, name, startDate, endDate));
                //holBindingSource.ResetBindings(false);
            }
            else if (matchCount > 1)
            {
                //MessageBox.Show("Error determining if item already exisits - more than 1 result returned");
            }
            else if (matchCount == 1)
            {
                //ISSUE WITH LOGIC HERE - MESSAGE BOX KEEPS POPPING!
                //MessageBox.Show("Already exists");
            }
        }
    }
}
