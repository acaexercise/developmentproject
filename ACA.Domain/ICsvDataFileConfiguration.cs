namespace ACA.Domain
{
    public interface ICsvDataFileConfiguration
    {
        string DataFileLocation { get; set; }
        string FileSearchPattern { get; set; }
    }
}