using System.IO.Compression;

namespace PBIXInspectorLibrary
{
    internal class PbixFile : PbiFile, IDisposable
    {

        private ZipArchive _za = null;
        private bool disposedValue;

        public PbixFile(string pbiFilePath) : base(pbiFilePath)
        {
        }

        private ZipArchive Archive
        {
            get
            {
                try
                {
                    if (this._za == null) this._za = ZipFile.Open(FilePath, ZipArchiveMode.Read);
                    return _za;
                }
                catch (System.IO.FileNotFoundException e)
                {
                    throw new PBIXInspectorException(string.Format("File with path \"{0}\" does not exists.", this.FilePath), e);
                }
                catch (System.IO.InvalidDataException e)
                {
                    throw new PBIXInspectorException(string.Format("Could not open file with path \"{0}\". The file was not in a valid format. Please note that PBIX files with Microsoft Information Protection are not currently supported by \"PBIX Inspector\".", this.FilePath), e);
                }

            }
        }

        public override Stream GetEntryStream(string entryPath)
        {
            if (!string.IsNullOrEmpty(entryPath))
            {
                var zae = this.Archive.GetEntry(entryPath);
                if (zae != null)
                {
                    //TODO: raise msg
                    return zae.Open();
                }
            }

            return null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    if (this._za != null)
                    {
                        this._za.Dispose();
                        this._za = null;
                    }
                }


                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public override void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


    }
}