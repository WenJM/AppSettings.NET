using AppSettings.Client.Helper;
using AppSettings.Client.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace AppSettings.Client.Scan
{
    public class ScanWoker : IDisposable
    {
        public static ScanWoker _woker;

        private static object lockObj = new object();

        private static bool isStart = false;

        private DateTime? _lastModified;

        private string _uri;

        private int _interval;

        private Timer _timer;

        private Func<DateTime, DateTime> _func;

        public ScanWoker(string uri, int interval, DateTime lastModified, Func<DateTime, DateTime> func)
        {
            this._uri = uri;
            this._interval = interval;
            this._lastModified = lastModified;
            this._func = func;

            this.Init();
        }

        private void Init()
        {
            this._timer = new Timer();
            this._timer.Elapsed += new ElapsedEventHandler(DoScan);
            this._timer.Interval = this._interval;
            this._timer.AutoReset = true;
            this._timer.Enabled = false;
        }

        public void Start()
        {
            lock (lockObj)
            {
                if (!isStart)
                {
                    this._timer.Start();
                    isStart = true;
                }
            }
        }

        private void DoScan(object sender, ElapsedEventArgs e)
        {
            lock (lockObj)
            {
                this._lastModified = this._func?.Invoke(this._lastModified.Value);
            }
        }

        public void Dispose()
        {
            if (this._timer!= null)
            {
                this._timer.Stop();
            };
        }

        public static ScanWoker Current
        {
            get
            {
                if (_woker == null)
                {
                    lock (lockObj)
                    {
                        if (_woker == null)
                        {
                            _woker = new ScanWoker(
                            AppSettingConfig.AppSettingsPath,
                            AppSettingConfig.ScanInterval,
                            Utils.ReadLastModified(AppSettingConfig.AppSettingsPath),
                            (d) =>
                            {
                                var lastModified = Utils.ReadLastModified(AppSettingConfig.AppSettingsPath);
                                if (lastModified != d)
                                {
                                    XmlHelper.ResetXElementOfCache(AppSettingConfig.XElementCacheKey, AppSettingConfig.AppSettingsPath, AppSettingConfig.IsRemoteFile);
                                }
                                return lastModified;
                            });
                        }
                    }
                }
                return _woker;
            }
        }
    }
}
