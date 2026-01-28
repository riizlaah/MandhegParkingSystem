using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MandhegParkingSystem.Models;

public partial class ParkingDatum
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("license_plate")]
    [StringLength(20)]
    [Unicode(false)]
    public string LicensePlate { get; set; } = null!;

    [Column("vehicle_id")]
    public int? VehicleId { get; set; }

    [Column("employee_id")]
    public int EmployeeId { get; set; }

    [Column("hourly_rates_id")]
    public int HourlyRatesId { get; set; }

    [Column("datetime_in", TypeName = "datetime")]
    public DateTime DatetimeIn { get; set; }

    [Column("datetime_out", TypeName = "datetime")]
    public DateTime DatetimeOut { get; set; }

    [Column("amount_to_pay", TypeName = "decimal(10, 2)")]
    public decimal AmountToPay { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("last_updated_at", TypeName = "datetime")]
    public DateTime? LastUpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "datetime")]
    public DateTime? DeletedAt { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("ParkingData")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("HourlyRatesId")]
    [InverseProperty("ParkingData")]
    public virtual HourlyRate HourlyRates { get; set; } = null!;

    [ForeignKey("VehicleId")]
    [InverseProperty("ParkingData")]
    public virtual Vehicle? Vehicle { get; set; }
}
