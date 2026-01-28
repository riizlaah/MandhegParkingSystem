using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MandhegParkingSystem.Models;

[Table("Vehicle")]
[Index("LicensePlate", Name = "UQ__Vehicle__F72CD56E6AB30123", IsUnique = true)]
public partial class Vehicle
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("vehicle_type_id")]
    public int VehicleTypeId { get; set; }

    [Column("member_id")]
    public int MemberId { get; set; }

    [Column("license_plate")]
    [StringLength(20)]
    [Unicode(false)]
    public string LicensePlate { get; set; } = null!;

    [Column("notes")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Notes { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("last_updated_at", TypeName = "datetime")]
    public DateTime? LastUpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "datetime")]
    public DateTime? DeletedAt { get; set; }

    [ForeignKey("MemberId")]
    [InverseProperty("Vehicles")]
    public virtual Member Member { get; set; } = null!;

    [InverseProperty("Vehicle")]
    public virtual ICollection<ParkingDatum> ParkingData { get; set; } = new List<ParkingDatum>();

    [ForeignKey("VehicleTypeId")]
    [InverseProperty("Vehicles")]
    public virtual VehicleType VehicleType { get; set; } = null!;
}
