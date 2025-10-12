using System.ComponentModel.DataAnnotations;

namespace MusicoStore.DataAccessLayer.Entities;

public class BaseEntity
{
    [Key]
    public int Id { get; set; }
}

