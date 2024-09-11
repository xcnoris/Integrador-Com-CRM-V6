﻿// <auto-generated />
using System;
using Integrador_Com_CRM.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Integrador_Com_CRM.Migrations
{
    [DbContext(typeof(IntegradorDBContext))]
    partial class IntegradorDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Integrador_Com_CRM.Models.RelacaoOrdemServicoModels", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Cod_Oportunidade")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Data_Alteracao")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Data_Criacao")
                        .HasColumnType("datetime2");

                    b.Property<int>("Id_CategoriaOS")
                        .HasColumnType("int");

                    b.Property<int>("Id_OrdemServico")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Relacao_OS_Com_CRM");
                });
#pragma warning restore 612, 618
        }
    }
}
