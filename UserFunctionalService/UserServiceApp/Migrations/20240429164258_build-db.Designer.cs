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
    [Migration("20240429164258_build-db")]
    partial class builddb
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

                    b.HasKey("Id");

                    b.ToTable("Houses");
                });

            modelBuilder.Entity("UserServiceApp.Models.Medication", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MedicationRepoId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Validity")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("MedicationRepoId");

                    b.HasIndex("UserId");

                    b.ToTable("Medications");
                });

            modelBuilder.Entity("UserServiceApp.Models.MedicationRepo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

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

                    b.HasKey("UserId", "HouseId");

                    b.HasIndex("HouseId");

                    b.ToTable("UserHouses");
                });

            modelBuilder.Entity("UserServiceApp.Models.Medication", b =>
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