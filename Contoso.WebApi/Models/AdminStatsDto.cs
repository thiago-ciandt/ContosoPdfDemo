namespace Contoso.WebApi.Models
{
    public class AdminStatsDto
    {
        public double? AverageTimeSeconds { get; set; }
        public double? AverageSizeMb { get; set; }
        public double TotalSizeMb { get; set; }
    }
}
