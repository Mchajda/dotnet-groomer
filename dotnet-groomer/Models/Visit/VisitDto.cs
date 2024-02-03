using static dotnet_groomer.Functions.VisitsAnalytics;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using dotnet_groomer.Models.User;

public class VisitDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    [Column("start_date")]
    public DateTimeOffset Start { get; set; }
    [Column("end_date")]
    public DateTimeOffset End { get; set; }
    [Column("is_all_day")]
    public bool AllDay { get; set; }
    public int Price { get; set; }
    [Column("cleared")]
    public bool PaymentCleared { get; set; } = false;
    public List<ProductDto> Products { get; set; }
    public int? CustomerId { get; set; }
    public UserDto? Customer { get; set; }
}