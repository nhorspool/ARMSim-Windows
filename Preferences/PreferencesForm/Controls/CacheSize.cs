using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ARMSim.Preferences;

namespace ARMSim.Preferences.PreferencesForm.Controls
{
    /// <summary>
    /// This control defines the overall size and block size of the cache.
    /// The 2 numeric up-down controls allow the user to select a power of 2 number between
    /// the min and max cache sizes (defined in InstructionCachePreferences)
    /// </summary>
    public partial class CacheSize : UserControl
    {
        /// <summary>
        /// event fired when the block size has changed
        /// </summary>
        public event Associativity.numberBlocksChangedDelegate BlocksChanged;

        /// <summary>
        /// ctor
        /// </summary>
        public CacheSize()
        {
            InitializeComponent();
            nudCacheSize.UpDownHandler += cacheSizeUpDownHandler;
            nudBlockSize.UpDownHandler += blockSizeUpDownHandler;
        }

        /// <summary>
        /// Set the cache size given a block size and number of blocks
        /// </summary>
        /// <param name="blockSize"></param>
        /// <param name="numBlocks"></param>
        public void Set(uint blockSize, uint numberBlocks)
        {
            nudBlockSize.Value = blockSize;
            tbNumBlocks.Text = numberBlocks.ToString();
            nudCacheSize.Value = blockSize * numberBlocks;
        }

        private void FireNumberBlocksChanged()
        {
            if (BlocksChanged != null)
            {
                BlocksChanged(this.NumberBlocks);
            }
        }

        public uint BlockSize { get { return (uint)nudBlockSize.Value; } }
        public uint NumberBlocks
        {
            get 
            {
                uint numBlocks;
                if (uint.TryParse(tbNumBlocks.Text, out numBlocks))
                    return numBlocks;

                return 0;
            }
        }

        private static bool validateSizes(uint cacheSize, uint blockSize, uint numBlocks)
        {
            if (blockSize > InstructionCachePreferences._maxBlockSize ||
                blockSize < InstructionCachePreferences._minBlockSize) return false;

            if (cacheSize > InstructionCachePreferences._maxCacheSize ||
                cacheSize < blockSize) return false;

            if (numBlocks < 1 || numBlocks > InstructionCachePreferences._maxNumberBlocks) return false;

            return true;
        }

        public void blockSizeUpDownHandler(PowerOf2 sender, PowerOf2.UpDownDirection direction)
        {
            uint blockSize = direction == PowerOf2.UpDownDirection.Up ? (uint)nudBlockSize.Value << 1 : (uint)nudBlockSize.Value >> 1;
            uint cacheSize = (uint)nudCacheSize.Value;
            uint numBlocks = cacheSize / blockSize;

            if (!validateSizes(cacheSize, blockSize, numBlocks)) return;

            nudBlockSize.Value = blockSize;
            tbNumBlocks.Text = numBlocks.ToString();

            FireNumberBlocksChanged();
        }

        private void cacheSizeUpDownHandler(PowerOf2 sender, PowerOf2.UpDownDirection direction)
        {
            uint cacheSize = direction == PowerOf2.UpDownDirection.Up ? (uint)nudCacheSize.Value << 1 : (uint)nudCacheSize.Value >> 1;
            uint blockSize = (uint)nudBlockSize.Value;
            uint numBlocks = cacheSize / blockSize;

            if (!validateSizes(cacheSize, blockSize, numBlocks)) return;

            nudCacheSize.Value = cacheSize;
            tbNumBlocks.Text = numBlocks.ToString();

            FireNumberBlocksChanged();
        }//cacheSizeUpDownHandler

    }//class CacheSize
}
