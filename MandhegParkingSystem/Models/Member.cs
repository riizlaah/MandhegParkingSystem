using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MandhegParkingSystem.Models;

[Table("Member")]
[Index("Email", Name = "UQ__Member__AB6E616416BBD1FA", IsUnique = true)]
public partial class Member
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("membership_id")]
    public int MembershipId { get; set; }

    [Column("name")]
    [StringLength(200)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("email")]
    [StringLength(200)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

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

    [ForeignKey("MembershipId")]
    [InverseProperty("Members")]
    public virtual Membership Membership { get; set; } = null!;

    [InverseProperty("Member")]
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
