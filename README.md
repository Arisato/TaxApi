# TaxApi
Intro:
<br/>
<br/>
Coding challenge to build a web api enabling retrieval and manipulation of tax related data based on business rules provided.
<br/>
<br/>
Featuring SOLID principles and clean code practices.
<br/>
<br/>
xUnit tests are included to test anything with logic and are located in the TaxLedgerApiTests folder.
<br/>
<br/>
Context is generated by Entity Framework scaffolding feature and is based on SQL server database.

Running app sample via Swagger:
<br/>
<br/>
<img width="1268" alt="sample" src="https://github.com/user-attachments/assets/64fab2df-bcce-4443-be31-e82230ec5c6a" />

Manual:
1. Run the ```DatabaseScript.sql``` file located in the root of this repository to generate the required database, tables, constraints and relationships.
2. Add your connection string to ```appsettings.json``` located in the TaxLedgerAPI folder.
3. In the Visual Studio Package Manager Console window utilising Entity Framework run the following context scaffolding command:
   ```
   Scaffold-DbContext 'Name=ConnectionStrings:DefaultConnection' Microsoft.EntityFrameworkCore.SqlServer -Context 'Context' -Project DataEF -StartupProject TaxLedgerAPI -o Models -ContextDir ../DataEF -NoOnConfiguring -f
   ```
7. Run the app.
8. To visually observe api's and execute any endpoints against your database via UI you can simply input your local host link followed by /swagger in your browser window. Example: ```https://localhost:0000/swagger```.
