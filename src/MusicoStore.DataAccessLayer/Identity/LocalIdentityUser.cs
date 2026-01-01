using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using MusicoStore.Domain.Entities;

namespace MusicoStore.DataAccessLayer.Identity;

public class LocalIdentityUser : IdentityUser
{
    public int CustomerId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public virtual Customer? Customer { get; set; }
}
