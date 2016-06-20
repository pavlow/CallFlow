using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using CallFlowManager.UI.Models;

namespace CallFlowManager.UI.Business
{
    public class BusinessHoursServiceBeforePsTidy
    {
        //Hold local copy PsQueries
        private PsQueries _dataRetriever;

        public IList<CsBusHoursGroup> BusinessHoursGroups { get; private set; }
        public IList<CsBusHours> BusinessHours { get; private set; }

        public BusinessHoursServiceBeforePsTidy(PsQueries dataRetriever)
        {
            _dataRetriever = dataRetriever;
            BusinessHoursGroups = new List<CsBusHoursGroup>();
            BusinessHours = new List<CsBusHours>();
        }

        /// <summary>
        /// Processes Business Hours retrieved from PowerShell
        /// </summary>
        public void ProcessPsBusinessHours()
        {
            string regexBusHoursGroupNames = @"_\w{8}-\w{4}-\w{4}-\w{4}-\w{12}";
            //int i = 0;

            _dataRetriever.GetCsBusHours();
            //_dataRetriever.UpdateBusinessHoursGroups();

            if (_dataRetriever.BusHours != null && _dataRetriever.BusinessHoursGroups != null)
            {
                foreach (dynamic bhour in _dataRetriever.BusHours)
                {
                    if (bhour != null)
                    {
                        //Filter out Business Hours that contain GUID - these are system created for customised business hours on a queue by queue basis
                        Regex regex = new Regex(regexBusHoursGroupNames);
                        Match match = regex.Match(bhour.Name);

                        //Those that dont contain a GUID are deemed as user created
                        if (!match.Success)
                        {
                            if (bhour.Name != null)
                            {
                                BusHoursGroupAdd(bhour.Name, bhour.OwnerPool.ToString(), bhour.Identity.ToString());
                                BusHoursAddFromSfB(bhour.Name);
                            }

                            //MessageBox.Show(bhoursName.Name);
                            //metroGridBusHoursGroups.Rows.Add();
                            //metroGridBusHoursGroups.Rows[i].Cells["dataGridViewBusHoursGroups_Name"].Value = bhour.Name;
                            //i++;
                        }

                    }
                }
                foreach (var businessHour in BusinessHours)
                {
                    var csBusHoursGroup = BusinessHoursGroups.FirstOrDefault(p => p.Name.Equals(businessHour.Parent));
                    if (csBusHoursGroup != null)
                    {
                        if (csBusHoursGroup.BusinessHours == null)
                        {
                            csBusHoursGroup.BusinessHours = new List<CsBusHours>();
                        }
                    }
                    csBusHoursGroup.BusinessHours.Add(businessHour);
                }
            }


        }

