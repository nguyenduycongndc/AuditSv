﻿// <auto-generated />
using System;
using Audit_service.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    [DbContext(typeof(KitanoSqlContext))]
    [Migration("20211104084654_auditplan7")]
    partial class auditplan7
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.AuditAssignment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_at");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("integer")
                        .HasColumnName("created_by");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("deleted_at");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("integer")
                        .HasColumnName("deleted_by");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("end_date");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("modified_at");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer")
                        .HasColumnName("modified_by");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("start_date");

                    b.Property<int?>("user_id")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("user_id");

                    b.ToTable("AUDIT_ASSIGNMENT");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.AuditPlan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("Browsedate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("browsedate");

                    b.Property<string>("Code")
                        .HasColumnType("text")
                        .HasColumnName("code");

                    b.Property<DateTime?>("Createdate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("createdate");

                    b.Property<bool?>("IsDelete")
                        .HasColumnType("boolean")
                        .HasColumnName("isdelete");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Note")
                        .HasColumnType("text")
                        .HasColumnName("note");

                    b.Property<int?>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<string>("Target")
                        .HasColumnType("text")
                        .HasColumnName("target");

                    b.Property<int?>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("userid");

                    b.Property<int?>("Version")
                        .HasColumnType("integer")
                        .HasColumnName("version");

                    b.Property<int>("Year")
                        .HasColumnType("integer")
                        .HasColumnName("year");

                    b.HasKey("Id");

                    b.ToTable("AUDIT_PLAN");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.AuditWork", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasColumnType("text")
                        .HasColumnName("code");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_at");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("integer")
                        .HasColumnName("created_by");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("deleted_at");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("integer")
                        .HasColumnName("deleted_by");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("end_date");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("modified_at");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer")
                        .HasColumnName("modified_by");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int?>("NumOfAuditor")
                        .HasColumnType("integer")
                        .HasColumnName("num_of_auditor");

                    b.Property<int?>("NumOfWorkdays")
                        .HasColumnType("integer")
                        .HasColumnName("num_of_workdays");

                    b.Property<string>("Path")
                        .HasColumnType("text")
                        .HasColumnName("path");

                    b.Property<string>("ReqOther")
                        .HasColumnType("text")
                        .HasColumnName("req_other");

                    b.Property<string>("ReqOutsourcing")
                        .HasColumnType("text")
                        .HasColumnName("req_outsourcing");

                    b.Property<string>("ReqSkillForAudit")
                        .HasColumnType("text")
                        .HasColumnName("req_skill_audit");

                    b.Property<int?>("ScaleOfAudit")
                        .HasColumnType("integer")
                        .HasColumnName("scale_of_audit");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("start_date");

                    b.Property<string>("Target")
                        .HasColumnType("text")
                        .HasColumnName("target");

                    b.Property<int?>("person_in_charge")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("person_in_charge");

                    b.ToTable("AUDIT_WORK");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.CatAuditProcedures", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("Activationid")
                        .HasColumnType("integer")
                        .HasColumnName("activationid");

                    b.Property<string>("Code")
                        .HasColumnType("text")
                        .HasColumnName("code");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int?>("Processid")
                        .HasColumnType("integer")
                        .HasColumnName("processid");

                    b.Property<bool?>("Status")
                        .HasColumnType("boolean")
                        .HasColumnName("status");

                    b.Property<int?>("Unitid")
                        .HasColumnType("integer")
                        .HasColumnName("unitid");

                    b.HasKey("Id");

                    b.ToTable("CAT_AUDIT_PROCEDURES");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.CatControl", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("Activationid")
                        .HasColumnType("integer")
                        .HasColumnName("activationid");

                    b.Property<string>("Code")
                        .HasColumnType("text")
                        .HasColumnName("code");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int?>("Processid")
                        .HasColumnType("integer")
                        .HasColumnName("processid");

                    b.Property<bool?>("Status")
                        .HasColumnType("boolean")
                        .HasColumnName("status");

                    b.Property<int?>("Unitid")
                        .HasColumnType("integer")
                        .HasColumnName("unitid");

                    b.HasKey("Id");

                    b.ToTable("CAT_CONTROL");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.CatRisk", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("Activationid")
                        .HasColumnType("integer")
                        .HasColumnName("activationid");

                    b.Property<string>("Code")
                        .HasColumnType("text")
                        .HasColumnName("code");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int?>("Processid")
                        .HasColumnType("integer")
                        .HasColumnName("processid");

                    b.Property<bool?>("Status")
                        .HasColumnType("boolean")
                        .HasColumnName("status");

                    b.Property<int?>("Unitid")
                        .HasColumnType("integer")
                        .HasColumnName("unitid");

                    b.HasKey("Id");

                    b.ToTable("CAT_RISK");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.Department", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("ID")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasColumnType("text")
                        .HasColumnName("Code");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("CreateDate");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("integer")
                        .HasColumnName("UserCreate");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("Description");

                    b.Property<int?>("DomainId")
                        .HasColumnType("integer")
                        .HasColumnName("DomainId");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("Status");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("LastModified");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer")
                        .HasColumnName("ModifiedBy");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("Name");

                    b.Property<int?>("ObjectClassId")
                        .HasColumnType("integer")
                        .HasColumnName("ObjectClassId");

                    b.Property<string>("ObjectClassName")
                        .HasColumnType("text")
                        .HasColumnName("ObjectClassName");

                    b.Property<int?>("ParentId")
                        .HasColumnType("integer")
                        .HasColumnName("ParentId");

                    b.Property<string>("ParentName")
                        .HasColumnType("text")
                        .HasColumnName("ParentName");

                    b.HasKey("Id");

                    b.ToTable("AUDIT_FACILITY");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.Roles", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_at");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("integer")
                        .HasColumnName("created_by");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("deleted_at");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("integer")
                        .HasColumnName("deleted_by");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("modified_at");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer")
                        .HasColumnName("modified_by");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("ROLES");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.SystemParameter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Default_Note")
                        .HasColumnType("text")
                        .HasColumnName("default_note");

                    b.Property<string>("Default_Value")
                        .HasColumnType("text")
                        .HasColumnName("default_value");

                    b.Property<DateTime?>("Modified_At")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("modified_at");

                    b.Property<int?>("Modified_By")
                        .HasColumnType("integer")
                        .HasColumnName("modified_by");

                    b.Property<string>("Note")
                        .HasColumnType("text")
                        .HasColumnName("note");

                    b.Property<string>("Parameter_Name")
                        .HasColumnType("text")
                        .HasColumnName("parameter_name");

                    b.Property<DateTime?>("Reset_At")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("reset_at");

                    b.Property<string>("Sub_System")
                        .HasColumnType("text")
                        .HasColumnName("sub_system");

                    b.Property<string>("Value")
                        .HasColumnType("text")
                        .HasColumnName("value");

                    b.HasKey("Id");

                    b.ToTable("SYSTEM_PARAMETER");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.UnitType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("createdAt");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("integer")
                        .HasColumnName("createdBy");

                    b.Property<DateTime>("DeletedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("deletedAt");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("integer")
                        .HasColumnName("deletedBy");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("modifiedAt");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer")
                        .HasColumnName("modifiedBy");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<bool?>("Status")
                        .HasColumnType("boolean")
                        .HasColumnName("status");

                    b.HasKey("Id");

                    b.ToTable("UNIT_TYPE");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.Users", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Avartar")
                        .HasColumnType("text")
                        .HasColumnName("avartar");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTime?>("DateOfJoining")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_of_joining");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("deleted_at");

                    b.Property<int?>("DepartmentId")
                        .HasColumnType("integer")
                        .HasColumnName("department_id");

                    b.Property<int?>("DomainId")
                        .HasColumnType("integer")
                        .HasColumnName("domain_Id");

                    b.Property<string>("Email")
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("FullName")
                        .HasColumnType("text")
                        .HasColumnName("full_name");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<DateTime?>("LastOnline")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("last_online_at");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("modified_at");

                    b.Property<string>("Password")
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.Property<int?>("RoleId")
                        .HasColumnType("integer")
                        .HasColumnName("role_id");

                    b.Property<string>("SaltKey")
                        .HasColumnType("text")
                        .HasColumnName("salt");

                    b.Property<string>("UserName")
                        .HasColumnType("text")
                        .HasColumnName("user_name");

                    b.Property<int?>("UsersType")
                        .HasColumnType("integer")
                        .HasColumnName("users_type");

                    b.HasKey("Id");

                    b.ToTable("USERS");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.UsersGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_at");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("integer")
                        .HasColumnName("created_by");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("deleted_at");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("integer")
                        .HasColumnName("deleted_by");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("modified_at");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer")
                        .HasColumnName("modified_by");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("USERS_GROUP");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.UsersGroupMapping", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("group_id")
                        .HasColumnType("integer");

                    b.Property<int?>("users_id")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("group_id");

                    b.HasIndex("users_id");

                    b.ToTable("USERS_GROUP_MAPPING");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.UsersGroupRoles", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("group_id")
                        .HasColumnType("integer");

                    b.Property<int?>("roles_id")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("group_id");

                    b.HasIndex("roles_id");

                    b.ToTable("USERS_GROUP_ROLES");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.UsersRoles", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("roles_id")
                        .HasColumnType("integer");

                    b.Property<int?>("users_id")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("roles_id");

                    b.HasIndex("users_id");

                    b.ToTable("USERS_ROLES");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.UsersWorkHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_at");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("integer")
                        .HasColumnName("created_by");

                    b.Property<DateTime?>("DateOfJoining")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_of_joining");

                    b.Property<int?>("DepartmentID")
                        .HasColumnType("integer")
                        .HasColumnName("department_id");

                    b.Property<int?>("users_id")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("users_id");

                    b.ToTable("USERS_WORK_HISTORY");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.AuditAssignment", b =>
                {
                    b.HasOne("Audit_service.Models.MigrationsModels.Users", "Users")
                        .WithMany()
                        .HasForeignKey("user_id");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.AuditWork", b =>
                {
                    b.HasOne("Audit_service.Models.MigrationsModels.Users", "Users")
                        .WithMany()
                        .HasForeignKey("person_in_charge");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.UsersGroupMapping", b =>
                {
                    b.HasOne("Audit_service.Models.MigrationsModels.UsersGroup", "UsersGroup")
                        .WithMany("UsersGroupMappings")
                        .HasForeignKey("group_id");

                    b.HasOne("Audit_service.Models.MigrationsModels.Users", "Users")
                        .WithMany("UsersGroupMappings")
                        .HasForeignKey("users_id");

                    b.Navigation("Users");

                    b.Navigation("UsersGroup");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.UsersGroupRoles", b =>
                {
                    b.HasOne("Audit_service.Models.MigrationsModels.UsersGroup", "UsersGroup")
                        .WithMany("UsersGroupRoles")
                        .HasForeignKey("group_id");

                    b.HasOne("Audit_service.Models.MigrationsModels.Roles", "Roles")
                        .WithMany("UsersGroupRoles")
                        .HasForeignKey("roles_id");

                    b.Navigation("Roles");

                    b.Navigation("UsersGroup");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.UsersRoles", b =>
                {
                    b.HasOne("Audit_service.Models.MigrationsModels.Roles", "Roles")
                        .WithMany("UsersRoles")
                        .HasForeignKey("roles_id");

                    b.HasOne("Audit_service.Models.MigrationsModels.Users", "Users")
                        .WithMany("UsersRoles")
                        .HasForeignKey("users_id");

                    b.Navigation("Roles");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.UsersWorkHistory", b =>
                {
                    b.HasOne("Audit_service.Models.MigrationsModels.Users", "Users")
                        .WithMany()
                        .HasForeignKey("users_id");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.Roles", b =>
                {
                    b.Navigation("UsersGroupRoles");

                    b.Navigation("UsersRoles");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.Users", b =>
                {
                    b.Navigation("UsersGroupMappings");

                    b.Navigation("UsersRoles");
                });

            modelBuilder.Entity("Audit_service.Models.MigrationsModels.UsersGroup", b =>
                {
                    b.Navigation("UsersGroupMappings");

                    b.Navigation("UsersGroupRoles");
                });
#pragma warning restore 612, 618
        }
    }
}
