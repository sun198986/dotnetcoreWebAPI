using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Routine.Api.Migrations
{
    public partial class AddEmployeeData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("e56d4dcc-313b-4310-928f-01cc293fd7da"), new Guid("0d8c6be7-e984-4ade-8997-8a1e67ea0c22"), new DateTime(1986, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "G055", "Harry", 1, "Miko" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("a68c7721-beb7-453d-b9f1-a661a2040ed4"), new Guid("0d8c6be7-e984-4ade-8997-8a1e67ea0c22"), new DateTime(1976, 5, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "G001", "Live", 1, "Mai" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("2affcefc-9ae1-4bd5-bb6e-6100ab0b4faa"), new Guid("6fa484b4-26aa-405a-a1f6-7f82092f66b6"), new DateTime(1986, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "G016", "Love", 2, "Pi" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("124bcc74-7bc5-4a25-ad43-e43814014ef9"), new Guid("6fa484b4-26aa-405a-a1f6-7f82092f66b6"), new DateTime(1976, 5, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "G044", "Papa", 2, "Richardson" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("267b4f39-1641-4387-9e2d-584f5fec4bfd"), new Guid("658d4ed3-1505-4d02-b9ed-942e2a918e38"), new DateTime(1986, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "G003", "Marry", 2, "King" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("4d143264-be9a-41e7-83d2-a6bd5a0c7e7d"), new Guid("658d4ed3-1505-4d02-b9ed-942e2a918e38"), new DateTime(1976, 5, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "G004", "Kevin", 1, "Richardson" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("124bcc74-7bc5-4a25-ad43-e43814014ef9"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("267b4f39-1641-4387-9e2d-584f5fec4bfd"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("2affcefc-9ae1-4bd5-bb6e-6100ab0b4faa"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("4d143264-be9a-41e7-83d2-a6bd5a0c7e7d"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("a68c7721-beb7-453d-b9f1-a661a2040ed4"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("e56d4dcc-313b-4310-928f-01cc293fd7da"));
        }
    }
}
