﻿// <auto-generated />
using System;
using DataBase.IntegradorCRM.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Integrador_Com_CRM.Migrations
{
    [DbContext(typeof(IntegradorDBContext))]
    [Migration("20240919235153_AddTBOSACAO1")]
    partial class AddTBOSACAO1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Integrador_Com_CRM.Models.EF.BoletoAcoesCRMModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Codigo_Acao")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Data_Criacao")
                        .HasColumnType("datetime2");

                    b.Property<int>("Dias_Cobrancas")
                        .HasMaxLength(300)
                        .HasColumnType("int");

                    b.Property<string>("Mensagem_Atualizacao")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.HasKey("Id");

                    b.ToTable("boletoAcoes_CRM");
                });

            modelBuilder.Entity("Integrador_Com_CRM.Models.EF.CobrancasNaSegundaModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BoletoId")
                        .HasColumnType("int");

                    b.Property<string>("CodigoJornada")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NovoAtrasoBoleto")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BoletoId");

                    b.ToTable("Cobrancas_Na_Segunda_CRM");
                });

            modelBuilder.Entity("Integrador_Com_CRM.Models.EF.DadosAPIModels", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("Id");

                    b.ToTable("DadosAPI_CRM");
                });

            modelBuilder.Entity("Integrador_Com_CRM.Models.EF.OSAcoesCRMModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Codigo_Acao")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<DateTime>("Data_Criacao")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdCategoria")
                        .HasColumnType("int");

                    b.Property<string>("Mensagem_Atualizacao")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.HasKey("Id");

                    b.ToTable("OSAcao_CRM");
                });

            modelBuilder.Entity("Integrador_Com_CRM.Models.EF.RelacaoBoletoCRMModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CNPJ_CPF")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Celular_Entidade")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Cod_Oportunidade")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Data_Atualizacao")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Data_Criacao")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Data_Vencimento")
                        .HasColumnType("datetime2");

                    b.Property<int>("DiasEmAtraso")
                        .HasColumnType("int");

                    b.Property<string>("Email_Entidade")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Id_DocumentoReceber")
                        .HasColumnType("int");

                    b.Property<int>("Id_Entidade")
                        .HasColumnType("int");

                    b.Property<string>("Nome_Entidade")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Numero_Documento")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quitado")
                        .HasColumnType("int");

                    b.Property<int>("Situacao")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("RelacaoBoletoCRM");
                });

            modelBuilder.Entity("Integrador_Com_CRM.Models.EF.RelacaoOrdemServicoModels", b =>
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

            modelBuilder.Entity("Integrador_Com_CRM.Models.EF.CobrancasNaSegundaModel", b =>
                {
                    b.HasOne("Integrador_Com_CRM.Models.EF.RelacaoBoletoCRMModel", "Boleto")
                        .WithMany()
                        .HasForeignKey("BoletoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Boleto");
                });
#pragma warning restore 612, 618
        }
    }
}
