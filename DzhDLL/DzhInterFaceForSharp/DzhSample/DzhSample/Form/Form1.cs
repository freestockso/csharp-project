using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace DzhSample
{
    public enum SHOW_TYPE
    {
        Time,
        Tick,
        Fin,
    }
    public partial class Form1 : Form
    {
        delegate void REFRESHDATA();
        ListViewBase m_pListViewBase = null;
        string m_sStockCode = "";
        int m_nDataType = 0;
        bool m_bIsDynRefresh = false;

        SHOW_TYPE m_ShowType = SHOW_TYPE.Time;
        bool m_bIsClose = false;
        public Form1()
        {
            InitializeComponent();
            this.TopMost = true;
            m_pListViewBase = new ListViewBase();
            m_pListViewBase.Create(0, menuStrip1.Height +1, this.Width - 8, this.ClientRectangle.Height - menuStrip1.Height - 6, this);
            topmostToolStripMenuItem.Checked = this.TopMost;
            dynToolStripMenuItem.Checked = m_bIsDynRefresh;
            if (m_ShowType == SHOW_TYPE.Time)
            {
                timeToolStripMenuItem.Checked = true;
                tickToolStripMenuItem.Checked = false;
                finToolStripMenuItem.Checked = false;
                m_pListViewBase.AddTimeColumn();
            }
            else if (m_ShowType == SHOW_TYPE.Tick)
            {
                timeToolStripMenuItem.Checked = false;
                tickToolStripMenuItem.Checked = true;
                finToolStripMenuItem.Checked = false;
                m_pListViewBase.AddTickColumn();
            }
            else if (m_ShowType == SHOW_TYPE.Fin)
            {
                timeToolStripMenuItem.Checked = false;
                tickToolStripMenuItem.Checked = false;
                finToolStripMenuItem.Checked = true;
                m_pListViewBase.AddFinColumn();
            }
        }
        public bool IsClose()
        {
            return m_bIsClose ;
        }
        public SHOW_TYPE GetShowType()
        {
            return m_ShowType;
        }
        public bool IsRefreshData(string sStockCode , Int32 nDataType , Int32 nDataCount)
        {
            if (m_ShowType == SHOW_TYPE.Fin)
            {
                if(sStockCode == m_sStockCode)
                    return false;
            }
            else
            {
                if (!m_bIsDynRefresh)
                {
                    if (sStockCode == m_sStockCode && m_nDataType == nDataType && nDataCount <= m_pListViewBase.Items.Count)
                        return false;
                }
            }
            m_sStockCode = sStockCode;
            m_nDataType = nDataType;
            this.Text = m_sStockCode;
            return true;
        }
        public void RefreshData(ref float [] pFinData)
        {
            m_pListViewBase.RefreshData(ref pFinData);
            REFRESHDATA pRefreshData = new REFRESHDATA(_RefreshData);
            Invoke(pRefreshData);
        }
        public void RefreshData(ref STKDATA[] pStkData)
        {
            m_pListViewBase.RefreshData(ref pStkData);
            REFRESHDATA pRefreshData = new REFRESHDATA(_RefreshData);
            Invoke(pRefreshData);
        }
        public void RefreshData(ref STKDATA[] pStkData,ref STKDATAEx[] pStkDataEx)
        {
            m_pListViewBase.RefreshData(ref pStkData , ref pStkDataEx);
            REFRESHDATA pRefreshData = new REFRESHDATA(_RefreshData);
            Invoke(pRefreshData);
        }
        void _RefreshData()
        {
            m_pListViewBase.RefreshData();
            m_pListViewBase.OnPaint();
        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            m_pListViewBase.OnSize(0, menuStrip1.Height + 1, this.Width - 8, this.ClientRectangle.Height - menuStrip1.Height - 6);
        }
        private void SaveAsTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog pFileDialog = new SaveFileDialog();
            pFileDialog.FileName = this.Text;
            pFileDialog.DefaultExt = "txt";
            pFileDialog.Filter = "文本文件(*.txt)|*.txt|Excel文本(*.csv)|*.csv|全部文件(*.*)|*.*";
            if (pFileDialog.ShowDialog() == DialogResult.Cancel)
                return;
            m_pListViewBase.WriteToText(pFileDialog.FileName);
        }
        private void timeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_ShowType == SHOW_TYPE.Time)
                return;
            m_ShowType = SHOW_TYPE.Time;
            timeToolStripMenuItem.Checked = true;
            tickToolStripMenuItem.Checked = false;
            finToolStripMenuItem.Checked = false;
            m_pListViewBase.AddTimeColumn();
        }
        private void tickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_ShowType == SHOW_TYPE.Tick)
                return;
            m_ShowType = SHOW_TYPE.Tick;
            timeToolStripMenuItem.Checked = false;
            tickToolStripMenuItem.Checked = true;
            finToolStripMenuItem.Checked = false;
            m_pListViewBase.AddTickColumn();
        }
        private void finToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_ShowType == SHOW_TYPE.Fin)
                return;
            m_ShowType = SHOW_TYPE.Fin;
            timeToolStripMenuItem.Checked = false;
            tickToolStripMenuItem.Checked = false;
            finToolStripMenuItem.Checked = true;
            m_pListViewBase.AddFinColumn();

        }
        private void dynToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_bIsDynRefresh)
            {
                m_bIsDynRefresh = false;
                dynToolStripMenuItem.Checked = false;
            }
            else
            {
                m_bIsDynRefresh = true;
                dynToolStripMenuItem.Checked = true;
            }

        }
        private void topmostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.TopMost)
            {
                this.TopMost = false;
                topmostToolStripMenuItem.Checked = false;
            }
            else
            {
                this.TopMost = true;
                topmostToolStripMenuItem.Checked = true;
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_bIsClose = true;
        }
    }
}