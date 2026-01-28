using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MandhegParkingSystem.Models;

[Table("Employee")]
[Index("Email", Name = "UQ__Employee__AB6E61647B47E4A8", IsUnique = true)]
public partial class Employee
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(200)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("email")]
    [StringLength(200)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [Column("password")]
    [StringLength(64)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [Column("phone_number")]
    [StringLength(200)]
    [Unicode(false)]
    public string PhoneNumber { get; set; } = null!;

    [Column("address")]
    [StringLength(200)]
    [Unicode(false)]
    public string Address { get; set; } = null!;

    [Column("date_of_birth")]
    public DateOnly DateOfBirth { get; set; }

    [Column("gender")]
    [StringLength(10)]
    [Unicode(false)]
    public string Gender { get; set; } = null!;

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("last_updated_at", TypeName = "datetime")]
    public DateTime? LastUpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "datetime")]
    public DateTime? DeletedAt { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<ParkingDatum> ParkingData { get; set; } = new List<ParkingDatum>();
}
