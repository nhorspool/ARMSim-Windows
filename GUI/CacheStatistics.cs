using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ARMSim.GUI
{
    public partial class CacheStatistics : Form
    {
        private Simulator.Cache.DataCache _dataCache;
        private Simulator.Cache.L1Cache _codeCache;

        /// <summary>
        /// CacheStatistics ctor.
        /// </summary>
        /// <param name="codeCache">reference to instruction cache</param>
        /// <param name="dataCache">reference to data cache</param>
        public CacheStatistics(Simulator.Cache.L1Cache codeCache, Simulator.Cache.DataCache dataCache)
        {
            InitializeComponent();

            _codeCache = codeCache;
            _dataCache = dataCache;
        }

        private void loadForm()
        {
            lbRdHits.Text = _dataCache.ReadHits.ToString();
            lbRdMisses.Text = _dataCache.ReadMisses.ToString();
            uint totalReads = _dataCache.ReadHits + _dataCache.ReadMisses;
            lbRdHitrate.Text = rateToString(_dataCache.ReadHits, totalReads);
            lbRdMissrate.Text = rateToString(_dataCache.ReadMisses, totalReads);

            lbWrHits.Text = _dataCache.WriteHits.ToString();
            lbWrMisses.Text = _dataCache.WriteMisses.ToString();
            uint totalWrites = _dataCache.WriteHits + _dataCache.WriteMisses;
            lbWrHitrate.Text = rateToString(_dataCache.WriteHits, totalWrites);
            lbWrMissrate.Text = rateToString(_dataCache.WriteMisses, totalWrites);

            lbCRdHits.Text = _codeCache.ReadHits.ToString();
            lbCRdMisses.Text = _codeCache.ReadMisses.ToString();
            totalReads = _codeCache.ReadHits + _codeCache.ReadMisses;
            lbCRdHitrate.Text = rateToString(_codeCache.ReadHits, totalReads);
            lbCRdMissrate.Text = rateToString(_codeCache.ReadMisses, totalReads);
        }//loadForm

        static private string rateToString(uint hits, uint total)
        {
            double val = 0;
            if (total > 0)
            {
                val = ((double)hits / (double)total) * 100.0;
            }
            return val.ToString("F2") + "%";
        }//rateToString

        private void CacheStatistics_Load(object sender, EventArgs e)
        {
            loadForm();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _dataCache.ResetStats();
            _codeCache.ResetStats();
            loadForm();
        }
    }//class CacheStatistics
}
