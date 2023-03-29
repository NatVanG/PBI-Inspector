using System.IO.Compression;

namespace PBIXInspectorLibrary
{
    public class PbixFile : IDisposable
    {
        private string _filePath = null;
        private FileInfo _fileInfo = null;
        private ZipArchive _za = null;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pbixFilePath"></param>
        /// TODO: check Power BI Desktop version is supported
        public PbixFile(string pbixFilePath)
        {
            this._filePath = pbixFilePath;
            this._fileInfo = new FileInfo(_filePath);
        }

        public ZipArchive Archive
        {
            get
            {
                try
                {
                    this._za = ZipFile.Open(FilePath, ZipArchiveMode.Read);
                    return _za;
                }
                catch (System.IO.FileNotFoundException e)
                {
                    throw new PBIXInspectorException(string.Format("File with path \"{0}\" does not exists.", this.FilePath), e);
                }
                catch (System.IO.InvalidDataException e)
                {
                    throw new PBIXInspectorException(string.Format("Couldn't open file with path \"{0}\". PBIX files with Microsoft Information Protection are not currently supported by \"PBIX Inspector\".", this.FilePath), e);
                }

            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                if (this._za != null)
                {
                    this._za.Dispose();
                }
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PbixFile()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}