using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Controls;

namespace CallFlowManagerv2
{
    public class IvrMenu
    {
        //public string Name;
        //public string Parent;
        public int OptionLevel = -1; //Level of menu 0 = root or 1 = sub1
        public int OptionPosition = -1; //The option position in the menu (0-9) - keeps track of added/removed options - kinda like an ID
        public int OptionParent = -1; //Option parent no paretn = -1, 0-9 = has parent
        
        //EVENT HANDLERS
        //Remove root level option
        public delegate void RootOptionRemove(int optionNumberLevel, int optionNumber);
        public event RootOptionRemove RootRemove;

        //Add sub option at level 1
        public delegate void SubOption1Add(int optionNumberLevel, int optionNumber, int optionNumberParent);
        public event SubOption1Add SubAdd1;

        //Remove sub option at level 1
        public delegate void SubOption1Remove(int optionNumberLevel, int optionNumber, int optionNumberParent);
        public event SubOption1Add SubRemove1;

        //Dynamic Controls
        public MetroLabel labelIvr1;
        public MetroComboBox comboboxIvrDtmf1;
        public MetroTextBox textboxIvrVoiceResponse1;
        public MetroComboBox comboboxIvrQueue1;
        public MetroButton buttonIvrAddSubOption1;
        public MetroButton buttonIvrRemoveSubOption1;

        public MetroLabel[] labelIvr2;
        public MetroComboBox[] comboboxIvrDtmf2;
        public MetroTextBox[] textboxIvrVoiceResponse2;
        public MetroComboBox[] comboboxIvrQueue2;
        public MetroButton[] buttonIvrAddSubOption2;
        public MetroButton[] buttonIvrRemoveSubOption2;

        private int countIvr1 = -1;
        private int countIvr2 = -1;
        private int maxRowIvr2 = 10;
        //private int maxRowIvr1 = 10;
        //private int emptyCountIvr1 = 0;
        //private int emptyCountIvr2 = 0;
        //private int countIvr00 = -1;
        //private int countIvr01 = -1;

        public IvrMenu()
        {
            //Initialise
            labelIvr1 = new MetroLabel();
            comboboxIvrDtmf1 = new MetroComboBox();
            textboxIvrVoiceResponse1 = new MetroTextBox();
            comboboxIvrQueue1 = new MetroComboBox();
            buttonIvrAddSubOption1 = new MetroButton();
            buttonIvrRemoveSubOption1 = new MetroButton();

            labelIvr2 = new MetroLabel[maxRowIvr2];
            comboboxIvrDtmf2 = new MetroComboBox[maxRowIvr2];
            textboxIvrVoiceResponse2 = new MetroTextBox[maxRowIvr2];
            comboboxIvrQueue2 = new MetroComboBox[maxRowIvr2];
            buttonIvrAddSubOption2 = new MetroButton[maxRowIvr2];
            buttonIvrRemoveSubOption2 = new MetroButton[maxRowIvr2];

            //CreateOption();
        }

        public void CreateOption()
        {
            labelIvr1 = new MetroLabel();
            labelIvr1.Name = "labelIvr_" + OptionParent + "_" + OptionPosition;
            labelIvr1.Text = "Option " + OptionPosition;
            labelIvr1.AutoSize = true;
            labelIvr1.Location = new Point(5, (40 * (countIvr1)) + 2);

            comboboxIvrDtmf1 = new MetroComboBox();
            comboboxIvrDtmf1.Name = "comboboxIvrDtmf_" + OptionParent + "_" + OptionPosition;
            comboboxIvrDtmf1.Size = new Size(50, 25);
            comboboxIvrDtmf1.Text = "Key...";
            comboboxIvrDtmf1.Location = new Point(120, 25 * (countIvr1 + countIvr2));
            //Dtmf Keys
            comboboxIvrDtmf1.DataSource = new BindingSource(Globals.IvrDtmfKeys, null);
            comboboxIvrDtmf1.DisplayMember = "Key";
            comboboxIvrDtmf1.ValueMember = "Value";

            textboxIvrVoiceResponse1 = new MetroTextBox();
            textboxIvrVoiceResponse1.Name = "textboxIvrVoiceResponse_" + OptionParent + "_" + OptionPosition;
            textboxIvrVoiceResponse1.Size = new Size(300, 29);
            textboxIvrVoiceResponse1.Text = "Voice response (optional)...";
            textboxIvrVoiceResponse1.Location = new Point(200, 25 * (countIvr1 + countIvr2));

            comboboxIvrQueue1 = new MetroComboBox();
            comboboxIvrQueue1.Name = "comboboxIvrQueue_" + OptionParent + "_" + OptionPosition;
            comboboxIvrQueue1.Size = new Size(300, 25);
            comboboxIvrQueue1.Text = "Select a queue...";
            comboboxIvrQueue1.Location = new Point(550, 25 * (countIvr1 + countIvr2));
            

            if (OptionLevel == 0)
            {
                buttonIvrRemoveSubOption1 = new MetroButton();
                buttonIvrRemoveSubOption1.Name = "buttonIvrRemoveSubOption_" + OptionParent + "_" + OptionPosition;
                buttonIvrRemoveSubOption1.Text = "-";
                buttonIvrRemoveSubOption1.Size = new Size(25, 29);
                buttonIvrRemoveSubOption1.Location = new Point(800, 25*(countIvr1 + countIvr2));
                buttonIvrRemoveSubOption1.Click +=
                    (sender, e) => buttonIvrRemoveRootOption_Click(buttonIvrRemoveSubOption1.Name, sender, e);
            }


            if (OptionLevel == 1)
            {
                buttonIvrRemoveSubOption1 = new MetroButton();
                buttonIvrRemoveSubOption1.Name = "buttonIvrRemoveSubOption_" + OptionParent + "_" + OptionPosition;
                buttonIvrRemoveSubOption1.Text = "-";
                buttonIvrRemoveSubOption1.Size = new Size(25, 29);
                buttonIvrRemoveSubOption1.Location = new Point(800, 25 * (countIvr1 + countIvr2));
                buttonIvrRemoveSubOption1.Click +=
                    (sender, e) => buttonIvrRemoveSubOption1_Click(buttonIvrRemoveSubOption1.Name, sender, e);
            }

            if (OptionLevel == 0)
            {
                buttonIvrAddSubOption1 = new MetroButton();
                buttonIvrAddSubOption1.Name = "buttonIvrAddSubOption_" + OptionParent + "_" + OptionPosition;
                buttonIvrAddSubOption1.Text = "+";
                buttonIvrAddSubOption1.Size = new Size(25, 29);
                buttonIvrAddSubOption1.Location = new Point(800, 25*(countIvr1 + countIvr2));
                //buttonIvrAddSubOption1.Click += buttonIvrAddSubOption1_Click;
                buttonIvrAddSubOption1.Click +=
                    (sender, e) => buttonIvrAddSubOption1_Click(buttonIvrAddSubOption1.Name, sender, e);
            }
        }


