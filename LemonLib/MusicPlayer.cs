﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;

namespace LemonLib
{
    public class MusicPlayer
    {
        private int stream = -1024;
        private IntPtr wind;
        public MusicPlayer(IntPtr win) {
            wind = win;
            BassNet.Registration("lemon.app@qq.com", "2X52325160022");
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_CPSPEAKERS, win);
        }
        public void Load(string file) {
            if (stream != -1024) {
                Bass.BASS_ChannelStop(stream);
                Bass.BASS_StreamFree(stream);
            }
            stream = Bass.BASS_StreamCreateFile(file, 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);
        }
        private FileStream _fs = null;
        private byte[] _data;
        private string DownloadPath;
        private DOWNLOADPROC _download;
        public void LoadUrl(string url,string Saveloc) {
            if (stream != -1024)
            {
                Bass.BASS_ChannelStop(stream);
                Bass.BASS_StreamFree(stream);
            }
            DownloadPath = Saveloc;
            _download = new DOWNLOADPROC(download);
            stream = Bass.BASS_StreamCreateURL(url, 0, BASSFlag.BASS_SAMPLE_FLOAT, _download, IntPtr.Zero);
        }
        private async void download(IntPtr buffer, int length, IntPtr user) {
            try
            {
                if (_fs == null)
                {
                    _fs = File.OpenWrite(DownloadPath);
                }
                if (buffer == IntPtr.Zero)
                {
                    //下载完成...
                    await _fs.FlushAsync();
                    _fs.Close();
                    _fs = null;
                }
                else
                {
                    // increase the data buffer as needed
                    if (_data == null || _data.Length < length)
                        _data = new byte[length];
                    // copy from managed to unmanaged memory
                    Marshal.Copy(buffer, _data, 0, length);
                    // write to file
                    await _fs.WriteAsync(_data, 0, length);
                }
            }
            catch { }
        }
        public void Play() {
            Bass.BASS_ChannelPlay(stream,false);
        }
        public void Pause() {
            Bass.BASS_ChannelPause(stream);
        }
        public TimeSpan GetLength {
            get {
                double seconds =Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetLength(stream));
                return TimeSpan.FromSeconds(seconds);
            }
        }
        public TimeSpan Position
        {
            get
            {
                double seconds = Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetPosition(stream));
                return TimeSpan.FromSeconds(seconds);
            }
            set => Bass.BASS_ChannelSetPosition(stream, value.TotalSeconds);
        }

        public float[] GetFFTData()
        {
            float[] fft = new float[256];
            Bass.BASS_ChannelGetData(stream, fft, (int)BASSData.BASS_DATA_FFT256);
            return fft;
        }
        public void UpdataDevice() {
            var data = Bass.BASS_GetDeviceInfos();
            int index = -1;
            for (int i = 0; i < data.Length; i++)
                if (data[i].IsDefault)
                {
                    index = i;
                    break;
                }
            if (!data[index].IsInitialized)
                Bass.BASS_Init(index, 44100, BASSInit.BASS_DEVICE_CPSPEAKERS, wind);
            var a = Bass.BASS_ChannelGetDevice(stream);
            if (a != index){
                Bass.BASS_ChannelSetDevice(stream, index);
                Bass.BASS_SetDevice(index);
            }
        }
        public void Free() {
            Bass.BASS_ChannelStop(stream);
            Bass.BASS_StreamFree(stream);
            Bass.BASS_Stop();
            Bass.BASS_Free();
        }
    }
}
