﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace UserServiceApp.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240906131626_UpdateDeleteBehavior")]
    partial class UpdateDeleteBehavior
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("UserServiceApp.Models.CabinetRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CabinetName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateEnd")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateStart")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsApprove")
                        .HasColumnType("bit");

                    b.Property<bool>("IsHandle")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSenderSeen")
                        .HasColumnType("bit");

                    b.Property<string>("SenderPhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TargetPhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("CabinetRequests");
                });

            modelBuilder.Entity("UserServiceApp.Models.MedicationRepo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Barcode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BrochurePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DrugEnglishName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DrugHebrewName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EnglishDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HebrewDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImagePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ShelfLife")
                        .HasColumnType("int");

                    b.Property<int>("largestPackage")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("MedicationRepos");
                });

            modelBuilder.Entity("UserServiceApp.Models.MedicineCabinet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CreatorId")
                        .HasColumnType("int");

                    b.Property<string>("MedicineCabinetName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("MedicineCabinets");
                });

            modelBuilder.Entity("UserServiceApp.Models.MedicineCabinetUsers", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("MedicineCabinetId")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.HasKey("UserId", "MedicineCabinetId");

                    b.HasIndex("MedicineCabinetId");

                    b.ToTable("MedicineCabinetUsers");
                });

            modelBuilder.Entity("UserServiceApp.Models.PhoneMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PhoneNumber")
                        .HasColumnType("int");

                    b.Property<DateTime>("SentTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("PhoneMessages");
                });

            modelBuilder.Entity("UserServiceApp.Models.Reminder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsRecurring")
                        .HasColumnType("bit");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan>("RecurrenceInterval")
                        .HasColumnType("time");

                    b.Property<DateTime>("ReminderTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("UserMedicationId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("UserMedicationId");

                    b.ToTable("Reminders");
                });

            modelBuilder.Entity("UserServiceApp.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PhoneNumber")
                        .HasColumnType("int");

                    b.HasKey("UserId");

                    b.HasIndex("PhoneNumber")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("UserServiceApp.Models.UserMedications", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Barcode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CreatorId")
                        .HasColumnType("int");

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("bit");

                    b.Property<int>("MedicationRepoId")
                        .HasColumnType("int");

                    b.Property<int>("MedicineCabinetId")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfPills")
                        .HasColumnType("int");

                    b.Property<int?>("PillSize")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Validity")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.HasIndex("MedicationRepoId");

                    b.HasIndex("MedicineCabinetId");

                    b.ToTable("UserMedications");
                });

            modelBuilder.Entity("UserServiceApp.Models.MedicineCabinet", b =>
                {
                    b.HasOne("UserServiceApp.Models.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("UserServiceApp.Models.MedicineCabinetUsers", b =>
                {
                    b.HasOne("UserServiceApp.Models.MedicineCabinet", "MedicineCabinet")
                        .WithMany("MedicineCabinetUsers")
                        .HasForeignKey("MedicineCabinetId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("UserServiceApp.Models.User", "User")
                        .WithMany("MedicineCabinetUsersList")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("MedicineCabinet");

                    b.Navigation("User");
                });

            modelBuilder.Entity("UserServiceApp.Models.Reminder", b =>
                {
                    b.HasOne("UserServiceApp.Models.User", "User")
                        .WithMany("Reminders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("UserServiceApp.Models.UserMedications", "UserMedication")
                        .WithMany("Reminders")
                        .HasForeignKey("UserMedicationId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("UserMedication");
                });

            modelBuilder.Entity("UserServiceApp.Models.UserMedications", b =>
                {
                    b.HasOne("UserServiceApp.Models.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UserServiceApp.Models.MedicationRepo", "MedicationRepo")
                        .WithMany("Medications")
                        .HasForeignKey("MedicationRepoId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("UserServiceApp.Models.MedicineCabinet", "MedicineCabinet")
                        .WithMany("Medications")
                        .HasForeignKey("MedicineCabinetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");

                    b.Navigation("MedicationRepo");

                    b.Navigation("MedicineCabinet");
                });

            modelBuilder.Entity("UserServiceApp.Models.MedicationRepo", b =>
                {
                    b.Navigation("Medications");
                });

            modelBuilder.Entity("UserServiceApp.Models.MedicineCabinet", b =>
                {
                    b.Navigation("Medications");

                    b.Navigation("MedicineCabinetUsers");
                });

            modelBuilder.Entity("UserServiceApp.Models.User", b =>
                {
                    b.Navigation("MedicineCabinetUsersList");

                    b.Navigation("Reminders");
                });

            modelBuilder.Entity("UserServiceApp.Models.UserMedications", b =>
                {
                    b.Navigation("Reminders");
                });
#pragma warning restore 612, 618
        }
    }
}