        private void BusHoursGroupAdd(string name, string ownerPool, string identity)
        {
            var match = BusinessHoursGroups.Where(p => (p.Name == name));
            var matchCount = match.Count();

            if (matchCount == 0)
            {
                BusinessHoursGroups.Add(new CsBusHoursGroup(name, ownerPool, identity));
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

        private void BusHoursAddFromSfB(string groupName)
        {
            var busHoursFilter =
                        _dataRetriever.BusHours.Where(
                            x => x.Members["Name"].Value.ToString() == groupName)
                            .ToList();

            if (busHoursFilter != null)
            {
                foreach (dynamic bhour in busHoursFilter)
                {
                    if (bhour != null)
                    {
                        //Monday 1 only
                        if (bhour.MondayHours1 != null && bhour.MondayHours2 == null)
                        {
                            if (bhour.MondayHours1.OpenTime != null && bhour.MondayHours1.CloseTime != null)
                            {
                                BusHoursAdd(groupName, "Monday", bhour.MondayHours1.OpenTime.ToString(), bhour.MondayHours1.CloseTime.ToString(), "", "");
                            }
                        }
                        //Monday 2 only
                        else if (bhour.MondayHours1 == null && bhour.MondayHours2 != null)
                        {
                            if (bhour.MondayHours2.OpenTime != null && bhour.MondayHours2.CloseTime != null)
                            {
                                BusHoursAdd(groupName, "Monday", "", "", bhour.MondayHours2.OpenTime.ToString(), bhour.MondayHours2.CloseTime.ToString());
                            }
                        }
                        //Monday 1 and 2
                        else if (bhour.MondayHours1 != null && bhour.MondayHours2 != null)
                        {
                            if (bhour.MondayHours1.OpenTime != null && bhour.MondayHours1.CloseTime != null && bhour.MondayHours2.OpenTime != null && bhour.MondayHours2.CloseTime != null)
                            {
                                BusHoursAdd(groupName, "Monday", bhour.MondayHours1.OpenTime.ToString(), bhour.MondayHours1.CloseTime.ToString(), bhour.MondayHours2.OpenTime.ToString(), bhour.MondayHours2.CloseTime.ToString());
                            }
                        }


                        //Tuesday 1 only
                        if (bhour.TuesdayHours1 != null && bhour.TuesdayHours2 == null)
                        {
                            if (bhour.TuesdayHours1.OpenTime != null && bhour.TuesdayHours1.CloseTime != null)
                            {
                                BusHoursAdd(groupName, "Tuesday", bhour.TuesdayHours1.OpenTime.ToString(), bhour.TuesdayHours1.CloseTime.ToString(), "", "");
                            }
                        }
                        //Tuesday 2 only
                        else if (bhour.TuesdayHours1 == null && bhour.TuesdayHours2 != null)
                        {
                            if (bhour.TuesdayHours2.OpenTime != null && bhour.TuesdayHours2.CloseTime != null)
                            {
                                BusHoursAdd(groupName, "Tuesday", "", "", bhour.TuesdayHours2.OpenTime.ToString(), bhour.TuesdayHours2.CloseTime.ToString());
                            }
                        }
                        //Tuesday 1 and 2
                        else if (bhour.TuesdayHours1 != null && bhour.TuesdayHours2 != null)
                        {
                            if (bhour.TuesdayHours1.OpenTime != null && bhour.TuesdayHours1.CloseTime != null && bhour.TuesdayHours2.OpenTime != null && bhour.TuesdayHours2.CloseTime != null)
                            {
                                BusHoursAdd(groupName, "Tuesday", bhour.TuesdayHours1.OpenTime.ToString(), bhour.TuesdayHours1.CloseTime.ToString(), bhour.TuesdayHours2.OpenTime.ToString(), bhour.TuesdayHours2.CloseTime.ToString());
                            }
                        }




                        //Wednesday 1 only
                        if (bhour.WednesdayHours1 != null && bhour.WednesdayHours2 == null)
                        {
                            if (bhour.WednesdayHours1.OpenTime != null && bhour.WednesdayHours1.CloseTime != null)
                            {
                                BusHoursAdd(groupName, "Wednesday", bhour.WednesdayHours1.OpenTime.ToString(), bhour.WednesdayHours1.CloseTime.ToString(), "", "");
                            }
                        }
                        //Wednesday 2 only
                        else if (bhour.WednesdayHours1 == null && bhour.WednesdayHours2 != null)
                        {
                            if (bhour.WednesdayHours2.OpenTime != null && bhour.WednesdayHours2.CloseTime != null)
                            {
                                BusHoursAdd(groupName, "Wednesday", "", "", bhour.WednesdayHours2.OpenTime.ToString(), bhour.WednesdayHours2.CloseTime.ToString());
                            }
                        }
                        //Wednesday 1 and 2
                        else if (bhour.WednesdayHours1 != null && bhour.WednesdayHours2 != null)
                        {
                            if (bhour.WednesdayHours1.OpenTime != null && bhour.WednesdayHours1.CloseTime != null && bhour.WednesdayHours2.OpenTime != null && bhour.WednesdayHours2.CloseTime != null)
                            {
                                BusHoursAdd(groupName, "Wednesday", bhour.WednesdayHours1.OpenTime.ToString(), bhour.WednesdayHours1.CloseTime.ToString(), bhour.WednesdayHours2.OpenTime.ToString(), bhour.WednesdayHours2.CloseTime.ToString());
                            }
                        }




                        //Thursday 1 only
                        if (bhour.ThursdayHours1 != null && bhour.ThursdayHours2 == null)
                        {
                            if (bhour.ThursdayHours1.OpenTime != null && bhour.ThursdayHours1.CloseTime != null)
                            {
                                BusHoursAdd(groupName, "Thursday", bhour.ThursdayHours1.OpenTime.ToString(), bhour.ThursdayHours1.CloseTime.ToString(), "", "");
                            }
                        }
                        //Thursday 2 only
                        else if (bhour.ThursdayHours1 == null && bhour.ThursdayHours2 != null)
                        {
                            if (bhour.ThursdayHours2.OpenTime != null && bhour.ThursdayHours2.CloseTime != null)
                            {
                                BusHoursAdd(groupName, "Thursday", "", "", bhour.ThursdayHours2.OpenTime.ToString(), bhour.ThursdayHours2.CloseTime.ToString());
                            }
                        }
                        //Thursday 1 and 2
                        else if (bhour.ThursdayHours1 != null && bhour.ThursdayHours2 != null)
                        {
                            if (bhour.ThursdayHours1.OpenTime != null && bhour.ThursdayHours1.CloseTime != null && bhour.ThursdayHours2.OpenTime != null && bhour.ThursdayHours2.CloseTime != null)
                            {
                                BusHoursAdd(groupName, "Thursday", bhour.ThursdayHours1.OpenTime.ToString(), bhour.ThursdayHours1.CloseTime.ToString(), bhour.ThursdayHours2.OpenTime.ToString(), bhour.ThursdayHours2.CloseTime.ToString());
                            }
                        }



                        //Friday 1 only
                        if (bhour.FridayHours1 != null && bhour.FridayHours2 == null)
                        {
                            if (bhour.FridayHours1.OpenTime != null && bhour.FridayHours1.CloseTime != null)
                            {
                                BusHoursAdd(groupName, "Friday", bhour.FridayHours1.OpenTime.ToString(), bhour.FridayHours1.CloseTime.ToString(), "", "");
                            }
                        }
                        //Friday 2 only
                        else if (bhour.FridayHours1 == null && bhour.FridayHours2 != null)
                        {
                            if (bhour.FridayHours2.OpenTime != null && bhour.FridayHours2.CloseTime != null)
                            {
                                BusHoursAdd(groupName, "Friday", "", "", bhour.FridayHours2.OpenTime.ToString(), bhour.FridayHours2.CloseTime.ToString());
                            }
                        }
                        //Friday 1 and 2
                        else if (bhour.FridayHours1 != null && bhour.FridayHours2 != null)
                        {
                            if (bhour.FridayHours1.OpenTime != null && bhour.FridayHours1.CloseTime != null && bhour.FridayHours2.OpenTime != null && bhour.FridayHours2.CloseTime != null)
                            {
                                BusHoursAdd(groupName, "Friday", bhour.FridayHours1.OpenTime.ToString(), bhour.FridayHours1.CloseTime.ToString(), bhour.FridayHours2.OpenTime.ToString(), bhour.FridayHours2.CloseTime.ToString());
                            }
                        }



                        //Saturday 1 only
                        if (bhour.SaturdayHours1 != null && bhour.SaturdayHours2 == null)
                        {
                            if (bhour.SaturdayHours1.OpenTime != null && bhour.SaturdayHours1.CloseTime != null)
                            {
                                BusHoursAdd(groupName, "Saturday", bhour.SaturdayHours1.OpenTime.ToString(), bhour.SaturdayHours1.CloseTime.ToString(), "", "");
                            }
                        }
                        //Saturday 2 only
                        else if (bhour.SaturdayHours1 == null && bhour.SaturdayHours2 != null)
                        {
                            if (bhour.SaturdayHours2.OpenTime != null && bhour.SaturdayHours2.CloseTime != null)
                            {
                                BusHoursAdd(groupName, "Saturday", "", "", bhour.SaturdayHours2.OpenTime.ToString(), bhour.SaturdayHours2.CloseTime.ToString());
                            }
                        }
                        //Saturday 1 and 2
                        else if (bhour.SaturdayHours1 != null && bhour.SaturdayHours2 != null)
                        {
                            if (bhour.SaturdayHours1.OpenTime != null && bhour.SaturdayHours1.CloseTime != null && bhour.SaturdayHours2.OpenTime != null && bhour.SaturdayHours2.CloseTime != null)
                            {
                                BusHoursAdd(groupName, "Saturday", bhour.SaturdayHours1.OpenTime.ToString(), bhour.SaturdayHours1.CloseTime.ToString(), bhour.SaturdayHours2.OpenTime.ToString(), bhour.SaturdayHours2.CloseTime.ToString());
                            }
                        }



                        //Sunday 1 only
                        if (bhour.SundayHours1 != null && bhour.SundayHours2 == null)
                        {
                            if (bhour.SundayHours1.OpenTime != null && bhour.SundayHours1.CloseTime != null)
                            {
                                BusHoursAdd(groupName, "Sunday", bhour.SundayHours1.OpenTime.ToString(), bhour.SundayHours1.CloseTime.ToString(), "", "");
                            }
                        }
                        //Sunday 2 only
                        else if (bhour.SundayHours1 == null && bhour.SundayHours2 != null)
                        {
                            if (bhour.SundayHours2.OpenTime != null && bhour.SundayHours2.CloseTime != null)
                            {
                                BusHoursAdd(groupName, "Sunday", "", "", bhour.SundayHours2.OpenTime.ToString(), bhour.SundayHours2.CloseTime.ToString());
                            }
                        }
                        //Sunday 1 and 2
                        else if (bhour.SundayHours1 != null && bhour.SundayHours2 != null)
                        {
                            if (bhour.SundayHours1.OpenTime != null && bhour.SundayHours1.CloseTime != null && bhour.SundayHours2.OpenTime != null && bhour.SundayHours2.CloseTime != null)
                            {
                                BusHoursAdd(groupName, "Sunday", bhour.SundayHours1.OpenTime.ToString(), bhour.SundayHours1.CloseTime.ToString(), bhour.SundayHours2.OpenTime.ToString(), bhour.SundayHours2.CloseTime.ToString());
                            }
                        }
                    }
                }
            }
        }

        private void BusHoursAdd(string parent, string day, string openTime1, string closeTime1, string openTime2, string closeTime2)
        {
            var match = BusinessHours.Where(p => (p.OpenTime1 == openTime1 && p.Day == day && p.Parent == parent)
                || (p.CloseTime1 == closeTime1 && p.Day == day && p.Parent == parent)
                || (p.OpenTime2 == openTime2 && p.Day == day && p.Parent == parent)
                || (p.CloseTime2 == closeTime2 && p.Day == day && p.Parent == parent));

            var matchCount = match.Count();

            if (matchCount == 0)
            {

                BusinessHours.Add(new CsBusHours(parent, day, openTime1, closeTime1, openTime2, closeTime2));
                //busHoursBindingSource.ResetBindings(false);
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
    }
}
