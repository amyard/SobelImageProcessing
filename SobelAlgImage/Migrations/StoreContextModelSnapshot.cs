﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SobelAlgImage.Data;

namespace SobelAlgImage.Migrations
{
    [DbContext(typeof(StoreContext))]
    partial class StoreContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8");

            modelBuilder.Entity("SobelAlgImage.Models.ImageModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AmountOfThreads")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SourceOriginal")
                        .HasColumnType("TEXT");

                    b.Property<string>("SourceTransformFaster")
                        .HasColumnType("TEXT");

                    b.Property<string>("SourceTransformSlower")
                        .HasColumnType("TEXT");

                    b.Property<string>("SourceTransformTaskFaster")
                        .HasColumnType("TEXT");

                    b.Property<string>("SourceTransformTaskSlower")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ImageModels");
                });
#pragma warning restore 612, 618
        }
    }
}
