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
    [Migration("20240511170917_addPrivacyOnMedication")]
    partial class addPrivacyOnMedication
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("UserServiceApp.Models.House", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("FamilyName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Houses");
                });

            modelBuilder.Entity("UserServiceApp.Models.HouseRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateEnd")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateStart")
                        .HasColumnType("datetime2");

                    b.Property<int>("HouseId")
                        .HasColumnType("int");

                    b.Property<bool>("IsApprove")
                        .HasColumnType("bit");

                    b.Property<bool>("IsHandle")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSenderSeen")
                        .HasColumnType("bit");

                    b.Property<bool>("MergeToNewHouse")
                        .HasColumnType("bit");

                    b.Property<string>("SenderPhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("HouseRequests");
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

                    b.HasKey("Id");

                    b.ToTable("MedicationRepos");
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

                    b.Property<int>("HouseId")
                        .HasColumnType("int");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PhoneNumber")
                        .HasColumnType("int");

                    b.HasKey("UserId");

                    b.HasIndex("HouseId")
                        .IsUnique();

                    b.HasIndex("PhoneNumber")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("UserServiceApp.Models.UserHouse", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("HouseId")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.HasKey("UserId", "HouseId");

                    b.HasIndex("HouseId");

                    b.ToTable("UserHouses");
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

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("bit");

                    b.Property<int>("MedicationRepoId")
                        .HasColumnType("int");

                    b.Property<int?>("PillSize")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Validity")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("MedicationRepoId");

                    b.HasIndex("UserId");

                    b.ToTable("UserMedications");
                });

            modelBuilder.Entity("UserServiceApp.Models.User", b =>
                {
                    b.HasOne("UserServiceApp.Models.House", "House")
                        .WithOne("Manager")
                        .HasForeignKey("UserServiceApp.Models.User", "HouseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("House");
                });

            modelBuilder.Entity("UserServiceApp.Models.UserHouse", b =>
                {
                    b.HasOne("UserServiceApp.Models.House", "House")
                        .WithMany("UserHouses")
                        .HasForeignKey("HouseId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("UserServiceApp.Models.User", "User")
                        .WithMany("UserHouses")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("House");

                    b.Navigation("User");
                });

            modelBuilder.Entity("UserServiceApp.Models.UserMedications", b =>
                {
                    b.HasOne("UserServiceApp.Models.MedicationRepo", "MedicationRepo")
                        .WithMany("Medications")
                        .HasForeignKey("MedicationRepoId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("UserServiceApp.Models.User", "User")
                        .WithMany("Medications")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("MedicationRepo");

                    b.Navigation("User");
                });

            modelBuilder.Entity("UserServiceApp.Models.House", b =>
                {
                    b.Navigation("Manager");

                    b.Navigation("UserHouses");
                });

            modelBuilder.Entity("UserServiceApp.Models.MedicationRepo", b =>
                {
                    b.Navigation("Medications");
                });

            modelBuilder.Entity("UserServiceApp.Models.User", b =>
                {
                    b.Navigation("Medications");

                    b.Navigation("UserHouses");
                });
#pragma warning restore 612, 618
        }
    }
}
