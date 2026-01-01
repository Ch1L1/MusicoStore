using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MusicoStore.Domain.Entities;

[Index(nameof(ProductId), nameof(StorageId), IsUnique = true)]
public class Stock : BaseEntity
{
    public int ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public virtual Product? Product { get; set; }

    public int StorageId { get; set; }

    [ForeignKey(nameof(StorageId))]
    public virtual Storage? Storage { get; set; }

    public int CurrentQuantity { get; set; }
}
