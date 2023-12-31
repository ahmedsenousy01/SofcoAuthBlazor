﻿// <auto-generated />
using System;
using AuthServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AuthServer.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20231209093132_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true);

            modelBuilder.Entity("AuthServer.Data.Models.AccessToken", b =>
                {
                    b.Property<int>("AccessTokenId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AccessTokenString")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Expiration")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Revoked")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Scopes")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("AccessTokenId");

                    b.HasIndex("ClientId");

                    b.HasIndex("UserId");

                    b.ToTable("AccessTokens");

                    b.HasData(
                        new
                        {
                            AccessTokenId = 1,
                            ClientId = "sofcopayclientid",
                            Expiration = new DateTime(2023, 12, 9, 11, 46, 31, 657, DateTimeKind.Local).AddTicks(2580),
                            Revoked = false,
                            Scopes = "[]",
                            UserId = "adminuserid"
                        },
                        new
                        {
                            AccessTokenId = 2,
                            ClientId = "sofcosmsclientid",
                            Expiration = new DateTime(2023, 12, 9, 11, 46, 31, 657, DateTimeKind.Local).AddTicks(2585),
                            Revoked = false,
                            Scopes = "[]",
                            UserId = "adminuserid"
                        });
                });

            modelBuilder.Entity("AuthServer.Data.Models.AuthorizationCode", b =>
                {
                    b.Property<int>("AuthorizationCodeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AuthCodeString")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CodeChallenge")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CodeChallengeMethod")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Expiration")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Used")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("AuthorizationCodeId");

                    b.HasIndex("ClientId");

                    b.HasIndex("UserId");

                    b.ToTable("AuthorizationCodes");

                    b.HasData(
                        new
                        {
                            AuthorizationCodeId = 1,
                            AuthCodeString = "8c12fd67-6134-47c0-937a-cbf84a6f3064",
                            ClientId = "sofcopayclientid",
                            CodeChallenge = "authcode1challenge",
                            CodeChallengeMethod = "Plain",
                            Expiration = new DateTime(2023, 12, 9, 11, 41, 31, 657, DateTimeKind.Local).AddTicks(2496),
                            Used = false,
                            UserId = "adminuserid"
                        },
                        new
                        {
                            AuthorizationCodeId = 2,
                            AuthCodeString = "6d63ad91-f79c-40df-9810-0dce530eb455",
                            ClientId = "sofcosmsclientid",
                            CodeChallenge = "authcode2challenge",
                            CodeChallengeMethod = "Plain",
                            Expiration = new DateTime(2023, 12, 9, 11, 41, 31, 657, DateTimeKind.Local).AddTicks(2559),
                            Used = false,
                            UserId = "adminuserid"
                        });
                });

            modelBuilder.Entity("AuthServer.Data.Models.Client", b =>
                {
                    b.Property<string>("ClientId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClientSecret")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("RedirectURIs")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Scopes")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ClientId");

                    b.HasIndex("UserId");

                    b.ToTable("Clients");

                    b.HasData(
                        new
                        {
                            ClientId = "sofcopayclientid",
                            ClientSecret = "2c471168-e53c-4994-848b-ad63a3c2f81fc2816627-b988-4381-9d97-2a73f492f328",
                            RedirectURIs = "[\"http://localhost:3000\"]",
                            Scopes = "[\"openid\"]",
                            UserId = "adminuserid"
                        },
                        new
                        {
                            ClientId = "sofcosmsclientid",
                            ClientSecret = "9f9ec040-4026-4690-8d13-2f31be512fe4dd827a56-f841-468f-b370-6b397ee60ec4",
                            RedirectURIs = "[\"http://localhost:3001\"]",
                            Scopes = "[\"openid\"]",
                            UserId = "adminuserid"
                        });
                });

            modelBuilder.Entity("AuthServer.Data.Models.RefreshToken", b =>
                {
                    b.Property<int>("RefreshTokenId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AuthorizationCodeId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Expiration")
                        .HasColumnType("TEXT");

                    b.Property<string>("RefreshTokenString")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Revoked")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Used")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("RefreshTokenId");

                    b.HasIndex("AuthorizationCodeId");

                    b.HasIndex("ClientId");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshTokens");

                    b.HasData(
                        new
                        {
                            RefreshTokenId = 1,
                            AuthorizationCodeId = 1,
                            ClientId = "sofcopayclientid",
                            Expiration = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            RefreshTokenString = "304af016-f88e-4e07-b14b-abb2e3df5143",
                            Revoked = false,
                            Used = false,
                            UserId = "adminuserid"
                        },
                        new
                        {
                            RefreshTokenId = 2,
                            AuthorizationCodeId = 2,
                            ClientId = "sofcosmsclientid",
                            Expiration = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            RefreshTokenString = "d0439d46-fd2a-4772-9bf4-4801f25825d4",
                            Revoked = false,
                            Used = false,
                            UserId = "adminuserid"
                        });
                });

            modelBuilder.Entity("AuthServer.Data.Models.User", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("UserId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserId = "adminuserid",
                            Email = "admin@sofco.org",
                            EmailConfirmed = false,
                            FirstName = "John",
                            LastName = "Doe",
                            PasswordHash = "admin",
                            UserName = "admin"
                        });
                });

            modelBuilder.Entity("AuthServer.Data.Models.AccessToken", b =>
                {
                    b.HasOne("AuthServer.Data.Models.Client", null)
                        .WithMany("AccessTokens")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AuthServer.Data.Models.User", null)
                        .WithMany("AccessTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AuthServer.Data.Models.AuthorizationCode", b =>
                {
                    b.HasOne("AuthServer.Data.Models.Client", null)
                        .WithMany("AuthorizationCodes")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AuthServer.Data.Models.User", null)
                        .WithMany("AuthorizationCodes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AuthServer.Data.Models.Client", b =>
                {
                    b.HasOne("AuthServer.Data.Models.User", null)
                        .WithMany("Clients")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("AuthServer.Data.Models.RefreshToken", b =>
                {
                    b.HasOne("AuthServer.Data.Models.AuthorizationCode", null)
                        .WithMany("RefreshTokens")
                        .HasForeignKey("AuthorizationCodeId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("AuthServer.Data.Models.Client", null)
                        .WithMany("RefreshTokens")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AuthServer.Data.Models.User", null)
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AuthServer.Data.Models.AuthorizationCode", b =>
                {
                    b.Navigation("RefreshTokens");
                });

            modelBuilder.Entity("AuthServer.Data.Models.Client", b =>
                {
                    b.Navigation("AccessTokens");

                    b.Navigation("AuthorizationCodes");

                    b.Navigation("RefreshTokens");
                });

            modelBuilder.Entity("AuthServer.Data.Models.User", b =>
                {
                    b.Navigation("AccessTokens");

                    b.Navigation("AuthorizationCodes");

                    b.Navigation("Clients");

                    b.Navigation("RefreshTokens");
                });
#pragma warning restore 612, 618
        }
    }
}