        //SubRemove1 RootRemove


        private void buttonIvrRemoveRootOption_Click(string name, object sender, EventArgs e)
        {
            //Raise event
            if (RootRemove != null)
            {
                //Pass information about the IvrMenu
                RootRemove(OptionLevel, OptionPosition);
            }
        }

        /// <summary>
        /// Handles sub open add button click event
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonIvrAddSubOption1_Click(string name, object sender, EventArgs e)
        {
            //Raise event
            if(SubAdd1 != null)
            {
                //Pass information about the IvrMenu
                SubAdd1(OptionLevel, OptionPosition, OptionParent);
            }
        }

        /// <summary>
        /// Handles sub option remove click event
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonIvrRemoveSubOption1_Click(string name, object sender, EventArgs e)
        {
            //Raise class event
            if (SubRemove1 != null)
            {
                //Pass information about the IvrMenu
                SubRemove1(OptionLevel, OptionPosition, OptionParent);
            }
        }



        //private void buttonIvrAddSubOption1_Click(string name, object sender, EventArgs e)
        //{
        //    //var nameSplit = name.Split('_')[1];
        //    //var parent = nameSplit[1];
        //    //var option = nameSplit[2];

        //    //MessageBox.Show(x);
        //    //AddIvrSubOption(x);

        //    if (countIvr2 == maxRowIvr1 - 1)
        //    {
        //        MessageBox.Show("Maximum of 10 rows can be added");
        //        return;
        //    }
        //    else
        //    {
        //        countIvr2++;

        //        labelIvr2[countIvr2] = new MetroLabel();
        //        labelIvr2[countIvr2].Name = "labelIvr_" + OptionNumberParent + "-" + countIvr2;
        //        labelIvr2[countIvr2].Text = "Option " + countIvr2;
        //        labelIvr2[countIvr2].Location = new Point(5, (40*(countIvr1)) + 2);

        //        comboboxIvrDtmf2[countIvr2] = new MetroComboBox();
        //        comboboxIvrDtmf2[countIvr2].Name = "comboboxIvrDtmf_" + OptionNumberParent + "-" + countIvr2;
        //        comboboxIvrDtmf2[countIvr2].Size = new Size(25, 25);
        //        comboboxIvrDtmf2[countIvr2].Location = new Point(120, 25*(countIvr1 + countIvr2));

        //        textboxIvrVoiceResponse2[countIvr2] = new MetroTextBox();
        //        textboxIvrVoiceResponse2[countIvr2].Name = "textboxIvrVoiceResponse_" + OptionNumberParent + "-" + countIvr2;
        //        textboxIvrVoiceResponse2[countIvr2].Size = new Size(300, 20);
        //        textboxIvrVoiceResponse2[countIvr2].Location = new Point(200, 25*(countIvr1 + countIvr2));

        //        comboboxIvrQueue2[countIvr2] = new MetroComboBox();
        //        comboboxIvrQueue2[countIvr2].Name = "comboboxIvrQueue_" + OptionNumberParent + "-" + countIvr2;
        //        comboboxIvrQueue2[countIvr2].Location = new Point(550, 25*(countIvr1 + countIvr2));

        //        //buttonIvrAddSubOption2 = new MetroButton();
        //        //buttonIvrAddSubOption2.Name = "buttonIvrAddSubOption_" + OptionNumberParent + "-" + OptionNumber;
        //        //buttonIvrAddSubOption2.Text = "+";
        //        //buttonIvrAddSubOption2.Size = new Size(25, 25);
        //        //buttonIvrAddSubOption2.Location = new Point(800, 25 * (countIvr1 + countIvr2));
        //        ////buttonIvrAddSubOption1.Click += buttonIvrAddSubOption1_Click;
        //        //buttonIvrAddSubOption2.Click += (sender, e) => buttonIvrAddSubOption1_Click(buttonIvrAddSubOption1.Name, sender, e);

        //        //buttonIvrRemoveSubOption2 = new MetroButton();
        //        //buttonIvrRemoveSubOption2.Name = "buttonIvrRemoveSubOption_" + OptionNumberParent + "-" + OptionNumber;
        //        //buttonIvrRemoveSubOption2.Text = "-";
        //        //buttonIvrRemoveSubOption2.Size = new Size(25, 25);
        //        //buttonIvrRemoveSubOption2.Location = new Point(830, 25 * (countIvr1 + countIvr2));
        //    }

        //}

    }
}
