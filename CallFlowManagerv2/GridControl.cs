using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;

namespace CallFlowManagerv2
{
    public class GridControl
    {
        /// <summary>
        /// Requires ObservableCollection rather thean List I think...
        ///  ObservableCollection<CsQAgentGroups> agentGroups = new ObservableCollection<CsQAgentGroups>();
        ///private BindingSource agentGroupsBindingSource;
        /// </summary>
        /// <param name="dg"></param>

        public static void Add(DataGridView dg)
        {
            int rowCount = dg.RowCount;

            //dg.Rows.Add();
            //dg.Rows[rowCount].Cells["metroGridQGrid_Name"].Value = agent.Name.ToString();
            //dg.Rows[i].Cells["metroGridQGrid_AlertTime"].Value = agent.AgentAlertTime.ToString();
            //dg.Rows[i].Cells["metroGridQGrid_ParticipationPolicy"].Value = agent.ParticipationPolicy.ToString();
            //dg.Rows[i].Cells["metroGridQGrid_RoutingMethod"].Value = agent.RoutingMethod.ToString();
            //i++;
        }

        public static void ClearGrid(DataGridView dg)
        {
            dg.Rows.Clear();
        }

        public static void Delete (DataGridView dg)
        {
            if (dg.SelectedRows.Count > 0 && !dg.SelectedRows[0].IsNewRow)
            {
                foreach (DataGridViewRow item in dg.SelectedRows)
                {
                    dg.Rows.RemoveAt(item.Index);
                }
            }
            else
            {
                //MetroMessageBox.Show(this, "Please select at least 1 Agent Group to delete.", "Error");
                //MetroMessageBox.Show(Form1, "...", "...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MessageBox.Show("Please select at least 1 Agent Group to delete.", "Error");
            }
        }

        public static void MoveRowUp(DataGridView dgv)
        {
            if (dgv.RowCount <= 0)
                return;

            if (dgv.SelectedRows.Count <= 0)
                return;

            var index = dgv.SelectedCells[0].OwningRow.Index;

            if (index == 0)
                return;

            var rows = dgv.Rows;
            var prevRow = rows[index - 1];
            rows.Remove(prevRow);
            prevRow.Frozen = false;
            rows.Insert(index, prevRow);
            dgv.ClearSelection();
            dgv.Rows[index - 1].Selected = true;
        }

        public static void MoveRowDown(DataGridView dgv)
        {
            if (dgv.RowCount <= 0)
                return;

            if (dgv.SelectedRows.Count <= 0)
                return;

            var rowCount = dgv.Rows.Count;
            var index = dgv.SelectedCells[0].OwningRow.Index;

            if (index == (rowCount - 1)) // include header: -2, dont include header -1
                return;

            var rows = dgv.Rows;
            var nextRow = rows[index + 1];
            rows.Remove(nextRow);
            nextRow.Frozen = false;
            rows.Insert(index, nextRow);
            dgv.ClearSelection();
            dgv.Rows[index + 1].Selected = true;
        }
    }
}
