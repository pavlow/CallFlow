using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CallFlowManagerv2
{
    /// <summary>
    /// DataGridView Binding soruce control
    /// </summary>
    class BsControl
    {
        /// <summary>
        /// Move selected grid item up
        /// http://stackoverflow.com/questions/1012708/datagridview-selected-row-move-up-and-down
        /// </summary>
        /// <param name="bs"></param>
        public static void Up(BindingSource bs)
        {
            int position = bs.Position;
            if (position == 0) return;  // already at top

            if (bs.Count > 0)
            {
                bs.RaiseListChangedEvents = false;

                //CsQAgentGroups current = (CsQAgentGroups)agentGroupsBindingSource.Current;
                var current = bs.Current;
                bs.Remove(current);

                position--;

                bs.Insert(position, current);
                bs.Position = position;

                bs.RaiseListChangedEvents = true;
            }
        }

        /// <summary>
        /// Move selected grid item down
        /// http://stackoverflow.com/questions/1012708/datagridview-selected-row-move-up-and-down
        /// </summary>
        /// <param name="bs"></param>
        public static void Down(BindingSource bs)
        {
            int position = bs.Position;
            if (position == bs.Count - 1) return;  // already at bottom

            if (bs.Count > 0)
            {
                bs.RaiseListChangedEvents = false;

                var current = bs.Current;
                bs.Remove(current);

                position++;

                bs.Insert(position, current);
                bs.Position = position;

                bs.RaiseListChangedEvents = true;
                bs.ResetBindings(false);
            }
        }

        /// <summary>
        /// Remove selected row
        /// http://stackoverflow.com/questions/867349/c-sharp-best-way-to-delete-a-row-from-typed-dataset-bounded-by-binding-source
        /// </summary>
        /// <param name="bs"></param>
        public static void RemoveSelected(BindingSource bs)
        {
            if (bs.Count > 0)
            bs.RemoveCurrent();
        }
    }
}
