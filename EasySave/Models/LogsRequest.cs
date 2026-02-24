
namespace EasySave.Models
{
    /// <summary>
    /// Model used to send the logs content and format to the view
    /// </summary>
    public class LogsRequest 
    {
        public string? Content { get; set; }
        public string? Format { get; set; }
    }
}
