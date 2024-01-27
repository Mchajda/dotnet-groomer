using static dotnet_groomer.Functions.VisitsAnalytics;
using System.Collections.Generic;

public class VisitDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<ProductDto> Products { get; set; }
}