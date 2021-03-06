﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProcastinationKiller.Models;

namespace ProcastinationKiller.Migrations
{
    [DbContext(typeof(UsersContext))]
    partial class UsersContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085");

            modelBuilder.Entity("ProcastinationKiller.Models.BaseEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<bool>("Hidden");

                    b.Property<int?>("Points");

                    b.Property<int?>("StateId");

                    b.Property<int?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("StateId");

                    b.HasIndex("UserId");

                    b.ToTable("BaseEvent");

                    b.HasDiscriminator<string>("Discriminator").HasValue("BaseEvent");
                });

            modelBuilder.Entity("ProcastinationKiller.Models.RegistartionCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime?>("ConfirmationDate");

                    b.Property<bool>("IsConfirmed");

                    b.HasKey("Id");

                    b.ToTable("RegistartionCode");
                });

            modelBuilder.Entity("ProcastinationKiller.Models.TodoItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Completed");

                    b.Property<string>("Description");

                    b.Property<DateTime?>("FinishTime");

                    b.Property<string>("Name");

                    b.Property<DateTime>("Regdate");

                    b.Property<DateTime>("TargetDate");

                    b.Property<int?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Todos");
                });

            modelBuilder.Entity("ProcastinationKiller.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CodeId");

                    b.Property<int?>("CurrentStateId");

                    b.Property<string>("Email");

                    b.Property<string>("Password");

                    b.Property<DateTime>("Regdate");

                    b.Property<string>("Token");

                    b.Property<int>("UserStatus");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.HasIndex("CodeId");

                    b.HasIndex("CurrentStateId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ProcastinationKiller.Models.UserState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CurrentLoginStreak");

                    b.Property<int>("DailyLogins");

                    b.Property<DateTime?>("LastLoginDate");

                    b.Property<int>("LongestLoginStreak");

                    b.Property<int>("Points");

                    b.Property<int>("TotalTodosCompleted");

                    b.Property<int>("WeeklyLogins");

                    b.HasKey("Id");

                    b.ToTable("UserState");
                });

            modelBuilder.Entity("ProcastinationKiller.Models.DailyLoginEvent", b =>
                {
                    b.HasBaseType("ProcastinationKiller.Models.BaseEvent");


                    b.ToTable("DailyLoginEvent");

                    b.HasDiscriminator().HasValue("DailyLoginEvent");
                });

            modelBuilder.Entity("ProcastinationKiller.Models.TodoCompletedEvent", b =>
                {
                    b.HasBaseType("ProcastinationKiller.Models.BaseEvent");

                    b.Property<int?>("CompletedItemId");

                    b.HasIndex("CompletedItemId");

                    b.ToTable("TodoCompletedEvent");

                    b.HasDiscriminator().HasValue("TodoCompletedEvent");
                });

            modelBuilder.Entity("ProcastinationKiller.Models.WeeklyLoginEvent", b =>
                {
                    b.HasBaseType("ProcastinationKiller.Models.BaseEvent");


                    b.ToTable("WeeklyLoginEvent");

                    b.HasDiscriminator().HasValue("WeeklyLoginEvent");
                });

            modelBuilder.Entity("ProcastinationKiller.Models.BaseEvent", b =>
                {
                    b.HasOne("ProcastinationKiller.Models.UserState", "State")
                        .WithMany()
                        .HasForeignKey("StateId");

                    b.HasOne("ProcastinationKiller.Models.User")
                        .WithMany("Events")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("ProcastinationKiller.Models.TodoItem", b =>
                {
                    b.HasOne("ProcastinationKiller.Models.User")
                        .WithMany("UserTodos")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("ProcastinationKiller.Models.User", b =>
                {
                    b.HasOne("ProcastinationKiller.Models.RegistartionCode", "Code")
                        .WithMany()
                        .HasForeignKey("CodeId");

                    b.HasOne("ProcastinationKiller.Models.UserState", "CurrentState")
                        .WithMany()
                        .HasForeignKey("CurrentStateId");
                });

            modelBuilder.Entity("ProcastinationKiller.Models.TodoCompletedEvent", b =>
                {
                    b.HasOne("ProcastinationKiller.Models.TodoItem", "CompletedItem")
                        .WithMany()
                        .HasForeignKey("CompletedItemId");
                });
#pragma warning restore 612, 618
        }
    }
}
