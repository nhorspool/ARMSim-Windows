using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ARMSim.Preferences.PreferencesForm.Controls
{
    public partial class Associativity : UserControl
    {
        public delegate void numberBlocksChangedDelegate(uint numberCacheBlocks);
        private uint _numberCacheBlocks;

        public Associativity()
        {
            InitializeComponent();
            po2BlocksPerSet.UpDownHandler += blocksUpDownHandler;
        }

        public void Set(uint numberCacheBlocks, uint blocksPerSet)
        {
            po2BlocksPerSet.Value = Math.Min(blocksPerSet, numberCacheBlocks);
            _numberCacheBlocks = numberCacheBlocks;

            if (po2BlocksPerSet.Value == numberCacheBlocks)
            {
                rbFullyAssociative.Checked = true;
            }
            else if (po2BlocksPerSet.Value == 1)
            {
                rbDirectMapped.Checked = true;
            }
            else
            {
                rbSetAssociative.Checked = true;
            }
        }

        public uint Blocks { get { return (uint)po2BlocksPerSet.Value; } }

        public void numberBlocksChangedHandler(uint numberCacheBlocks)
        {
            _numberCacheBlocks = numberCacheBlocks;

            if (rbFullyAssociative.Checked)
            {
                po2BlocksPerSet.Value = _numberCacheBlocks;
            }
            else if (rbSetAssociative.Checked)
            {
                po2BlocksPerSet.Value = Math.Min(po2BlocksPerSet.Value, _numberCacheBlocks);
            }
        }

        public void SetInternalEnabled(bool v)
        {
            foreach (Control c in groupBox1.Controls)
            {
                if (c.Name == "po2BlocksPerSet")
                    c.Enabled = v ? rbSetAssociative.Checked : false;
                else
                    c.Enabled = v;
            }
        }

        private void blocksUpDownHandler(PowerOf2 sender, PowerOf2.UpDownDirection direction)
        {
            uint blocksPerSet = direction == PowerOf2.UpDownDirection.Up ? (uint)po2BlocksPerSet.Value << 1 : (uint)po2BlocksPerSet.Value >> 1;
            if (blocksPerSet < 2) return;
            if (blocksPerSet > _numberCacheBlocks / 2) return;

            po2BlocksPerSet.Value = blocksPerSet;
        }

        private void rbFullyAssociative_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                po2BlocksPerSet.Value = _numberCacheBlocks;
                po2BlocksPerSet.Enabled = false;
            }
        }

        private void rbSetAssociative_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                po2BlocksPerSet.Enabled = true;
                if (po2BlocksPerSet.Value >= _numberCacheBlocks)
                {
                    po2BlocksPerSet.Value = _numberCacheBlocks / 2;
                }

                if (po2BlocksPerSet.Value == 1)
                {
                    po2BlocksPerSet.Value = 2;
                }

            }
        }

        private void rbDirectMapped_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                po2BlocksPerSet.Value = 1;
                po2BlocksPerSet.Enabled = false;
            }
        }

    }//class Associativity
}
