using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SmartQuant;
using SmartQuant.FIX;
using SmartQuant.Providers;

namespace HuaQuant.Data.News
{
    public class HuaNewsProvider:IProvider,INewsProvider
    {
        #region IProvider 成员
        private bool isConnected = false;
        public void Connect(int timeout)
        {
            this.Connect();
            ProviderManager.WaitConnected(this, timeout);
        }

        public void Connect()
        {
            //if (System.IO.Directory.Exists(this.dzhDataPath))
            //{
            //    EmitStatusChangedEvent();
            //    isConnected = true;
            //    EmitConnectedEvent();
            //}
            //else
            //{
            //    this.EmitError(-1, -1, "大智慧数据文件夹不存在:" + this.dzhDataPath);
            //}
        }

        public void Disconnect()
        {
            EmitStatusChangedEvent();
            isConnected = false;
            EmitDisconnectedEvent();
        }
        [Category("信息")]
        public byte Id
        {
            get { return 254; }
        }
        [Category("信息")]
        public bool IsConnected
        {
            get { return this.isConnected; }
        }
        [Category("信息")]
        public string Name
        {
            get { return "HuaNewsProvider"; }
        }
        public void Shutdown()
        {
            this.Disconnect();
        }
        [Category("信息")]
        public ProviderStatus Status
        {
            get
            {
                if (!IsConnected)
                    return ProviderStatus.Disconnected;
                else
                    return ProviderStatus.Connected;
            }
        }
        [Category("信息")]
        public string Title
        {
            get { return "华新闻提供者"; }
        }
        [Category("信息")]
        public string URL
        {
            get { return "www.huaquant.com"; }
        }

        /*事件定义与激活方法*/
        public event EventHandler Connected;
        public event EventHandler StatusChanged;
        public event EventHandler Disconnected;
        public event ProviderErrorEventHandler Error;
        private void EmitConnectedEvent()
        {
            if (Connected != null)
            {
                Connected(this, EventArgs.Empty);
            }
        }
        private void EmitStatusChangedEvent()
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, EventArgs.Empty);
            }
        }
        private void EmitDisconnectedEvent()
        {
            if (Disconnected != null)
            {
                Disconnected(this, EventArgs.Empty);
            }
        }
        private void EmitError(int id, int code, string message)
        {
            if (Error != null)
                Error(new ProviderErrorEventArgs(new ProviderError(Clock.Now, this, id, code, message)));
        }
        #endregion


        public event NewsEventHandler News;
        public void EmitNews(FIXNews news)
        {
            if (this.News != null)
            {
                this.News(this, new NewsEventArgs(news));
                
            }
        }

        public void SendNewsCancelRequest()
        {
            throw new NotImplementedException();
        }

        public void SendNewsRequest()
        {
            throw new NotImplementedException();
        }
    }
}
