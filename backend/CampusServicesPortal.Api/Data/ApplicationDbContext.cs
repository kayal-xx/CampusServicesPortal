using CampusServicesPortal.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Hostel> Hostels => Set<Hostel>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<HostelApplication> HostelApplications => Set<HostelApplication>();
    public DbSet<Lab> Labs => Set<Lab>();
    public DbSet<LabBooking> LabBookings => Set<LabBooking>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventRegistration> EventRegistrations => Set<EventRegistration>();
    public DbSet<ComplaintCategory> ComplaintCategories => Set<ComplaintCategory>();
    public DbSet<Complaint> Complaints => Set<Complaint>();
    public DbSet<CertificateRequest> CertificateRequests => Set<CertificateRequest>();
    public DbSet<FeePayment> FeePayments => Set<FeePayment>();
    public DbSet<Notification> Notifications => Set<Notification>();
}