using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBIXInspectorLibrary
{
    internal abstract class PbiFile : IDisposable
    {
        private string _filePath = null;
        private FileInfo _fileInfo = null;


        private bool disposedValue;

        public string ReportName
        {
            get { return _fileInfo.Name; }
        }

        public string FilePath
        {
            get { return _filePath; }
        }

        public string DirectoryPath
        {
            get { return _fileInfo.DirectoryName; }
        }

        public PBIFileTypeEnum FileType
        {
            get { return PBIFileType(this._filePath); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pbixFilePath"></param>
        /// TODO: check Power BI Desktop version is supported
        public PbiFile(string pbiFilePath)
        {
            this._filePath = pbiFilePath;
            this._fileInfo = new FileInfo(_filePath);
        }

        public enum PBIFileTypeEnum
        {
            None,
            PBIX,
            PBIP,
            PBIPReport,
            Other
        }

        public static PBIFileTypeEnum PBIFileType(string PBIFilePath)
        {
            if (string.IsNullOrEmpty(PBIFilePath)) return PBIFileTypeEnum.None;

            if (PBIFilePath.ToLower().EndsWith(".pbix"))
            {
                return PBIFileTypeEnum.PBIX;
            }

            if (PBIFilePath.ToLower().EndsWith(".pbip"))
            {
                return PBIFileTypeEnum.PBIP;
            }

            if (PBIFilePath.ToLower().EndsWith(".report"))
            {
                return PBIFileTypeEnum.PBIPReport;
            }

            return PBIFileTypeEnum.Other;
        }

        public abstract Stream GetEntryStream(string entryPath);

        public abstract void Dispose();
    }
}
