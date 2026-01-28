using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MandhegParkingSystem.Models;

public partial class HourlyRate
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("membership_id")]
    public int MembershipId { get; set; }

    [Column("vehicle_type_id")]
    public int VehicleTypeId { get; set; }

    [Column("value", TypeName = "decimal(10, 2)")]
    public decimal Value { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("last_updated_at", TypeName = "datetime")]
    public DateTime? LastUpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "datetime")]
    public DateTime? DeletedAt { get; set; }

    [ForeignKey("MembershipId")]
    [InverseProperty("HourlyRates")]
    public virtual Membership Membership { get; set; } = null!;

    [InverseProperty("HourlyRates")]
    public virtual ICollection<ParkingDatum> ParkingData { get; set; } = new List<ParkingDatum>();

    [ForeignKey("VehicleTypeId")]
    [InverseProperty("HourlyRates")]
    public virtual VehicleType VehicleType { get; set; } = null!;
}
