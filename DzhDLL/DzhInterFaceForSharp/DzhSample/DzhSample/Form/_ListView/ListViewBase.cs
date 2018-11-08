using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using MyTable;
using System.Diagnostics;
namespace DzhSample
{
    public partial class ListViewBase : ListView
    {
        SortOrder m_SortOrder = SortOrder.None;
        int m_nSortColumn = -1;

        _Table m_pTable = new _Table ();
        public void Create(int nLeft, int nTop, int nWidth, int nHeight, Form pForm)
        {
            Create(nLeft, nTop, nWidth, nHeight);
            pForm.Controls.Add(this);
        }
        public void Create(int nLeft, int nTop, int nWidth, int nHeight)
        {

            this.Name = "ListView";
            this.TabIndex = 1;
            this.SetStyle(ControlStyles.UserPaint, true);
            this.View = View.Details;
            this.FullRowSelect = true;
            this.GridLines = true;
            this.HideSelection = false;

            this.Top = nTop;
            this.Left = nLeft;
            this.Height = nHeight;
            this.Width = nWidth;

            //AddTimeColumn();
            /*
            m_pTable.AddCol("序号", typeof(int), 4);
            m_pTable.AddCol("日期", typeof(int), 4);
            m_pTable.AddCol("时间", typeof(int), 4);
            m_pTable.AddCol("开盘", typeof(float), 4);
            m_pTable.AddCol("最高", typeof(float), 4);
            m_pTable.AddCol("最低", typeof(float), 4);
            m_pTable.AddCol("收盘", typeof(float), 4);
            m_pTable.AddCol("成交量", typeof(float), 4);
            m_pTable.AddCol("成交额", typeof(float), 4);
            m_pTable.AddCol("涨家数", typeof(UInt16), 2);
            m_pTable.AddCol("跌家数", typeof(UInt16), 2);

            int i;
            for (i = 0; i < m_pTable.GetColCount(); i++)
                this.Columns.Add(m_pTable.GetColName(i), 80);
             */
        }
        public void AddTimeColumn()
        {
            m_pTable.ClearCol();
            this.Items.Clear();
            this.Columns.Clear();
            m_pTable.AddCol("序号", typeof(int), 4);
            m_pTable.AddCol("日期", typeof(int), 4);
            m_pTable.AddCol("时间", typeof(int), 4);
            m_pTable.AddCol("开盘", typeof(float), 4);
            m_pTable.AddCol("最高", typeof(float), 4);
            m_pTable.AddCol("最低", typeof(float), 4);
            m_pTable.AddCol("收盘", typeof(float), 4);
            m_pTable.AddCol("成交量", typeof(float), 4);
            m_pTable.AddCol("成交额", typeof(float), 4);
            m_pTable.AddCol("涨家数", typeof(UInt16), 2);
            m_pTable.AddCol("跌家数", typeof(UInt16), 2);
            int i;
            for (i = 0; i < m_pTable.GetColCount(); i++)
                this.Columns.Add(m_pTable.GetColName(i), 80);
        }
        public void AddTickColumn()
        {
            m_pTable.ClearCol();
            this.Items.Clear();
            this.Columns.Clear();
            m_pTable.AddCol("序号", typeof(int), 4);
            m_pTable.AddCol("日期", typeof(int), 4);
            m_pTable.AddCol("时间", typeof(int), 4);
            m_pTable.AddCol("收盘", typeof(float), 4);
            m_pTable.AddCol("成交量", typeof(float), 4);
            m_pTable.AddCol("成交额", typeof(float), 4);
            m_pTable.AddCol("卖5量", typeof(float), 4);
            m_pTable.AddCol("卖5价", typeof(float), 4);
            m_pTable.AddCol("卖4量", typeof(float), 4);
            m_pTable.AddCol("卖4价", typeof(float), 4);
            m_pTable.AddCol("卖3量", typeof(float), 4);
            m_pTable.AddCol("卖3价", typeof(float), 4);
            m_pTable.AddCol("卖2量", typeof(float), 4);
            m_pTable.AddCol("卖2价", typeof(float), 4);
            m_pTable.AddCol("卖1量", typeof(float), 4);
            m_pTable.AddCol("卖1价", typeof(float), 4);
            m_pTable.AddCol("买1价", typeof(float), 4);
            m_pTable.AddCol("买1量", typeof(float), 4);
            m_pTable.AddCol("买2价", typeof(float), 4);
            m_pTable.AddCol("买2量", typeof(float), 4);
            m_pTable.AddCol("买3价", typeof(float), 4);
            m_pTable.AddCol("买3量", typeof(float), 4);
            m_pTable.AddCol("买4价", typeof(float), 4);
            m_pTable.AddCol("买4量", typeof(float), 4);
            m_pTable.AddCol("买5价", typeof(float), 4);
            m_pTable.AddCol("买5量", typeof(float), 4);
            int i;
            for (i = 0; i < m_pTable.GetColCount(); i++)
                this.Columns.Add(m_pTable.GetColName(i), 80);
        }
        public void AddFinColumn()
        {
            m_pTable.ClearCol();
            this.Items.Clear();
            this.Columns.Clear();
            m_pTable.AddCol("序号", typeof(int), 4);
            m_pTable.AddCol("名称", typeof(string), 64);
            m_pTable.AddCol("数值", typeof(float), 4);
            int i;
            for (i = 0; i < m_pTable.GetColCount(); i++)
                this.Columns.Add(m_pTable.GetColName(i), 80);
        }
        public void OnSize(int nLeft, int nTop, int nWidth, int nHeight)
        {
            this.Top = nTop;
            this.Left = nLeft;
            this.Height = nHeight;
            this.Width = nWidth;
        }
        public void WriteToText(string sFileName)
        {
            m_pTable.WriteToText(sFileName);
        }
        public void RefreshData(ref float[] pFinData)
        {
            if (pFinData == null)
                return;
            m_pTable.ClearRow();
            m_SortOrder = SortOrder.None;
            m_nSortColumn = -1;
            string[] sName ={
            "总股本(万股)","国家股","发起人法人股","法人股","B股","H股","流通A股","职工股","A2转配股","总资产(千元)","流动资产",
            "固定资产","无形资产","长期投资","流动负债","长期负债","资本公积金","每股公积金","股东权益","主营收入","主营利润","其他利润",
            "营业利润","投资收益","补贴收入","营业外收支","上年损益调整","利润总额","税后利润","净利润","未分配利润","每股未分配",
            "每股收益","每股净资产","调整每股净资","股东权益比","净资收益率"};
            int i = 0;
            for (i = 0; i < pFinData.Length && i < sName.Length; i++)
            {
                m_pTable.AddRow();
                m_pTable.SetValue(i, 0, i + 1);
                m_pTable.SetValue(i, 1, sName[i]);
                m_pTable.SetValue(i, 2, pFinData[i]);
            }
        }
        public void RefreshData(ref STKDATA[] pStkData)
        {
            if (pStkData == null)
                return;
            m_pTable.ClearRow();
            m_SortOrder = SortOrder.None;
            m_nSortColumn = -1;
            int i = 0, nDate = 0, nTime = 0;
            for (i = 0; i < pStkData.Length; i++)
            {
                m_pTable.AddRow();
                nDate = _TimetToDateTime(pStkData[i].m_time, ref nTime);
                m_pTable.SetValue(i, 0, i + 1);
                m_pTable.SetValue(i, 1, nDate);
                m_pTable.SetValue(i, 2, nTime);
                m_pTable.SetValue(i, 3, pStkData[i].m_fOpen);
                m_pTable.SetValue(i, 4, pStkData[i].m_fHigh);
                m_pTable.SetValue(i, 5, pStkData[i].m_fLow);
                m_pTable.SetValue(i, 6, pStkData[i].m_fClose);
                m_pTable.SetValue(i, 7, pStkData[i].m_fVolume);
                m_pTable.SetValue(i, 8, pStkData[i].m_fAmount);
                m_pTable.SetValue(i, 9, pStkData[i].m_wAdvance);
                m_pTable.SetValue(i, 10, pStkData[i].m_wDecline);
            }
        }
        public void RefreshData(ref STKDATA[] pStkData,ref STKDATAEx[] pStkDataEx)
        {
            if (pStkData == null)
                return;
            m_pTable.ClearRow();
            m_SortOrder = SortOrder.None;
            m_nSortColumn = -1;
            int i = 0, nDate = 0, nTime = 0;
            for (i = 0; i < pStkData.Length; i++)
            {
                m_pTable.AddRow();
                nDate = _TimetToDateTime(pStkData[i].m_time, ref nTime);
                m_pTable.SetValue(i, 0, i + 1);
                m_pTable.SetValue(i, 1, nDate);
                m_pTable.SetValue(i, 2, nTime);
                m_pTable.SetValue(i, 3, pStkData[i].m_fClose);
                m_pTable.SetValue(i, 4, pStkData[i].m_fVolume);
                m_pTable.SetValue(i, 5, pStkData[i].m_fAmount);
                if (pStkDataEx != null)
                {
                    m_pTable.SetValue(i, 6, pStkDataEx[i].m_fSellVol5);
                    m_pTable.SetValue(i, 7, pStkDataEx[i].m_fSellPrice5);
                    m_pTable.SetValue(i, 8, pStkDataEx[i].m_fSellVol4);
                    m_pTable.SetValue(i, 9, pStkDataEx[i].m_fSellPrice4);
                    m_pTable.SetValue(i, 10, pStkDataEx[i].m_fSellVol3);
                    m_pTable.SetValue(i, 11, pStkDataEx[i].m_fSellPrice3);
                    m_pTable.SetValue(i, 12, pStkDataEx[i].m_fSellVol2);
                    m_pTable.SetValue(i, 13, pStkDataEx[i].m_fSellPrice2);
                    m_pTable.SetValue(i, 14, pStkDataEx[i].m_fSellVol1);
                    m_pTable.SetValue(i, 15, pStkDataEx[i].m_fSellPrice1);
                    m_pTable.SetValue(i, 16, pStkDataEx[i].m_fBuyPrice1);
                    m_pTable.SetValue(i, 17, pStkDataEx[i].m_fBuyVol1);
                    m_pTable.SetValue(i, 18, pStkDataEx[i].m_fBuyPrice2);
                    m_pTable.SetValue(i, 19, pStkDataEx[i].m_fBuyVol2);
                    m_pTable.SetValue(i, 20, pStkDataEx[i].m_fBuyPrice3);
                    m_pTable.SetValue(i, 21, pStkDataEx[i].m_fBuyVol3);
                    m_pTable.SetValue(i, 22, pStkDataEx[i].m_fBuyPrice4);
                    m_pTable.SetValue(i, 23, pStkDataEx[i].m_fBuyVol4);
                    m_pTable.SetValue(i, 24, pStkDataEx[i].m_fBuyPrice5);
                    m_pTable.SetValue(i, 25, pStkDataEx[i].m_fBuyVol5);
                }
                else
                {
                    m_pTable.SetValue(i, 6, (float)0);
                    m_pTable.SetValue(i, 7, (float)0);
                    m_pTable.SetValue(i, 8, (float)0);
                    m_pTable.SetValue(i, 9, (float)0);
                    m_pTable.SetValue(i, 10, (float)0);
                    m_pTable.SetValue(i, 11, (float)0);
                    m_pTable.SetValue(i, 12, (float)0);
                    m_pTable.SetValue(i, 13, (float)0);
                    m_pTable.SetValue(i, 14, (float)0);
                    m_pTable.SetValue(i, 15, (float)0);
                    m_pTable.SetValue(i, 16, (float)0);
                    m_pTable.SetValue(i, 17, (float)0);
                    m_pTable.SetValue(i, 18, (float)0);
                    m_pTable.SetValue(i, 19, (float)0);
                    m_pTable.SetValue(i, 20, (float)0);
                    m_pTable.SetValue(i, 21, (float)0);
                    m_pTable.SetValue(i, 22, (float)0);
                    m_pTable.SetValue(i, 23, (float)0);
                    m_pTable.SetValue(i, 24, (float)0);
                    m_pTable.SetValue(i, 25, (float)0);
                }
            }
        }
        public void RefreshData()
        {
            this.BeginUpdate();
            int i;
            for(i = this.Items.Count ; i < m_pTable .GetRowCount () ; i++)
                this.Items.Add("");
            int nItemCount = this.Items.Count;
            int nDeleteRowIndex = m_pTable.GetRowCount();
            for (i = nDeleteRowIndex; i < nItemCount; i++)
                this.Items.RemoveAt(nDeleteRowIndex);
            this.EndUpdate();
        }
        public void AddItem(ref STKDATA[] pStkData)
        {
            this.BeginUpdate();
            //this.Items.Clear();
            //m_pTable.ClearRow();
            m_SortOrder = SortOrder.None;
            m_nSortColumn = -1;
            int i = 0, nDate = 0, nTime = 0;
            /*
            for (i = 0 ; i < pStkData.Length; i++)
            {
                this.Items.Add("");
                m_pTable.AddRow();
                nDate = _TimetToDateTime(pStkData[i].m_time, ref nTime);
                m_pTable.SetValue(i, 0, i + 1);
                m_pTable.SetValue(i, 1, nDate);
                m_pTable.SetValue(i, 2, nTime);
                m_pTable.SetValue(i, 3, pStkData[i].m_fOpen);
                m_pTable.SetValue(i, 4, pStkData[i].m_fHigh);
                m_pTable.SetValue(i, 5, pStkData[i].m_fLow);
                m_pTable.SetValue(i, 6, pStkData[i].m_fClose);
                m_pTable.SetValue(i, 7, pStkData[i].m_fVolume);
                m_pTable.SetValue(i, 8, pStkData[i].m_fAmount);
                m_pTable.SetValue(i, 9, pStkData[i].m_wAdvance);
                m_pTable.SetValue(i, 10, pStkData[i].m_wDecline);
            }
             * */
            for (i = 0; i < pStkData.Length && i < m_pTable.GetRowCount() && i < this.Items.Count; i++)
            {
                nDate = _TimetToDateTime(pStkData[i].m_time, ref nTime);
                m_pTable.SetValue(i, 0, i + 1);
                m_pTable.SetValue(i, 1, nDate);
                m_pTable.SetValue(i, 2, nTime);
                m_pTable.SetValue(i, 3, pStkData[i].m_fOpen);
                m_pTable.SetValue(i, 4, pStkData[i].m_fHigh);
                m_pTable.SetValue(i, 5, pStkData[i].m_fLow);
                m_pTable.SetValue(i, 6, pStkData[i].m_fClose);
                m_pTable.SetValue(i, 7, pStkData[i].m_fVolume);
                m_pTable.SetValue(i, 8, pStkData[i].m_fAmount);
                m_pTable.SetValue(i, 9, pStkData[i].m_wAdvance);
                m_pTable.SetValue(i, 10, pStkData[i].m_wDecline);
            }
            for (; i < pStkData.Length; i++)
            {
                this.Items.Add("");
                m_pTable.AddRow();
                nDate = _TimetToDateTime(pStkData[i].m_time, ref nTime);
                m_pTable.SetValue(i, 0, i + 1);
                m_pTable.SetValue(i, 1, nDate);
                m_pTable.SetValue(i, 2, nTime);
                m_pTable.SetValue(i, 3, pStkData[i].m_fOpen);
                m_pTable.SetValue(i, 4, pStkData[i].m_fHigh);
                m_pTable.SetValue(i, 5, pStkData[i].m_fLow);
                m_pTable.SetValue(i, 6, pStkData[i].m_fClose);
                m_pTable.SetValue(i, 7, pStkData[i].m_fVolume);
                m_pTable.SetValue(i, 8, pStkData[i].m_fAmount);
                m_pTable.SetValue(i, 9, pStkData[i].m_wAdvance);
                m_pTable.SetValue(i, 10, pStkData[i].m_wDecline);
            }
            int nRowCount = this.Items.Count;
            int nDeleteRowIndex = i;
            for (; i < nRowCount; i++)
            {
                this.Items.RemoveAt(nDeleteRowIndex);
            }
            if (nDeleteRowIndex < nRowCount)
                m_pTable.DeleteRow(nDeleteRowIndex , nRowCount - nDeleteRowIndex);
            this.EndUpdate();
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {            
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            OnPaint();
        }
        public virtual void OnPaint()
        {
            if (this.ClientSize.Width <= 0 || this.ClientSize.Height <= 0)
                return;
            Bitmap pBitmapMem = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            Graphics pGraphicsMem = Graphics.FromImage(pBitmapMem);
            pGraphicsMem.Clear(Color.White);

            Rectangle rItem;
            SolidBrush hBrushFore;
            SolidBrush hBrushNormal = new SolidBrush(Color.Black);
            SolidBrush hBrushSelected = new SolidBrush(Color.FromArgb(0x80, 0x80, 0x80));

            int nRowIndex, nColumnIndex;
            nRowIndex = 0;
            if (this.TopItem != null)
                nRowIndex = this.TopItem.Index;
            for (; nRowIndex < this.Items.Count && nRowIndex < m_pTable .GetRowCount (); nRowIndex++)
            {
                    hBrushFore = hBrushNormal;

                rItem = GetItemRect(nRowIndex);
                if (rItem.Y >= this.Bottom)
                    break;
                if (this.Items[nRowIndex].Selected)
                {
                    pGraphicsMem.FillRectangle(hBrushSelected, rItem);
                }
                for (nColumnIndex = 0; nColumnIndex < this.Columns.Count && nColumnIndex < m_pTable .GetColCount (); nColumnIndex++)
                {
                    if (rItem.X >= this.Right)
                        break;
                    rItem.Width = this.Columns[nColumnIndex].Width;
                    pGraphicsMem.DrawString(m_pTable.GetValueByString(nRowIndex, nColumnIndex), this.Font, hBrushFore, rItem);
                    rItem.X += this.Columns[nColumnIndex].Width;
                }
            }
            Graphics pThisGraphics = this.CreateGraphics();
            pThisGraphics.DrawImage(pBitmapMem, 0, 0);

            hBrushNormal.Dispose();

            pBitmapMem.Dispose();
            pThisGraphics.Dispose();
            pGraphicsMem.Dispose();
        }
        Int32 _TimetToDateTime(Int32 nTimet,ref Int32 nTime)
        {
            nTime = 0;
            if (nTimet <= 0)
                return 0;
            //抄来的
            DateTime now = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified).AddSeconds(nTimet);
            nTime = now.Hour * 10000 + now.Minute * 100 + now.Second;
            return now.Year * 10000 + now.Month * 100 + now.Day;

        }
        protected override void OnColumnClick(ColumnClickEventArgs e)
        {
            if (e.Column < 0)
                return;
            this.BeginUpdate();
            if (m_nSortColumn != e.Column)
            {
                m_SortOrder = SortOrder.None;
                m_nSortColumn = e.Column;
            }
            int nDirect = 0;
            if (m_SortOrder == SortOrder.None || m_SortOrder == SortOrder.Descending)
            {
                m_SortOrder = SortOrder.Ascending;
                nDirect = 1;//up
            }
            else
            {
                m_SortOrder = SortOrder.Descending;
                nDirect = 0;//down
            }
            m_pTable.Sort(m_nSortColumn, nDirect);
            this.EndUpdate();
            OnPaint();
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {            
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            ListViewHitTestInfo pListViewHitTestInfo = this.HitTest(e.X, e.Y);
            if (pListViewHitTestInfo.Item == null)
                return;
            Point pPoint = new Point(e.X, e.Y);
            int nRowIndex = pListViewHitTestInfo.Item.Index;
            Rectangle rItem = GetItemRect(nRowIndex);
            rItem.X = this.Columns[0].Width;
            rItem.Width = this.Columns[1].Width;
            if(rItem.Contains (pPoint))
                this.Cursor = System.Windows.Forms.Cursors.Hand;
        }
        protected override void  OnMouseDown(MouseEventArgs e)
        {
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            ListViewHitTestInfo pListViewHitTestInfo = this.HitTest(e.X, e.Y);
            if (pListViewHitTestInfo.Item == null)
                return;
            Point pPoint = new Point(e.X, e.Y);
            int nRowIndex = pListViewHitTestInfo.Item.Index;
            Rectangle rItem = GetItemRect(nRowIndex);
            rItem.X = this.Columns[0].Width;
            rItem.Width = this.Columns[1].Width;
            if (rItem.Contains(pPoint))
            {
            }
        }
    }
}
