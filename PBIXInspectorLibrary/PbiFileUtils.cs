using System.Runtime.CompilerServices;

[assembly: InternalsVisibleToAttribute("PBIXInspectorTests")]

namespace PBIXInspectorLibrary
{
    internal class PbiFileUtils
    {
        internal static PbiFile InitPbiFile(string pbiFilePath)
        {
            if (string.IsNullOrEmpty(pbiFilePath)) throw new PBIXInspectorException("PBI file path is empty.");

            switch (PbiFile.PBIFileType(pbiFilePath))
            {
                case PbiFile.PBIFileTypeEnum.PBIX:
                    return new PbixFile(pbiFilePath);
                case PbiFile.PBIFileTypeEnum.PBIP:
                    return new PbipFile(pbiFilePath);
                case PbiFile.PBIFileTypeEnum.PBIPReport:
                    return new PbipReportFile(pbiFilePath);
                default:
                    throw new PBIXInspectorException(string.Format("Could not determine the extension of PBI file with path \"{0}\".", pbiFilePath));
            }
        }
    }
}
