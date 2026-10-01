using System.ComponentModel.DataAnnotations;

namespace WiFiAnalyzer.Core.Models;

public interface IEntityBase
{
    [Key]
    long Id { get; set; }
}