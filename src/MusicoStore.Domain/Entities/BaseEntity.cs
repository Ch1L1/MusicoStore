using System.ComponentModel.DataAnnotations;

namespace MusicoStore.Domain.Entities;

public class BaseEntity
{
    [Key]
    public int Id { get; set; }
}

