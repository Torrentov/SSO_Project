﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UserCRUD.Data;

#nullable disable

namespace UserCRUD.Migrations
{
    [DbContext(typeof(ApiTokenDbContext))]
    [Migration("20220518203154_UserToken")]
    partial class UserToken
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("UserCRUD.Models.ApiToken", b =>
                {
                    b.Property<string>("token_type")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("access_token")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("token_type");

                    b.ToTable("ApiToken");
                });

            modelBuilder.Entity("UserCRUD.Models.UserToken", b =>
                {
                    b.Property<string>("email")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("token_type")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("email");

                    b.HasIndex("token_type");

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("UserCRUD.Models.UserToken", b =>
                {
                    b.HasOne("UserCRUD.Models.ApiToken", "token")
                        .WithMany()
                        .HasForeignKey("token_type");

                    b.Navigation("token");
                });
#pragma warning restore 612, 618
        }
    }
}
