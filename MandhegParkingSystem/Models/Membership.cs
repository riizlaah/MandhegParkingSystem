using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MandhegParkingSystem.Models;

[Table("Membership")]
public partial class Membership
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(20)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("last_updated_at", TypeName = "datetime")]
    public DateTime? LastUpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "datetime")]
    public DateTime? DeletedAt { get; set; }

    [InverseProperty("Membership")]
    public virtual ICollection<HourlyRate> HourlyRates { get; set; } = new List<HourlyRate>();

    [InverseProperty("Membership")]
    public virtual ICollection<Member> Members { get; set; } = new List<Member>();
}
