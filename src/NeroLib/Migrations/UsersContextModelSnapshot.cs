﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using NeroLib;

namespace NeroLib.Migrations
{
    [DbContext(typeof(UsersContext))]
    partial class UsersContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.0-preview1-28290");

            modelBuilder.Entity("NeroLib.Parse", b =>
                {
                    b.Property<string>("ParseId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Difficulty");

                    b.Property<double>("HistoricalCount");

                    b.Property<double>("HistoricalPercent");

                    b.Property<string>("JobAbrv");

                    b.Property<string>("JobName");

                    b.Property<string>("Kill");

                    b.Property<string>("Name");

                    b.Property<double>("PerSecondAmount");

                    b.Property<double>("Percent");

                    b.Property<ulong>("ServerId");

                    b.Property<string>("Size");

                    b.Property<ulong>("UserId");

                    b.HasKey("ParseId");

                    b.HasIndex("ServerId");

                    b.HasIndex("UserId");

                    b.ToTable("Parses");
                });

            modelBuilder.Entity("NeroLib.Server", b =>
                {
                    b.Property<ulong>("ServerId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("Population");

                    b.HasKey("ServerId");

                    b.ToTable("Servers");
                });

            modelBuilder.Entity("NeroLib.User", b =>
                {
                    b.Property<ulong>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<ulong>("ServerId");

                    b.Property<string>("Username");

                    b.Property<int>("WorldId");

                    b.HasKey("UserId");

                    b.HasIndex("ServerId");

                    b.HasIndex("WorldId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("NeroLib.World", b =>
                {
                    b.Property<int>("WorldId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DataCenter");

                    b.Property<uint>("Population");

                    b.Property<string>("Region");

                    b.Property<string>("WorldName");

                    b.HasKey("WorldId");

                    b.ToTable("Worlds");
                });

            modelBuilder.Entity("NeroLib.Parse", b =>
                {
                    b.HasOne("NeroLib.Server")
                        .WithMany("Parses")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NeroLib.User")
                        .WithMany("Parses")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NeroLib.User", b =>
                {
                    b.HasOne("NeroLib.Server", "Server")
                        .WithMany("Users")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NeroLib.World", "World")
                        .WithMany("Users")
                        .HasForeignKey("WorldId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
