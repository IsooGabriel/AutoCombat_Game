using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using UnityEngine;

public class FileSelector : MonoBehaviour
{
    [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName(ref OpenFileName ofn);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct OpenFileName
    {
        public int lStructSize;
        public IntPtr hwndOwner;
        public IntPtr hInstance;
        public string lpstrFilter;
        public string lpstrCustomFilter;
        public int nMaxCustFilter;
        public int nFilterIndex;
        public string lpstrFile;
        public int nMaxFile;
        public string lpstrFileTitle;
        public int nMaxFileTitle;
        public string lpstrInitialDir;
        public string lpstrTitle;
        public int Flags;
        public short nFileOffset;
        public short nFileExtension;
        public string lpstrDefExt;
        public IntPtr lCustData;
        public IntPtr lpfnHook;
        public string lpTemplateName;
        public IntPtr pvReserved;
        public int dwReserved;
        public int FlagsEx;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string path = OpenFileDialog();
            if (!string.IsNullOrEmpty(path))
            {
               UnityEngine.Debug.Log("選択されたファイル: " + path);
            }
        }
    }

    string OpenFileDialog()
    {
        OpenFileName ofn = new OpenFileName();
        ofn.lStructSize = Marshal.SizeOf(ofn);
        ofn.lpstrFilter = "All Files\0*.*\0\0";
        ofn.lpstrFile = new string(new char[512]);
        ofn.nMaxFile = ofn.lpstrFile.Length;
        ofn.lpstrFileTitle = new string(new char[128]);
        ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
        ofn.lpstrTitle = "ファイルを選択";
        ofn.hwndOwner = Process.GetCurrentProcess().MainWindowHandle;

        if (GetOpenFileName(ref ofn))
        {
            return ofn.lpstrFile;
        }
        return null;
    }
}